$(".datepicker, .datepickerfuture, .datepickerpast, .datemask").mask("99-99-9999");
$(".timepicker24mask").mask("99:99");
$(".datetimepicker24, .datetimepicker24future, .datetimepicker24past").mask("99-99-9999 99:99");
//$('.timepicker').timepicker();
$(".timepicker").clockface({ format: 'hh:mm A' });


$('.datepicker').datetimepicker({
	format: 'DD-MM-YYYY',
	useCurrent: false
});

$('.datepickerfuture').datetimepicker({
	format: 'DD-MM-YYYY',
	useCurrent: false,
	minDate: new Date()
});

$('.datepickerpast').datetimepicker({
	format: 'DD-MM-YYYY',
	useCurrent: false,
	maxDate: new Date()
});

$('.datetimepicker24').datetimepicker({
	format: 'DD-MM-YYYY HH:mm',
	sideBySide: true,
	//inline: true,
	useCurrent: false
});

$('.datetimepicker24future').datetimepicker({
	format: 'DD-MM-YYYY HH:mm',
	sideBySide: true,
	minDate: new Date(),
	useCurrent: false
});

$('.datetimepicker24past').datetimepicker({
	format: 'DD-MM-YYYY HH:mm',
	sideBySide: true,
	maxDate: new Date(),
	useCurrent: false
});

$('input').not(".autocomplete").attr('autocomplete', 'off');

$(".side-toggle").click(function () {
	//$(this).next().toggle("slide");
	$(this).next().toggle();
	$(this).next().addClass("zoomInRight animated");
});
//Loadr On Self-----------------------------------------------
$(".rowloader tr td").dblclick(function () {
	$(this).parent().addClass("load-row");
	NotAllowClick("");
});

$(".chkGridRow").change(function () {
	$(this).parent().parent().addClass("load-row");
	NotAllowClick("");
});

$(".editrowclick").click(function () {
	$(this).parent().parent().addClass("load-row");
	NotAllowClick("");
});

$(".clickloader").click(function () {
	$(this).addClass("loader-stripe");
	NotAllowClick("");
});

$(".SpinButton").click(function () {
	$(this).addClass("spin");
	NotAllowClick("");
});

$(".SpinsButton").click(function () {
	$(this).addClass("spins");
	NotAllowClick("");
});

$(".btnspinner").click(function () {
	$(this).children().addClass("spin");
	NotAllowClick("");
});

//Loadr On Control With ClassName-----------------------------------------------
function addLoader(ClassName) {
	$('.' + ClassName).addClass("loader-stripe");
	NotAllowClick("");
}

function addSpinner(ClassName) {
	$('.' + ClassName).addClass("spin");
	NotAllowClick("");
}

function addRegionLoader(ClassName) {
	$('.' + ClassName).addClass("csspinner traditional");
	NotAllowClick("");
}

function removeRegionLoader(ClassName) {
	$('.' + ClassName).removeClass("csspinner traditional");
	AllowClick("");
}

function removeLoader(ClassName) {
	$('.' + ClassName).removeClass("loader-stripe");
	AllowClick("");
}

function addSpinsLoader(ClassName) {
	$('.' + ClassName).addClass("spins");
	NotAllowClick("");
}

function addHotelSearchLoader(ClassName) {
	$('.' + ClassName).addClass("csspinnerimg");
	NotAllowClick("");
}

function removeHotelSearchLoader(ClassName) {
	$('.' + ClassName).removeClass("csspinnerimg");
	AllowClick("");
}


function NotAllowClick(ClassName) {
	if (ClassName == "")
		ClassName = "checkallowclick";

	$('.' + ClassName).css('pointer-events', 'none');
	$('.' + ClassName).addClass('NotAllowClick');
	$('.' + ClassName).removeClass('AllowClick');
}

function AllowClick(ClassName) {
	if (ClassName == "")
		ClassName = "checkallowclick";

	$('.' + ClassName).css('pointer-events', 'painted');
	$('.' + ClassName).addClass('AllowClick');
	$('.' + ClassName).removeClass('NotAllowClick');
}

