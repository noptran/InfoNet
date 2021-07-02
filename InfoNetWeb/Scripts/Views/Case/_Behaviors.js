$(document).ready(function () {

    // Check if 'None Observed' is checked for In and Out, disable checkboxes if checked
    disableInCheckboxes();
    disableOutCheckboxes();

    // Enable/Disable all In checkboxes when 'None Observed In' changes
    $('#ChildBehavioralIssues_NoneObserved').change(function () {
        $('.childBehavior_in').prop('disabled', $(this).is(':checked'));
    });

    // Enable/Disable all In checkboxes when 'None Observed Out' changes
    $('#ChildBehavioralIssues_NoneObservedAtOuttake').change(function () {
        $('.childBehavior_out').prop('disabled', $(this).is(':checked'));
    });

});

function disableInCheckboxes()
{
    $('.childBehavior_in').prop('disabled', $('#ChildBehavioralIssues_NoneObserved').is(':checked'));
}

function disableOutCheckboxes() {
    $('.childBehavior_out').prop('disabled', $('#ChildBehavioralIssues_NoneObservedAtOuttake').is(':checked'));
}