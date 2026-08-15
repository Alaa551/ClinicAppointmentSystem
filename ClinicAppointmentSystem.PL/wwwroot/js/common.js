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

function refreshIcons() {
    if (typeof feather !== 'undefined') feather.replace();
}

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

function initDatePicker(selector, options) {
    if (typeof flatpickr === 'undefined') return null;

    return flatpickr(selector, Object.assign({
        altInput: true,
        altFormat: 'd-m-Y',
        dateFormat: 'Y-m-d',
        allowInput: true
    }, options));
}

function initAutocomplete(selector, url, options) {
    return $(selector).select2(Object.assign({
        allowClear: true,
        minimumInputLength: 0,
        ajax: {
            url: url,
            dataType: 'json',
            delay: 300,
            data: params => ({ term: params.term || '' }),
            processResults: response => ({
                results: (response.data || []).map(x => ({ id: x.id, text: x.text }))
            })
        }
    }, options));
}

function initSimpleDataTable(tableId, options) {
    const callerDrawCallback = options.drawCallback;
    const callerInitComplete = options.initComplete;
    const ajaxUrl = options.ajax.url;

    const merged = Object.assign({
        serverSide: true,
        processing: true,
        bFilter: true,
        sDom: 'lfrtip',
        ordering: false,
        pageLength: 10,
        language: {
            info: '_START_ - _END_ of _TOTAL_ items',
            emptyTable: 'No records found',
            paginate: {
                previous: '<i data-feather="chevron-left"></i>',
                next: '<i data-feather="chevron-right"></i>'
            }
        }
    }, options);

    merged.ajax = function (requestData, callback) {
        const pageNumber = Math.floor(requestData.start / requestData.length) + 1;
        const pageSize = requestData.length;
        const search = requestData.search.value;

        $.get(ajaxUrl, { pageNumber, pageSize, search }, function (response) {
            callback({
                draw: requestData.draw,
                recordsTotal: response.totalCount,
                recordsFiltered: response.totalCount,
                data: response.items
            });
        });
    };

    merged.drawCallback = function () {
        refreshIcons();
        if (callerDrawCallback) callerDrawCallback.apply(this, arguments);
    };

    merged.initComplete = function () {
        const wrapper = $(this).closest('.dataTables_wrapper');
        if (!wrapper.find('.dt-footer-row').length) {
            wrapper.find('.dataTables_info')
                .add(wrapper.find('.dataTables_paginate'))
                .wrapAll('<div class="dt-footer-row"></div>');
        }
        if (callerInitComplete) callerInitComplete.apply(this, arguments);
    };

    return $('#' + tableId).DataTable(merged);
}

function initGridSearch(inputId, getTable) {
    const input = $('#' + inputId);
    let debounceTimer;

    input.on('input', function () {
        clearTimeout(debounceTimer);
        const value = this.value;
        debounceTimer = setTimeout(() => {
            const table = getTable();
            if (table) table.search(value).draw();
        }, 250);
    });
}

function getPhoneNumberValue(iti, phoneInput) {
    if (!iti) return phoneInput.value;
    const digits = phoneInput.value.replace(/\D/g, '');
    if (!digits) return '';
    const dialCode = iti.getSelectedCountryData().dialCode;
    return '+' + dialCode + digits;
}
