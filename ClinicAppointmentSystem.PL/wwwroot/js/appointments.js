let appointmentsTable = null;

document.addEventListener('DOMContentLoaded', function () {
    initAppointmentsTable();
    initGridSearch('appointmentSearchInput', () => appointmentsTable);
    $('#appointmentForm').validate({ ignore: [] });
});

function initAppointmentsTable() {
    appointmentsTable = initSimpleDataTable('appointmentsTable', {
        ajax: { url: '/Appointments/GetAll' },
        columns: [
            { data: 'doctorName' },
            { data: 'patientName' },
            { data: 'appointmentDate', render: d => d ? new Date(d).toLocaleDateString() : '-' },
            {
                data: 'startTime',
                render: (t, type, row) => `${formatTimeSpan(row.startTime)} - ${formatTimeSpan(row.endTime)}`
            },
            { data: 'status', render: renderStatusBadge },
            {
                data: 'appointmentID',
                orderable: false,
                render: (id, type, row) => `
                    <span class="action-btn action-btn-delete" onclick="confirmDeleteAppointment(${id})" title="Delete">
                        <i data-feather="trash-2"></i>
                    </span>
                    ${row.status === 'Booked'
                        ? `<span class="action-btn action-btn-edit" onclick="openEditAppointmentModal(${id})" title="Edit">
                               <i data-feather="edit"></i>
                           </span>
                           <span class="action-btn action-btn-cancel" onclick="confirmCancelAppointment(${id})" title="Cancel">
                               <i data-feather="x-circle"></i>
                           </span>`
                        : ''}`
            }
        ]
    });
}

function refreshAppointmentsTable() {
    appointmentsTable?.ajax.reload(null, false);
}

function renderStatusBadge(status) {
    const map = { Booked: 'badge-booked', Cancelled: 'badge-cancelled', Completed: 'badge-completed' };
    return `<span class="badge ${map[status] || 'badge-cancelled'}">${status}</span>`;
}

function formatTimeSpan(ts) {
    if (!ts) return '-';
    const parts = ts.split(':');
    const date = new Date();
    date.setHours(parseInt(parts[0]), parseInt(parts[1]));
    return date.toLocaleTimeString([], { hour: 'numeric', minute: '2-digit' });
}

function openBookingModal() {
    clearAppointmentForm();
    setVal('AppointmentID', '0');
    document.getElementById('appointmentModalTitle').innerText = 'New appointment';
    document.getElementById('appointmentSaveBtn').innerText = 'Book appointment';
    initDoctorAutocomplete();
    initPatientAutocomplete();
    initAppointmentDatePicker();
    new bootstrap.Modal(document.getElementById('appointmentModal')).show();
}

function openEditAppointmentModal(id) {
    $.get('/Appointments/GetById', { id }, function (response) {
        if (!response.success) {
            showError(response.message);
            return;
        }

        const appointment = response.data;
        clearAppointmentForm();
        setVal('AppointmentID', appointment.appointmentID);
        document.getElementById('appointmentModalTitle').innerText = 'Edit appointment';

        initDoctorAutocomplete();
        initPatientAutocomplete();
        initAppointmentDatePicker();

        const doctorOption = new Option(appointment.doctorName, appointment.doctorID, true, true);
        $('#DoctorAuto').append(doctorOption).trigger('change.select2');

        const patientOption = new Option(appointment.patientName, appointment.patientID, true, true);
        $('#PatientAuto').append(patientOption).trigger('change.select2');

        setVal('AppointmentDate', formatDateForInput(appointment.appointmentDate));

        refreshFreeSlots(appointment.startTime.substring(0, 5));

        new bootstrap.Modal(document.getElementById('appointmentModal')).show();
    }).fail(() => showError('Could not load appointment.'));
}

function clearAppointmentForm() {
    $('#DoctorAuto').empty().append('<option value=""></option>');
    $('#PatientAuto').empty().append('<option value=""></option>');
    setVal('AppointmentDate', '');
    $('#StartTime').empty().append('<option value="">Select doctor and date first</option>');
    clearValidationErrors('appointmentForm');
}

function initDoctorAutocomplete() {
    $('#DoctorAuto').select2({
        dropdownParent: $('#appointmentModal'),
        placeholder: 'Click to search doctors...',
        allowClear: true,
        minimumInputLength: 0,
        ajax: {
            url: '/Appointments/SearchDoctors',
            dataType: 'json',
            delay: 300,
            data: params => ({ term: params.term || '' }),
            processResults: response => ({
                results: (response.data || []).map(d => ({ id: d.id, text: d.text }))
            })
        }
    }).off('change').on('change', () => refreshFreeSlots());
}

