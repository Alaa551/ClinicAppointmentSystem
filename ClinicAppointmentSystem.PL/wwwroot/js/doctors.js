// Doctor grid (Doctors/Index) + the shared Add/Edit doctor modal
// (also used from the Appointments booking modal via the "+" button)

let doctorsTable = null;
let doctorPhoneIti = null;

document.addEventListener('DOMContentLoaded', function () {
    doctorPhoneIti = initPhoneInput('DoctorPhoneNumber');

    if (document.getElementById('doctorsTable')) {
        initDoctorsTable();
    }
});

// GRID

function initDoctorsTable() {
    doctorsTable = initSimpleDataTable('doctorsTable', {
        ajax: { url: '/Doctors/GetAll', dataSrc: 'data' },
        columns: [
            { data: 'name' },
            { data: 'specialization' },
            { data: 'phoneNumber' },
            { data: 'email' },
            {
                data: 'isActive',
                render: active => active
                    ? '<span class="badge badge-active">Active</span>'
                    : '<span class="badge badge-inactive">Inactive</span>'
            },
            {
                data: 'doctorID',
                orderable: false,
                className: 'text-end',
                render: id => `
                    <i class="ti ti-edit action-icon me-3" onclick="openEditDoctorModal(${id})" title="Edit"></i>
                    <i class="ti ti-trash action-icon danger" onclick="confirmDeleteDoctor(${id})" title="Delete"></i>`
            }
        ]
    });
}

function refreshDoctorsTable() {
    doctorsTable?.ajax.reload(null, false);
}

// ADD / EDIT (modal is shared — lives in _Layout via _DoctorModal partial)

function openAddDoctorModal() {
    clearDoctorForm();
    document.getElementById('doctorModalTitle').innerText = 'Add doctor';
    new bootstrap.Modal(document.getElementById('doctorModal')).show();
}

function openEditDoctorModal(id) {
    $.get('/Doctors/GetById', { id }, function (response) {
        if (!response.success) {
            showError(response.message);
            return;
        }

        const doctor = response.data;
        setVal('DoctorID', doctor.doctorID);
        setVal('Name', doctor.name);
        setVal('Specialization', doctor.specialization);
        doctorPhoneIti ? doctorPhoneIti.setNumber(doctor.phoneNumber || '') : setVal('DoctorPhoneNumber', doctor.phoneNumber);
        setVal('Email', doctor.email);
        $('#IsActive').prop('checked', doctor.isActive);

        document.getElementById('doctorModalTitle').innerText = 'Edit doctor';
        new bootstrap.Modal(document.getElementById('doctorModal')).show();
    }).fail(() => showError('Could not load doctor.'));
}

function clearDoctorForm() {
    setVal('DoctorID', '0');
    setVal('Name', '');
    setVal('Specialization', '');
    doctorPhoneIti ? doctorPhoneIti.setNumber('') : setVal('DoctorPhoneNumber', '');
    setVal('Email', '');
    $('#IsActive').prop('checked', true);
    clearValidationErrors('doctorForm');
}

function saveDoctor() {
    const form = $('#doctorForm');
    if (!form.valid()) return;

    if (doctorPhoneIti) {
        setVal('DoctorPhoneNumber', doctorPhoneIti.getNumber());
    }

    const doctorId = parseInt(getVal('DoctorID')) || 0;
    const url = doctorId > 0 ? '/Doctors/Edit' : '/Doctors/Add';

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

            bootstrap.Modal.getInstance(document.getElementById('doctorModal'))?.hide();
            showSuccess('Doctor saved.');

            if (doctorsTable) refreshDoctorsTable();

            const doctorAuto = $('#DoctorAuto');
            if (doctorId === 0 && doctorAuto.length) {
                const newDoctor = response.data;
                const option = new Option(newDoctor.name, newDoctor.doctorID, true, true);
                doctorAuto.append(option).trigger('change');
            }
        },
        error: () => showError('Could not save doctor.')
    });
}

// DELETE

function confirmDeleteDoctor(id) {
    Swal.fire({
        title: 'Delete this doctor?',
        text: 'This cannot be undone.',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Delete',
        cancelButtonText: 'Cancel'
    }).then(result => {
        if (result.isConfirmed) deleteDoctor(id);
    });
}

function deleteDoctor(id) {
    $.ajax({
        url: '/Doctors/Delete',
        method: 'POST',
        data: { id },
        headers: { RequestVerificationToken: getAntiForgeryToken() },
        success: function (response) {
            if (!response.success) {
                showError(response.message);
                return;
            }
            showSuccess('Doctor deleted.');
            refreshDoctorsTable();
        },
        error: () => showError('Could not delete doctor.')
    });
}
