// provide support for attributes: data-icjia-broadcast=<target$> and data-icjia-broadcast-default=<text>

/* forces all broadcasts to be refreshed */
var rebroadcast = null;

(function () {

	function selectedText(select$) {
		return $(select$).find('option:selected:not([value=""])').text();
	}

	function valForText(select$, text) {
		return $(select$).find('option').filter(function () { return $.trim($(this).text()) == text; }).val();
	}

	function allowsVal(select$, value) {
		return $(select$).find('option').filter(function () { return $(this).val() == value; }).length > 0;
	}

	function coalesce(a, b) {
		return a == undefined || a == '' || (Array.isArray(a) && a.length == 0) ? b : a;
	}

	var rules = [{
		source: ':checkbox',
		target: ':not(:input)',
		broadcast: function ($source, $target, def) {
			$target.text($source.is(':checked') ? 'Yes' : 'No');
		}
	},
	{
		source: ':input:not(select)',
		target: ':not(:input)',
		broadcast: function ($source, $target, def) {
			$target.text(coalesce($source.val(), def));
		}
	}, {
		source: 'select:not([multiple])',
		target: ':not(:input)',
		broadcast: function ($source, $target, def) {
			$target.text(coalesce(selectedText($source), def));
		}
	}, {
		source: ':input:not(select)',
		target: ':input:not(select)',
		broadcast: function ($source, $target, def) {
			$target.val(coalesce($source.val(), def));
		}
	}, {
		source: ':input:not(:text):not(textarea):not(select[multiple])',
		target: ':input:not(:text):not(textarea):not(select[multiple])',
		broadcast: function ($source, $target, def) {
			$target.val(coalesce($source.val(), def));
		}
	}, {
		source: 'select:not([multiple])',
		target: ':text,textarea',
		broadcast: function ($source, $target, def) {
			$target.val(coalesce(selectedText($source), def));
		}
	}, {
		source: ':text,textarea',
		target: 'select:not([multiple])',
		broadcast: function ($source, $target, def) {
			var value = coalesce(valForText($target, $source.val()), def);
			if (allowsVal($target, value)) //KMS DO needed?
				$target.val(value);
		}
	}, {
		source: 'select[multiple]',
		target: 'select[multiple]',
		broadcast: function ($source, $target, def) {
			$target.val(coalesce($source.val(), def));
		}
	}, {
		source: 'select[multiple]',
		target: ':not(:input)',
		broadcast: function ($source, $target, def) {
			var text = '';
			$source.find('option:selected').each(function () {
				text = text + $(this).text() + ", ";
			});
			$target.text(coalesce(text.substr(0, text.length - 2), def));
		}
	}];

	function triggerBroadcast(broadcaster$) {
		var $source = $(broadcaster$);
		var $target = $($source.data('icjia-broadcast'));
		if ($target.length == 0) {
			//console.log("no broadcast target found");
			return;
		}
		var def = $source.data('icjia-broadcast-default');
		if (def == undefined)
			def = '';

		for (var i = 0; i < rules.length; i++)
			if ($source.is(rules[i].source) && $target.is(rules[i].target)) {
				rules[i].broadcast($source, $target, def);
				return;
			}
		throw "no broadcast rule applicable to source and target";
	}

	rebroadcast = function () {
		$('[data-icjia-broadcast]').each(function () {
			triggerBroadcast(this);
		});
	};

	$(document).on('change', '[data-icjia-broadcast]', function () {
		triggerBroadcast(this);
	});

	$(document).on('keyup', ':text[data-icjia-broadcast],textarea[data-icjia-broadcast]', function () {
		triggerBroadcast(this);
	});

	$(rebroadcast);

})();