let doctorsTable = null;
let doctorPhoneIti = null;
let specializationsLoaded = false;

document.addEventListener('DOMContentLoaded', function () {
    doctorPhoneIti = initPhoneInput('DoctorPhoneNumber');
    loadSpecializations();

    if (document.getElementById('doctorsTable')) {
        initDoctorsTable();
        initGridSearch('doctorSearchInput', () => doctorsTable);
    }
});

function loadSpecializations() {
    $.get('/Doctors/GetSpecializations', function (response) {
        if (!response.success) return;

        const select = $('#DoctorSpecialization');
        select.find('option:not(:first)').remove();

        response.data.forEach(s => {
            select.append(`<option value="${s.id}">${s.name}</option>`);
        });

        specializationsLoaded = true;
    });
}

function initDoctorsTable() {
    doctorsTable = initSimpleDataTable('doctorsTable', {
        ajax: { url: '/Doctors/GetAll' },
        columns: [
            { data: 'name' },
            { data: 'specializationName' },
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
                render: id => `
                    <span class="action-btn action-btn-delete" onclick="confirmDeleteDoctor(${id})" title="Delete">
                        <i data-feather="trash-2"></i>
                    </span>
                    <span class="action-btn action-btn-edit" onclick="openEditDoctorModal(${id})" title="Edit">
                        <i data-feather="edit"></i>
                    </span>
                    <span class="action-btn action-btn-view" onclick="viewDoctor(${id})" title="View details">
                        <i data-feather="eye"></i>
                    </span>`
            }
        ]
    });
}

function refreshDoctorsTable() {
    doctorsTable?.ajax.reload(null, false);
}

function viewDoctor(id) {
    window.location.href = `/Doctors/Details/${id}`;
}

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
        setVal('DoctorSpecialization', doctor.specializationID);
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
    setVal('DoctorSpecialization', '');
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
