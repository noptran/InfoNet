$(document).ready(function () {
	$('#Center_Telephone, #Center_Fax').on("blur",
	function formatNumber() {
		var numbers = this.value.replace(/\D/g, '');
		var char = { 0: '(', 3: ') ', 6: '-' }; 
		this.value = '';
		for (var i = 0; i < numbers.length; i++) {
			this.value += (char[i] || '') + numbers[i];
		}
		$(this).valid();
	});

	$('#Center_Zipcode').on("blur", function () {
		if ($(this).val() != "") {

			var stateList = $('select[name$="Center.StateID"]');
			var countyList = $('select[name$="CountyID"]');
			var cityList = $('select[name$="CityID"]');

			var zip = $(this).val();
			$.ajax({
				url: "/USPS/ListStatesCountiesCitiesByZip?zip=" + zip,
				type: "GET",
				data: "json",
				success: function (data) {
					$(stateList).find('option').remove().end();
					$.each(data, function (i, state) {
						$(stateList).append($('<option>').text(data[i].Name).attr('value', data[i].ID));
						if (i == 0) {
							$(countyList).find('option').remove().end();
							$.each(data[i].Children, function (i, county) {
								$(countyList).append($('<option>').text(county.Name).attr('value', county.ID));
								if (i == 0) {
									$(cityList).find('option').remove().end(); 
									$.each(data[i].Children[i].Children, function (i, city) {
										$(cityList).append($('<option>').text(city.Name).attr('value', city.ID));
									});
								}
							});
						}
					});
				},
				error: function (xhr, ajaxSettings, thrownError) {
				}
			});
		}
	});
});

$("#Center_Telephone, #Center_Fax").keyup(function () { return false });