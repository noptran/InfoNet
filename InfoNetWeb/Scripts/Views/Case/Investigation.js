$.validator.setDefaults({ ignore: ":hidden:not(select)" });

// delete confirmation dialog for Allegations, Petitions, Team, Interview and Medical Visits
(function () {

	var pluralize = function (noun, count) { return noun + (count > 1 ? 's' : ''); }

	var deletesConfirmed = false;

	$(document).on('submit-valid.icjia', '#main', function (e) {
		if (deletesConfirmed) {
			deletesConfirmed = false;
			return;
		}

		var $main = $(this);

		var allegationsDeleted = $main.find('[data-icjia-role="allegation.index"][value^="-"]').length;
		var petitionsDeleted = $main.find('[data-icjia-role="petition.index"][value^="-"]').length;
		var teamMembersDeleted = $main.find('[data-icjia-role="mdteam.index"][value^="-"]').length;
		
		var $interviewsDeleted = $main.find('.well:has([data-icjia-role="interview.index"][value^="-"])');
		var $interviewsRetained = $main.find('.well:has([data-icjia-role="interview.index"][value^="="])');
		var interviewsDeleted = $interviewsDeleted.length;
		var deletedInterviewObservers = $interviewsDeleted.find('[data-icjia-role="interview.observer.index"]:regex(value, ^[\\-=])').length;
		var observersDeleted = $interviewsRetained.find('[data-icjia-role="interview.observer.index"][value^="-"]').length;

		var $medicalVisitsDeleted = $main.find('.panel:has([data-icjia-role="medicalvisit.index"][value^="-"])');
		var medicalVisitsDeleted = $medicalVisitsDeleted.length;

		var total = allegationsDeleted + petitionsDeleted + teamMembersDeleted + interviewsDeleted + observersDeleted + medicalVisitsDeleted;
		if (total == 0)
			return;

		var buffer = '';

		if (allegationsDeleted > 0)
			buffer += '<li>' + allegationsDeleted + pluralize(' Allegation', allegationsDeleted) + '</li>';
		if (petitionsDeleted > 0)
			buffer += '<li>' + petitionsDeleted + pluralize(' Petition', petitionsDeleted) + '</li>';
		if (teamMembersDeleted > 0)
			buffer += '<li>' + teamMembersDeleted + pluralize(' Team Member', teamMembersDeleted) + '</li>';

		if (observersDeleted > 0)
			buffer += '<li>' + observersDeleted + pluralize(' Observer', observersDeleted) + '</li>';
		if (interviewsDeleted > 0) {
			buffer += '<li>' + interviewsDeleted + pluralize(' Interview', interviewsDeleted);
			if (deletedInterviewObservers > 0) {
				buffer += ' <span style="font-style: italic; font-weight: normal">(including ';
				if (deletedInterviewObservers > 0)
					buffer += deletedInterviewObservers + pluralize(' Observer', deletedInterviewObservers);
				buffer += ')</span>';
			}
			buffer += '</li>';
		}

		if (medicalVisitsDeleted > 0)
			buffer += '<li>' + medicalVisitsDeleted + pluralize(' Medical Visit', medicalVisitsDeleted) + '</li>';


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