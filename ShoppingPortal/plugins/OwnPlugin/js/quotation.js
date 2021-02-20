LoadQuotationjs();
function LoadQuotationjs() {

	$(".btnSave").click(function () {
		if (CheckValidation("div-check-validation")) {
			addLoader('btnSave');
			return true;
		}
		else {
			return false;
		}
	});

	$(".txtQuotationName").keyup(function () {
		CheckQuotationDetail();
	});

	CheckQuotationDetail();
	function CheckQuotationDetail() {
		if ($(".txtQuotationName").val() != "") {
			$(".tdQuotationNameDetail").addClass("tdcomplitequotdetail");
		}
		else {
			$(".tdQuotationNameDetail").removeClass("tdcomplitequotdetail");
		}
	}

	//Room Detail
	$(".roomspinner").spinner('changing', function (e, newVal, oldVal) {
		var isHotelRoom = $(this).attr("ishotelroom");
		var isValidate = true;
		if ($(this).attr("ishotelroom") == "true"
			&& parseInt($(".ddlStayat").val()) == 1) {

			var MaxAllowRoom = parseInt($(".lblHotelMaxRoom").text());
			isValidate = (MaxAllowRoom >= parseInt(newVal));
			if (!isValidate) {
				$(this).val(oldVal);
				CreateSweetAlert("Hey ! You can add max " + MaxAllowRoom + " Room at a time.");
			}
		}

		if (isValidate)
			ManageRoom(isHotelRoom);
	});

	$(".adultspinner").spinner('changing', function (e, newVal, oldVal) {
		var tr = $(this).parent().parent().parent();
		if ((parseInt(tr.find(".txtChild").val()) + newVal) > 4)
			tr.find(".txtAdults").val(oldVal);

		ManageRoom($(this).attr("ishotelroom"));
	});

	$(".childspinner").spinner('changing', function (e, newVal, oldVal) {
		var tr = $(this).parent().parent().parent();
		if ((parseInt(tr.find(".txtAdults").val()) + newVal) > 4)
			tr.find(".txtChild").val(oldVal);

		ManageRoom($(this).attr("ishotelroom"));
	});

	$(".childagespinner, .hotelnightspinner").spinner('changing', function (e, newVal, oldVal) {
		ManageRoom($(this).attr("ishotelroom"));
	});

	$(".chkChild1WithBed, .chkChild2WithBed").change(function () {
		ManageRoom($(this).parent().attr("ishotelroom"));
	});


	ManageRoom('false');
	ManageRoom('true');
	function ManageRoom(IsHotelRoomDetail) {

		IsHotelRoomDetail = (IsHotelRoomDetail.toLowerCase() == 'true');

		var TotalRoom = 0;
		var trClass = "trRoomDetail";
		if (IsHotelRoomDetail) {
			TotalRoom = parseInt($(".txtTotalHotelRooms").val());
			trClass = "trHotelRoomDetail";

			if (parseInt($(".ddlStayat").val()) == 1) {
				var MaxAllowRoom = parseInt($(".lblHotelMaxRoom").text());
				if (MaxAllowRoom < TotalRoom) {
					$(".txtTotalHotelRooms").val(MaxAllowRoom);
					TotalRoom = MaxAllowRoom;
				}
			}
		}
		else {
			TotalRoom = parseInt($(".txtTotalRooms").val());
			trClass = "trRoomDetail";
		}

		var TotalValChild = 0;

		$("." + trClass).each(function () {
			var RoomNo = parseInt($(this).find(".lblRoomNo").text());

			var divChildAge1Room = $(this).find(".divChildAge1Room");
			var divChildAge2Room = $(this).find(".divChildAge2Room");

			var txtAdults = $(this).find(".txtAdults");
			var txtChild = $(this).find(".txtChild");
			var txtChildAge1 = $(this).find(".txtChildAge1");
			var txtChildAge2 = $(this).find(".txtChildAge2");

			var divChild1WithBed = $(this).find(".divChild1WithBed");
			var divChild2WithBed = $(this).find(".divChild2WithBed");

			var chkChild1WithBed = $(this).find(".chkChild1WithBed input[type='checkbox']");
			var chkChild2WithBed = $(this).find(".chkChild2WithBed input[type='checkbox']");

			if (RoomNo <= TotalRoom) {
				$(this).removeClass("hidden");

				var TotalAdult = parseInt(txtAdults.val());
				var TotalChild = parseInt(txtChild.val());

				if (TotalChild < 1) {
					divChildAge1Room.addClass("hidden");
					txtChildAge1.val("1");
				}
				else {
					divChildAge1Room.removeClass("hidden");
				}

				if (TotalChild < 2) {
					divChildAge2Room.addClass("hidden");
					txtChildAge2.val("1");
				}
				else {
					divChildAge2Room.removeClass("hidden");
				}

				/* Manage Extra Pax */

				if (TotalAdult <= 2 && parseInt(txtChildAge1.val()) > 2) {
					divChild1WithBed.removeClass("hidden");
				} else {
					divChild1WithBed.addClass("hidden");
					chkChild1WithBed.prop('checked', false);
				}

				if (TotalAdult <= 2 && parseInt(txtChildAge2.val()) > 2)
					divChild2WithBed.removeClass("hidden");
				else {
					divChild2WithBed.addClass("hidden");
					chkChild2WithBed.prop('checked', false);
				}


				if (chkChild1WithBed.prop('checked')) {
					divChild2WithBed.addClass("hidden");
					chkChild2WithBed.prop('checked', false);
				}

				if (chkChild2WithBed.prop('checked')) {
					divChild1WithBed.addClass("hidden");
					chkChild1WithBed.prop('checked', false);
				}

				/* End Manage Extra Pax */

				TotalValChild += TotalChild;
			}
			else {
				$(this).addClass("hidden");

				if (parseInt(txtAdults.attr("data-max")) == 1)
					txtAdults.val("1");
				else
					txtAdults.val("2");

				txtChild.val("0");
				txtChildAge1.val("1");
				txtChildAge2.val("1");

				divChildAge1Room.addClass("hidden");
				divChildAge2Room.addClass("hidden");

				chkChild1WithBed.prop('checked', false);
				chkChild2WithBed.prop('checked', false);
			}
		});

		var lblAgeOfChild = $("." + trClass).parent().find(".lblAgeOfChild");
		if (TotalValChild > 0)
			lblAgeOfChild.removeClass("hide");
		else
			lblAgeOfChild.addClass("hide");
	}

	//Nationality
	$(".ddlNationality").change(function () {
		SetNationality();
	});

	SetNationality();
	function SetNationality() {
		$(".lblNationality").text("Nationality " + $('.ddlNationality').find(":selected").text());
	}

	$(".aNationality").click(function () {
		$(".NationalityIconRight").toggleClass("hidden");
		$(".NationalityIconDown").toggleClass("hidden");
		$(".divNationality").toggleClass("hidden");
	});

	//Passenger Detail
	$(".btnDonePassengerDetail").click(function () {
		if (CheckValidation("div-validat-PassengerDetail")) {
			addLoader('btnDonePassengerDetail');
			return true;
		}
		else {
			return false;
		}
	});
	/* Passenger Detail */
	$(".txtDOB").change(function () {
		var txtDOBval = $(this).val();
		var txtAge = $(this).parent().parent().find(".txtAge");
		if (txtDOBval != "" && txtAge.val() == "") {
			var dob = GetDate(txtDOBval);
			var today = new Date();
			var age = Math.floor((today - dob) / (365.25 * 24 * 60 * 60 * 1000));
			txtAge.val(age);
		}
	});

	$(".txtDOB").change(function () {
		SetDOB($(this), $(this).parent().parent().find(".txtAge"));
	});

	$(".txtAge").blur(function () {
		SetDOB($(this).parent().parent().find(".txtDOB"), $(this));
	});

	$(".txtDOB").each(function () {
		SetDOB($(this), $(this).parent().parent().find(".txtAge"));
	});

	function SetDOB(txtDOB, txtAge) {
		var age = "";
		if (txtDOB.val() != "") {
			age = Math.floor((new Date() - GetDate(txtDOB.val())) / (365.25 * 24 * 60 * 60 * 1000));
			if (txtAge.val() == "")
				txtAge.val(age);
		}

		txtAge.css("color", (txtAge.val() != "" && txtDOB.val() != "" && txtAge.val() != age) ? "red" : "initial");
	}

	/* Hover Edit Textbox */
	$(".txthoveredit").each(function () {
		Settxthoveredit($(this));
	});

	$(".txthoveredit").blur(function () {
		Settxthoveredit($(this));
	});

	$(".txthoveredit").focus(function () {
		if ($(this).hasAttr("changetext"))
			$(this).attr("placeholder", $(this).attr("changetext"));
	});

	function Settxthoveredit(Control) {
		if (Control.val() == "") {
			Control.addClass("txthovereditactive");
			if (Control.hasAttr("addtext"))
				Control.attr("placeholder", Control.attr("addtext"));
		}
		else {
			Control.removeClass("txthovereditactive");
			if (Control.hasAttr("changetext"))
				Control.attr("placeholder", Control.attr("changetext"));
		}
	}

	//Hover Hide Show Price Textbox
	$('.txthoverhideprice, .txthovershowprice').mouseover(function () {
		SetHoverHideShowPrice($(this).parent().parent(), true, $(this).parent().attr("PaxType"), false);
	});
	$('.txthoverhideprice').focus(function () {
		SetHoverHideShowPrice($(this).parent().parent(), true, $(this).parent().attr("PaxType"), true);
	});
	$('.txthovershowprice').focus(function () {
		SetHoverHideShowPrice($(this).parent().parent(), true, $(this).parent().attr("PaxType"), true);
	});

	$('.txthoverhideprice, .txthovershowprice').mouseout(function () {
		if (!$(this).parent().find(".txthovershowprice").is(":focus"))
			SetHoverHideShowPrice($(this).parent().parent(), false, $(this).parent().attr("PaxType"), false);
	});
	$('.txthovershowprice').focusout(function () {
		SetHoverHideShowPrice($(this).parent().parent(), false, $(this).parent().attr("PaxType"), false);
	});


	function SetHoverHideShowPrice(tr, isShowHover, PaxType, IsSetFocus) {

		var tdsingleprice = tr.find(".tdsingleprice");
		var tdtwinprice = tr.find(".tdtwinprice");
		var tdadultprice = tr.find(".tdadultprice");
		var tdchildwbprice = tr.find(".tdchildwbprice");
		var tdchildnbprice = tr.find(".tdchildnbprice");

		var issingleprice = tdsingleprice.length > 0 && tdsingleprice.attr("PaxType") == PaxType;
		var istwinprice = tdtwinprice.length > 0 && tdtwinprice.attr("PaxType") == PaxType;
		var isadultprice = tdadultprice.length > 0 && tdadultprice.attr("PaxType") == PaxType;
		var ischildwbprice = tdchildwbprice.length > 0 && tdchildwbprice.attr("PaxType") == PaxType;
		var ischildnbprice = tdchildnbprice.length > 0 && tdchildnbprice.attr("PaxType") == PaxType;

		//console.log(ischildwbprice);

		if (issingleprice)
			tdsingleprice.addClass("hoverhideprice");

		if (istwinprice)
			tdtwinprice.addClass("hoverhideprice");

		if (isadultprice)
			tdadultprice.addClass("hoverhideprice");

		if (ischildwbprice)
			tdchildwbprice.addClass("hoverhideprice");

		if (ischildnbprice)
			tdchildnbprice.addClass("hoverhideprice");

		var SetShowControl = tdtwinprice;
		if (istwinprice)
			SetShowControl = tdtwinprice;
		else if (issingleprice)
			SetShowControl = tdsingleprice;
		else if (isadultprice)
			SetShowControl = tdadultprice;
		else if (ischildwbprice)
			SetShowControl = tdchildwbprice;
		else if (ischildnbprice)
			SetShowControl = tdchildnbprice;

		if (isShowHover) {
			var colspan = 0;
			if (issingleprice)
				colspan += 1;
			if (istwinprice)
				colspan += 1;
			if (isadultprice)
				colspan += 1;
			if (ischildwbprice)
				colspan += 1;
			if (ischildnbprice)
				colspan += 1;

			SetShowControl.removeClass("hoverhideprice");
			SetShowControl.addClass("hovershowprice");

			SetShowControl.attr('colspan', colspan);

			if (IsSetFocus)
				SetShowControl.find("input").focus();
		}
		else {
			if (issingleprice)
				tdsingleprice.removeClass("hoverhideprice");
			if (istwinprice)
				tdtwinprice.removeClass("hoverhideprice");
			if (isadultprice)
				tdadultprice.removeClass("hoverhideprice");
			if (ischildwbprice)
				tdchildwbprice.removeClass("hoverhideprice");
			if (ischildnbprice)
				tdchildnbprice.removeClass("hoverhideprice");

			SetShowControl.removeClass("hovershowprice");
			SetShowControl.attr('colspan', "1");
		}

		var setValue = SetShowControl.find("input").val();
		var txthoverprice = tr.find(".txthoverprice" + PaxType);
		if (!isNaN(setValue))
			setValue = setValue;
		else
			setValue = "";

		txthoverprice.val(setValue);
		Settxthoveredit(txthoverprice);
	}

	/* Manage Hotel & Inclusion */

	$(".aHotelPhotos").click(function () {
		//alert($(this).parent().parent().find(".imgHotelImage").attr("class"));
		//$(this).parent().parent().find(".imgHotelImage")[0].click();
		//$(this).parent().parent().find(".imgHotelImage").click();

		//alert($(this).parent().parent().find(".imgHotelImage").length);
		if ($(this).parent().parent().find(".imgHotelImage").length > 1) {
			$(this).parent().parent().find(".imgHotelImage")[0].click();
		}
		else {
			$(this).parent().parent().find(".imgHotelImage").click();
		}
	});

	$(".tblInclusion").tableDnD({
		dragHandle: ".lnkMoveInclusion",
		onDrop: function (table, row) {
			//console.log("onDrop");
			var tr = $("#" + row.id);
			var setTr = tr.prev();
			if (typeof (setTr.attr("id")) === "undefined")
				setTr = tr.next();

			var setddlInDate = setTr.find(".ddlInDate");
			tr.find(".ddlInDate").val(setddlInDate.val());

			SetInclusionSerialNo();
		}
	});

	SetInclusionSerialNo();
	function SetInclusionSerialNo() {
		var txtTravelDate = $(".txtTravelDate").val();
		var TravelDate = GetDate(txtTravelDate);
		var trInclusion = $(".trInclusionTrFackData").html();

		var SerialNo = 0;

		$(".tblInclusion").each(function (eInclusion) {

			SerialNo++;
			var tblInclusionClass = "tblInclusion" + SerialNo;
			$(this).addClass(tblInclusionClass);

			var trDestination = $(this).parent().parent();
			var lblFromDate = trDestination.find(".lblFromDate");
			var FromDate = GetDate(lblFromDate.text());
			var ToDate = GetDate((trDestination.find(".lblToDate")).text());

			var arrDay = new Array();
			var arrAddkNewDay = new Array();
			var arrAddkDay = new Array();
			var arrRemoveDay = new Array();
			var arrBlankDay = new Array();

			//Set arrDay
			for (var i = 0; i <= dateDiff(FromDate, ToDate); i++) {
				var Day = dateDiff(TravelDate, GetDate(AddDays(lblFromDate.text(), i))) + 1;
				arrDay.push(Day);
			}

			//Set arrAddkDay & arrBlankDay
			$(this).find("tr").each(function (e) {
				var ddlInDate = $(this).find(".ddlInDate");
				var Day = parseInt(ddlInDate.val());

                if (Day > 0) {
					var lbleStatus = $(this).find(".lbleStatus");
					if (parseInt(lbleStatus.text()) == 1)
						arrAddkDay.push(Day);
					else
						arrBlankDay.push(Day);
				}
			});

			//Find Remove Row
			arrAddkDay.forEach(function (item) {
				var index = arrBlankDay.indexOf(item);
				if (index !== -1) {
					arrBlankDay.splice(index, 1);
					arrRemoveDay.push(item);
				}
			});

			//Find New Added Row
			arrDay.forEach(function (item) {
				if (arrAddkDay.indexOf(item) < 0 && arrBlankDay.indexOf(item) < 0)
					arrAddkNewDay.push(item);
			});

			//Remove Balnk Row
			$(this).find("tr").each(function (e) {
				var lbleStatus = $(this).find(".lbleStatus");
				var ddlInDate = $(this).find(".ddlInDate");
				var Day = parseInt(ddlInDate.val());
				if (Day == 0 || (arrRemoveDay.indexOf(Day) >= 0 && parseInt(lbleStatus.text()) == 2)) // Remove Row 
					$(this).remove();
			});

			//Add Not Added Row
			arrAddkNewDay.forEach(function (item) {
				var trInclusionClass = "trInclusion" + item;
				$("." + tblInclusionClass + " tbody").append("<tr id='" + trInclusionClass + "' class='trInclusionData " + trInclusionClass + "'>" + trInclusion + "</tr>");

				//Assign Value	
				$("." + trInclusionClass + " .lblInclusionDay").text("DAY " + ("0" + item).slice(-2));
				$("." + trInclusionClass + " .lbleStatus").text("2");

				var DateFormate = GetddmmmyyyyDate(AddDays(txtTravelDate, item - 1)).split(' ');
				$("." + trInclusionClass + " .ddlInDate").attr("id", "ddlInDate" + item);
				$("." + trInclusionClass + " .ddlInDate").append("<option value='" + item + "'>" + (DateFormate[0] + " " + DateFormate[1]).toUpperCase() + "</option>");
			});
		});

		SortingInclusionDetail();

		$(".tblInclusion").each(function (eInclusion) {
			var OldDay = 0;
			$(this).find("tr").each(function (e) {
				var lblInclusionDay = $(this).find(".lblInclusionDay");
				var lblInclusionDate = $(this).find(".lblInclusionDate");
				var ddlInDate = $(this).find(".ddlInDate");
				var Day = parseInt(ddlInDate.val());
				if (Day > 0 && OldDay != Day) {
					OldDay = Day;
					lblInclusionDay.text("DAY " + ("0" + Day).slice(-2));
				}
				else
					lblInclusionDay.text("");

				if (Day > 0)
					lblInclusionDate.text($("option:selected", ddlInDate).text());
				else
					lblInclusionDate.text("");
			});
		});
	}

	$(".txtTimeSlot").change(function () {
		SetInclusionSerialNo();
	});

	function SortingInclusionDetail() {
		$(".tblInclusion").each(function () {
			$(this).find(".trInclusionData").each(function () {
				var Tr = $(this);
				var Date = parseFloat(Tr.find(".ddlInDate").val());
				if (Date == 0)
					Date = 100;

				try {
					var Time = (Tr.find(".txtTimeSlot").val().split("-")[0]).replace(":", "");
					if (isNaN(parseFloat(Time)))
						Time = "2461";

					Date = parseFloat(Date.toString() + Time.toString());
				}
				catch (e) {
					Date = parseFloat(Date.toString() + "2461");
				}

				//var Type = parseInt(Tr.find(".lblInclusionAddType").text());

				var position = (Date == 0 ? 3000 : Date);
				//if (Type == 5)//5 == Manually
				//	position = position + 9999;

				Tr.attr("position", position);
			});

			var $table = $(this);
			var rows = $table.find('.trInclusionData').get();
			rows.sort(function (a, b) {
				var keyA = parseFloat($(a).attr('position'));
				var keyB = parseFloat($(b).attr('position'));
				if (keyA > keyB) return 1;
				if (keyA < keyB) return -1;
				return 0;
			});

			$.each(rows, function (index, row) {
				$table.children('tbody').append(row);
			});
		});
	}

	/* Hotel Detail */
	$(".setHotelDetailLoader").click(function () {
		var IsSetLoader = true;
		var TotalCount = 0;
		var Control = $(this);

		while (IsSetLoader) {
			if (Control.attr("class") != undefined && Control.attr("class").indexOf("tdHotelLoader") >= 0) {
				$("#" + Control.attr("id")).addClass("csspinner traditional");
				IsSetLoader = false;
			}
			else {
				Control = Control.parents();
			}

			TotalCount++;
			if (TotalCount > 5)
				IsSetLoader = false;
		}
	});

	/* Inclusion Detail */
	$(".setInclusionDetailLoader").click(function () {
		var IsSetLoader = true;
		var TotalCount = 0;
		var Control = $(this);

		while (IsSetLoader) {
			if (Control.attr("class") != undefined && Control.attr("class").indexOf("tdInclusionLoader") >= 0) {
				$("#" + Control.attr("id")).addClass("csspinner traditional");
				IsSetLoader = false;
			}
			else {
				Control = Control.parents();
			}

			TotalCount++;
			if (TotalCount > 8) {
				IsSetLoader = false;
			}
		}
    });

    $(".ddlInclusionDetailLoader").change(function () {
        var IsSetLoader = true;
        var TotalCount = 0;
        var Control = $(this);

        while (IsSetLoader) {
            if (Control.attr("class") != undefined && Control.attr("class").indexOf("tdInclusionLoader") >= 0) {
                $("#" + Control.attr("id")).addClass("csspinner traditional");
                IsSetLoader = false;
            }
            else {
                Control = Control.parents();
            }

            TotalCount++;
            if (TotalCount > 8) {
                IsSetLoader = false;
            }
        }
    });

	/* Search Hotel */
	if ($(".lblMinPrice").attr("class") != undefined) {
		var minPrice = parseInt($(".lblMinPrice").text());
		var maxPrice = parseInt($(".lblMaxPrice").text());

		var minSearchPrice = parseInt($(".txtSearchMinPrice").val());
		var maxSearchPrice = parseInt($(".txtSearchMaxPrice").val());

		if ($.isNumeric(minPrice) && $.isNumeric(maxPrice) && $.isNumeric(minSearchPrice) && $.isNumeric(maxSearchPrice)) {
			$("#searchpricerange").slider({
				range: true,
				min: minPrice,
				max: maxPrice,
				values: [minSearchPrice, maxSearchPrice],
				slide: function (event, ui) {
					SetPriceSearchValue(ui.values[0], ui.values[1]);
				}, stop: function (event, ui) {
					// ui.values[0];
					//ui.values[1];
				}
			});

			SetPriceSearchValue($("#searchpricerange").slider("values", 0), $("#searchpricerange").slider("values", 1));
		}
	}

	function SetPriceSearchValue(MinPrice, MaxPrice) {
		$("#searchpriceselect").text("From: " + MinPrice + " INR  " + "To: " + MaxPrice + " INR");
		$(".txtSearchMinPrice").val(MinPrice);
		$(".txtSearchMaxPrice").val(MaxPrice);
	}

	$(".ddlStayat").change(function () {
		SetStayatPosition();
		ManageRoom('true');
	});

	SetStayatPosition();
	function SetStayatPosition() {
		var StayAt = parseInt($(".ddlStayat").val());
		if (StayAt == 1) {
			//Hotel
			$(".divfilterby, .divHotelRoomDetail, .divHotelList, .divHotelListNotFound").show();
			$(".divManuallHotelDetail").hide();
			$(".lnkHotelSearchModifySearch").text("Search");
		}
		else {
			$(".divfilterby, .divHotelRoomDetail, .divManuallHotelDetail, .divHotelList, .divHotelListNotFound").hide();
			$(".lnkHotelSearchModifySearch").text("Add");

			if (StayAt == 10) {
				//Manual Hotel
				$(".divHotelRoomDetail").show();
				$(".divManuallHotelDetail").show();
			}
		}
	}

	$(".lnkHotelSearchModifySearch").click(function () {
		var IsValid = true;
		var Stayat = parseInt($(".ddlStayat").val());
		if (Stayat == 1 || Stayat == 10) {

			//var TotalAdult = GetTotalPax(true, false);
			//var TotalHotelAdult = GetTotalPax(true, true);
			//if (TotalAdult < TotalHotelAdult) {
			//	IsValid = false;
			//	CreateSweetAlert("Hey ! You can add more adult(s) in quotation page only.");
			//}

			//if (IsValid) {
			//	var TotalChild = GetTotalPax(false, false);
			//	var TotalHotelChild = GetTotalPax(false, true);
			//	if (TotalChild < TotalHotelChild) {
			//		IsValid = false;
			//		CreateSweetAlert("Hey ! You can add more child(s) in quotation page only.");
			//	}
			//}

			if (Stayat == 10) {
				if (IsValid) {
					if ($(".txtManuallHotelName").val() == "") {
						IsValid = false;
						CreateSweetAlert("Hey ! Please Enter Hotel Name.");
					}
				}

				if (IsValid) {
					if ($(".txtManuallHotelNetRate").val() == "") {
						IsValid = false;
						CreateSweetAlert("Hey ! Please Enter Net Rate.");
					}
				}

				if (IsValid) {
					if ($(".txtManuallHotelRoomType").val() == "") {
						IsValid = false;
						CreateSweetAlert("Hey ! Please Enter Room Type.");
					}
				}
			}
		}

		if (IsValid) {
			SetHotelSearchLoader();
			//addRegionLoader('LoaderHotelDetail');
		}

		return IsValid;
	});

	var slideIndex = 1;
	var intHotelSearchLoader;
	clearInterval(intHotelSearchLoader);

	function SetHotelSearchLoader() {
		showDivs(slideIndex);
		intHotelSearchLoader = setInterval(function () {
			showDivs(slideIndex += 1);
		}, 5000);

		$(".divSelectHotel").addClass('hide');
		$(".divSearchHotelLoader").removeClass('hide');

		$(".pnlSelectHotel").attr('class', 'pnlSelectHotel modelpopup notscrool col-lg-6 col-md-6 col-sm-8 col-xs-12 p0');
		$find("PopupBehaviorID2").show();
	}

	function showDivs(n) {
		var i;
		var x = document.getElementsByClassName("imgHotelSearchLoader");
		if (x.length > 0) {
			if (n > x.length) { slideIndex = 1 }
			if (n < 1) { slideIndex = x.length };
			for (i = 0; i < x.length; i++) {
				x[i].style.display = "none";
			}

			x[slideIndex - 1].style.display = "inline";
		}
	}

	$("#lblIsPageReady").text("0");
}

