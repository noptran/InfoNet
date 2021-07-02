$(document).ready(function () {
    
    SetNumberOfAppearances();

    if ($('select[name$="OrderOfProtectionId"]').val() != "")
        $('select[name$=".OrderTypeId"]').attr('disabled', false);
    else
        $('select[name$=".OrderTypeId"]').attr('disabled', true);


    if ($('select[name$="CivilNoContactOrderId"]').val() != "") {
        $('select[name$="CivilNoContactOrderTypeId"]').attr('disabled', false);
        $('select[name$="CivilNoContactOrderRequestId"]').attr('disabled', false);
    }
    else {
        $('select[name$="CivilNoContactOrderTypeId"]').attr('disabled', true);
        $('select[name$="CivilNoContactOrderRequestId"]').attr('disabled', true);
    }

    // Police/Prosecution court appearance add button 
    $(document).on('click', '[data-icjia-role="courtAppearance.add"]', function () {
        var $this = $(this);
        var params = $this.data('icjia-mustache-next');
        var $result = $this.find('[data-icjia-role="mustache"]').mustache(params).appendTo($this.closest('table').children('tbody'));
        params.key++;

        rescanUnobtrusiveValidation('#main');
        rescanTooltipsForSR($result.parent());

        $('#main').addClass('icjia-make-dirty').trigger('dirty.dirtyforms'); /* new record cannot be removed from page */

       // SetNumberOfAppearances();
        $result.find('.well.collapse').collapse('show');
        window.setTimeout(function () {
            $result.find('input').first().focus();
        }, 350); /* rotate-90-if-collapsed transition ms */
    });

    //  Police/Prosecution court appearance delete button 
    $(document).on('click', '[data-icjia-role="courtAppearance.delete"]', function () {
        var $primaryRow = $(this).closest('tr');

        $primaryRow.find('[data-icjia-role="courtAppearance.index"]').each(function () {
            var sign = this.value.substring(0, 1) == '=' ? '-' : '~';
            this.value = sign + this.value.substring(1);
            $(this).change();
        });
        $primaryRow.addClass('hidden');
        $primaryRow.next().removeClass('hidden');
       // SetNumberOfAppearances();
    });

    //  Police/Prosecution court appearance restore
    $(document).on('click', '[data-icjia-role="courtAppearance.restore"]', function () {
        var $primaryRow = $(this).closest('tr');

        $primaryRow.prev().find('[data-icjia-role="courtAppearance.index"]').each(function () {
            var sign = this.value.substring(0, 1) == '-' ? '=' : '+';
            this.value = sign + this.value.substring(1);
            $(this).change();
        });
        $primaryRow.addClass('hidden');
        $primaryRow.prev().removeClass('hidden');
        //SetNumberOfAppearances();
    });

    $(document).on('change', 'select[name$="CourtContinuanceID"]', function () {
       // SetNumberOfAppearances();
    });
});

$(document).on('change', 'select[name$="OrderOfProtectionId"]', function () {
    if ($(this).val() != "")
        $('select[name$=".OrderTypeId"]').attr('disabled', false);
    else {
        $('select[name$=".OrderTypeId"]').attr('disabled', true);
        $('[name$=".OrderTypeId"]').val("");
    }
});

$(document).on('change', 'select[name$="CivilNoContactOrderId"]', function () {
    if ($(this).val() != ""){
        $('select[name$="CivilNoContactOrderTypeId"]').attr('disabled', false);
        $('select[name$="CivilNoContactOrderRequestId"]').attr('disabled', false);
    }
    else {
        $('select[name$="CivilNoContactOrderTypeId"]').attr('disabled', true).val("").removeClass('input-validation-error').closest('div').removeClass('has-error').find('span').html('');
        $('select[name$="CivilNoContactOrderRequestId"]').attr('disabled', true).val("").removeClass('input-validation-error').closest('div').removeClass('has-error').find('span').html('');
    }
});

function SetNumberOfAppearances() {
    var numberOfDefense = $('select[id$="CourtContinuanceID"]').not('[disabled]').find('option[value="1"]:selected').length;
    var numberOfProsecution = $('select[id$="CourtContinuanceID"]').not('[disabled]').find('option[value="2"]:selected').length;
    var numberOfNoContinuances = $('select[id$="CourtContinuanceID"]').not('[disabled]').find('option[value="3"]:selected').length;   
    var numberOfCourtApr = $('select[id$="CourtContinuanceID"]').not('[disabled]').length;
    $('#numbOfDefCont').text(numberOfDefense);
    $('#numbOfPrscCont').text(numberOfProsecution);
    $('#numbOfCtApprs').text(numberOfCourtApr);
    $('#numbOfNoCont').text(numberOfNoContinuances);
}

//delete confirmation dialog for court appearances in SA
(function () {
    /*Confirmation for delete only on SA, because the confirmation for court appearances 
    in DV is combined with orders of protection and located in _OrdersOfProtection.js
    (SA does not have orders of protection)*/

    var isDV = $('div[data-target="#ordersCollapse"]').length > 0;


    if (!isDV) {

        var pluralize = function (noun, count) {
            return noun + (count > 1 ? 's' : '');
        }
        var deletesConfirmed = false;

        $(document).on('submit-valid.icjia', '#main', function (e) {
            if (deletesConfirmed) {
                deletesConfirmed = false;
                return;
            }

            var $main = $(this);

            var $courtAppearanceDeleted = $main.find('#tbl_courtAppearance_appearances tr:has([data-icjia-role="courtAppearance.index"][value^="-"])');
            var courtAppearancesDeleted = $courtAppearanceDeleted.length;

            if (courtAppearancesDeleted == 0)
                return;

            var buffer = '';
            if (courtAppearancesDeleted > 0)
                buffer += '<li>' + courtAppearancesDeleted + pluralize(' Court Appearance', courtAppearancesDeleted) + '</li>';

            e.preventDefault();
            $.confirm({
                text: "You've marked the following for deletion: <ul class='text-danger' style='font-weight: bold; list-style: square; margin-top: 10px;'>" + buffer + "</ul> If you continue, these records will be <span style='font-weight: bold'>permanently deleted</span>.",
                confirmButtonClass: "btn-danger",
                dialogClass: "modal-dialog icjia-modal-danger",
                confirm: function () {
                    deletesConfirmed = true;
                    $main.submit();
                }
            });
        });
    }
})();