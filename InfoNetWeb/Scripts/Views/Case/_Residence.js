stateID = "";
countyID = "";

$(document).ready(function () {
	checkUnknownOrNotReported();
    setTypeaheads(document);
	disableZipcodeIfUnknownOrNotReported();

	// On State Changed
	$('#tbl_residences').on('change', 'select[name$="StateID"]', function () {
		if ($(this).val() != "") {
			var $parent = $(this).closest('.well');
			var countyList = $parent.find('select[name$="CountyID"]');
			var stateID = $(this).val();
			$.ajax({
				url: "/USPS/ListCountiesByState?stateID=" + stateID,
				type: "GET",
				data: "json",
				success: function (data) {
					$(countyList).find('option').remove().end();
					$(countyList).append($('<option>').text("<Pick One>").attr('value', ""));
					$.each(data, function (i, county) {
						//console.log('County Name: ' + data[i].Name + ', ID: ' + data[i].ID);
						$(countyList).append($('<option>').text(data[i].Name).attr('value', data[i].ID));
					});
					countyList.change();
					clearAddressFields($parent);
				},
				error: function (xhr, ajaxSettings, thrownError) {
					//console.log(xhr.responseText);
				}
			});
		}
	});


	// On County Changed
	$('#tbl_residences').on('change', 'select[name$="CountyID"]', function () {
		var $parent = $(this).closest('.well');
		clearAddressFields($parent);
	});

	// Zipcode - Unkown changed
	$('#tbl_residences').on('change', '.res_cb_unknown', function () {
		var $this = $(this);
		var name = null;
		var input = $this.parent().parent().find('input[id$="Zipcode"]');
		var notReported = $this.parent().parent().find('.res_cb_notreported');
		if (this.checked) {
			// Get name from input or other checkbox
			(notReported.is(':checked')) ? name = notReported.attr('name') : name = input.attr('name');
			input.removeAttr('name').prop('disabled', true).typeahead('val', '');
			notReported.removeAttr('name').prop('checked', false);
			$this.attr('name', name);
			$('#tbl_residences').find($this.attr('data-icjia-broadcast-change')).val('Unknown');
		}
		else {
			if (!notReported.is(':checked')) {
				name = $this.attr('name');
				input.attr('name', name);
				input.prop('disabled', false);
				$this.removeAttr('name');
			}
		}
	});

	// Zipcode - Not Reported changed
	$('#tbl_residences').on('change', '.res_cb_notreported', function () {
		var $this = $(this);
		var name = null;
		var input = $this.parent().parent().find('input[id$="Zipcode"]');
		var unknown = $this.parent().parent().find('.res_cb_unknown');
		if (this.checked) {
			// Get name from input or other checkbox
			(unknown.is(':checked'))? name = unknown.attr('name') : name = input.attr('name');
			input.removeAttr('name').prop('disabled', true).typeahead('val', '');
			unknown.removeAttr('name').prop('checked', false);
			$this.attr('name', name);
			$('#tbl_residences').find($this.attr('data-icjia-broadcast-change')).val('Not Reported');
		}
		else {
			if (!unknown.is(':checked')) {
				name = $this.attr('name');
				input.attr('name', name);
				input.prop('disabled', false);
				$this.removeAttr('name');
			}
		}
	});


	// Onfocus event to set global variable of StateID & CountyID
	$('#tbl_residences').on('focus', 'input[data-icjia-role$="typeahead"]', function () {
		stateID = $(this).closest('.well').find('select[name$="StateID"]').val();
		countyID = $(this).closest('.well').find('select[name$="CountyID"]').val();
    });

    $('#tbl_residences').on('input', 'input[data-icjia-role="residence.zipcode.typeahead"]', function () {
        if ($(this).hasClass('input-validation-error')) {
            $(this).removeClass('input-validation-error');
            $(this).closest('.form-group').removeClass('has-error');
            $(this).closest('.input-group').next().removeClass('field-validation-error').html('');
        }
        if (!$(this).closest('.twitter-typeahead').length) {
            setZipCodeTypeahead($(this).closest('.form-group'));
            $(this).focus();
        }
    });
});

