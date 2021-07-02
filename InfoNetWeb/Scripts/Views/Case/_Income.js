$(document).ready(function () {
    //Provider = DV
    if (!$("#IsUnknownIncomeSourceSelected").is(":checked") && !$("#IsNoneIncomeSourceSelected").is(":checked")) {
        $('.financialResource').each(function () {
            var thisElementCheckbox = $(this).closest('.currentResource').find('.incomeSource ');
            if ($(this).val() == "" && !thisElementCheckbox.is(':checked')) {
                $(this).prop('disabled', true);
            }
        });
    }

    //On Load
    //On error from serverside enable the field
    $('.form-group.currentResource.has-error').each(function () {
        var $checkBox = $(this).find('input.incomeSource');
        $checkBox.prop("checked", "checked");
        $checkBox.change();
    });

    //Unknown Selected
    if ($("#IsUnknownIncomeSourceSelected").is(":checked")) {
        $('.financialResource').prop('disabled', true);
        $('.incomeSource').prop('checked', false);
        $('.incomeSource').prop('disabled', true);
        $('#IsNoneIncomeSourceSelected').prop('disabled', true);
    }

    //No Financial Resource selected
    if ($("#IsNoneIncomeSourceSelected").is(":checked")) {
        $('.financialResource').prop('disabled', true);
        $('.incomeSource').prop('checked', false);
        $('.incomeSource').prop('disabled', true);
        $('#IsUnknownIncomeSourceSelected').prop('disabled', true);
    }

    //On Change
    //Unknown Selected
    $("#IsUnknownIncomeSourceSelected").change(function () {
        if ($(this).is(":checked")) {
            $('.financialResource').prop('disabled', true);
            $('.incomeSource').prop('checked', false);
            $('.incomeSource').prop('disabled', true);
            $('#IsNoneIncomeSourceSelected').prop('checked', false); //.prop('disabled', true);
        }
        else if ($(this).not(":checked")) {
            $('.incomeSource').prop('disabled', false);
            $('#IsNoneIncomeSourceSelected').prop('disabled', false);
        }
    });

    //No Financial Resource selected
    $("#IsNoneIncomeSourceSelected").change(function () {
        if ($(this).is(":checked")) {
            $('.financialResource').prop('disabled', true);
            $('.incomeSource').prop('checked', false);
            $('.incomeSource').prop('disabled', true);
            $('#IsUnknownIncomeSourceSelected').prop('checked', false); //.prop('disabled', true);
        }
        else if ($(this).not(":checked")) {
            $('.incomeSource').prop('disabled', false);
            $('#IsUnknownIncomeSourceSelected').prop('disabled', false);
        }
    });

    //On Load
    //If there is amount in any of the fields disable the checkboxes
    $(".financialResource").each(function () {
        if ($(this).val() != "") {
            // $(this).closest('.currentResource').find('label').prop('class', 'icjia-required');
            $('#IsUnknownIncomeSourceSelected').prop('disabled', true);
            $('#IsNoneIncomeSourceSelected').prop('disabled', true);
        }
    });

    //Provider = SA
    //On Load
    if ($("#ClientIncome_OtherIncome").is(':checked')) {
        $('#ClientIncome_WhatOther').removeAttr('readonly');
    }
});

$("#ClientIncome_OtherIncome").on('click', function () {
    if ($(this).is(':checked')) 
        $('#ClientIncome_WhatOther').removeAttr('readonly');
    else {
        $('#ClientIncome_WhatOther').val('');
        $('#ClientIncome_WhatOther').attr('readonly', 'readonly');
    }
});

$(".financialResource").blur(function () {
    if ($(this).val() != "" && $(this).val() != -1 && !isNaN($(this).val()))
        fixDollarAmountFormat('#' + $(this).prop('id'));
});

    //On change
    $(".incomeSource").on('change', function (e) {
        var $this = $(this);
        var thisIncome = $this.closest('.currentResource').find('.financialResource');
        if ($this.is(':checked')) {
            thisIncome.prop('disabled', false);
            thisIncome.attr('aria-required', 'true');
            thisIncome.attr('data-val-required', 'Income category has been selected without an amount.  Enter a monthly amount of -1 if the amount is unknown.');
            $('#IsUnknownIncomeSourceSelected').prop('disabled', true);
            $('#IsNoneIncomeSourceSelected').prop('disabled', true);

            rescanUnobtrusiveValidation('#main');
        } else {
            thisIncome.removeAttr('data-val-required');
            thisIncome.removeAttr('aria-required');
            thisIncome.prop('disabled', true);
            thisIncome.val('');
            var input = 0;
            $(".incomeSource").each(function () {
                if ($(this).is(':checked')) {
                    input = input + 1;
                }
            });
            if (input > 0) {
                $('#IsUnknownIncomeSourceSelected').prop('disabled', true);
                $('#IsNoneIncomeSourceSelected').prop('disabled', true);
            }
            else {
                $('#IsUnknownIncomeSourceSelected').prop('disabled', false);
                $('#IsNoneIncomeSourceSelected').prop('disabled', false);
            }

            rescanUnobtrusiveValidation('#main');
            thisIncome.valid();
        }
    });

    function fixDollarAmountFormat(currentIncome) {
        var value = parseFloat($(currentIncome).val());
        $(currentIncome).val(value.toFixed(2));
    }