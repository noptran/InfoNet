function systemGrowl(messageTitle, message, type, delayTime) {
	if (typeof delayTime == 'undefined') { delayTime = null }

	$.bootstrapGrowl('<strong>' + messageTitle + ' </strong>' + message, {
		ele: 'body',
		type: type,
		offset: { from: 'bottom', amount: 15 },
		align: 'right',
		width: 250,
		delay: delayTime,
		allow_dismiss: true,
		stackup_spacing: 15
	});
}