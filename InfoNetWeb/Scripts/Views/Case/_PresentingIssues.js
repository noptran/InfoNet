$(document).ready(function () {
    sexualPassiveIsChecked = $('#PresentingIssues_IsSexualOtherPassive').is(':checked');
    sexualActiveIsChecked = $('#PresentingIssues_IsSexualOtherActive').is(':checked');
    physicalIsChecked = $('#PresentingIssues_IsPhysicalOther').is(':checked');
    if (sexualActiveIsChecked || sexualPassiveIsChecked) {
        $('#PresentingIssues_SexualComment').removeAttr('readonly');
    }
    if (physicalIsChecked) {
        $('#PresentingIssues_PhysicalComment').removeAttr('readonly');
    }

    $('textarea').each(function () {
        updateCountdown($(this));
    });
});

$('#presentingIssues').on('change keyup', 'textarea', function () {
    updateCountdown($(this));
});


$(document).on('change', 'select[name="PresentingIssues.StateID"]', function () {
    var $stateID = $(this);
    var $countyID = $(this).closest('div.row').find('select[name$="CountyID"]');
    updateStateCounty($stateID);
    setTimeout(
      function () {
          updateCityDropdown($stateID, $countyID);
          updateTownships($stateID, $countyID);
      }, 0.1);
});

$(document).on('change', 'select[name="PresentingIssues.CountyID"]', function () {
    var $countyID = $(this);
    var $stateID = $(this).closest('div.row').find('select[name$="StateID"]');
    updateCityDropdown($stateID, $countyID);
    updateTownships($stateID, $countyID);
});

function updateCountdown($this) {
	var $numOfCharactersPlaceHolder = $this.closest('div').find('#pi_charRemaining');
	var $numOfCharactersText = $this.closest('div').find('#pi_charRemainingText');
    // 256 is the max message length
    var remaining = 256 - jQuery($this).val().length;
    if (remaining < 0) {
    	$numOfCharactersText.removeClass('text-info').addClass('text-danger');
    }
    else {
    	$numOfCharactersText.removeClass('text-danger').addClass('text-info');
    }
    $numOfCharactersPlaceHolder.text(remaining);
}

function updateCityDropdown($state, $county) {
    if ($state.val() != "") {
        var cityList = $state.closest('div.row').find('select[name$="CityID"]');
        var stateID = $state.val();
        var countyID = $county.val();
        $.ajax({
            url: "/USPS/ListCitiesByStateAndCounty?stateID=" + stateID + "&countyID=" + countyID,
            type: "GET",
            data: "json",
            success: function (data) {
                //console.log(data);
                $(cityList).find('option').remove().end();
                $(cityList).append($('<option>').text("<Pick One>").attr('value', ""));
                $.each(data, function (i, county) {
                    $(cityList).append($('<option>').text(data[i].CityName).attr('value', data[i].ID));
                });
            },
            error: function (xhr, ajaxSettings, thrownError) {
                //console.log(xhr.responseText);
            }
        });
    }
}

function updateStateCounty($this) {
    if ($this.val() != "") {
        var $countyList = $this.closest('div.row').find('select[name$="CountyID"]');
        var stateID = $this.val();
        $.ajax({
            url: "/USPS/ListCountiesByState?stateID=" + stateID,
            type: "GET",
            data: "json",
            success: function (data) {
                // console.log(data);
                $countyList.find('option').remove().end();
                $countyList.append($('<option>').text("<Pick One>").attr('value', ""));
                $.each(data, function (i, county) {
                    $countyList.append($('<option>').text(data[i].Name).attr('value', data[i].ID));
                });
            },
            error: function (xhr, ajaxSettings, thrownError) {
                //console.log(xhr.responseText);
            }
        });
    }
}

function updateTownships($state, $county) {
    //if the state is IL find townships
    if ($state.val() != "") {
        var $townshipList = $state.closest('div.row').find('select[name$="TownshipID"]');
        var stateID = $state.val();
        $.ajax({
            url: "/USPS/ListTownships?stateID=" + stateID + "&countyID=" + $county.val(),
            type: "GET",
            data: "json",
            success: function (data) {
                // console.log(data);
                $townshipList.find('option').remove().end();
                $townshipList.append($('<option>').text("<Pick One>").attr('value', ""));
                $.each(data, function (i, county) {
                    $townshipList.append($('<option>').text(data[i].TownshipName).attr('value', data[i].ID));
                });
            },
            error: function (xhr, ajaxSettings, thrownError) {
                //console.log(xhr.responseText);
            }
        });
    }
}

$(document).on('click', 'input[name^="PresentingIssues.IsSexualOther"]', function () {
    var passiveIsChecked = $('#PresentingIssues_IsSexualOtherPassive').is(':checked');
    var activeIsChecked = $('#PresentingIssues_IsSexualOtherActive').is(':checked');
    if (activeIsChecked || passiveIsChecked) {
        $('#PresentingIssues_SexualComment').removeAttr('readonly');
    } else if (!passiveIsChecked && !passiveIsChecked) {
        $('#PresentingIssues_SexualComment').val("");
         $('#PresentingIssues_SexualComment').attr('readonly','readonly');
    }
});

$(document).on('click', 'input[name^="PresentingIssues.IsPhysicalOther"]', function () {
    if ($(this).is(':checked')) {
        $('#PresentingIssues_PhysicalComment').removeAttr('readonly');
    } else {
        $('#PresentingIssues_PhysicalComment').val("");
        $('#PresentingIssues_PhysicalComment').attr('readonly', 'readonly');
    }
});