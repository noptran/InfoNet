$(document).ready(function () {
	$('#Username').focus();

	if (CapsLock.isOn()) {
		alert('Caps Lock is On!');
		$(document).find("#capsWarn").show();
	}
	CapsLock.addListener(
    function (status) {
    	if (status) {
    		$(document).find("#capsWarn").show();
    	}
    	else {
			$(document).find("#capsWarn").hide();
		}
    });
});