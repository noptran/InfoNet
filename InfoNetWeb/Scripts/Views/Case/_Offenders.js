//KMS DO validate state county combinations?
//KMS DO submit with invalid Sentence...expand, scroll, and focus don't go far enough
//KMS DO check focus on toggling delete/restore buttons for Sentences


// Offender add button clicks
$(document).on('click', '[data-icjia-role="offender.add"]', function () {
    var $this = $(this);
    var $window = $(window);
    var $scrollspy = $('.icjia-scrollspy');

    var params = $this.data('icjia-mustache-next');
    var $result = $this.find('[data-icjia-role="mustache"]').mustache(params).insertBefore('#main > div:last-child');
    var $link = $('<li><a href="#' + $result.attr('id') + '">Someone</a></li>').appendTo('.icjia-nav-stacked > li.active > .nav');
    params.key++;

    rescanUnobtrusiveValidation('#main');
    rescanTooltipsForSR($result.parent());
    $('#main').addClass('icjia-make-dirty').trigger('dirty.dirtyforms'); /* new record cannot be removed from page */

    //animate
    var buttonBottomToBottom = $this.get(0).getBoundingClientRect().bottom - $window.innerHeight();
    var scrollToBottom = $window.scrollTop() + buttonBottomToBottom + 20 /* 20 px margin */;
    var scrollToTop = $result.offset().top;
    var newScrollTop = scrollToTop < scrollToBottom ? scrollToTop : scrollToBottom;
    if (newScrollTop < 0)
        newScrollTop = 0;
    $scrollspy.removeClass('on');
    $result.hide().slideDown();
    $link.hide().slideThenFadeIn(resizeNavStacked);
    $('html, body').animate({ scrollTop: newScrollTop }).promise().then(function () {
        $result.find(':text:enabled:not([readonly])').first().focus();
        $scrollspy.addClass('on');
        refreshScrollspy();
    });
});


// Offender delete button clicks
$(document).on('click', '[data-icjia-role="offender.delete"]', function () {
    var $panel = $(this).closest('.panel');

    $panel.find('[data-icjia-role="offender.index"]').each(function () {
        var sign = this.value.substring(0, 1) == '=' ? '-' : '~';
        this.value = sign + this.value.substring(1);
        $(this).change();
    });
    $panel.children('.panel-collapse').collapse('hide');
    window.setTimeout(function () {
        $panel.addClass('deleted');
        var bookmarkId = $panel.closest('.icjia-bookmark').attr('id');
        if (bookmarkId != undefined)
            $('.icjia-nav-stacked a[href="#' + bookmarkId + '"]').parent('li').addClass('deleted');
        $panel.find('[data-icjia-role="panel.expand"]').addClass('hide');
        $panel.find('[data-icjia-role="panel.restore"]').removeClass('hide');
    }, 350); /* rotate-90-if-collapsed transition ms */
});


// Offender restore button clicks
$(document).on('click', '[data-icjia-role="panel.restore"]', function (evt) {
    var $panel = $(this).closest('.panel');

    $panel.find('[data-icjia-role="offender.index"]').each(function () {
        var sign = this.value.substring(0, 1) == '-' ? '=' : '+';
        this.value = sign + this.value.substring(1);
        $(this).change();
    });
    $panel.removeClass('deleted');
    var bookmarkId = $panel.closest('.icjia-bookmark').attr('id');
    if (bookmarkId != undefined)
        $('.icjia-nav-stacked a[href="#' + bookmarkId + '"]').parent('li').removeClass('deleted');
    var $expandButton = $panel.find('[data-icjia-role="panel.expand"]').removeClass('hide');
    $panel.find('[data-icjia-role="panel.restore"]').addClass('hide');
    $expandButton.focus();
    $panel.children('.panel-collapse').collapse('show');
});


// disable panel collapse when Offender has been deleted
$(document).on('show.bs.collapse', '.panel-collapse:has([data-icjia-role="offender.index"]:regex(value, ^[-~]))', function (event) {
    event.preventDefault();
});


