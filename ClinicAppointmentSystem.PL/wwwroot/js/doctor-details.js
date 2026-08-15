
const DAY_NAMES = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];
const DAY_LABELS = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];

let scheduleByDay = {};

document.addEventListener('DOMContentLoaded', function () {
    loadDoctorInfo();
    loadSchedule();
});

function loadDoctorInfo() {
    $.get('/Doctors/GetById', { id: window.currentDoctorId }, function (response) {
        if (!response.success) {
            showError(response.message);
            return;
        }

        const doctor = response.data;
        $('#doctorNameDisplay').text(doctor.name);
        $('#doctorMetaDisplay').text(`${doctor.specializationName} · ${doctor.phoneNumber || '-'} · ${doctor.email || '-'}`);
        $('#doctorStatusDisplay').html(doctor.isActive
            ? '<span class="badge badge-active">Active</span>'
            : '<span class="badge badge-inactive">Inactive</span>');
        $('#doctorAvatar').text(initialsFromName(doctor.name));
    }).fail(() => showError('Could not load doctor.'));
}

function initialsFromName(name) {
    if (!name) return '--';
    const parts = name.trim().split(' ');
    return (parts[0][0] + (parts[1]?.[0] || '')).toUpperCase();
}

function loadSchedule() {
    $.get('/Doctors/GetSchedules', { doctorId: window.currentDoctorId }, function (response) {
        if (!response.success) {
            showError(response.message);
            return;
        }

        scheduleByDay = {};
        (response.data || []).forEach(row => {
            scheduleByDay[row.dayOfWeek] = row;
        });

        renderWeekGrid();
    }).fail(() => showError('Could not load schedule.'));
}

function renderWeekGrid() {
    const grid = $('#scheduleWeekGrid');
    grid.empty();

    DAY_NAMES.forEach((dayName, index) => {
        grid.append(buildDayCard(dayName, DAY_LABELS[index]));
    });

    refreshIcons();
}

function buildDayCard(dayName, label) {
    const row = scheduleByDay[dayName];
    const isOff = !row;

    const card = $(`<div class="schedule-day-card ${isOff ? 'is-off' : ''}" data-day="${dayName}"></div>`);
    card.append(`<div class="day-label">${label}</div>`);

    if (isOff) {
        card.append(`<div class="day-hours off-text">Off</div>`);
        card.append(`<span class="day-action-icon" title="Add hours" onclick="startEditDay('${dayName}')"><i data-feather="plus"></i></span>`);
    } else {
        card.append(`<div class="day-hours">${formatTimeSpan(row.startTime)}&ndash;${formatTimeSpan(row.endTime)}</div>`);
        card.append(`<span class="day-action-icon" title="Edit hours" onclick="startEditDay('${dayName}')"><i data-feather="edit"></i></span>`);
    }

    return card;
}

function formatTimeSpan(ts) {
    if (!ts) return '-';
    const parts = ts.split(':');
    const date = new Date();
    date.setHours(parseInt(parts[0]), parseInt(parts[1]));
    return date.toLocaleTimeString([], { hour: 'numeric', minute: '2-digit' });
}

function startEditDay(dayName) {
    const row = scheduleByDay[dayName];
    const card = $(`.schedule-day-card[data-day="${dayName}"]`);
    const label = DAY_LABELS[DAY_NAMES.indexOf(dayName)];

    card.empty();
    card.append(`<div class="day-label">${label}</div>`);
    card.append(`
        <div class="day-edit-form">
            <input type="time" class="form-control form-control-sm" id="editStart_${dayName}" value="${row ? row.startTime.substring(0, 5) : '09:00'}" />
            <input type="time" class="form-control form-control-sm" id="editEnd_${dayName}" value="${row ? row.endTime.substring(0, 5) : '17:00'}" />
        </div>
    `);

    const actions = $(`<div class="day-edit-actions"></div>`);
    actions.append(`<span class="day-action-icon" title="Save" onclick="saveDay('${dayName}')"><i data-feather="check"></i></span>`);
    if (row) {
        actions.append(`<span class="day-action-icon" title="Remove" onclick="removeDay('${dayName}', ${row.scheduleID})"><i data-feather="trash-2"></i></span>`);
    }
    actions.append(`<span class="day-action-icon" title="Cancel" onclick="renderWeekGrid()"><i data-feather="x"></i></span>`);
    card.append(actions);

    refreshIcons();
}

function saveDay(dayName) {
    const start = $(`#editStart_${dayName}`).val();
    const end = $(`#editEnd_${dayName}`).val();

    if (!start || !end) {
        showError('Enter both a start and end time.');
        return;
    }

    const existing = scheduleByDay[dayName];
    const scheduleId = existing ? existing.scheduleID : 0;

    const formData = $.param({
        ScheduleID: scheduleId,
        DoctorID: window.currentDoctorId,
        DayOfWeek: dayName,
        StartTime: start,
        EndTime: end
    });

    const url = scheduleId > 0 ? '/Doctors/EditSchedule' : '/Doctors/AddSchedule';

    $.ajax({
        url,
        method: 'POST',
        headers: { RequestVerificationToken: getAntiForgeryToken() },
        data: formData,
        success: function (response) {
            if (!response.success) {
                showError(response.message);
                return;
            }
            showSuccess('Schedule saved.');
            loadSchedule();
        },
        error: () => showError('Could not save schedule.')
    });
}

function removeDay(dayName, scheduleId) {
    Swal.fire({
        title: 'Remove this day\'s hours?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Remove',
        cancelButtonText: 'Cancel'
    }).then(result => {
        if (!result.isConfirmed) return;

        $.ajax({
            url: '/Doctors/DeleteSchedule',
            method: 'POST',
            data: { id: scheduleId },
            headers: { RequestVerificationToken: getAntiForgeryToken() },
            success: function (response) {
                if (!response.success) {
                    showError(response.message);
                    return;
                }
                showSuccess('Schedule removed.');
                loadSchedule();
            },
            error: () => showError('Could not remove schedule.')
        });
    });
}