function initPatientAutocomplete() {
    $('#PatientAuto').select2({
        dropdownParent: $('#appointmentModal'),
        placeholder: 'Click to search patients...',
        allowClear: true,
        minimumInputLength: 0,
        ajax: {
            url: '/Appointments/SearchPatients',
            dataType: 'json',
            delay: 300,
            data: params => ({ term: params.term || '' }),
            processResults: response => ({
                results: (response.data || []).map(p => ({ id: p.id, text: p.text }))
            })
        }
    });
}

function initAppointmentDatePicker() {
    if (typeof flatpickr !== 'undefined') {
        flatpickr('#AppointmentDate', {
            altInput: true,
            altFormat: 'd-m-Y',
            dateFormat: 'Y-m-d',
            allowInput: true,
            minDate: 'today',
            appendTo: document.getElementById('appointmentModal'),
            onChange: () => $('#AppointmentDate').trigger('change')
        });
        $('#AppointmentDate').off('change').on('change', () => refreshFreeSlots());
    }
}

function refreshFreeSlots(preselectValue) {
    const doctorId = $('#DoctorAuto').val();
    const date = getVal('AppointmentDate');
    const appointmentId = parseInt(getVal('AppointmentID')) || 0;
    const slotSelect = $('#StartTime');

    slotSelect.empty();
    if (!doctorId || !date) {
        slotSelect.append('<option value="">Select doctor and date first</option>');
        return;
    }

    slotSelect.append('<option value="">Loading...</option>');

    const params = { doctorId, date };
    if (appointmentId > 0) params.excludeAppointmentId = appointmentId;

    $.get('/Appointments/GetFreeSlots', params, function (response) {
        slotSelect.empty();

        if (!response.success) {
            slotSelect.append('<option value="">Unavailable</option>');
            return;
        }

        const slots = response.data || [];
        if (slots.length === 0) {
            slotSelect.append('<option value="">No free slots on this day</option>');
            return;
        }

        slotSelect.append('<option value="">Select a free slot</option>');
        slots.forEach(s => slotSelect.append(`<option value="${s.value}">${s.label}</option>`));

        if (preselectValue) {
            slotSelect.val(preselectValue);
        }
    }).fail(() => {
        slotSelect.empty();
        slotSelect.append('<option value="">Could not load slots</option>');
    });
}

function bookAppointment() {
    const form = $('#appointmentForm');
    if (!form.valid()) return;

    const appointmentId = parseInt(getVal('AppointmentID')) || 0;
    const url = appointmentId > 0 ? '/Appointments/Edit' : '/Appointments/Create';

    $.ajax({
        url,
        method: 'POST',
        headers: { RequestVerificationToken: getAntiForgeryToken() },
        data: form.serialize(),
        success: function (response) {
            if (!response.success) {
                showError(response.message);
                return;
            }
            bootstrap.Modal.getInstance(document.getElementById('appointmentModal'))?.hide();
            showSuccess(appointmentId > 0 ? 'Appointment updated.' : 'Appointment booked.');
            refreshAppointmentsTable();
        },
        error: () => showError('Could not save appointment.')
    });
}

function confirmCancelAppointment(id) {
    Swal.fire({
        title: 'Cancel this appointment?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Cancel appointment',
        cancelButtonText: 'Back'
    }).then(result => { if (result.isConfirmed) cancelAppointment(id); });
}

function cancelAppointment(id) {
    $.ajax({
        url: '/Appointments/Cancel',
        method: 'POST',
        data: { id },
        headers: { RequestVerificationToken: getAntiForgeryToken() },
        success: function (response) {
            if (!response.success) { showError(response.message); return; }
            showSuccess('Appointment cancelled.');
            refreshAppointmentsTable();
        },
        error: () => showError('Could not cancel appointment.')
    });
}

function confirmDeleteAppointment(id) {
    Swal.fire({
        title: 'Delete this appointment?',
        text: 'This cannot be undone.',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Delete',
        cancelButtonText: 'Cancel'
    }).then(result => { if (result.isConfirmed) deleteAppointment(id); });
}

function deleteAppointment(id) {
    $.ajax({
        url: '/Appointments/Delete',
        method: 'POST',
        data: { id },
        headers: { RequestVerificationToken: getAntiForgeryToken() },
        success: function (response) {
            if (!response.success) { showError(response.message); return; }
            showSuccess('Appointment deleted.');
            refreshAppointmentsTable();
        },
        error: () => showError('Could not delete appointment.')
    });
}
