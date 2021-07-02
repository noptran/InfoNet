$(function () {
	$('form').not("[role='search']").each(function () {
		$('<input>').attr({ 'type': 'hidden', 'name': 'PageSize', 'value': $('#icjia-pagedlist-drop-menu').val() }).appendTo($(this));
	});

	$.urlParam('PageSize').length ? $(".icjia-pagedlist-drop-menu").val($.urlParam('PageSize')) : $(".pagination-serverside-drop-menu").prop('selectedIndex', 0);

	if ($("#pagedListRecordCount").val() == 0) {
		$(".icjia-pagedlist-drop-menu").hide();
	}
});

$.urlParam = function (name) {
	var results = new RegExp('[\?&]' + name + '=([^&#]*)').exec(window.location.href);
	if (results == null) {
		return "";
	}
	else {
		return results[1] || 0;
	}
}

$(function () {
	$('.PagedList-ellipses').each(function () {
		var curPageHasParams = true;
		var newHref = '';
		var $prevLiA = $(this).prev('li').find('a');
		var $nextLiA = $(this).next('li').find('a');

		if (window.location.search.indexOf('&') == -1) { curPageHasParams = false }

		if ($.isNumeric($prevLiA.text())) {
			elipsisPageNumber = parseInt($prevLiA.text()) + 1;
			if (curPageHasParams) {
				newHref = $prevLiA.attr('href').replace(/\page=.*?\&/, 'page=' + elipsisPageNumber + '&');
			} else {
				newHref = $prevLiA.attr('href').split('page=')[0] + 'page=' + elipsisPageNumber;
			}
		} else {
			if ($.isNumeric($nextLiA.text()))
				elipsisPageNumber = parseInt($nextLiA.text()) - 1;
			if (curPageHasParams) {
				newHref = $nextLiA.attr('href').replace(/\page=.*?\&/, 'page=' + elipsisPageNumber + '&');
			} else {
				newHref = $nextLiA.attr('href').split('page=')[0] + 'page=' + elipsisPageNumber;
			}
		}
		if (newHref.trim()) { 
			$(this).removeClass('disabled');
			$(this).find('a').attr('href', newHref);
		}
	});
});

$(document.body).on("change", ".icjia-pagedlist-drop-menu", function () {
	if (!$(this).attr('data-pagination-multiples')) {
		$('#main').find('input:hidden[name="PageSize"]').val($(this).val());
		$('#main').submit();
	}
});