// resize side nav when relationship changes (in case description wraps)
$(document).on('change', 'select[id ^= "OffendersById_"][id $= "_RelationshipToClientId"]', function () {
    resizeNavStacked();
});


// Police and Trial Charge add button clicks
$(document).on('click', ':regex(data-icjia-role, ^offender\\.(police|trial)charge\\.add$)', function () {
    var $this = $(this);
    var params = $this.data('icjia-mustache-next');
    var $result = $this.find('[data-icjia-role="mustache"]').mustache(params).appendTo($this.closest('table').children('tbody'));
    params.key++;

    rescanUnobtrusiveValidation('#main');
    rescanTooltipsForSR($result.parent());
    $('#main').addClass('icjia-make-dirty').trigger('dirty.dirtyforms'); /* new record cannot be removed from page */

    $result.find('.well.collapse').collapse('show');
    window.setTimeout(function () {
        $result.find('select').first().focus();
    }, 350); /* rotate-90-if-collapsed transition ms */
});


// Police and Trial Charge delete button clicks
$(document).on('click', ':regex(data-icjia-role, ^offender\\.(police|trial)charge\\.delete$)', function () {
    var $primaryRow = $(this).closest('tr').prev();

    $primaryRow.find(':regex(data-icjia-role, ^offender\\.(police|trial)charge\\.index$)').each(function () {
        var sign = this.value.substring(0, 1) == '=' ? '-' : '~';
        this.value = sign + this.value.substring(1);
        $(this).change();
    });
    $(this).closest('.collapse').collapse('hide');
    window.setTimeout(function () {
        $primaryRow.addClass('deleted');
        $primaryRow.find(':regex(data-icjia-role, ^offender\\.(police|trial)charge\\.expand$)').addClass('hide');
        $primaryRow.find(':regex(data-icjia-role, ^offender\\.(police|trial)charge\\.restore$)').removeClass('hide');
    }, 350); /* rotate-90-if-collapsed transition ms */
});


// Police and Trial Charge restore button clicks
$(document).on('click', ':regex(data-icjia-role, ^offender\\.(police|trial)charge\\.restore$)', function () {
    var $primaryRow = $(this).closest('tr');

    $primaryRow.find(':regex(data-icjia-role, ^offender\\.(police|trial)charge\\.index$)').each(function () {
        var sign = this.value.substring(0, 1) == '-' ? '=' : '+';
        this.value = sign + this.value.substring(1);
        $(this).change();
    });
    $primaryRow.removeClass('deleted');
    var $expandButton = $primaryRow.find(':regex(data-icjia-role, ^offender\\.(police|trial)charge\\.expand$)').removeClass('hide');
    $primaryRow.find(':regex(data-icjia-role, ^offender\\.(police|trial)charge\\.restore$)').addClass('hide');
    $primaryRow.next().find('.collapse').collapse('show');
    $expandButton.focus();
});


// disable well collapse when Police or Trial Charge has been deleted
$(document).on('show.bs.collapse', 'tr:has([data-icjia-role^="offender."][data-icjia-role$="charge.index"]:regex(value, ^[-~])) + tr .collapse', function (event) {
    event.preventDefault();
});


// Sentence add button clicks
$(document).on('click', '[data-icjia-role="offender.trialcharge.sentence.add"]', function () {
    var $this = $(this);
    var params = $this.data('icjia-mustache-next');
    var $result = $this.find('[data-icjia-role="mustache"]').mustache(params).appendTo($this.closest('table').children('tbody'));
    params.key++;

    rescanUnobtrusiveValidation('#main');
    rescanTooltipsForSR($result.parent());
    $('#main').addClass('icjia-make-dirty').trigger('dirty.dirtyforms'); /* new record cannot be removed from page */
    $result.find(':input').first().focus();
});


