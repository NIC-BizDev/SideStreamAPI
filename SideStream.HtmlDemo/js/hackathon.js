




$(document).ready(function(){
	

	////////////////////////////////////////////////////////////////////////////////
	// Foundation
	////////////////////////////////////////////////////////////////////////////////	
	$(document).foundation();


	////////////////////////////////////////////////////////////////////////////////
	// General
	////////////////////////////////////////////////////////////////////////////////	
	function addScript(url,callback){
		// $.getScript(url,callback);
		$('body').append('<scr'+'ipt src="'+url+'"></scr'+'ipt>');
	}
	function addStyle(url){
		$('body').append('<li'+'nk rel="stylesheet" href="'+url+'">');
	}

	// If Phone
	var phone = false;
	if($(window).width() < 600) phone = true;

	// Hash
	if(window.location.hash) var hash = window.location.hash.replace('#','');

	// If Element
	function ifElement(element,func){
		var ele = $(element);
		if(ele.length > 0) {
			return func(ele);
		}
		else return false;
	}


	////////////////////////////////////////////////////////////////////////////////
	// Tabs
	////////////////////////////////////////////////////////////////////////////////	
	function tabs(ele){
		
		ele.each(function(){
			
			var tabs = $(this).find('.tab-options'),
					group = tabs.data('tabgroup'),
					content = $(this).find('.tab-content');
						
			tabs.addClass('enabled');
			content.addClass('enabled');
			
			/* -----------------------------------------
				 Display tab on page load (by hash or default)
			----------------------------------------- */
			var selected = false;	
			var hashText = '';
			content.find('> li').each(function(){
				var ele = $(this),
						id = ele.data('tab');
				if(hash == id) {
					ele.addClass('active');
					tabs.find('> li[data-tab='+hash+']').addClass('active');
					hashText = tabs.find('> li[data-tab='+hash+']').text();
					selected = true;
				}
			});
			if(!selected) {
				content.find('> li:first-child').addClass('active');
				tabs.find('> li:first-child').addClass('active');
			}
			
			/* -----------------------------------------
				 Mobile tab
			----------------------------------------- */
			tabs.prepend('<li class="mobile-show mobile-tab icon-menu"><span class="tab-label">'+tabs.find('li:first-child').text()+'</span><span class="icon-menu">Menu</span><span class="icon-cancel">Close</span></li>');
			var mobileTab = tabs.find('.mobile-tab');
			mobileTab.click(function(){
				mobileTab.toggleClass('active');
				tabs.toggleClass('active');
			})
			if(hashText != '') mobileTab.find('.tab-label').text(hashText);
			
			
			/* -----------------------------------------
				 Tab clicks
			----------------------------------------- */
			tabs.find('> li:not(.mobile-tab)').click(function(){
				var ele = $(this),
						id = ele.data('tab'),
						tabLabel = ele.text();		
				tabs.find('> li').removeClass('active');
				ele.addClass('active');	
				content.find('> li').hide();
				content.find('> li[data-tab="'+id+'"]').show();		
				mobileTab.click().find('.tab-label').text(tabLabel);					
			});			
			
		});
		
		
	}
	ifElement('.tabs',tabs);


	////////////////////////////////////////////////////////////////////////////////
	// Accordion
	////////////////////////////////////////////////////////////////////////////////
	function accordion(ele){
		
		ele.each(function(){
			
			var acc = $(this);
			
			acc.find('> dd').slideUp(0);
							
			acc.find('> dt').addClass('icon-angle-right').click(function(){
				
				var ele = $(this);
				
				acc.find('> dt').not(this).removeClass('active icon-angle-down');
				acc.find('> dd').removeClass('active').slideUp(200);
				
				if(ele.hasClass('active')) {
					ele.removeClass('active icon-angle-down');
					ele.next('dd').removeClass('active').slideUp(200);
				} else {
					ele.addClass('active icon-angle-down');
					ele.next('dd').addClass('active').slideDown(200);
				}

				ifElement('table',tableMessage);
				
			});
							
		});
		
	}
	ifElement('dl.accordion',accordion);

	


})