function clearAddressFields($parent) {
	$parent.find('input[id$="Zipcode"]').typeahead('val', '');
	$parent.find('input[id$="CityOrTown"]').typeahead('val', '');
	$parent.find('input[id$="Township"]').typeahead('val', '');
	$parent.find('.res_cb_unknown').prop('checked', false).change();
	$parent.find('.res_cb_notreported').prop('checked', false).change();
	$parent.find('input[id$="Zipcode"]').val('').change();
	$parent.find('input[id$="CityOrTown"]').val('').change();
	$parent.find('input[id$="Township"]').val('').change();
}

function checkUnknownOrNotReported() {
	$('input[id$="Zipcode"]').each(function () {
		var value = $(this).val();
		if (value < 0 && value > -3) {
			$(this).val('');
			$(this).attr('value', '');
			var $parent = $(this).closest('.form-group');
			var name = $(this).attr('name');
			$(this).removeAttr('name');
			if (value == -1) {
				$parent.find('.res_cb_unknown').prop('checked', true).attr('name', name);
			}else if (value == -2) {
				$parent.find('.res_cb_notreported').prop('checked', true).attr('name', name);
			}
		}
	});
}

function disableZipcodeIfUnknownOrNotReported() {
	$('input[id$="Zipcode"]').each(function () {
		var $parent = $(this).closest('.form-group');
		if ($parent.find('.res_cb_unknown').is(':checked') || $parent.find('.res_cb_notreported').is(':checked')) {
			$(this).prop('disabled', true);
		}
	});
}

function setTypeaheads(element) {
	setTownshipTypeahead(element);
	setCityOrTownTypeahead(element);
	setZipCodeTypeahead(element);
}

function setTownshipTypeahead(element) {
    $(element).find('input[data-icjia-role="residence.township.typeahead"]').typeahead({
        hint: true,
        limit: 8,
        minLength: 1
    },
	{
		name: 'Name',
		display: function (item) { return item.Name; },
		source: function (query, syncResults, asyncResults) {
			countyID = (countyID == "") ? 0 : countyID;
			$.ajax('/USPS/SearchTownshipName?input=' + query + '&StateID=' + stateID + '&CountyID=' + countyID,
			{
				dataType: 'json',
                success: function (data) { asyncResults(data); },
                error: function (xhr) { }
			});
		},
		templates: {
			empty: ['<div class="empty-message tt-suggestion">No Results</div>'],
			suggestion: function (value) { return '<div>' + value.Name + '</div>'; }
		}
    }).on('typeahead:selected', function () { $(this).change(); });;
}

function setCityOrTownTypeahead(element) {
    $(element).find('input[data-icjia-role="residence.cityortown.typeahead"]').typeahead({
        hint: true,
        limit: 8,
        minLength: 1
    },
	{
		name: 'Name',
		display: function (item) { return item.Name; },
		source: function (query, syncResults, asyncResults) {
			countyID = (countyID == "") ? 0 : countyID;
			$.ajax('/USPS/SearchCityName?input=' + query + '&StateID=' + stateID + '&CountyID=' + countyID,
			{
				dataType: 'json',
                success: function (data) { asyncResults(data); },
                error: function (xhr) { }
			});
		},
		templates: {
			empty: ['<div class="empty-message tt-suggestion">No Results</div>'],
			suggestion: function (value) { return '<div>' + value.Name + '</div>'; }
		}
    }).on('typeahead:selected', function () { $(this).change(); });
}

$('input[data-icjia-role="residence.zipcode.typeahead"]').focusout(function () {
    if ($(this).val() == $('input[id="Hidden_' + $(this).attr("id") + '"][type="hidden"]').val())
        if ($(this).hasClass('input-validation-error')) return false;
});

$('input[name$=".MoveDate"]').focusout(function () {
    if ($(this).val() == $('input[id="Hidden_' + $(this).attr("id") + '"][type="hidden"]').val())
        if ($(this).hasClass('input-validation-error')) return false;
});

