var loadingTimeout;

//#region AjaxStartStop
$(document).ajaxStart(function () {
	loadingTimeout = setTimeout(function () {
		$(".icjia-loading").show();
	}, 100);
});

$(document).ajaxStop(function () {
	clearTimeout(loadingTimeout);
	$(".icjia-loading").hide();
});
//#endregion