//Select All Checkbox-----------------------------------------------
function SelectAll(AllClassName, SingleClassName) {
	var chk = $('.' + AllClassName + ' input[type="checkbox"]').prop('checked');
	$('.' + SingleClassName + ' input[type="checkbox"]').each(function () {
		$(this).prop("checked", chk);
	});
}

//Select Header Checkbox If All Checkbox Checked-----------------------------------------------
function SelectAllCheck(AllClassName, SingleClassName) {
	var AllSelect = true;
	$('.' + SingleClassName + ' input[type="checkbox"]').each(function () {
		if (!$(this).prop('checked')) {
			AllSelect = false;
		}
	});
	$('.' + AllClassName + ' input[type="checkbox"]').prop("checked", AllSelect);
}

//Gritter Message
function AlertMsg(title, DisplaySecond, desc, cls) {
	$.gritter.removeAll();
	$.gritter.add({
		title: title,
		text: desc,
		class_name: cls,
		time: DisplaySecond
	});
}

function SetSuccessMessage(Message) {
	AlertMsg("Success", 3000, Message, "with-icon check-circle success");
}

function SetErrorMessage(Message) {
	AlertMsg("Error", 3000, Message, "with-icon times-circle danger");
}

function CloseSlidebar() {
	$(".main-sidebar").addClass("closed");
}

function OpenSlidebar() {
	$(".main-sidebar").removeClass("closed");
}

//Hide toltrip after postback
$(".popover, .select2-dropdown--below").hide();

//Error : Notify Null In Chrome-----------------------------------------------
Sys.Browser.WebKit = {};
if (navigator.userAgent.indexOf('WebKit/') > -1) {
	Sys.Browser.agent = Sys.Browser.WebKit;
	Sys.Browser.version = parseFloat(navigator.userAgent.match(/WebKit\/(\d+(\.\d+)?)/)[1]);
	Sys.Browser.name = 'WebKit';
}

Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequestHandler);
function EndRequestHandler(sender, args) {
	if (args.get_error() != undefined) {
		args.set_errorHandled(true);
	}
}

//Select File Then Set Focus
$('input[type=file]').change(function () {
	$(this).parent().parent().next().find('input').focus();
});


$(window).resize(function () {
	SetAlltabHeight();
});

SetAlltabHeight();

function SetAlltabHeight() {
	var i = 0;
	for (i = 1; i < 12; i++) {
		var tabHeight = parseInt($(".lbltabHeight" + i).text());
		if (!isNaN(tabHeight)) {
			if (parseInt(document.documentElement.clientWidth) < 768) {
				var tabHeightSmall = parseInt($(".lbltabHeightSmall" + i).text());
				if (!isNaN(tabHeightSmall))
					tabHeight = tabHeightSmall;
			}

			setTabHeight(tabHeight, i);
		}
	}

	$(".CustomScroll").mCustomScrollbar({
		theme: "minimal-dark",
		scrollInertia: 500
	});
}

function setTabHeight(tabHeight, i) {
	var tabClass = "tabHeight" + i;
	var clientWidth = parseInt(document.documentElement.clientWidth);

	var lblTabHeight = $(".lbltabHeight" + i);
	var minwidth = parseInt(lblTabHeight.attr("min-width"));
	if (isNaN(minwidth)) {
		minwidth = 0;
	}

	if (minwidth == 0 || clientWidth > minwidth) {
		var html = document.documentElement;
		var tabheight = parseInt(html.clientHeight);

		$('.' + tabClass).css("height", (tabheight - tabHeight));
		$('.' + tabClass).addClass("tabHeight");
		$('.' + tabClass).attr("tabClass", tabClass);

		//11 For Manu
		if (i == 11
			|| (clientWidth > 768 && lblTabHeight.attr("SetCustomScroll") == "true")) {
			$('.' + tabClass).addClass("CustomScroll");
		}
	}
}

