var getUrlParameter = function getUrlParameter(name, href) {
	var results = new RegExp('[\?&]' + name + '=([^&#]*)').exec(href == null ? window.location.href : href);
	if (results == null) {
		return null;
	}
	else {
		return decodeURI(results[1]) || 0;
	}
};