// Sentence delete button clicks
$(document).on('click', '[data-icjia-role="offender.trialcharge.sentence.delete"]', function () {
    var $primaryRow = $(this).closest('tr');

    $primaryRow.find('[data-icjia-role="offender.trialcharge.sentence.index"]').each(function () {
        var sign = this.value.substring(0, 1) == '=' ? '-' : '~';
        this.value = sign + this.value.substring(1);
        $(this).change();
    });

    $primaryRow.addClass('hide');
    $primaryRow.next().removeClass('hide');
    //$restoreButton.focus();
});


// Sentence restore button clicks
$(document).on('click', '[data-icjia-role="offender.trialcharge.sentence.restore"]', function () {
    var $primaryRow = $(this).closest('tr').prev();

    $primaryRow.find('[data-icjia-role="offender.trialcharge.sentence.index"]').each(function () {
        var sign = this.value.substring(0, 1) == '-' ? '=' : '+';
        this.value = sign + this.value.substring(1);
        $(this).change();
    });

    $primaryRow.removeClass('hide');
    $primaryRow.next().addClass('hide');
    //$deleteButton.focus();
});


// refresh sentence count on add, delete, and restore clicks
$(document).on('click', ':regex(data-icjia-role, ^offender\\.trialcharge\\.sentence\\.(add|delete|restore))', function () {
    var $sentencesTable = $(this).closest('table');
    $sentencesTable.closest('tr').prev().find('[data-icjia-role="offender.trialcharge.sentence.count"]').text($sentencesTable.find('[data-icjia-role="offender.trialcharge.sentence.index"]:regex(value, ^[+=])').length);
});


// initialize sentence counts on load
$(function () {
    $('[data-icjia-role="offender.trialcharge.sentence.count"]').each(function () {
        var $this = $(this);
        $this.text($this.closest('tr').next().find('[data-icjia-role="offender.trialcharge.sentence.index"]:regex(value, ^[+=])').length);
    });
});