//Create Search Dropdown
/*$("select").each(function()
{
if(!$(this).hasClass("select-search") && !$(this).hasClass("ddlNotSearch"))
{
$(this).addClass("select-search");
}
});
$('.select-search').select2();*/

SetAllowOnbtn();

function SetAllowOnbtn() {
	var SelectRow = GetGridSelectRowCount();
	$('body *[allowon]').each(function () {
		var btn = $(this);
		var allowon = btn.attr("allowon");
		if ((allowon == SelectRow) || (allowon > 1 && SelectRow > 0))
			btn.removeClass("disabled");
		else
			btn.addClass("disabled");
	});
}

//Single Select In Grid TD Click
//$(".selectonrowclick td").click(function () {

//});

$(document).off('click', '.selectonrowclick td').on('click', '.selectonrowclick td', function (e) {
	SetTrstyle($(this), false);
	SetAllowOnbtn();
});

$(".selectonrowclick td").each(function () {
	SetTrstyle($(this), true);
});

function SetTrstyle(element, IsCheck) {
	var tr = $(element.closest("tr"));
	var checkbox = tr.find('input:checkbox').eq(0);
	//checkbox = $('#'+checkbox.attr('id'))
	if (!IsCheck) {
		var IsAllowMultipale = checkbox.parent().hasClass("AllowMultiple");
		var IsSelect = (!checkbox.is(":checked"));
		$(".selectonrowclick tr").each(function () {
			if (!IsAllowMultipale)
				$(this).find('input:checkbox').eq(0).prop("checked", false);

			SetTrstyle($(this), true);
		});

		checkbox.prop("checked", IsSelect);
	}

	if (checkbox.is(":checked"))
		tr.addClass("select-tr");
	else
		tr.removeClass("select-tr");
}

//Single Select In Grid TH Click
$(".selectonrowclick th").click(function () {
	SetThstyle($(this));
	SetAllowOnbtn();
});

function SetThstyle(element) {
	var tr = $(element.closest("tr"));

	var IsSelect = false;
	$(".selectonrowclick tr").each(function () {
		if ($(this).find('input:checkbox').eq(0).is(":checked")) {
			IsSelect = true;
		}
	});

	$(".selectonrowclick tr").each(function () {
		$(this).find('input:checkbox').eq(0).prop("checked", !IsSelect);
		SetTrstyle($(this), true);
	});
}

SetFixScrool();

function SetFixScrool() {
	var i = 0;
	for (i = 1; i < 12; i++) {
		var tabHeight = parseInt($(".lbltabHeight" + i).text());
		if (!isNaN(tabHeight)) {
			var ScroolPosition = parseInt($(".lblScrooltabHeight" + i).text());
			if (isNaN(ScroolPosition)) {
				$("#div_position").append("<label class='lblScrooltabHeight" + i + "'>0</label>");
				ScroolPosition = 0;
			}

			$(".tabHeight" + i).scrollTop(ScroolPosition);
		}
	}

	$(".tabHeight").scroll(function () {
		var tabclass = $(this).attr("tabclass");
		$('.lblScrool' + tabclass).text($("." + tabclass).scrollTop());
	});

	var ScroolPositionpopupscrool = parseInt($(".lblScroolpopupscrool").text());
	if (isNaN(ScroolPositionpopupscrool)) {
		$("#div_position").append("<label class='lblScroolpopupscrool'>0</label>");
		ScroolPositionpopupscrool = 0;
	}

	$(".divPopupArea").scrollTop(ScroolPositionpopupscrool);
	$(".divPopupArea").scroll(function () {
		$('.lblScroolpopupscrool').text($(".divPopupArea").scrollTop());
	});
}

function ClearScrool() {
	$(".lblScroolpopupscrool").text("0");
}

function GetGridSelectRowCount() {
	return $(".selectonrowclick tr input:checkbox:checked").length;
}

function GetGridSelectRowValue(lblValue) {
	var val = ""

	$(".selectonrowclick tr").each(function () {
		if ($(this).find('input:checkbox').eq(0).is(":checked")) {
			val = $(this).find('.' + lblValue).eq(0).text();
		}
	});

	return val;
}

