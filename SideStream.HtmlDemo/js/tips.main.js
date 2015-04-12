$(function(){
	$(document).foundation();  

	$('#searchRidb').click(function (e) {
		var id = $("#ridbID").val();

		$.ajax({
			url: 'http://ridb-tips.elasticbeanstalk.com/tour/'+id+'/tips',
			dataType: 'json'
		})
		.done(function(tips) {
			console.log(tips);

			tips.forEach(function (tip, i) {
				$('#tips').append(
						'<div class="row">'+
						    '<div class="large-24 columns">'+
						        '<div class="panel">'+

						            '<i class="step fi-heart size-21"></i>'+
						            '<i class="step fi-heart size-21"></i>'+
						            '<i class="step fi-heart size-21"></i>'+
						            '<i class="step fi-heart size-21"></i>'+
						            '<i class="step fi-heart size-21"></i>'+
						            '<div>'+
						                tip.description+
						            '</div>'+
						        '</div>'+
						    '</div>'
				);
			});
		});
	});


	$('#postTip').click(function(event) {
		var id = $("#ridbID").val();
		var tip = $("#userTip").val();

		// $.post('http://ridb-tips.elasticbeanstalk.com/tour/tip', { 
		//   		"ridbId": id, 
		//   		"description": tip
		//   }, "json");
		
		$.ajax({
		  type: "POST",
		  url: 'http://ridb-tips.elasticbeanstalk.com/tour/tip',
		  dataType : "json",
		  contentType: "application/json",
		  data: JSON.stringify({ 
		  		"ridbId": id, 
		  		"description": tip
		  })
		}).complete(function (e) {
			$("#userTip").val("");
			//console.log("haha")
		});
	});
	
});