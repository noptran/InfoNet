$(function () {
    if ($('#race6').is(":checked"))
        $(".race").not('#race6').prop("disabled", true);

    if ($(".race").not('#race6').is(":checked")) {
        $("#race6").prop("disabled", true);
        $(".race").not('#race6').prop("disabled", false);
    }

    $(".race").not('#race6').change(function () {
        var numberofCheckedRaces = $(".race:not(#race6):checked").length;
        if ($(this).is(':checked'))
            $("#race6").prop("disabled", true);
        else if (numberofCheckedRaces == 0)
            $("#race6").prop("disabled", false);
    });

    $('#race6').change(function () {
        if ($(this).is(':checked'))
            $(".race").not(this).prop("disabled", true);
        else
            $('.race').prop('disabled', false);
    });
});