function setZipCodeTypeahead(element) {
    $(element).find('input[data-icjia-role="residence.zipcode.typeahead"]:not(.input-validation-error)').each(function () {
        var isInvalid = $(this).hasClass('input-validation-error');
        $(this).typeahead({ hint: true, limit: 8, minLength: 1 },
            {
                name: 'Name',
                display: function (item) { return item.Name; },
                source: function (query, syncResults, asyncResults) {
                    countyID = (countyID == "") ? 0 : countyID;
                    $.ajax('/USPS/SearchZip?input=' + query + '&StateID=' + stateID + '&CountyID=' + countyID,
                        {
                            dataType: 'json',
                            success: function (data) { asyncResults(data); },
                            error: function (xhr) { /*console.log(xhr.responseText);*/ }
                        })
                },
                templates: {
                    empty: ['<div class="empty-message tt-suggestion">No Results</div>'],
                    suggestion: function (value) { return '<div>' + value.Name + '</div>'; }
                }
            }).on('typeahead:selected', function () { $(this).change(); });
        if (isInvalid) {
            $(this).addClass('input-validation-error');
            $(this).closest('.form-group').addClass('has-error');
            $(this).closest('.input-group').next().addClass('field-validation-error').html('Invalid Zip Code for County');
        }
    });

}

// provide support for data-icjia-broadcast=<target$>
(function () {

    $(document).on('change keyup', 'input[data-icjia-broadcast]:not(:checkbox)', function () {
		broadcastText(this, $(this).data('icjia-broadcast'));
	});

	$(document).on('change keyup', 'input[data-icjia-broadcast-zip]', function () {
		broadcastTextZip(this, $(this).data('icjia-broadcast-zip'));
	});

	$(document).on('change blur', 'select[data-icjia-broadcast]:not([multiple])', function () {
		broadcastSelectOne(this, $(this).data('icjia-broadcast'));
	});

	$(function () {
        $('input[data-icjia-broadcast]:not(:checkbox)').each(function () {
			broadcastText(this, $(this).data('icjia-broadcast'));
		});
		$('input[data-icjia-broadcast-zip]').each(function () {
			broadcastTextZip(this, $(this).data('icjia-broadcast-zip'));
		});
		$('select[data-icjia-broadcast]:not([multiple])').each(function () {
			broadcastSelectOne(this, $(this).data('icjia-broadcast'));
		});
	});

})();

// Residence add button click
$(document).on('click', '[data-icjia-role="residence.add"]', function () {
	var $this = $(this);
	var params = $this.data('icjia-mustache-next');
	var $result = $this.find('[data-icjia-role="mustache"]').mustache(params).appendTo($this.closest('table').find('tbody'));
	params.key++;

	if ($('#tbl_residences tbody tr').length == 2) {
		$result.find('input[name$=".MoveDate"]').val($('input[name$="FirstContactDate"]').val());
	}
	else {
		$result.value = '';
		$result.find('input[name$=".MoveDate"]').val('');
	}
	$result.find('select[data-icjia-broadcast]:not([multiple])').each(function () {
		broadcastSelectOne(this, $(this).data('icjia-broadcast'));
	});
	setClientCaseIds($result);
	setTypeaheads($result);

	rescanUnobtrusiveValidation('#main');
	rescanTooltipsForSR($result.parent());
	$('#main').addClass('icjia-make-dirty').trigger('dirty.dirtyforms'); /* new record cannot be removed from page */
	$result.find('.well.collapse').collapse('show');
	window.setTimeout(function () {
		$result.find('input[name$=".MoveDate"]').focus();
	}, 350); /* rotate-90-if-collapsed transition ms */
});

// Residence delete button clicks
$(document).on('click', '[data-icjia-role="residence.delete"]', function () {
    var $primaryRow = $(this).closest('tr').prev();
    
	$primaryRow.find('[data-icjia-role="residence.index"]').each(function () {
		var sign = this.value.substring(0, 1) == '=' ? '-' : '~';
		this.value = sign + this.value.substring(1);
		$(this).change();
	});
	$(this).closest('.collapse').collapse('hide');
	window.setTimeout(function () {
		$primaryRow.addClass('deleted');
		$primaryRow.find('[data-icjia-role="residence.expand"]').addClass('hide');
		$primaryRow.find('[data-icjia-role="residence.restore"]').removeClass('hide');
	}, 350); /* rotate-90-if-collapsed transition ms */
});

