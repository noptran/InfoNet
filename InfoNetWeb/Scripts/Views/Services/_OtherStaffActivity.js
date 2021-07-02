var prevDate;

//Save the form
$('#saveAddButton').click(function () {
    $('#saveAddNew').val('1');
    $('#main').submit();
});

$('#saveButton').click(function () {
    $('#main').submit();
});

$(document).ready(function () {
    $('#main').on('focusin blur', '[data-provide="datepicker"] input', function (event) {
        //console.log('prevDate = ' + prevDate);
        //console.log('currentDate = ' + $(this).val());
        if (event.type === 'focusin') {
            prevDate = $(this).val();
        }
        else {
            if (prevDate != $(this).val()) {
                var ajaxURL = "/Service/GetStaff?serviceDate=" + $(this).val();

                $.ajax({
                    url: ajaxURL,
                    success: function (data) {
                        //console.log(data);
                        $('select[name$="SVID"]').each(function () {
                            var selectList = this;
                            var selectedOption = $(this).find('option:selected');
                            var selectedValue = $(selectedOption).val();
                            var isPresent = false;
                            $(this).find('option').remove().end();
                            $(this).append($('<option>').text("<Pick One>").attr('value', ""));
                            $.each(data, function (i, staff) {
                                if (selectedValue == data[i].SVID) {
                                    $(selectList).append($('<option>').text(data[i].EmployeeName).attr('value', data[i].SVID).attr('selected', 'selected'));
                                    isPresent = true;
                                }
                                else {
                                    $(selectList).append($('<option>').text(data[i].EmployeeName).attr('value', data[i].SVID));
                                }
                            });
                            if (!isPresent && selectedValue != "") {
                                $(selectList).find('option:selected').removeAttr('selected');
                                $(selectedOption).attr('selected', 'selected');
                                $(selectList).append(selectedOption);
                            }
                        });
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        //throw error
                    }
                });
            }
        }
    });
});


function validateMyForm() {
}