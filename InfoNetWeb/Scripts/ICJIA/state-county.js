$().ready(function () {

	// Place data-icjia-role="StateCounty" & data-icjia-county="{county dropdown selector}" to use

	// On State Changed
	$(document).on('change', 'select[data-icjia-role="StateCounty"]', function () {
		if ($(this).val() != "") {
			var countyList = $($(this).data('icjia-county'));
			var stateID = $(this).val();
			$.ajax({
				url: "/USPS/ListCountiesByState?stateID=" + stateID,
				type: "GET",
				data: "json",
				success: function (data) {
					//console.log(data);
					$(countyList).find('option').remove().end();
					$(countyList).append($('<option>').text("<Pick One>").attr('value', ""));
					$.each(data, function (i, county) {
						$(countyList).append($('<option>').text(data[i].Name).attr('value', data[i].ID));
					});
				},
				error: function (xhr, ajaxSettings, thrownError) {
					//console.log(xhr.responseText);
				}
			});
		}
	});
});