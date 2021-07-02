$(function () {
    jQuery.validator.addMethod('betweennineteenninetytoday', function (value, element, params) {
        var entry = Date.parse(value);
        return !entry || Date.parse("01/01/1990") <= entry && entry <= new Date();
    }, '');

    jQuery.validator.unobtrusive.adapters.add('betweennineteenninetytoday', function (options) {
        options.rules['betweennineteenninetytoday'] = {};
        options.messages['betweennineteenninetytoday'] = options.message;
    });

}(jQuery));