
let patientsTable = null;
let patientPhoneIti = null;

document.addEventListener('DOMContentLoaded', function () {
    patientPhoneIti = initPhoneInput('PPhoneNumber');
    initPatientModalDatePicker();

    if (document.getElementById('patientsTable')) {
        initPatientsTable();
        initGridSearch('patientSearchInput', () => patientsTable);
    }
});

function initPatientModalDatePicker() {
    if (typeof flatpickr !== 'undefined') {
        flatpickr('#PBirthDate', { dateFormat: 'd-m-Y', allowInput: true, maxDate: 'today' });
    }
}

function initPatientsTable() {
    patientsTable = initSimpleDataTable('patientsTable', {
        ajax: { url: '/Patients/GetAll' },
        columns: [
            { data: 'name' },
            { data: 'birthDate', render: d => d ? new Date(d).toLocaleDateString() : '-' },
            { data: 'gender' },
            { data: 'phoneNumber' },
            { data: 'address' },
            {
                data: 'patientID',
                orderable: false,
                render: id => `
                    <span class="action-btn action-btn-delete" onclick="confirmDeletePatient(${id})" title="Delete">
                        <i data-feather="trash-2"></i>
                    </span>
                    <span class="action-btn action-btn-edit" onclick="openEditPatientModal(${id})" title="Edit">
                        <i data-feather="edit"></i>
                    </span>`
            }
        ]
    });
}

function refreshPatientsTable() {
    patientsTable?.ajax.reload(null, false);
}

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
        patientPhoneIti ? patientPhoneIti.setNumber(patient.phoneNumber || '') : setVal('PPhoneNumber', patient.phoneNumber);
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
    patientPhoneIti ? patientPhoneIti.setNumber('') : setVal('PPhoneNumber', '');
    setVal('PAddress', '');
    clearValidationErrors('patientForm');
}

function savePatient() {
    const form = $('#patientForm');
    if (!form.valid()) return;

    if (patientPhoneIti) {
        setVal('PPhoneNumber', patientPhoneIti.getNumber());
    }

    const patientId = parseInt(getVal('PatientID')) || 0;
    const url = patientId > 0 ? '/Patients/Edit' : '/Patients/Add';

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

            bootstrap.Modal.getInstance(document.getElementById('patientModal'))?.hide();
            showSuccess('Patient saved.');

            if (patientsTable) refreshPatientsTable();

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