function CreateSweetAlert(Messsage) {
	swal({
		html: true,
		title: 'Warning',
		text: Messsage,
		confirmButtonColor: "#d23939",
		confirmButtonText: "Oky!!"
	});
}

//???
function GetTotalPax(GetTotalAdult, IsHotelPax) {
	var TotalRoom = IsHotelPax ? parseInt($(".txtTotalHotelRooms").val()) : parseInt($(".txtTotalRooms").val());
	var trClassName = IsHotelPax ? "trHotelRoomDetail" : "trRoomDetail";

	var TotalPax = 0;

	$("." + trClassName).each(function () {
		var RoomNo = parseInt($(this).find(".lblRoomNo").text());
		if (RoomNo <= TotalRoom) {
			if (GetTotalAdult) {
				TotalPax += parseInt($(this).find(".txtAdults").val());
			}
			else {
				TotalPax += parseInt($(this).find(".txtChild").val());
			}
		}
	});

	return TotalPax;
}

$(".SetAccordion").click(function (e) {
	if (!$(e.target).hasClass("notcolspan"))
		SetAccordion($(this).parent().attr("accordioncontrol"));
});

function SetFocus(control) {
	control.trigger('focus');

	var strLength = control.val().length * 2;
	control[0].setSelectionRange(strLength, strLength);
}

function ShowCancellationPolicy() {
	$("#lblIsPageReady").text("1");
	var myVar = setInterval(function () {
		if ($("#lblIsPageReady").text() == "0") {
			ShowModel("cancellation-policy-modal");
			clearInterval(myVar);
		}
	}, 500);
}

function LoadHotelDetail() {
	$("#lblIsPageReady").text("1");
	var myVar = setInterval(function () {
		if ($("#lblIsPageReady").text() == "0") {
			ShowModel('hotel-detail-modal');
			$('#carousel-hotel-image').carousel('cycle');
			clearInterval(myVar);
		}
	}, 500);
}