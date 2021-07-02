$(document).on('change', ':file', function() {
	var $this = $(this);
	var numFiles = $this.get(0).files ? $this.get(0).files.length : 1;
	var label = $this.val().replace(/\\/g, '/').replace(/.*\//, '');
	$this.trigger('fileselect', [numFiles, label]);
});

$(function () {
	$('#UploadedFile').on('fileselect', function (event, numFiles, label) {
		var $text = $('#UploadedFileText');
		if ($text.val() != label) {
			$text.val(label);
			$text.change(); //for dirtyforms
		}
	});

	$('#UploadedFile').on('change', function() {
		$(this).valid();
	});

	$('#main').on('submit-valid.icjia', function() {
		$('.icjia-loading').show();
	});
});