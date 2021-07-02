//REQUIRES jquery.js

(function ($) {

	$.fn.fire = function (callback, args) {
		this.each(function () {
			var callbacks = $.Callbacks();
			callbacks.add(callback);
			callbacks.fireWith(this, args);
		});
	};

	$.fn.fadeThenSlideUp = function (fadeDuration, fadeEasing, slideDuration, slideEasing, complete) {
		if (arguments.length == 0 || jQuery.isFunction(fadeDuration)) {
			complete = fadeDuration;
			fadeDuration = slideDuration = 400;
			fadeEasing = 'linear';
			slideEasing = 'swing';
		}

		return this.fadeTo(fadeDuration, 0, fadeEasing).slideUp(slideDuration, slideEasing, function () {
			$(this).css({ opacity: '' });
			if (complete != undefined)
				$(this).fire(complete);
		});
	};

	$.fn.slideThenFadeIn = function (slideDuration, slideEasing, fadeDuration, fadeEasing, complete) {
		if (arguments.length == 0 || jQuery.isFunction(slideDuration)) {
			complete = slideDuration;
			slideDuration = fadeDuration = 400;
			slideEasing = 'swing';
			fadeEasing = 'linear';
		}

		this.css({ opacity: 0 });
		return this.slideDown(slideDuration, slideEasing).fadeTo(fadeDuration, 1, fadeEasing, function () {
			$(this).css({ opacity: '' });
			if (complete != undefined)
				$(this).fire(complete);
		});
	};

}(jQuery));