// Shared helpers used across doctors.js / patients.js / appointments.js

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

// Client-side validation now comes from jQuery Validation Unobtrusive,
// wired automatically off the data-val-* attributes that asp-for /
// asp-validation-for generate from each ViewModel's DataAnnotations.
// Call this only to clear stale server-side error text before a fresh submit.
function clearValidationErrors(formId) {
    $('#' + formId).find('[data-valmsg-for]').text('');
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

// Country-code phone input (intl-tel-input), matching the pattern used
// elsewhere in the system. Returns the iti instance so callers can read
// iti.getNumber() when saving.
function initPhoneInput(inputId) {
    const phoneInput = document.getElementById(inputId);
    if (!phoneInput || !window.intlTelInput) return null;

    const iti = window.intlTelInput(phoneInput, {
        initialCountry: 'eg',
        preferredCountries: ['eg', 'sa', 'ae', 'kw', 'qa'],
        separateDialCode: true,
        nationalMode: false,
        formatOnDisplay: false,
        autoPlaceholder: 'off',
        loadUtils: () => import('https://cdn.jsdelivr.net/npm/intl-tel-input@23.7.1/build/js/utils.js')
    });

    phoneInput.addEventListener('input', function () {
        this.value = this.value.replace(/\D/g, '');
    });

    return iti;
}

// Simple Bootstrap-styled DataTable init (client filtering only, no
// server paging) — matches the lightweight config used across the system.
function initSimpleDataTable(tableId, options) {
    return $('#' + tableId).DataTable(Object.assign({
        bFilter: true,
        sDom: 'fBtlpi',
        ordering: true,
        pageLength: 10,
        deferRender: true,
        language: {
            search: ' ',
            sLengthMenu: '_MENU_',
            searchPlaceholder: 'Search...',
            info: '_START_ - _END_ of _TOTAL_ items',
            emptyTable: 'No records found',
            paginate: {
                previous: '<i class="ti ti-chevron-left"></i>',
                next: '<i class="ti ti-chevron-right"></i>'
            }
        }
    }, options));
}
