//REQUIRES jquery.js and mustache.js

(function ($) {
	$.fn.mustacheParse = function () {
		var $this = this.first();

		var content = $this.html();
		var tags = $this.data('icjia-mustache-tags');
		Mustache.parse(content, tags);

		var renderFunction = function (viewObject, partialsObject) {
			return Mustache.render(content, viewObject, partialsObject);
		};
		$this.data('icjia-mustache-render', renderFunction);
		return renderFunction;
	};

	$.fn.mustache = function (view, partials) {
		var $this = this.first();
		var render = $this.data('icjia-mustache-render');
		if (render == undefined)
			render = $this.mustacheParse();

		return $(render(view, partials));
	};

	$.fn.fire = function (callback, args) {
		this.each(function () {
			var callbacks = $.Callbacks();
			callbacks.add(callback);
			callbacks.fireWith(this, args);
		});
	};

}(jQuery));

$(function () {
	$('[data-icjia-role="mustache"]').each(function () {
		$(this).mustacheParse();
	});
});