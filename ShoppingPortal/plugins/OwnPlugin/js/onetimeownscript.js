"use strict";
$(document).ready(function() {
    $(".sidebar-toggle").on("click", function() {
        $(".main-sidebar").removeClass("closedxs");
		$(".main-sidebar").toggleClass("closed");
		setTimeout(function () {
			FixGridHeader(false);
		}, 300);
    });
    $(".right-sidebar-toggle").on("click", function() { $(".right-sidebar").toggleClass("closed") });

    $(window).resize(function() {
        if ($(window).width() < 768) {
            $(".main-sidebar").addClass("closed");
        }
        else {
            $(".main-sidebar").removeClass("closed");
        }
    });
});

//Close Popup in Esc
$(document).keydown(function(e) {
    try {
        if ($(".AllowClick").length == 0) {
            return false;
        }
        else if (e.keyCode == 27) //27 = escape
        {
            ClosePopup();
        }
        else if (e.keyCode == 38 || e.keyCode == 40) //38 = up arrow, 40 = down arrow
        {
            //Select Row in Updown Key
            SelectRowUpdown(e.keyCode);
        }
        else if (e.altKey && e.keyCode == 78)// alt + n
        {
            AddControl();
        }
        else if (e.altKey && e.keyCode == 85)// alt + u
        {
            EditControl();
        }
        else if (e.altKey && e.keyCode == 65)// alt + a
        {
            ActiveControl();
        }
        else if (e.altKey && e.keyCode == 82)// alt + r
        {
            DeactiveControl();
        }
        else if (e.altKey && e.keyCode == 88)// alt + x
        {
            DeleteControl();
        }
        //Extra
        else if (e.altKey && e.keyCode == 83)// alt + s
        {
            alts();
        }
        else if (e.altKey && e.keyCode == 68)// alt + d
        {
            altd();
        }
        else if (e.altKey && e.keyCode == 67)// alt + c
        {
            altc();
        }
        else if (e.altKey && e.keyCode == 84)// alt + t
        {
            altt();
        }
    }
    catch (err)
	{ }
});

function SelectRowUpdown(keyCode) {
    $(".selectonrowclick tr").each(function() {
        if ($(this).find('input:checkbox').eq(0).is(":checked")) {
            var IsChange = false;
            var IsFind = false;
            if (keyCode == 38) {
                IsFind = ($($(this).prev().closest("tr")).find('input:checkbox').eq(0).length > 0);
                if (IsFind) {
                    if ($(this).prev() == null)
                        IsChange = true;
                    $(this).prev().find('input:checkbox').eq(0).prop("checked", true);
                    SetTrstyle($(this).prev(), true);
                }
            }
            else if (keyCode == 40) {
                IsFind = ($($(this).next().closest("tr")).find('input:checkbox').eq(0).length > 0);
                if (IsFind) {
                    if ($(this).next() == null)
                        IsChange = true;
                    $(this).next().find('input:checkbox').eq(0).prop("checked", true);
                    //alert($(this).next('input:checkbox').eq(0).is(":checked"));                
                    SetTrstyle($(this).next(), true);
                }
            }

            if (IsFind) {
                $(this).find('input:checkbox').eq(0).prop("checked", IsChange);
                SetTrstyle($(this), true);
            }
            return false;

        }
    });
}