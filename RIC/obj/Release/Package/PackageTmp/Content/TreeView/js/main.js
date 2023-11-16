jQuery(document).ready(function(){
	var accordionsMenu = $('.cd-accordion-menu');

	if( accordionsMenu.length > 0 ) {
	
	    accordionsMenu.each(function () {
           // debugger
			var accordion = $(this);
			//detect change in the input[type="checkbox"] value
			accordion.on('change', 'input[type="checkbox"]', function(){
				var checkbox = $(this);
				console.log(checkbox.prop('checked'));
				( checkbox.prop('checked') ) ? checkbox.siblings('ul').attr('style', 'display:none;').slideDown(300) : checkbox.siblings('ul').attr('style', 'display:block;').slideUp(300);
			});
		});
	}

	//$('#mySidenav ul li:first ul').css("display", "block").slideDown(300);
	//$('#mySidenav ul li:first ').find('[type=checkbox]').prop('checked', true)
});