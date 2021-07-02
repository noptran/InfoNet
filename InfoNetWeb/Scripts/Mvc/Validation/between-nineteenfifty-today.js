$(function () {
    jQuery.validator.addMethod('betweennineteenfiftytoday', function (value, element, params) {
        var entry = Date.parse(value);
        return !entry || Date.parse("01/01/1950") <= entry && entry <= new Date();
    }, '');

    jQuery.validator.unobtrusive.adapters.add('betweennineteenfiftytoday', function (options) {
        options.rules['betweennineteenfiftytoday'] = {};
        options.messages['betweennineteenfiftytoday'] = options.message;
    });

}(jQuery));