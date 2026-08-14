// Patient grid (Patients/Index) + the shared Add/Edit patient modal
// (also used from the Appointments booking modal via the "+" button)

let patientsTable = null;

document.addEventListener('DOMContentLoaded', function () {
    initPatientModalDatePicker();

    if (document.getElementById('patientsTable')) {
        initPatientsTable();
    }

    initFormValidation('patientForm', {
        Name: { required: true, maxlength: 100 },
        BirthDate: { required: true },
        Gender: { required: true },
        PhoneNumber: { required: true, maxlength: 20 },
        Address: { maxlength: 200 }
    }, {
        Name: { required: 'Patient name is required.' },
        BirthDate: { required: 'Birth date is required.' },
        Gender: { required: 'Gender is required.' },
        PhoneNumber: { required: 'Phone number is required.' }
    });
});

function initPatientModalDatePicker() {
    if (typeof flatpickr !== 'undefined') {
        flatpickr('#PBirthDate', { dateFormat: 'd-m-Y', allowInput: true, maxDate: 'today' });
    }
}

// GRID

function initPatientsTable() {
    patientsTable = $('#patientsTable').DataTable({
        ajax: { url: '/Patients/GetAll', dataSrc: 'data' },
        columns: [
            { data: 'name' },
            { data: 'birthDate', render: d => d ? new Date(d).toLocaleDateString() : '-' },
            { data: 'gender' },
            { data: 'phoneNumber' },
            { data: 'address' },
            {
                data: 'patientID',
                orderable: false,
                className: 'text-end',
                render: id => `
                    <i class="ti ti-edit action-icon me-3" onclick="openEditPatientModal(${id})" title="Edit"></i>
                    <i class="ti ti-trash action-icon danger" onclick="confirmDeletePatient(${id})" title="Delete"></i>`
            }
        ]
    });
}

function refreshPatientsTable() {
    patientsTable?.ajax.reload(null, false);
}

// ADD / EDIT (modal is shared — lives in _Layout via _PatientModal partial)

function openAddPatientModal() {
    clearPatientForm();
    document.getElementById('patientModalTitle').innerText = 'Add patient';
    new bootstrap.Modal(document.getElementById('patientModal')).show();
}

function openEditPatientModal(id) {
    $.get('/Patients/GetById', { id }, function (response) {
        if (!response.success) {
            showError(response.message);
            return;
        }

        const patient = response.data;
        setVal('PatientID', patient.patientID);
        setVal('PName', patient.name);
        setVal('PBirthDate', formatDateForInput(patient.birthDate));
        setVal('PGender', patient.gender);
        setVal('PPhoneNumber', patient.phoneNumber);
        setVal('PAddress', patient.address);

        document.getElementById('patientModalTitle').innerText = 'Edit patient';
        new bootstrap.Modal(document.getElementById('patientModal')).show();
    }).fail(() => showError('Could not load patient.'));
}

function clearPatientForm() {
    setVal('PatientID', '0');
    setVal('PName', '');
    setVal('PBirthDate', '');
    setVal('PGender', '');
    setVal('PPhoneNumber', '');
    setVal('PAddress', '');
    clearValidationErrors('patientForm');
}

function savePatient() {
    if (!$('#patientForm').valid()) return;

    const patientId = parseInt(getVal('PatientID')) || 0;
    const dto = {
        patientID: patientId,
        name: getVal('PName'),
        birthDate: parseDateFromInput(getVal('PBirthDate')),
        gender: getVal('PGender'),
        phoneNumber: getVal('PPhoneNumber'),
        address: getVal('PAddress')
    };

    const url = patientId > 0 ? '/Patients/Edit' : '/Patients/Add';

    $.ajax({
        url,
        method: 'POST',
        contentType: 'application/json',
        headers: { RequestVerificationToken: getAntiForgeryToken() },
        data: JSON.stringify(dto),
        success: function (response) {
            if (!response.success) {
                showError(response.message);
                return;
            }

            bootstrap.Modal.getInstance(document.getElementById('patientModal'))?.hide();
            showSuccess('Patient saved.');

            // Refresh the Patients grid if it's on this page
            if (patientsTable) refreshPatientsTable();

            // If we're inside the Appointments booking modal, select the new patient
            const patientAuto = $('#PatientAuto');
            if (patientId === 0 && patientAuto.length) {
                const newPatient = response.data;
                const option = new Option(newPatient.name, newPatient.patientID, true, true);
                patientAuto.append(option).trigger('change');
            }
        },
        error: () => showError('Could not save patient.')
    });
}

// DELETE

function confirmDeletePatient(id) {
    Swal.fire({
        title: 'Delete this patient?',
        text: 'This cannot be undone.',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Delete',
        cancelButtonText: 'Cancel'
    }).then(result => {
        if (result.isConfirmed) deletePatient(id);
    });
}

function deletePatient(id) {
    $.ajax({
        url: '/Patients/Delete',
        method: 'POST',
        data: { id },
        headers: { RequestVerificationToken: getAntiForgeryToken() },
        success: function (response) {
            if (!response.success) {
                showError(response.message);
                return;
            }
            showSuccess('Patient deleted.');
            refreshPatientsTable();
        },
        error: () => showError('Could not delete patient.')
    });
}
