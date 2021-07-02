var resizeNavStacked = null;
var navStackedMode = null;

$(function () {
	refreshScrollspyBuffer();

	var $window = $(window);
	var $nav = $('.icjia-scrollspy ul.nav.icjia-nav-stacked');
	var $undoButton = $('[data-icjia-role="dirty.page.confirm"]');

	var expandTo = [
		null,
		function () { $nav.addClass('icjia-nav-stacked-accordion').removeClass('icjia-nav-stacked-collapsed'); },
		function () { $nav.removeClass('icjia-nav-stacked-accordion'); }
	];
	var collapseTo = [
		function () { $nav.addClass('icjia-nav-stacked-collapsed').removeClass('icjia-nav-stacked-accordion'); },
		function () { $nav.addClass('icjia-nav-stacked-accordion'); },
		null
	];
	var animateExpandTo = [
			null,
			function () {
				$nav.find('li.active .nav').slideThenFadeIn(function () {
					$nav.addClass('icjia-nav-stacked-accordion').removeClass('icjia-nav-stacked-collapsed');
					$(this).css({ display: '' });
				});
			},
			function () {
				$nav.find('li:not(.active) .nav').slideThenFadeIn(function () {
					$nav.removeClass('icjia-nav-stacked-accordion');
					$(this).css({ display: '' });
				});
			}
	];
	var animateCollapseTo = [
		function () {
			$nav.find('li.active .nav').fadeThenSlideUp(function () {
				$nav.addClass('icjia-nav-stacked-collapsed').removeClass('icjia-nav-stacked-accordion');
				$(this).css({ display: '' });
			});
		},
		function () {
			$nav.find('li:not(.active) .nav').fadeThenSlideUp(function () {
				$nav.addClass('icjia-nav-stacked-accordion');
				$(this).css({ display: '' });
			});
		},
		null
	];

	navStackedMode = function () {
		if ($nav.hasClass('icjia-nav-stacked-collapsed'))
			return 0;
		if ($nav.hasClass('icjia-nav-stacked-accordion'))
			return 1;
		return 2;
	};

	var resize = function (animate) {
		var originalMode = navStackedMode();
		var mode = originalMode;
		if (mode == 0)
			$nav.removeClass('icjia-nav-stacked-accordion'); /* if both accordion and collapsed are present, collapsed takes precedence so clean it up */

		var lastMargin = null;
		while (true) {
			//http://upshots.org/javascript/jquery-test-if-element-is-in-viewport-visible-on-screen
			//var margin = $window.innerHeight() - ($undoButton.offset().top + $undoButton.outerHeight());
			var margin = $window.innerHeight() - $undoButton.get(0).getBoundingClientRect().bottom;
			if (margin >= 0 && margin < 20) /* where 20px = nav-stacked nav li.height */
				break;
			if (mode == 0 && margin < 0)
				break;
			if (mode == 2 && margin > 0)
				break;
			if (lastMargin != null && lastMargin < 0 && margin >= 0)
				break;

			if (margin < 0)
				collapseTo[--mode]();
			else
				expandTo[++mode]();

			lastMargin = margin;
		}

		if (animate == true) {
			var targetMode = mode;

			while (mode < originalMode)
				expandTo[++mode]();
			while (mode > originalMode)
				collapseTo[--mode]();

			while (mode < targetMode)
				animateExpandTo[++mode]();
			while (mode > targetMode)
				animateCollapseTo[--mode]();
		}

		return mode;
	};

	resizeNavStacked = debounce(function () { resize(true); }, 400);

	resizeNavStacked();

	$window.on('resize', resizeNavStacked);

	$('#newCaseLink').click(function (e) {
		e.preventDefault();
		if($('#NoDatesAvail').val() == "True"){
			$.confirm({
				dialogClass: "modal-dialog icjia-modal-warning",
				title: "Unable to add a new case",
				text: "There are no valid First Contact Dates available.",
				confirmButton: 'OK',
				confirmButtonClass: 'btn btn-warning',
				cancelButton: null
			});
		}
		else if ($('#warningMessage').length) {
			$.confirm({
				title: "Are you sure you want to do that?",
				text: $('#warningMessage').html(),
				confirm: function () {
					window.location.href = "/Case/EditRedirect?clientId=" + $('#clientId').val();
				}
			});
		}
		else {
			window.location.href = "/Case/EditRedirect?clientId=" +$('#clientId').val();
		}
	});
});

$(document).on('activate.bs.scrollspy', function () {
	var hash = $(this).find("li.active:last a").attr("href");
	$("#hash").val(hash);
	$("#btnUndo").attr('href', '/Case/RefreshPage?url=' + [location.protocol, '//', location.host, location.pathname].join('') + '&hash=' + hash.replace('#', ''));
});

$(window).on('resize', function () {
	refreshScrollspyBuffer();
});

function refreshScrollspyBuffer() {
	var newHeight = $(window).height() - $('#navbar').height() - 20 /* 20px navbar margin-bottom */ - 30 /* 30px bar above navbar */ - $('.icjia-sticky-footer').height();
	$('#scrollspyBuffer').animate({ height: newHeight });
}

$('#newCaseLink').parent('li').tooltip({ placement: "bottom" });