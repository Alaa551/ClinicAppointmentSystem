// Appointment grid + the booking modal (Appointments/Index)
// Doctor/Patient add modals live in doctors.js / patients.js (shared, global)

let appointmentsTable = null;

document.addEventListener('DOMContentLoaded', function () {
    initAppointmentsTable();
});

// GRID

function initAppointmentsTable() {
    appointmentsTable = initSimpleDataTable('appointmentsTable', {
        ajax: { url: '/Appointments/GetAll', dataSrc: 'data' },
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
                className: 'text-end',
                render: (id, type, row) => `
                    ${row.status === 'Booked'
                        ? `<i class="ti ti-circle-x action-icon me-3" onclick="confirmCancelAppointment(${id})" title="Cancel"></i>`
                        : ''}
                    <i class="ti ti-trash action-icon danger" onclick="confirmDeleteAppointment(${id})" title="Delete"></i>`
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

// BOOKING MODAL

function openBookingModal() {
    clearAppointmentForm();
    initDoctorAutocomplete();
    initPatientAutocomplete();
    initAppointmentDatePicker();
    new bootstrap.Modal(document.getElementById('appointmentModal')).show();
}

function clearAppointmentForm() {
    $('#DoctorAuto').empty().append('<option value=""></option>');
    $('#PatientAuto').empty().append('<option value=""></option>');
    setVal('AppointmentDate', '');
    $('#StartTime').empty().append('<option value="">Select doctor and date first</option>');
    clearValidationErrors('appointmentForm');
}

// Autocomplete (Select2, remote AJAX). minimumInputLength: 0 means opening
// the dropdown by clicking it — with no text typed — immediately searches
// with an empty term, so the top matches show right away; typing re-queries.
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
    }).off('change').on('change', refreshFreeSlots);
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
            dateFormat: 'd-m-Y',
            allowInput: true,
            minDate: 'today',
            appendTo: document.getElementById('appointmentModal'),
            onChange: () => $('#AppointmentDate').trigger('change')
        });
        $('#AppointmentDate').off('change').on('change', refreshFreeSlots);
    }
}

// FREE SLOTS (depends on doctor + date both being selected)

function refreshFreeSlots() {
    const doctorId = $('#DoctorAuto').val();
    const date = parseDateFromInput(getVal('AppointmentDate'));
    const slotSelect = $('#StartTime');

    slotSelect.empty();

    if (!doctorId || !date) {
        slotSelect.append('<option value="">Select doctor and date first</option>');
        return;
    }

    slotSelect.append('<option value="">Loading...</option>');

    $.get('/Appointments/GetFreeSlots', { doctorId, date }, function (response) {
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
    }).fail(() => {
        slotSelect.empty();
        slotSelect.append('<option value="">Could not load slots</option>');
    });
}

// BOOK

function bookAppointment() {
    const form = $('#appointmentForm');
    if (!form.valid()) return;

    $.ajax({
        url: '/Appointments/Create',
        method: 'POST',
        headers: { RequestVerificationToken: getAntiForgeryToken() },
        data: form.serialize(),
        success: function (response) {
            if (!response.success) {
                showError(response.message);
                return;
            }
            bootstrap.Modal.getInstance(document.getElementById('appointmentModal'))?.hide();
            showSuccess('Appointment booked.');
            refreshAppointmentsTable();
        },
        error: () => showError('Could not book appointment.')
    });
}

// CANCEL / DELETE

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
