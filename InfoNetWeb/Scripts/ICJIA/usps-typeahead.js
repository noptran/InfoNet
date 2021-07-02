$().ready(function () {

	// State And County Queries
	$('input[data-icjia-role="typeahead.township"][data-icjia-state][data-icjia-county]').typeahead({ hint: true, limit: 8, minLength: 1 },
	{
		name: 'Name',
		display: function (item) { return item.Name; },
		source: function (query, syncResults, asyncResults) {
			stateID = $('[data-icjia-role="typeahead.source.state"][data-icjia-state-source="' + $(this).data('icjia-state') + '"]').val();
			//console.log(stateID);
			countyID = $('[data-icjia-role="typeahead.source.county"][data-icjia-county-source="' + $(this).data('icjia-county') + '"]').val();
			//console.log(countyID);
			$.ajax('/USPS/SearchTownshipName?input=' + query + '&StateID=' + stateID + '&CountyID=' + countyID,
			{
				dataType: 'json',
				success: function (data) { asyncResults(data); },
				error: function (xhr) {
					//console.log(xhr.responseText);
				}
			});
		},
		templates: {
			empty: ['<div class="empty-message tt-suggestion">No Results</div>'],
			suggestion: function (value) { return '<div>' + value.Name + '</div>'; }
		}
	}).on('typeahead:selected', function () { $(this).change(); });

	$('input[data-icjia-role="typeahead.cityortown"][data-icjia-state][data-icjia-county]').typeahead({ hint: true, limit: 8, minLength: 1 },
	{
		name: 'Name',
		display: function (item) { return item.Name; },
		source: function (query, syncResults, asyncResults) {
			stateID = $('[data-icjia-role="typeahead.source.state"][data-icjia-state-source="' + $(this).data('icjia-state') + '"]').val();
			//console.log(stateID);
			countyID = $('[data-icjia-role="typeahead.source.county"][data-icjia-county-source="' + $(this).data('icjia-county') + '"]').val();
			//console.log(countyID);
			$.ajax('/USPS/SearchCityName?input=' + query + '&StateID=' + stateID + '&CountyID=' + countyID,
			{
				dataType: 'json',
				success: function (data) { asyncResults(data); },
				error: function (xhr) {
					//console.log(xhr.responseText);
				}
			});
		},
		templates: {
			empty: ['<div class="empty-message tt-suggestion">No Results</div>'],
			suggestion: function (value) { return '<div>' + value.Name + '</div>'; }
		}
	}).on('typeahead:selected', function () { $(this).change(); });

	$('input[data-icjia-role="typeahead.zipcode"][data-icjia-state][data-icjia-county]').typeahead({ hint: true, limit: 8, minLength: 1 },
	{
		name: 'Name',
		display: function (item) { return item.Name; },
		source: function (query, syncResults, asyncResults) {
			stateID = $('[data-icjia-role="typeahead.source.state"][data-icjia-state-source="' + $(this).data('icjia-state') + '"]').val();
			//console.log(stateID);
			countyID = $('[data-icjia-role="typeahead.source.county"][data-icjia-county-source="' + $(this).data('icjia-county') + '"]').val();
			//console.log(countyID);
			$.ajax('/USPS/SearchZip?input=' + query + '&StateID=' + stateID + '&CountyID=' + countyID,
			{
				dataType: 'json',
				success: function (data) { asyncResults(data); },
				error: function (xhr) { /* TODO Do Something for the error? */ }
			});
		},
		templates: {
			empty: ['<div class="empty-message tt-suggestion">No Results</div>'],
			suggestion: function (value) { return '<div>' + value.Name + '</div>'; }
		}
	}).on('typeahead:selected', function () { $(this).change(); });



	// County Only Queries
	$('input[data-icjia-role="typeahead.township"][data-icjia-county]:not([data-icjia-state])').typeahead({ hint: true, limit: 8, minLength: 1 },
	{
		name: 'Name',
		display: function (item) { return item.Name; },
		source: function (query, syncResults, asyncResults) {
			value = $(this).val();
			countyID = $('[data-icjia-role="typeahead.source.county"][data-icjia-county-source="' + $(this).data('icjia-county') + '"]').val();
			//console.log(countyID);
			$.ajax('/USPS/SearchTownshipName?input=' + query + 'CountyID=' + countyID,
			{
				dataType: 'json',
				success: function (data) { asyncResults(data); },
				error: function (xhr) {
					//console.log(xhr.responseText);
				}
			});
		},
		templates: {
			empty: ['<div class="empty-message tt-suggestion">No Results</div>'],
			suggestion: function (value) { return '<div>' + value.Name + '</div>'; }
		}
	}).on('typeahead:selected', function () { $(this).change(); });

	$('input[data-icjia-role="typeahead.cityortown"][data-icjia-county]:not([data-icjia-state])').typeahead({ hint: true, limit: 8, minLength: 1 },
	{
		name: 'Name',
		display: function (item) { return item.Name; },
		source: function (query, syncResults, asyncResults) {
			countyID = $('[data-icjia-role="typeahead.source.county"][data-icjia-county-source="' + $(this).data('icjia-county') + '"]').val();
			$.ajax('/USPS/SearchCityName?input=' + query + 'CountyID=' + countyID,
			{
				dataType: 'json',
				success: function (data) { asyncResults(data); },
				error: function (xhr) { /* TODO Do Something for the error? */ }
			});
		},
		templates: {
			empty: ['<div class="empty-message tt-suggestion">No Results</div>'],
			suggestion: function (value) { return '<div>' + value.Name + '</div>'; }
		}
	}).on('typeahead:selected', function () { $(this).change(); });

	$('input[data-icjia-role="typeahead.zipcode"][data-icjia-county]:not([data-icjia-state])').typeahead({ hint: true, limit: 8, minLength: 1 },
	{
		name: 'Name',
		display: function (item) { return item.Name; },
		source: function (query, syncResults, asyncResults) {
			countyID = $('[data-icjia-role="typeahead.source.county"][data-icjia-county-source="' + $(this).data('icjia-county') + '"]').val();
			//console.log(countyID);
			$.ajax('/USPS/SearchZip?input=' + query + '&CountyID=' + countyID,
			{
				dataType: 'json',
				success: function (data) { asyncResults(data); },
				error: function (xhr) {
					//console.log(xhr.responseText);
				}
			});
		},
		templates: {
			empty: ['<div class="empty-message tt-suggestion">No Results</div>'],
			suggestion: function (value) { return '<div>' + value.Name + '</div>'; }
		}
	}).on('typeahead:selected', function () { $(this).change(); });

});