FixGridHeader(true);

$(window).resize(function () {
	FixGridHeader(true);
});

//Set Grid Header Fixed
function FixGridHeader(IsSetTimeOut) {
	if ($(".fixheader").length > 0) {
		$(".fixheader thead").css("position", "relative");

		//Remove Already Exisst Hidden TR
		if ($(".hiddentr").length > 0)
			$(".hiddentr").remove();

		var clientWidth = parseInt(document.documentElement.clientWidth);
		if (clientWidth > 768) {

			var headerCellWidths = new Array();
			$('.fixheader tbody tr:first td').not(".hide").each(function (i) {
				headerCellWidths[i] = $(this).outerWidth();
			});

			$('.fixheader thead tr th').not(".hide").each(function (i) {
				$(this).css('width', headerCellWidths[i] - 0 + 'px');
			});


			$('.fixheader tbody tr:first td').not(".hide").each(function (i) {
				$(this).css('width', headerCellWidths[i] + 'px');
			});

			$(".fixheader thead").css("position", "fixed");

			var thHeight = $(".fixheader thead").outerHeight() - 2;
			if (thHeight > 0) {

				var firsttr = "<tr class='hiddentr' style='height: " + thHeight + "px;'>";

				$('.fixheader thead tr th').each(function (i) {
					if ($(this).hasClass("hide"))
						firsttr += "<td class='hide'></td>";
					else
						firsttr += "<td></td>";
				});
				firsttr += "</tr>";

				$(firsttr).prependTo(".fixheader.table > tbody");
			}

			if (IsSetTimeOut) {
				setTimeout(function () {
					FixGridHeader(false);
					//alert("Hello");
				}, 50);
			}
		}
	}
}

function ClearMaxScrollHeight() {
	$('.lblMaxScrollHeight').text('0');
}


//Desabal Back Button In Browser
function disableBack() {
	//window.history.forward(); 
}

window.onload = disableBack();

window.onpageshow = function (evt) { if (evt.persisted) disableBack() }


function AdjustTextaria(GetClassName, SetClassName) {

	var txt = $("." + GetClassName);
	var length = txt.val().length;
	//alert(length);
	var fontSize = "16";
	if (length > 918) {
		fontSize = "12";
	}
	else if (length > 765) {
		fontSize = "13";
	}
	else if (length > 612) {
		fontSize = "14";
	}
	else if (length > 459) {
		fontSize = "15";
	}

	$("." + GetClassName + ", ." + SetClassName).css("height", "1px");

	$("." + GetClassName + ", ." + SetClassName).css("font-size", fontSize + "px");
	$("." + GetClassName + ", ." + SetClassName).css("height", (25 + txt.prop('scrollHeight')) + "px");
}

function ManageDetailTab(arractivetab, arractivepanel, activetab, activepanel, txtsettabindex) {
	arractivetab = arractivetab.split(',');
	arractivetab.forEach(function (item) {
		$("." + item).removeClass("active");
	});

	arractivepanel = arractivepanel.split(',');
	arractivepanel.forEach(function (item) {
		$("." + item).hide();
	});

	if (activetab == "" || activepanel == "") {
		var indextab = parseInt($("." + txtsettabindex).val());
		if (isNaN(indextab) || indextab < 0)
			indextab = 0;

		activetab = arractivetab[indextab];
		activepanel = arractivepanel[indextab];
	}

	$("." + txtsettabindex).val(arractivetab.indexOf(activetab));

	$("." + activetab).addClass("active");
	$("." + activepanel).show();
}

function GetddmmmyyyyDate(strdate) {
	if (strdate != "") {
		var monthNames = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];

		var newdate = strdate.split("-")
		var date = new Date(newdate[2] + "-" + newdate[1] + "-" + newdate[0]);

		return newdate[0] + " " + monthNames[date.getMonth()] + " " + newdate[2];
	}
	else {
		return strdate;
	}
}

