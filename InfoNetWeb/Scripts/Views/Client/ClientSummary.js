$(document).ready(function () {
	$('#saveButton').click(function () {
		if ($('#ClientCode').hasClass('dirty')) {
			$.confirm({
				title: "Are you sure you want to do that?",
				text: "You are about to edit the Client ID. This change connot be undone and will affect all cases for this client. Are you sure want to proceed?",
				confirm: function () {
					$('#main').submit();
				}
			});
		}
		else {
			$('#main').submit();
		}
	});

	$('#upgradeType').click(function () {
		$.confirm({
			title: "Are you sure you want to do that?",
			text: "You are about to upgrade this client to a Child Victim. This change connot be undone and will affect all cases for this client. Are you sure want to proceed?",
			confirm: function () {
				$('#ClientTypeId').find('option:selected').removeAttr('selected');
				var text = $('#ClientTypeId').find('option[value="7"]').attr('selected', 'selected').text();
				$('#ClientTypeId').val('7');
				$('#ClientType_6__Description').val(text);
				$('#ClientType_6__Description').change();
				$('#upgradeType').hide();
			}
		});
	});

	$('#newCaseLink').click(function (e) {
		e.preventDefault();
		if ($('#NoDatesAvail').val() == "True") {
			$.confirm({
				dialogClass: "modal-dialog icjia-modal-warning",
				title: "Unable to add a new case",
				text: "There are no valid First Contact Dates available.",
				confirmButton: 'OK',
				confirmButtonClass: 'btn btn-warning',
				cancelButton: null
			});
		}
		else if ($('#warningMessage').length) {
			$.confirm({
				title: "Are you sure you want to do that?",
				text: $('#warningMessage').html(),
				confirm: function () {
					window.location.href = "/Case/EditRedirect?clientId=" + $('#ClientId').val();
				}
			});
		}
		else {
			window.location.href = "/Case/EditRedirect?clientId=" + $('#ClientId').val();
		}
	});
});