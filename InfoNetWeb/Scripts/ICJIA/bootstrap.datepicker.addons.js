//REQUIRES jquery.js and bootstrap-datepicker-icjia.js and optionally jquery.validate.js

//set global defaults for Bootstrap.Datepicker (these can be overridden with data-* attributes)
$.fn.datepicker.defaults.todayBtn = 'linked';
$.fn.datepicker.defaults.daysOfWeekHighlighted = '0,6';
$.fn.datepicker.defaults.autoclose = true;
$.fn.datepicker.defaults.todayHighlight = true;
$.fn.datepicker.defaults.assumeNearbyYear = true;
$.fn.datepicker.defaults.orientation = 'bottom left';

(function () {

    // attach event delegates for inline-date (i.e. inline-date - example not in input-group inside of td instead... inline-date and date class are directly on input)
    var inlineDate = 'input.inline-date.date[data-provide="datepicker"]';
    $(document).on('show', inlineDate, function () {
        $(this).datepicker('update').addClass('icjia-datepicker-visible').focus();
    });

    $(document).on('hide', inlineDate, function () {
		var $this = $(this);
		$this.removeClass('icjia-datepicker-visible');
		if(jQuery().validate)
			$this.valid();
    });

    $(document).on('click', inlineDate, function () {
		var $this = $(this);
		if (!$this.hasClass('icjia-datepicker-visible'))
            $this.datepicker('show');
	});


    // attach event delegates for dateGroups (i.e. input-groups with datepickers that contain exactly one date)
    var dateGroup = '.input-group.date[data-provide="datepicker"]';
    $(document).on('show', dateGroup, function () {
		var $this = $(this);
		$this.datepicker('update');
        $this.find('input').first().focus();
        $this.addClass('icjia-datepicker-visible');
    });

    $(document).on('hide', dateGroup, function () {
		var $this = $(this);
		$this.removeClass('icjia-datepicker-visible');
		if(jQuery().validate)
			$this.find('input').first().valid();
    });

    $(document).on('click', dateGroup + ' input', function () {
		var $inputGroup = $(this).closest(dateGroup);
		if (!$inputGroup.hasClass('icjia-datepicker-visible'))
            $inputGroup.datepicker('show');
	});

    $(document).on('click', dateGroup + ' .input-group-addon', function () {
		var $inputGroup = $(this).closest(dateGroup);
		if ($inputGroup.hasClass('icjia-datepicker-visible'))
            $inputGroup.datepicker('hide');
        else
            $inputGroup.datepicker('show');
	});


    // attach event delegates for dateRanges (i.e. input-groups with datepickers that specify a start and end date)
    var dateRange = '.input-group.input-daterange[data-provide="datepicker"]';
    $(document).on('show', dateRange + ' input', function () {
        var $this = $(this);
        $this.datepicker('update');
        $this.focus();
        $this.addClass('icjia-datepicker-visible');
    });

    $(document).on('click', dateRange + ' input', function () {
        var $this = $(this);
        if (!$this.hasClass('icjia-datepicker-visible'))
            $this.datepicker('show');
    });

    $(document).on('hide', dateRange + ' input', function () {
		var $this = $(this);
        $this.removeClass('icjia-datepicker-visible');
		if(jQuery().validate)
			$this.valid();
    });
})();


$(function () {

    $('body').on('blur', '[data-provide="datepicker"] input, input[data-provide="datepicker"]', function () {
        var $this = $(this);
        var month, day, year;
        var regExp = /^(\d{1,2})[\/\-](\d{1,2})[\/\-](\d{1,4})$/;
        var value = $this.val();
        var regs;
        if (regs = value.match(regExp)) {
            var monthNum = parseInt(regs[1]);
            var dayNum = parseInt(regs[2]);
            var yearNum = parseInt(regs[3]);

            month = "" + ((monthNum < 10) ? "0" + monthNum : monthNum);
            day = "" + ((dayNum < 10) ? "0" + dayNum : dayNum);
            var yearLength = regs[3].length;
            switch (yearLength) {
                case 1:
                    year = "" + (yearNum + 2000);
                    break;
                case 2:
                    year = "" + ((yearNum < 70) ? yearNum + 2000 : yearNum + 1900);
                    break;
                case 3:
                    year = "" + yearNum;
                    break;
                case 4:
                    year = "" + yearNum;
                    break;
                default:
                    return;
            }
            var formattedValue = "" + month + "/" + day + "/" + year;
            if (formattedValue != value) {
                $this.val(formattedValue);
            }
            $this.change();
			if(jQuery().validate)
				$this.valid();
        }
    });

});