function GetddmmmyyhhmmDate(strdate) {
	if (strdate != "") {
		var monthNames = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];

		var newdate = strdate.split("-")
		var yeardate = newdate[2].split(" ");

		var date = new Date(yeardate[0] + "-" + newdate[1] + "-" + newdate[0]);

		var Day = newdate[0];
		var Month = monthNames[date.getMonth()];
		var Year = yeardate[0].toString().substr(2, 2);

		var trimtime = yeardate[1].split(":");
		return Day + " " + Month + " " + Year + " (" + trimtime[0] + "." + trimtime[1] + ")";
	}
	else {
		return strdate;
	}
}

function GetDate(txtdateval) {
	if (txtdateval != "") {
		var newdate = txtdateval.split("-")
		return (new Date(newdate[2] + "-" + newdate[1] + "-" + newdate[0]));
	}
	else {
		return txtdateval;
	}
}

function AddDays(strdate, Days) {
	if (strdate != "") {
		var newdate = strdate.split("-")
		var date = new Date(newdate[2] + "-" + newdate[1] + "-" + newdate[0]);
		date.setDate(date.getDate() + Days)

		return date.getDate() + "-" + (date.getMonth() + 1) + "-" + date.getFullYear();
	}
	else {
		return strdate;
	}
}

function todayDate() {
	var today = new Date();
	var dd = today.getDate();
	var mm = today.getMonth() + 1; //January is 0!

	var yyyy = today.getFullYear();
	if (dd < 10)
		dd = '0' + dd;
	if (mm < 10)
		mm = '0' + mm;
	return dd + '-' + mm + '-' + yyyy;
}

function dateDiff(first, second) {
	// Take the difference between the dates and divide by milliseconds per day.
	// Round to nearest whole number to deal with DST.
	return Math.round((second - first) / (1000 * 60 * 60 * 24));
}

//Custom Attribute
$.fn.hasAttr = function (name) {
	return this.attr(name) !== undefined;
};

function IsValidRowSelection() {
	var SelectRow = GetGridSelectRowCount();
	if (SelectRow == 0) {
		SetErrorMessage("Please Select Record.");
		return false;
	}
	else if (SelectRow > 1) {
		SetErrorMessage("You Can Select Only One Record At A Time.");
		return false;
	}
	else {
		return true;
	}
}

$(".capitalize").keyup(function (e) {
	if (e.which >= 65 && e.which <= 90)
		$(this).val($(this).val().toLowerCase());
	else
		e.preventDefault();
});




//Open New Tab From C# By JScript-----------------------------------------------
function OpenNewWindow(url) {
	window.open(url, "_blank");
	//var newWindow = window.open(url, "_blank");
	//newWindow.location.href = url;
	//window.open(url);
}

function OpenNewPopupWindow(url) {
	//var newWindow = window.open(url);
	window.open(url, 'Print', 'scrollbars=1, width=1000, height=600, toolbar=0, resizable=0');
}

//Close Current Tab From C# By JScript-----------------------------------------------
function CloseWindow() {
	//window.opener = self;
	window.close();
	window.opener.location.reload();
}

//Open New Tab From C# By JScript-----------------------------------------------
function OpenPrintWindow(url) {
	var printWindow = window.open(url, 'Print', 'scrollbars=1, width=1000, height=600, toolbar=0, resizable=0');
	printWindow.addEventListener('load', function () {
		printWindow.print();
		printWindow.close();
	}, true);
}

//Download File
function DownloadFile(url, FileName) {
	var link = document.createElement("a");
	if (link.download !== undefined) {
		link.setAttribute("href", url);
		link.setAttribute("download", FileName);
		document.body.appendChild(link);
		link.click();
		document.body.removeChild(link);
	}
	else {
		alert('File export only works in Chrome, Firefox, and Opera.');
	}
}


CheckPopupOpen();

function CheckPopupOpen() {
	var IsPopupOpen = false;
	if ($('.modelpopup:visible').not(".notscrool").length > 0)
		IsPopupOpen = true;

	if (IsPopupOpen)
		$(".divPopupArea").addClass("popupscrool");
	else
		$(".divPopupArea").removeClass("popupscrool");
}

