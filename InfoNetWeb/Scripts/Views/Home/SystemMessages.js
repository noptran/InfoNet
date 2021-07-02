(function () {
	var overflow = 300;
	var pageSize = 9;
	var pagesLoaded = 0;
	var isLoading = false;

	function getMoreMessages() {
		if (pagesLoaded * pageSize >= $('#messages').data('icjia-total-available'))
			return;

		if (isLoading)
			return;

		isLoading = true;
		$.ajax({
			type: 'GET',
			url: '/home/GetMoreMessages',
			data: { "pageIndex": pagesLoaded },
			success: function(partial) {
				if (partial != null) {
					$(partial).find('div[data-icjia-role="cardWrapper"]').each(function() {
						var item = document.createElement('div');
						var grid = document.querySelector('#messages');
						salvattore.appendElements(grid, [item]);
						item.outerHTML = $(this).html();
					});
					pagesLoaded++;
				}
			},
			beforeSend: function() {
				$('#progress').removeClass('hide');
			},
			complete: function() {
				$('#progress').addClass('hide');
				isLoading = false;

				if ($(document).height() - $('footer.icjia-sticky-footer').height() < $(window).height() + overflow)
					getMoreMessages();
			},
			error: function(xhr) {
				//console.log(xhr.responseText);
			}
		});
	}

	$(function() {
		getMoreMessages();

		$(window).on('resize',
			debounce(function() {
				if ($(document).height() - $('footer.icjia-sticky-footer').height() < $(window).height() + overflow)
						getMoreMessages();
				},
				400));

		$(window).scroll(function () {
			if (Math.abs($(window).scrollTop() - ($(document).height() - $(window).height())) < overflow)
				getMoreMessages();
		});
	});
})();