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
				var stars = Math.floor(Math.random() * 5) + 1;

				var str = '<div class="row">'+
						    '<div class="large-24 columns">'+
						        '<div class="panel">';

				for(var i = 0; i < stars; i++ ){
					str = str + '<i class="step fi-heart size-21"></i>';	
				}
				
			            str = str + '<div>'+
			                tip.description+
			            '</div>'+
			        '</div>'+
			    '</div>';

				$('#tips').append(str);
			});
		});
	});


	$('#postTip').click(function(event) {
		var id = $("#ridbID").val();
		var tip = $("#userTip").val();
		
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