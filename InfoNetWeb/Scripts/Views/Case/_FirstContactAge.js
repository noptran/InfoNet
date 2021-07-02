$(function () {
	$('.first-contact-age').blur(function () {
		if ($("#clientType").val() === "6" || $("#clientType").val() === "7") {
			var age = $(this).val();
			if (age > 25)
				systemGrowl('Warning!', 'The age entered is greater than the maximum Child age limit of 25. However, you will still be able to save the record.', 'warning', 10000);
		}
	});
});