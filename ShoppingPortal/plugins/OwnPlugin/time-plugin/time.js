(function ( $ ) {
 
    $.fn.time = function( options ) {
		
		var clickcnt = 0;
		var keycnt = 0;
		var h_ampm;
			
        $(this).focus(function(){
			
			var dt 		= new Date();
			var h  		= parseInt(dt.getHours());
			var m 		= parseInt(dt.getMinutes());
			var s 		= parseInt(dt.getSeconds());
			var ms		= parseInt(dt.getMilliseconds());
			var y		= parseInt(dt.getFullYear());
			var mt		= parseInt(dt.getMonth());
			var d		= parseInt(dt.getDay());
			var dtt 	= parseInt(dt.getDate());
			
			if(clickcnt == 0){
				$(this).attr("data_time",h+":"+m+":"+s);
				$(this).val(formatDate(h,m,s));
				clickcnt++;
			}
			
		}).click(function(e){
			var position = $(this).prop("selectionStart");
			
			//alert(val.slice(0, el.selectionStart).length);
	    }).keydown(function(e){
			switch(e.which) {
				case 37: // left
					time = $(this).attr("data_time");
					time = time.split(":");
					h = parseInt(time[0]) - 1;
					m = parseInt(time[1]);
					s = parseInt(time[2]);
					if(h <= 0){
						h = 24;
					}
					$(this).attr("data_time",h+":"+m+":"+s);
					$(this).val(formatDate(h,m,s));
					
				break;

				case 38: // up
					var position = $(this).prop("selectionStart");
					OnMinute($(this));
					
				break;

				case 39: // right
					time = $(this).attr("data_time");
					time = time.split(":");
					h = parseInt(time[0]) + 1;
					m = parseInt(time[1]);
					s = parseInt(time[2]);
					if(h >= 24){
						h = 1;
					}
					$(this).attr("data_time",h+":"+m+":"+s);
					$(this).val(formatDate(h,m,s));
				break;

				case 40: // down
					time = $(this).attr("data_time");
					time = time.split(":");
					h = parseInt(time[0]);
					m = parseInt(time[1]) - 1;
					s = parseInt(time[2]);
					if(m < 0){
						m = 59;
						h = h - 1;
					}
					if(h < 0 && m > 0){
						h = 24;
					}
					
					$(this).attr("data_time",h+":"+m+":"+s);
					$(this).val(formatDate(h,m,s));
				break;

				default: return false; // exit this handler for other keys
			}
			e.preventDefault(); 
		});
 
    };
 
}( jQuery ));
function formatDate(h,m,s) {
    var hh = h;
    var m = m;
    var s = s;
    var dd = "AM";
    var h = hh;
    if (h >= 12) {
        h = hh-12;
        dd = "PM";
    }
    if (h == 0) {
        h = 12;
    }
    m = m<10?"0"+m:m;
    
    s = s<10?"0"+s:s;

    /* if you want 2 digit hours: */
    h = h<10?"0"+h:h;

    var pattern = new RegExp("0?"+hh+":"+m+":"+s);
    return (h+":"+m+":"+s+" "+dd)
}
function OnMinute(textbox){
	time = textbox.attr("data_time");
	time = time.split(":");
	h = parseInt(time[0]);
	m = parseInt(time[1]) + 1;
	s = parseInt(time[2]);
	if(m > 59){
		m = 0;
		h = h + 1;
	}
	if(h >= 24 && m > 0){
		h = 0;
	}
	
	textbox.attr("data_time",h+":"+m+":"+s);
	textbox.val(formatDate(h,m,s));
}
function setCaretPosition(elemId, caretPos) {
    var elem = document.getElementById(elemId);

    if(elem != null) {
        if(elem.createTextRange) {
            var range = elem.createTextRange();
            range.move('character', caretPos);
            range.select();
        }
        else {
            if(elem.selectionStart) {
                elem.focus();
                elem.setSelectionRange(caretPos, caretPos);
            }
            else
                elem.focus();
        }
    }
}

/*Input cursor position*/
    $.fn.setCursorPosition = function(pos) {
        if ($(this).get(0).setSelectionRange) {
            $(this).get(0).setSelectionRange(pos, pos);
        } else if ($(this).get(0).createTextRange) {
            var range = $(this).get(0).createTextRange();
            range.collapse(true);
            range.moveEnd('character', pos);
            range.moveStart('character', pos);
            range.select();
        }
    }