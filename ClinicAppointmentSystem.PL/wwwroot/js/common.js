// Shared helpers used across doctors.js / patients.js / appointments-*.js

function getAntiForgeryToken() {
    const el = document.querySelector('input[name="__RequestVerificationToken"]');
    return el ? el.value : '';
}

function setVal(id, v) {
    const $el = $('#' + id);
    if ($el.length) $el.val(v).trigger('change');
}

function getVal(id) {
    return ($('#' + id).val() || '').trim();
}

function clearValidationErrors(formId) {
    $('#' + formId).find('[data-valmsg-for]').text('');
}

// jQuery Validation setup — errors render into the existing
// <span data-valmsg-for="FieldName"> spans instead of a popup/alert.
function initFormValidation(formId, rules, messages) {
    $('#' + formId).validate({
        ignore: [],                // don't skip hidden inputs (Select2 hides its <select>)
        rules: rules,
        messages: messages,
        errorElement: 'span',
        errorPlacement: function (error, element) {
            const fieldName = element.attr('name');
            $(`[data-valmsg-for="${fieldName}"]`).text(error.text());
        },
        highlight: function (element) {
            $(element).addClass('is-invalid');
        },
        unhighlight: function (element) {
            $(element).removeClass('is-invalid');
        },
        success: function (label, element) {
            const fieldName = $(element).attr('name');
            $(`[data-valmsg-for="${fieldName}"]`).text('');
        }
    });
}

function showSuccess(msg) {
    if (typeof Swal === 'undefined') { alert(msg); return; }
    const toast = Swal.mixin({ toast: true, position: 'top-end', showConfirmButton: false, timer: 2500 });
    toast.fire({ icon: 'success', title: msg });
}

function showError(msg) {
    if (typeof Swal === 'undefined') { alert(msg); return; }
    const toast = Swal.mixin({ toast: true, position: 'top-end', showConfirmButton: false, timer: 3500 });
    toast.fire({ icon: 'error', title: msg || 'Something went wrong.' });
}

// Date helpers: flatpickr inputs display "dd-mm-yyyy",
// but the server expects ISO ("yyyy-mm-dd").

function parseDateFromInput(displayValue) {
    if (!displayValue) return null;
    const parts = displayValue.split('-');
    if (parts.length !== 3) return null;
    const [day, month, year] = parts;
    return `${year}-${month.padStart(2, '0')}-${day.padStart(2, '0')}`;
}

function formatDateForInput(isoValue) {
    if (!isoValue) return '';
    const date = new Date(isoValue);
    const day = String(date.getDate()).padStart(2, '0');
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const year = date.getFullYear();
    return `${day}-${month}-${year}`;
}