// delete confirmation dialog for Offenders, Police Charges, Trial Charges, and Sentences
(function () {

    var pluralize = function (noun, count) { return noun + (count > 1 ? 's' : ''); };
    var deletesConfirmed = false;

    $(document).on('submit-valid.icjia', '#main', function (e) {
        if (deletesConfirmed) {
            deletesConfirmed = false;
            return;
        }

        var $main = $(this);

        var $offendersDeleted = $main.find('.panel:has([data-icjia-role="offender.index"][value^="-"])');
        var $offendersRetained = $main.find('.panel:has([data-icjia-role="offender.index"][value^="="])');
        var offendersDeleted = $offendersDeleted.length;
        var deletedOffenderPoliceCharges = $offendersDeleted.find('[data-icjia-role="offender.policecharge.index"]:regex(value, ^[\\-=])').length;
        var deletedOffenderTrialCharges = $offendersDeleted.find('[data-icjia-role="offender.trialcharge.index"]:regex(value, ^[\\-=])').length;
        var deletedOffenderSentences = $offendersDeleted.find('[data-icjia-role="offender.trialcharge.sentence.index"]:regex(value, ^[\\-=])').length;

        var policeChargesDeleted = $offendersRetained.find('[data-icjia-role="offender.policecharge.index"][value^="-"]').length;

        var $trialChargesDeleted = $offendersRetained.find('tr:has([data-icjia-role="offender.trialcharge.index"][value^="-"]) + tr');
        var $trialChargesRetained = $offendersRetained.find('tr:has([data-icjia-role="offender.trialcharge.index"][value^="="]) + tr');
        var trialChargesDeleted = $trialChargesDeleted.length;
        var deletedTrialChargeSentences = $trialChargesDeleted.find('[data-icjia-role="offender.trialcharge.sentence.index"]:regex(value, ^[\\-=])').length;

        var sentencesDeleted = $trialChargesRetained.find('[data-icjia-role="offender.trialcharge.sentence.index"][value^="-"]').length;

        var total = offendersDeleted + policeChargesDeleted + trialChargesDeleted + sentencesDeleted;
        if (total == 0)
            return;

        var buffer = '';
        if (sentencesDeleted > 0)
            buffer += '<li>' + sentencesDeleted + pluralize(' Sentence', sentencesDeleted) + '</li>';
        if (policeChargesDeleted > 0)
            buffer += '<li>' + policeChargesDeleted + pluralize(' Police Charge', policeChargesDeleted) + '</li>';
        if (trialChargesDeleted > 0) {
            buffer += '<li>' + trialChargesDeleted + pluralize(' Trial Charge', trialChargesDeleted);
            if (deletedTrialChargeSentences > 0)
                buffer += ' <span style="font-style: italic; font-weight: normal">(including ' + deletedTrialChargeSentences + pluralize(' Sentence', deletedTrialChargeSentences) + ')</span>';
            buffer += '</li>';
        }
        if (offendersDeleted > 0) {
            buffer += '<li>' + offendersDeleted + pluralize(' Offender', offendersDeleted);
            if (deletedOffenderPoliceCharges + deletedOffenderTrialCharges > 0) {
                buffer += ' <span style="font-style: italic; font-weight: normal">(including ';
                if (deletedOffenderPoliceCharges > 0)
                    buffer += deletedOffenderPoliceCharges + pluralize(' Police Charge', deletedOffenderPoliceCharges);
                if (deletedOffenderPoliceCharges > 0 && deletedOffenderTrialCharges > 0)
                    buffer += deletedOffenderSentences > 0 ? ', ' : ' and ';
                if (deletedOffenderTrialCharges > 0)
                    buffer += deletedOffenderTrialCharges + pluralize(' Trial Charge', deletedOffenderTrialCharges);
                if (deletedOffenderSentences > 0)
                    buffer += ' and ' + deletedOffenderSentences + pluralize(' Sentence', deletedOffenderSentences);
                buffer += ')</span>';
            }
            buffer += '</li>';
        }

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

})();


// refresh counties of residence when state changes
$(document).on('change', 'select[name$=".StateId"]', function () {
    var stateId = $(this).val();
    if (stateId == "")
        return;

    var $countySelect = $('#' + $(this).attr('id').replace('StateId', 'CountyId').replace(':', '\\:'));
    $.ajax({
        url: "/USPS/ListCountiesByState?stateID=" + stateId,
        type: "GET",
        data: "json",
        success: function (data) {
            $countySelect.prop('disabled', data.length == 0);
            $countySelect.find('option').remove();
            $countySelect.append($('<option/>').text('<Pick One>').attr('value', ''));
            $.each(data, function (i, county) {
                $countySelect.append($('<option/>').text(county.Name).attr('value', county.ID));
            });
        },
        error: function (xhr, ajaxSettings, thrownError) {
            //console.log(xhr.responseText);
        }
    });
});


$('#searchExistingOffenders').click(function () {
    $('#offenderResultCount').html(0);
    $('#offenderSearchTotal').html(0);
    $('.offenders > tbody').empty();
    SearchOffenders();
});

$('#searchOffender').click(function () {
    $('#searchExistingOffenders').click();
});

$('#loadMore').click(function () {
    SearchOffenders();
});

$(document).on('click', '.AddOffender', function () {
    var $this = $(this);
    var $primaryRow = $this.closest('tr');

    var offenderListingId = $primaryRow.find('input').val();
    var offenderCode = $primaryRow.find('td.offenderCode').text();
    var genderIdentity = $primaryRow.find('input.genderIdentityId').val();
    var raceId = $primaryRow.find('input.raceId').val();
    var age = $primaryRow.find('td.age').text();

    $('[data-icjia-role="offender.add"]').click();
    var $addedPanel = $('div.panel').last();

    $addedPanel.find('input[name$="OffenderListingId"]').val(offenderListingId);
    $addedPanel.find('input[name$="OffenderCode"]').val(offenderCode).prop('readonly', true);
    $addedPanel.find('input[name$="Age"]').val(age);
    $addedPanel.find('select[name$="SexId"]').val(genderIdentity);
    $addedPanel.find('select[name$="RaceId"]').val(raceId);

    $('#searchOffenderModal').modal('toggle');
});

function SearchOffenders() {
    var skip = 0;
    if (parseInt($('#offenderResultCount').html()) != parseInt($('#offenderSearchTotal').html()))
        skip = parseInt($('#offenderResultCount').html());

    $.ajax({
        type: 'GET',
        url: "/Offender/GetOffenders",
        data: {
            // FirstContactDate: $('#FirstContactDate').first().val(),
            OffenderCode: $("#OffenderCode").val(),
            AgeFrom: $("#AgeFrom").val(),
            AgeTo: $("#AgeTo").val(),
            GenderIdentityId: $("#SexId").val(),
            RaceId: $("#RaceId").val(),
            skip: skip,
            ExistingOffenders: getAllExistingOffenders()
        },
        dataType: 'json',
        beforeSend: function () {
            $("#searchExistingOffenders").addClass('icjia-spinner-active');
        },
        success: function (data) {
            //console.log(data);
            var total = 0;
            var len = parseInt(data.length);
            var resultCount = parseInt($('#offenderResultCount').html()) + len;
            $('#offenderResultCount').html(resultCount);
            var txt = "";
            if (len > 0) {
                total = data[0].total;
                $('.noResultsAlert').hide();
                $('#offenderResultCount').html(resultCount);
                $('#offenderSearchTotal').html(total);
                $('#icjia-results').show();
                for (var i = 0; i < len; i++) {
                    if (data[i].Age < 0)
                        data[i].Age = -1;
                    txt += "<tr class=\"offender\">"
                                    + "<input type=\"hidden\" value=" + data[i].OffenderListingId + "\>"
                                    + "<input class=\"genderIdentityId\" type=\"hidden\" value=" + data[i].GenderIdentityId + "\>"
                                    + "<input class=\"raceId\" type=\"hidden\" value=" + data[i].RaceId + "\>"
                                    + "<td class=\"offenderCode\">" + data[i].OffenderCode + "</td>"
                                    + "<td>" + data[i].GenderIdentity + "</td>"
                                    + "<td>" + data[i].Race + "</td>"
                                    + "<td class=\"age\">" + data[i].Age + "</td>"
                                    + "<td><button type=\"button\" class=\"btn btn-xs btn-primary btn-outline AddOffender\">Select Offender</button></td>"
                            + "</tr>";
                }
                $(".offenders > tbody").append(txt);
                $(".offenders").show();
                $("#loadMore").removeClass("hidden");
                $(".searchResults").height("300px");
            }
            else {
                $('.noResultsAlert').show();
                $('#offenderSearchTotal').html(len);
            }
            hideShowLoadButton((resultCount >= total));
        },
        error: function (xhr) {
            //console.log(xhr.responseText);
        },
        complete: function () {
            $("#searchExistingOffenders").removeClass('icjia-spinner-active');
        }
    });
}

function getAllExistingOffenders() {
    var alreadyExists = {};
    var existingOffenderListingIds = $('input[name$="OffenderListingId"]');
    existingOffenderListingIds.each(function (i) {
        alreadyExists[i] = $(this).val();
    });
    return alreadyExists;
}

function hideShowLoadButton(allShown) {
    $("#loadMore").toggle(!allShown);
}

$(document).on('focus', '[data-icjia-role="charge"]', function () {
    var groupCount = 0;
    $(this).find('option').each(function (i) {


        if ($(this).text().toUpperCase().match("^A") && !$(this).prev('option').text().toUpperCase().match("^A")) { groupCount++; }
        $(this).css('background-color', changeColor($('#provider').val(), groupCount));
    });
});

function changeColor(provider, groupNumber) {
	if (provider == 'DV' || provider == 'SA' || provider == 'CAC') {
		if (groupNumber == 1)
			return '#e4b9b9';
		if (groupNumber == 2)
			return '#afd9ee';
		if (groupNumber == 3)
			return '#f7f4d3';
	}
	return 'white';
}