// Residence restore
$(document).on('click', '[data-icjia-role="residence.restore"]', function () {
	var $primaryRow = $(this).closest('tr');
	$primaryRow.removeClass('deleted');
	$primaryRow.find('[data-icjia-role="residence.index"]').each(function () {
		var sign = this.value.substring(0, 1) == '-' ? '=' : '+';
		this.value = sign + this.value.substring(1);
		$(this).change();
	});
	var $expandButton = $primaryRow.find('[data-icjia-role="residence.expand"]').removeClass('hide');
	$primaryRow.find('[data-icjia-role="residence.restore"]').addClass('hide');
	$primaryRow.next().find('.collapse').collapse('show');
	$expandButton.focus();
});

// disable well collapse when Residence has been deleted
$(document).on('show.bs.collapse', 'tr:has([data-icjia-role="residence.index"]:regex(value, ^[-~])) + tr .collapse', function (event) {
	event.preventDefault();
});

function setClientCaseIds($element) {
	$element.find('input[name$=".ClientID"]').val($('#clientId').val());
	$element.find('input[name$=".CaseID"]').val($('#caseId').val());
}

function broadcastText(input, target$) {
	$(target$).text(input.value);
}

function broadcastTextZip(input, target$) {
	var $unknown = $(input).closest('tr').find('.res_cb_unknown');
	var $notReported = $(input).closest('tr').find('.res_cb_notreported');
	var $zipcode = $(input).closest('tr').find('input[id$="Zipcode"]');

	if ($unknown.is(':checked')) {
		$(target$).text('Unknown');
	}
	else if ($notReported.is(':checked')) {
		$(target$).text('Not Reported');
	}
	else {
		$(target$).text($zipcode.val());
	}

}

function broadcastSelectOne(select, target$) {
	if (select.selectedIndex == -1 || select.value == '')
		$(target$).text('');
	else
		$(target$).text(select.options[select.selectedIndex].text);
}

$(function () {
    jQuery.validator.addMethod('isresidencevalid', function (value, element, params) {
        var resp = $.ajax({
            url: "/Case/IsResidenceValid",
            type: "post",
            dataType: 'json',
            data: function () {
                return $("#main").serializeArray();
            }
        });

        return resp;
    }, '');

    jQuery.validator.unobtrusive.adapters.add('isresidencevalid', function (options) {
        options.rules['isresidencevalid'] = {};
        options.messages['isresidencevalid'] = options.message;
    });

}(jQuery));

// delete confirmation dialog for Residence
(function () {

	var deletesConfirmed = false;

	$(document).on('submit-valid.icjia', '#main', function (e) {
		if (deletesConfirmed) {
			deletesConfirmed = false;
			return;
		}

		var $main = $(this);

		var residencesDeleted = $main.find('[data-icjia-role="residence.index"][value^="-"]').length;
		var total = residencesDeleted;
		if (total == 0)
			return;

		var message = "You have marked the following for deletion from the database: <ul>";

		// append Residence message
		message = (residencesDeleted > 0) ? message + "<li class='text-danger' style='font-weight:bold; list-style: square;'><b>" + residencesDeleted + " Residence" + ((residencesDeleted > 1) ? "s" : "") + "</b></li>" : "";
		message = message + "</ul>If you continue, their records will be <strong>permanently deleted</strong>.";
		e.preventDefault();

		//KMS DO default these somehow or somewhere?
		$.confirm({
			title: "Are you sure you want to do that?",
			text: message,
			confirmButtonClass: "btn-danger",
			dialogClass: "modal-dialog icjia-modal-danger",
			confirm: function () {
				deletesConfirmed = true;
				$main.submit();
			}
		});
	});
});