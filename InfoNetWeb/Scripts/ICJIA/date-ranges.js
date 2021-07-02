//********************************************** Calculating The Date Range For DropDown ************************************************//


$('select[data-icjia-role="dateRanges"][data-icjia-default-range]').each(function () {
    $(this).val($(this).attr('data-icjia-default-range'));
    //calculateSelectedDateRange($(this), $(this).attr('data-icjia-start'), $(this).attr('data-icjia-end'));
});

$('select[data-icjia-role="dateRanges"][data-icjia-start][data-icjia-end]').each(function () {
    var $this = $(this);
    var start = $($(this).attr('data-icjia-start'));
    var end = $($(this).attr('data-icjia-end'));
    $(start).on("change", function (e) {
        $this.val(0);
    });
    $(end).on("change", function (e) {
        $this.val(0);
    });
});

$('select[data-icjia-role="dateRanges"][data-icjia-start][data-icjia-end]').change(function () {
	calculateSelectedDateRange($(this), $(this).attr('data-icjia-start'), $(this).attr('data-icjia-end'));
});

//KMS DO id="datepicker" seems problematic
$(document).ready(function () {
    $('div[id="datepicker"]').each(function () {
	    $(this).children().each(function() {
            var currentval = $(this).val();
            $(this).val(formatDate(currentval));        
        });
    });
});

function calculateSelectedDateRange(rangeSelector, startDateSelector, endDateSelector) {
    // Start and End of the chosen period by user
    var startOfPeriod, endOfPeriod;

    var today = new Date();
    var dd = today.getDate(),
		mm = today.getMonth(),
		yyyy = today.getFullYear();

    var currentQuarter = getQuarter(mm);

    switch (rangeSelector.find('option:selected').attr('value')) {
        case "1": //This Month
            {
                startOfPeriod = new Date(yyyy, mm, 1);
                endOfPeriod = new Date(yyyy, mm + 1, 0);
            }
            break;

        case "2": // This Quarter
            {
                startOfPeriod = new Date(yyyy, getStartingMonth(currentQuarter) - 1, 1);
                endOfPeriod = new Date(yyyy, getStartingMonth(currentQuarter) + 2, 0);
            }
            break;

        case "3": // This Fiscal Year
            {
                if (mm >= 0 && mm <= 5)
                { yyyy--; }
                startOfPeriod = new Date(yyyy, 6, 1);
                endOfPeriod = new Date(yyyy + 1, 6, 0);
            }
            break;

        case "4": // This Calendar Year
            {
                startOfPeriod = new Date(yyyy, 0, 1);
                endOfPeriod = new Date(yyyy, 12, 0);
            }
            break;

        case "5": // Last Month
            {
                startOfPeriod = new Date(yyyy, mm - 1, 1);
                endOfPeriod = new Date(yyyy, mm, 0);
            }
            break;

        case "6": // Last Quarter
            {
                startOfPeriod = new Date(yyyy, getStartingMonth(currentQuarter) - 4, 1);
                endOfPeriod = new Date(yyyy, getStartingMonth(currentQuarter) - 1, 0);
            }
            break;

        case "7": // Last Fiscal Year
            {
                if (mm >= 0 && mm <= 5)
                { yyyy--; }
                startOfPeriod = new Date(yyyy - 1, 6, 1);
                endOfPeriod = new Date(yyyy, 6, 0);
            }
            break;

        case "8": // Last Calendar Year
            {
                startOfPeriod = new Date(yyyy - 1, 0, 1);
                endOfPeriod = new Date(yyyy - 1, 12, 0);
            }
            break;

        case "9": // 1st Quarter (of this Fiscal Year)
            {
                if (mm >= 0 && mm <= 5)
                { yyyy--; }
                startOfPeriod = new Date(yyyy, 6, 1);
                endOfPeriod = new Date(yyyy, 9, 0);
            }
            break;

        case "10": // 2nd Quarter (of this Fiscal Year)

            {
                if (mm >= 0 && mm <= 5)
                { yyyy--; }
                startOfPeriod = new Date(yyyy, 9, 1);
                endOfPeriod = new Date(yyyy, 12, 0);
            }
            break;

        case "11": // 3rd Quarter (of this Fiscal Year)
            {
                if (mm >= 6 && mm <= 12)
                { yyyy++; }
                startOfPeriod = new Date(yyyy, 0, 1);
                endOfPeriod = new Date(yyyy, 3, 0);
            }
            break;

        case "12": // 4th Quarter (of this Fiscal Year)
            {
                if (mm >= 6 && mm <= 12)
                { yyyy++; }
                startOfPeriod = new Date(yyyy, 3, 1);
                endOfPeriod = new Date(yyyy, 6, 0);
            }
            break;

        case "13": // Last 3 Months
            {
                startOfPeriod = new Date(today.getFullYear(), mm - 3, dd);
                endOfPeriod = new Date(yyyy, mm, dd);
            }
            break;

        case "14": // Last 6 Months
            {
                startOfPeriod = new Date(today.getFullYear(), mm - 6, dd);
                endOfPeriod = new Date(yyyy, mm, dd);
            }
            break;

        case "15": // Last 12 Months
            {
                startOfPeriod = new Date(today.getFullYear(), mm - 12, dd);
                endOfPeriod = new Date(yyyy, mm, dd);
            }
            break;

        case "16": // Last 2 Years
            {
                startOfPeriod = new Date(today.getFullYear() - 2, mm, dd);
                endOfPeriod = new Date(yyyy, mm, dd);
            }
            break;


        case "17": // Last 3 Years
            {
                startOfPeriod = new Date(today.getFullYear() - 3, mm, dd);
                endOfPeriod = new Date(yyyy, mm, dd);
            }
            break;


        case "18": // Last 4 Years
            {
                startOfPeriod = new Date(today.getFullYear() - 4, mm, dd);
                endOfPeriod = new Date(yyyy, mm, dd);
            }
            break;


        case "19": // Last 5 Years
            {
                startOfPeriod = new Date(today.getFullYear() - 5, mm, dd);
                endOfPeriod = new Date(yyyy, mm, dd);
            }
            break;

        case "20": // Last 30 Days
            {
                startOfPeriod = new Date(yyyy, mm, dd - 30);
                endOfPeriod = new Date(yyyy, mm, dd);
            }
            break;

        default: // Sets the value of the datepickers to blank
            startOfPeriod = "";
            endOfPeriod = "";
            break;
    }

    $(startDateSelector).val(formatDate(startOfPeriod));
    $(endDateSelector).val(formatDate(endOfPeriod));
    $(startDateSelector).valid();
    $(endDateSelector).valid();
}

function formatDate(date) {
    if (date == " " || date == "") {
        return "";
    }
    var d = new Date(date),
		month = '' + (d.getMonth() + 1),
		day = '' + d.getDate(),
		year = d.getFullYear();

    if (month.length < 2) month = '0' + month;
    if (day.length < 2) day = '0' + day;

    return [month, day, year].join('/');
}

function getQuarter(month) {
    return Math.ceil((month + 1) / 3);
}

function getStartingMonth(currentQuarter) {
    var start;
    switch (currentQuarter) {
        case 1:
            start = 1;
            break;
        case 2:
            start = 4;
            break;
        case 3:
            start = 7;
            break;
        case 4:
            start = 10;
            break;
    }
    return start;
}
