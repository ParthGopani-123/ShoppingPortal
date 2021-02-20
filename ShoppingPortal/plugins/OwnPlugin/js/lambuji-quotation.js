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

	/* Set Arrival Date */
	//$('.divarrivaldate').click(function () {
	//	$('.txtArrivalDate').trigger('focus');
	//});

	//Room Detail
	$(".btnDoneRoomDetail").click(function () {
		if (!CheckValidation("validateRoomDetail"))
			return false;

		$(".divRoomDetail").slideUp();
		AllowClick("");

		var ReturnValue = false;
		if ($(".txtTotalRooms").val() != $(".lblTotalRoomsOld").text())
			ReturnValue = true;

		if (!ReturnValue) {
			$(".trRoomDetail").each(function () {
				if (!ReturnValue
					&& parseInt($(".txtTotalRooms").val()) >= parseInt($(this).find(".lblRoomNo").text())) {
					var Adults = $(this).find(".txtAdults").val();
					var Child = $(this).find(".txtChild").val();
					var ChildAge1 = $(this).find(".txtChildAge1").val();
					var ChildAge2 = $(this).find(".txtChildAge2").val();
					var chkChild1WithBed = $(this).find('.chkChild1WithBed  input[type="checkbox"]').prop('checked');
					var chkChild2WithBed = $(this).find('.chkChild2WithBed  input[type="checkbox"]').prop('checked');

					var AdultsOld = $(this).find(".lblAdultsOld").text();
					var ChildOld = $(this).find(".lblChildOld").text();
					var ChildAge1Old = $(this).find(".lblChildAge1Old").text();
					var ChildAge2Old = $(this).find(".lblChildAge2Old").text();
					var chkChild1WithBedOld = $(this).find('.chkChild1WithBedOld  input[type="checkbox"]').prop('checked');
					var chkChild2WithBedOld = $(this).find('.chkChild2WithBedOld  input[type="checkbox"]').prop('checked');

					if (Adults != AdultsOld)
						ReturnValue = true;

					if (Child != ChildOld)
						ReturnValue = true;

					if (chkChild1WithBed != chkChild1WithBedOld)
						ReturnValue = true;

					if (chkChild2WithBed != chkChild2WithBedOld)
						ReturnValue = true;

					if (Child >= 1) {
						if (ChildAge1 != ChildAge1Old)
							ReturnValue = true;
					}

					if (Child >= 2) {
						if (ChildAge2 != ChildAge2Old)
							ReturnValue = true;
					}
				}
			});
		}

		if (!ReturnValue) {
			if ($(".ddlNationality").val() != $(".lblNationalityOld").text())
				ReturnValue = true;
		}

		var IsCheckDoneRoomDetail = $(".txtCheckRoomStatus").val() == "1";

		$(".txtCheckRoomStatus").val("0");

		if (ReturnValue) {
			addRegionLoader('loaderaccordion');
		}
		else if (IsCheckDoneRoomDetail) {
			CheckQuotationDetail();
			ManageRoom('false');
		}

		return ReturnValue;
	});

	$(".ManageRoomDetail").click(function () {
		AllowClick("divRoomDetail");
		if ($(".divRoomDetail").is(':hidden')) {
			NotAllowClick("");
			$(".divRoomDetail").slideDown();
			$(".txtTotalRooms").focus();
		}
		else {
			AllowClick("");
			$(".divRoomDetail").slideUp();
		}
	});

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

		var TotalGuests = 0;

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

				if (!IsHotelRoomDetail)
					TotalGuests += (TotalAdult + TotalChild);
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

		if (!IsHotelRoomDetail) {
			if ($(".txtCheckRoomStatus").val() == "1") {
				$(".ManageRoomDetail").text("Select Room");
				$(".ManageRoomDetail").addClass("link");
			} else {
				$(".ManageRoomDetail").text(TotalRoom + " Rooms / " + TotalGuests + " Guests");
				$(".ManageRoomDetail").removeClass("link");
			}
		}
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

		if ($(".txtCheckRoomStatus").val() == "0") {
			$(".tdRoomPaxDetail").addClass("tdcomplitequotdetail");
		}
		else {
			$(".tdRoomPaxDetail").removeClass("tdcomplitequotdetail");
		}
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
		SetHoverHideShowPrice($(this).parent().parent(), true, $(this).attr("PaxType"));
	});
	$('.txthoverhideprice').focus(function () {
		SetHoverHideShowPrice($(this).parent().parent(), true, $(this).attr("PaxType"));
		$(this).parent().parent().find(".txthovershowprice").focus();
	});
	$('.txthovershowprice').focus(function () {
		SetHoverHideShowPrice($(this).parent().parent(), true, $(this).attr("PaxType"));
	});

	$('.txthoverhideprice, .txthovershowprice').mouseout(function () {
		if (!$(this).parent().find(".txthovershowprice").is(":focus"))
			SetHoverHideShowPrice($(this).parent().parent(), false, $(this).attr("PaxType"));
	});
	$('.txthovershowprice').focusout(function () {
		SetHoverHideShowPrice($(this).parent().parent(), false, $(this).attr("PaxType"));
	});


	function SetHoverHideShowPrice(tr, isShowHover, PaxType) {

		var IsAdult = PaxType == "adult";
		if (IsAdult) {
			var tdsingleprice = tr.find(".tdsingleprice");
			var tdadultprice = tr.find(".tdadultprice");
			var tdtwinprice = tr.find(".tdtwinprice");

			if (isShowHover) {
				tdsingleprice.addClass("hoverhideprice");
				tdadultprice.addClass("hoverhideprice");
				tdtwinprice.addClass("hovershowprice");

				var isadultprice = $('.tdadultprice:visible').length > 0;
				var issingleprice = $('.tdsingleprice:visible').length > 0;
				var istwinprice = $('.tdtwinprice:visible').length > 0;

				if (!istwinprice) {
					tdtwinprice.attr("IsHide", "Hide");
				}

				if (istwinprice)
					istwinprice = tdtwinprice.attr("IsHide") != "Hide";

				var Width = 0;
				if (isadultprice)
					Width += 90;
				if (issingleprice)
					Width += 90;
				if (istwinprice)
					Width += 90;

				tdtwinprice.attr('style', 'min-width: ' + Width + 'px !important');
			}
			else {
				tdsingleprice.removeClass("hoverhideprice");
				tdadultprice.removeClass("hoverhideprice");
				tdtwinprice.removeClass("hovershowprice");
				tdtwinprice.css('min-width', '');

				if (tdtwinprice.attr("IsHide") == "Hide") {
					tdtwinprice.css("display", "none");
				}
			}

			if (tdsingleprice.find("input").val() == "")
				tdsingleprice.removeClass("hoverhidepricenotactive");
			else
				tdsingleprice.addClass("hoverhidepricenotactive");

			if (tdadultprice.find("input").val() == "")
				tdadultprice.removeClass("hoverhidepricenotactive");
			else
				tdadultprice.addClass("hoverhidepricenotactive");
		}
		else {
			var tdchildnbprice = tr.find(".tdchildnbprice");
			var tdchildwbprice = tr.find(".tdchildwbprice");

			if (isShowHover) {
				tdchildwbprice.addClass("hoverhideprice");
				tdchildnbprice.addClass("hovershowprice");

				var ischildwbprice = $('.tdchildwbprice:visible').length > 0;
				var ischildnbprice = $('.tdchildnbprice:visible').length > 0;

				if (!ischildnbprice) {
					tdchildnbprice.attr("IsHide", "Hide");
				}

				if (ischildnbprice)
					ischildnbprice = tdchildnbprice.attr("IsHide") != "Hide";

				var Width = 0;
				if (ischildwbprice)
					Width += 90;
				if (ischildnbprice)
					Width += 90;

				tdchildnbprice.attr('style', 'min-width: ' + Width + 'px !important');
			}
			else {
				tdchildwbprice.removeClass("hoverhideprice");
				tdchildnbprice.removeClass("hovershowprice");
				tdchildnbprice.css('min-width', '');

				if (tdchildnbprice.attr("IsHide") == "Hide") {
					tdchildnbprice.css("display", "none");
				}
			}

			if (tdchildwbprice.find("input").val() == "")
				tdchildwbprice.removeClass("hoverhidepricenotactive");
			else
				tdchildwbprice.addClass("hoverhidepricenotactive");
		}

		var txthoverhideprice = tr.find(".txthoverhideprice" + PaxType);
		var txthovershowprice = tr.find(".txthovershowprice" + PaxType);

		var setValue = parseFloat(txthovershowprice.val());
		if (!isNaN(setValue))
			setValue = setValue.toFixed(2)
		else
			setValue = "";

		txthoverhideprice.val(setValue);
		txthovershowprice.val(setValue);

		Settxthoveredit(txthoverhideprice);
		Settxthoveredit(txthovershowprice);
	}

	$(".ihideprice, .ishowprice").click(function () {
		if ($(".txtPriceSectionAcc").val() == "0")
			$(".txtPriceSectionAcc").val("1");
		else
			$(".txtPriceSectionAcc").val("0");

		SetPriceAcc();
	});

	//Increase/Decrease Price
	$(".aDecreaseAdultPrice").click(function () {
		SetIncrDecrPrice($(this).parent().parent().parent().parent(), false, false, true);
		SetCeilAmount();
	});

	$(".aIncreaseAdultPrice").click(function () {
		SetIncrDecrPrice($(this).parent().parent().parent().parent(), false, true, true);
		SetCeilAmount();
	});

	$(".aDecreaseChildPrice").click(function () {
		SetIncrDecrPrice($(this).parent().parent().parent().parent(), true, false, true);
		SetCeilAmount();
	});

	$(".aIncreaseChildPrice").click(function () {
		SetIncrDecrPrice($(this).parent().parent().parent().parent(), true, true, true);
		SetCeilAmount();
	});

	function SetIncrDecrPrice(tr, IsChild, IsIncrease, SetValue) {
		var trHotel = tr.parents('.trHotel');
		var lblHotelRoomAdults = trHotel.find(".lblHotelRoomAdults").text();
		var lblHotelRoomChildren = trHotel.find(".lblHotelRoomChildren").text();
		var lblHotelRoomChildrenType = trHotel.find(".lblHotelRoomChildrenType").text();

		var txtTwinPrice = tr.find(".txtTwinPrice");
		var lblTwinPrice = tr.find(".lblTwinPrice");
		var lblChangeAdultPrice = tr.find(".lblChangeAdultPrice");

		var txtChildNBPrice = tr.find(".txtChildNBPrice");
		var lblChildNBPrice = tr.find(".lblChildNBPrice");
		var lblChangeChildPrice = tr.find(".lblChangeChildPrice");

		var SinglePrice = parseFloat(tr.find(".lblSinglePrice").text());
		var ExAdultPrice = parseFloat(tr.find(".lblExAdultPrice").text());
		var ChildWBPrice = parseFloat(tr.find(".lblChildWBPrice").text());
		var AdultPrice = parseFloat(txtTwinPrice.val());
		var ChildNBPrice = parseFloat(txtChildNBPrice.val());

		SinglePrice = isNaN(SinglePrice) ? 0 : SinglePrice;
		ExAdultPrice = isNaN(ExAdultPrice) ? 0 : ExAdultPrice;
		ChildWBPrice = isNaN(ChildWBPrice) ? 0 : ChildWBPrice;
		AdultPrice = isNaN(AdultPrice) ? 0 : AdultPrice;
		ChildNBPrice = isNaN(ChildNBPrice) ? 0 : ChildNBPrice;

		var TotalAdult = 0, TotalChild = 0;
		var lstRoomAdults = lblHotelRoomAdults.split('|');
		var lstRoomChildren = lblHotelRoomChildren.split('|');
		var lstRoomChildrenType = lblHotelRoomChildrenType.split('|');
		for (i = 0; i < lstRoomAdults.length; i++) {
			var Adult = parseInt(lstRoomAdults[i]);
			var Child = parseInt(lstRoomChildren[i]);

			if (Adult == 2 || (Adult == 1 && Child != 0))
				TotalAdult += Adult;
			else if (Adult == 3)
				TotalAdult += 2;


			if (Child != 0) {
				var lstChildrenType = lstRoomChildrenType[i].split('*');

				if (Child >= 1 && parseInt(lstChildrenType[0]) == 2)
					TotalChild++;

				if (Child >= 2 && parseInt(lstChildrenType[1]) == 2)
					TotalChild++;
			}
		}

		var OldTwinPrice = parseFloat(tr.find(".lblOldTwinPrice").text());
		OldTwinPrice = isNaN(OldTwinPrice) ? 0 : OldTwinPrice;

		var OldChildPrice = parseFloat(tr.find(".lblOldChildNBPrice").text());
		OldChildPrice = isNaN(OldChildPrice) ? 0 : OldChildPrice;

		var TotalPrice = (OldTwinPrice * TotalAdult) + (OldChildPrice * TotalChild);

		if (SetValue) {
			var arrPrice = GetIncressDecressPrice(IsChild, IsIncrease, TotalAdult, TotalChild, TotalPrice, AdultPrice, ChildNBPrice, SinglePrice, ExAdultPrice, ChildWBPrice);
			if (arrPrice != "") {
				arrPrice = arrPrice.split("~");
				AdultPrice = parseFloat(arrPrice[0]);
				ChildNBPrice = parseFloat(arrPrice[1]);
				txtTwinPrice.val(AdultPrice.toFixed(2));
				txtChildNBPrice.val(ChildNBPrice.toFixed(2));
			}
		}

		var setTwinPrice = txtTwinPrice.val() == "" ? 0 : parseFloat(txtTwinPrice.val());
		lblTwinPrice.text(setTwinPrice.toFixed(2));
		lblChangeAdultPrice.text(setTwinPrice.toFixed(2));

		var setChildPrice = txtChildNBPrice.val() == "" ? 0 : parseFloat(txtChildNBPrice.val());
		lblChildNBPrice.text(setChildPrice.toFixed(2));
		lblChangeChildPrice.text(setChildPrice.toFixed(2));

		//Set Incress / Decress Price
		//Check Increase Adult Price
		var aIncreaseAdultPrice = tr.find(".aIncreaseAdultPrice");
		if (GetIncressDecressPrice(false, true, TotalAdult, TotalChild, TotalPrice, AdultPrice, ChildNBPrice, SinglePrice, ExAdultPrice, ChildWBPrice) != "")
			aIncreaseAdultPrice.removeClass("desable");
		else
			aIncreaseAdultPrice.addClass("desable");

		//Check Decrease Adult Price
		var aDecreaseAdultPrice = tr.find(".aDecreaseAdultPrice");
		if (GetIncressDecressPrice(false, false, TotalAdult, TotalChild, TotalPrice, AdultPrice, ChildNBPrice, SinglePrice, ExAdultPrice, ChildWBPrice) != "")
			aDecreaseAdultPrice.removeClass("desable");
		else
			aDecreaseAdultPrice.addClass("desable");

		//Check Increase Child Price
		var aIncreaseChildPrice = tr.find(".aIncreaseChildPrice");
		if (GetIncressDecressPrice(true, true, TotalAdult, TotalChild, TotalPrice, AdultPrice, ChildNBPrice, SinglePrice, ExAdultPrice, ChildWBPrice) != "")
			aIncreaseChildPrice.removeClass("desable");
		else
			aIncreaseChildPrice.addClass("desable");

		//Check Decrease Child Price
		var aDecreaseChildPrice = tr.find(".aDecreaseChildPrice");
		if (GetIncressDecressPrice(true, false, TotalAdult, TotalChild, TotalPrice, AdultPrice, ChildNBPrice, SinglePrice, ExAdultPrice, ChildWBPrice) != "")
			aDecreaseChildPrice.removeClass("desable");
		else
			aDecreaseChildPrice.addClass("desable");
	}

	function GetIncressDecressPrice(IsChild, IsIncrease, TotalAdult, TotalChild, TotalPrice, AdultPrice, ChildNBPrice, SinglePrice, ExAdultPrice, ChildWBPrice) {

		if (TotalChild > 0 && TotalAdult > 0) {
			var IncrDecrPrice = 300;
			if (IsChild) {
				ChildNBPrice = IsIncrease ? (ChildNBPrice + IncrDecrPrice) : (ChildNBPrice - IncrDecrPrice);
				AdultPrice = ((TotalPrice - (ChildNBPrice * TotalChild)) / TotalAdult);
			}
			else {
				AdultPrice = IsIncrease ? (AdultPrice + IncrDecrPrice) : (AdultPrice - IncrDecrPrice);
				ChildNBPrice = ((TotalPrice - (AdultPrice * TotalAdult)) / TotalChild);
			}
		}
		//alert(TotalAdult + " " + TotalChild);

		var IsValid = true;
		if (TotalAdult <= 0 || TotalChild <= 0) {
			//alert("0");
			IsValid = false; //Adult is 0, Child is 0
		} else if (AdultPrice < 0) {
			//alert("1");
			IsValid = false; //Adult Price is 0
		} else if (ChildNBPrice < 0) {
			//alert("2");
			IsValid = false; //Child Price is 0
		} else if (ChildNBPrice > AdultPrice) {
			//alert("3");
			IsValid = false; //Child Price is large
		} else if (SinglePrice != 0 && AdultPrice > SinglePrice) {
			//alert("4");
			IsValid = false; //large Single Price
		}
		//else if (ExAdultPrice != 0 && (ExAdultPrice > AdultPrice || ChildNBPrice > ExAdultPrice)) {
		//	alert(ExAdultPrice + " > " + AdultPrice + " > " + ChildNBPrice);
		//	alert("5");
		//	IsValid = false;    //large Extra Edult Price
		//}
		else if (ChildWBPrice != 0 && (ChildWBPrice > AdultPrice || ChildNBPrice > ChildWBPrice)) {
			//alert("6");
			IsValid = false;    //large Child With Bed Price Price
		}

		//alert(IsValid);

		if (IsValid)
			return AdultPrice + "~" + ChildNBPrice;
		else
			return "";
	}

	$(".aResetPrice").click(function () {
		var tr = $(this).parent().parent().parent();
		var OldTwinPrice = parseFloat(tr.find(".lblOldTwinPrice").text());
		if (isNaN(OldTwinPrice))
			OldTwinPrice = 0;

		tr.find(".txtTwinPrice").val(OldTwinPrice);
		tr.find(".lblTwinPrice").text(OldTwinPrice);
		tr.find(".lblChangeAdultPrice").text(OldTwinPrice);

		var OldChildPrice = parseFloat(tr.find(".lblOldChildNBPrice").text());
		if (isNaN(OldChildPrice))
			OldChildPrice = 0;

		tr.find(".txtChildNBPrice").val(OldChildPrice);
		tr.find(".lblChildNBPrice").text(OldChildPrice);
		tr.find(".lblChangeChildPrice").text(OldChildPrice);

		SetIncrDecrPrice(tr, true, true, false);
		SetCeilAmount();
	});

	$(".trHotelPrice").each(function () {
		SetIncrDecrPrice($(this), true, true, false);
	});


	/* Air Ticket */
	$(".txtAirline, .txtAirTicketFrom, .txtAirTicketTo, .txtAirTwinPrice, .txtAirChildNBPrice, .txtAirInfPrice").keyup(function () {
		SetIcon("accAirTicket", true);
	});

	$('.txtAirTicketDeparture').each(function () {
		var start = $(this);
		var end = $(this).parent().parent().parent().find('.txtAirTicketArrival');

		start.datetimepicker(
			{
				format: 'DD-MM-YYYY HH:mm',
				sideBySide: true,
				minDate: new Date(),
				useCurrent: false
			}).on('dp.change', function (selected) {
				end.data("DateTimePicker").minDate(selected.date);
				SetIcon("accAirTicket", true);
			});

		end.datetimepicker(
			{
				format: 'DD-MM-YYYY HH:mm',
				sideBySide: true,
				minDate: new Date(),
				useCurrent: false
			}).on('dp.change', function (selected) {
				start.data("DateTimePicker").maxDate(selected.date);
				SetIcon("accAirTicket", true);
			});

		if (end.val() != "")
			start.data("DateTimePicker").maxDate(end.val());

		if (start.val() != "")
			end.data("DateTimePicker").minDate(start.val());
	});

	/* Hotel */
	/* Manage Hotel */
	$(".divHotelHover").hover(function () {
		ManageHotelHover($(this));
	});

	$(".divHotelHover").each(function () {
		ManageHotelHover($(this));
	});

	function ManageHotelHover(td) {
		var value = td.parent().parent().find(".lblHotelId").text();
		var setText = td.attr("changetext");
		if ((value == "" || value == "0") && td.parent().parent().find(".lbleStayat").text() == "1") {
			setText = td.attr("addtext");
			td.addClass("divhoveractive");
		}
		else {
			td.removeClass("divhoveractive");
		}
		td.find(".lnkChangeHotel").text(setText);
	}

	/* Inclusion */
	$(".ddlInDate").change(function () {
		SetIcon("accInclusion", true);
		SortingInclusionDetail();
	});

	$(".txtTimeSlot").change(function () {
		SortingInclusionDetail();
	});

	/* Visa */
	$(".ddlVisaManageBy, .chkVisaDocReceived, .chkVisaApplied").change(function () {
		SetIcon("accVisa", true);
	});

	SortingInclusionDetail();
	function SortingInclusionDetail() {
		$(".trInclusion").each(function () {
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
			catch (e) { }

			//var Type = parseInt(Tr.find(".lblInclusionAddType").text());

			var position = (Date == 0 ? 3000 : Date);
			//if (Type == 5)//5 == Manually
			//	position = position + 9999;

			Tr.attr("position", position);
		});

		var $table = $('.tblInclusion');

		var rows = $table.find('.trInclusion').get();
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
	}

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

	/* Set Cill Price */
	SetCeilAmount();
	function SetCeilAmount() {
		$(".lblCeilAmount").each(function () {
			var Amount = Math.ceil(parseFloat($(this).text()));
			if (!isNaN(Amount)) {
				$(this).text(Amount.toFixed(2));
			}
		});
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