$(".ClosePopup").click(function () {
	return ClosePopup();
});

function ClosePopup() {
	AddAnimation("modal-dialog", "Out");
	setTimeout(function () {
		var i = 0;
		for (i = 1; i < 25; i++) {
			try { $find("PopupBehaviorID" + i).hide(); }
			catch (err) { }
		}
		CheckPopupOpen();
	}, 500);
	return false;
}

function ShowModel(ModelClass) {
	$('.' + ModelClass).modal();
	CheckPopupOpen();
}


function ShowDetailPopup(PopupBehaviorId, divLoader, textbox, value) {
	$("." + textbox).val(value);
	addRegionLoader(divLoader);
	AddAnimation("modal-dialog", "In");
	$find("PopupBehaviorID" + PopupBehaviorId).show();
	CheckPopupOpen();
}

function ShowPopupAndLoader(PopupBehaviorId, divLoader) {
	addRegionLoader(divLoader);
	AddAnimation("modal-dialog", "In");
	$find("PopupBehaviorID" + PopupBehaviorId).show();
	CheckPopupOpen();
}

function ShowOnlyPopup(PopupBehaviorId) {
	AddAnimation("modal-dialog", "In");
	$find("PopupBehaviorID" + PopupBehaviorId).show();
	CheckPopupOpen();
}

function AddAnimation(DivClass, InOut) {
	var Random = 1 + Math.floor(Math.random() * 6);
	var Div = $("." + DivClass);
	Div.removeClass("animated bounceInDown fadeIn fadeInUpBig bounceIn bounceInUp fadeInDownBig");
	Div.removeClass("bounceOutDown fadeOut fadeOutUpBig bounceOut bounceOutUp fadeOutDownBig");

	if (Random == 6)
		Div.addClass("animated bounce" + InOut + "Down");
	else if (Random == 5)
		Div.addClass("animated fade" + InOut);
	else if (Random == 4)
		Div.addClass("animated fade" + InOut + "UpBig");
	else if (Random == 3)
		Div.addClass("animated bounce" + InOut);
	else if (Random == 2)
		Div.addClass("animated bounce" + InOut + "Up");
	else
		Div.addClass("animated fade" + InOut + "DownBig");
}


/* Search Textbox */
function SetSearchtxtValue(txtSearch, txtSearchId, lblJSON) {

	$("." + txtSearch).removeClass("ComListLoading");
	var SearchValue = $("." + txtSearch).val().replace(/\s/g, "").toLowerCase();
	var IsFound = false;
	var setSearchValueId = "", setSearchValueName = "";

	if (SearchValue.length > 2 && $("." + lblJSON).text() != "") {
		var JSONparsed = JSON.parse($("." + lblJSON).text());
		for (var x in JSONparsed) {
			if (!IsFound && SearchValue == ((JSONparsed[x][1]).replace(/\s/g, "").toLowerCase())) {
				IsFound = true;
				setSearchValueId = (JSONparsed[x][0]);
				setSearchValueName = (JSONparsed[x][1]);
			}
		}
	}

	if (setSearchValueId == "0") {
		setSearchValueId = "";
		setSearchValueName = "";
	}

	$("." + txtSearchId).val(setSearchValueId);
	$("." + txtSearch).val(setSearchValueName);

	try { DestinationChange(); }
	catch (e) { }
}

function ClientPopulating(txtSearch, lblJSON) {
	$("." + txtSearch).addClass("ComListLoading");
	$("." + lblJSON).text("");
}

function ClientPopulated(txtSearch, lblJSON, Valuelist) {
	$("." + txtSearch).removeClass("ComListLoading");

	var IDs = [];
	for (var i = 0; i < Valuelist.childNodes.length; i++) {
		IDs.push([Valuelist.childNodes[i]["_value"], Valuelist.childNodes[i].innerHTML]);
	}

	$("." + lblJSON).text(JSON.stringify(IDs));
}

function setMasonry() {
	setTimeout(function () {
		$('.Masonry').masonry();
	}, 300);
}
