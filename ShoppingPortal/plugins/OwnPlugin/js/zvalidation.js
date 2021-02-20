
console.log("\n ----------------------------------------------Version Information----------------------------------------------");
console.log("OCTFIS TECHNO LLP");
console.log("JS DEVELOPED BY JATIN, VISHAL & RAHUL ");
console.log("What Is New In Version 1.0.1? ");
console.log("\n\t (Last Updated On: 16th May 2015) ");
console.log("\n\t-Solved Password Match bugs.! ");
console.log("What Is New In Version 1.0.2? ");
console.log("\n\t (Last Updated On: 10th Jun 2015) ");
console.log("\n\t-Add Method For Accept Only Number For Mobile.( Note:-  This Functionlity is working on only keyup event.)");
console.log("\n\t-We Improve Security.!");
console.log("What Is New In Version 1.0.3? ");
console.log("\n\t (Last Updated On: 22nd Jun 2015) ");
console.log("\n\t-Add Method For Phone and Fax Note(Accept '-','+','White Space', 'Only Number')");

console.log("\n\n ----------------------------------------------For Developer----------------------------------------------");
console.log("\nExampale How to use js");
console.log("\n\t Set \"ZValidation='e=Event Name|v=Validate Function|m=Message Field Name'\" Property of Control");
console.log("\n\t And Finaly Use CheckValidation('Validate-area-class-name') Function on Buton Click.");
console.log("\n");

console.log("\nList of function available in js");
console.log("\n\t IsMatch (For Password) Parameter:- (TaxboxName1,TaxboxName2,LabelName,Message)");
console.log("\n\t IsSelect, IsSelectOther (For DropDown) Parameter:- (TaxboxName,LabelName,Message)");
console.log("\n\t IsRequired (For Requride Field) Parameter:- (TaxboxName,LabelName,Message) ");
console.log("\n\t IsURL, IsNullURL (For Web Url) Parameter:- (TaxboxName,LabelName,Message) ");
console.log("\n\t IsNumber, IsNullNumber (For Number Only Or Float Value) Parameter:- (TaxboxName,LabelName,Message) ");
console.log("\n\t IsNegativeNumber, IsNullNegativeNumber (For Negative Number Only Or Float Value) Parameter:- (TaxboxName,LabelName,Message) ");
console.log("\n\t IsPositiveNumber, IsNullPositiveNumber (For Positive Number Only Or Float Value) Parameter:- (TaxboxName,LabelName,Message) ");
console.log("\n\t IsMobileNumber, IsNullMobileNumber (For Mobile Number Accept Only 10 Digit) Parameter:- (TaxboxName,LabelName,Message) ");
console.log("\n\t IsEmail, IsNullEmail (For Email Ex. A@A.COM) Parameter:- (TaxboxName,LabelName,Message) ");
console.log("\n\t IsPhoneFaxNumber, IsNullPhoneFaxNumber (For Phone or Fax) Parameter:- (TaxboxName,LabelName,Message) ");
console.log("\n\t IsContactNumber, IsNullContactNumber (For Contact Number Mobile and Landline) Parameter:- (TaxboxName,LabelName,Message) ");
console.log("\n\t IsCustomDate, IsNullCustomDate (For Date With Age Limit) Parameter:- (TaxboxName,LabelName,Message,Year)");
console.log("\n\t IsCharacter (For Character, Accept Only Character Value) Parameter:- (TaxboxName,LabelName,Message) ");
console.log("\n\t IsDate, IsNullDate (For Date) Parameter:- (TaxboxName,LabelName,Message) ");
console.log("\n\t IsTime, IsNullTime(For Time) Parameter:- (TaxboxName,LabelName,Message) ");
console.log("\n\t IsLessDate(Compare Date) Parameter:- (TaxboxName1,TaxboxName2,LabelName,Message) ");
console.log("\n\t IsBeforeDate, IsNullBeforeDate (For  Before Today  Date) Parameter:- (TaxboxName,LabelName,Message) ");
console.log("\n\t IsAfterDate, IsNullAfterDate (For  After Today  Date) Parameter:- (TaxboxName,LabelName,Message) ");
console.log("\n\t IsMaxNumber, IsNullMaxNumber Parameter:- (TaxboxName,LabelName,Message,maxValue)(For Max Value)");
console.log("\n\t IsMinNumber, IsNullMinNumber Parameter:- (TaxboxName,LabelName,Message,minValue)(For Min Value)");
console.log("\n\t IsNumberBetween, IsNullNumberBetween Parameter:- (TaxboxName,LabelName,Message,minValue,maxValue)(For Value Between Min And Max Value)");
console.log("\n\t IsFileName, IsNullFileName (For Check FileName) Parameter:- (TaxboxName,LabelName,Message)");
console.log("\n\t IsCheckLength, IsCheckLengthMin Parameter:- (TaxboxName,LabelName,Message,maxValue)(For  Check Minimum Length)");
console.log("\n\t IsGST, IsNullGST Parameter:- (TaxboxName,LabelName,Message)(For Check GST Number)");
console.log("\n\t IsPANNo, IsNullPANNo Parameter:- (TaxboxName,LabelName,Message)(For Check PAN Number)");

$(document).ready(function () {
	CheckValidation("");
});

function CheckValidation(ValidateClassName) {
	var returnVal = true;
	if (ValidateClassName != "") //Only Add Event Listner
		ValidateClassName = "." + ValidateClassName;
	$(ValidateClassName + ' *[ZValidation]').each(function () {
		var objPara = GetPara($(this));
		if (ValidateClassName == "") {
			if (objPara.Event == "blur")
				$(this).blur(function () { IsValidControl(objPara); });
			else if (objPara.Event == "change")
				$(this).change(function () { IsValidControl(objPara); });
		}
		else if (!IsValidControl(objPara) && returnVal)
			returnVal = false;
	});

	return returnVal;
}


function IsValidControl(objPara) {
	return (window[objPara.ZValidation](objPara.Control, "", objPara.Message, objPara.Extra));
}

function GetPara(formControl) {
	var Control = "#" + formControl.attr("id");
	var Event = "", ZValidation = "", Message = "", Extra = "";

	var ZValidationControl = formControl.attr("ZValidation").split("|");
	ZValidationControl.forEach(function (item) {
		var para = item.split("=");
		switch (para[0].toLowerCase()) {
			case "e":
				Event = para[1];
				break;
			case "v":
				ZValidation = para[1];
				break;
			case "m":
				Message = (para[1] != "") ? para[1] : "Please Enter Valid Value";
				break;
			case "x":
				Extra = para[1];
				break;
		}
	});

	var objParamiters =
		{
			Control: Control,
			Event: Event,
			ZValidation: ZValidation,
			Message: Message,
			Extra: Extra,
		};
	return objParamiters;
}


function callAfterPostback_Old() {
	$('*[ZValidation]').each(function () {
		var Event = $(this).attr("ZValidation").split("|")[0].toLowerCase();;
		if (Event == "blur") {
			$(this).blur(function () {
				IsValidControl($(this));
			});
		}
		else if (Event == "change") {
			$(this).change(function () {
				IsValidControl($(this));
			});
		}
	});
}

function IsValidControl_Old(frmControl) {

	var ZValidationControl = frmControl.attr("ZValidation").split("|");
	var ZValidation = ZValidationControl[1];
	var Control = "#" + frmControl.attr("id");
	var Message = ZValidationControl[2];


	if (!Message || Message == "")
		Message = "Please Enter Valid Value";

	if (!window[ZValidation](Control, "", Message, ZValidationControl[3])) //Method With 3 Peramiter
		return false;

	return true;
}

//Select Validation
function ApplayClass(IsValid, TaxboxName, LabelName, Message) {
	if (IsValid)
		return Validate(TaxboxName, LabelName, Message);
	else {
		if (Message.toLowerCase().indexOf("please") == -1) {
			if ($(TaxboxName).val() == "")
				Message = "Please Enter " + Message;
			else
				Message = "Please Enter Valid " + Message;
		}
		return NotValidate(TaxboxName, LabelName, Message);
	}
}

//Accept Only Number Validation
function IsNumberApplayClass(TaxboxName, LabelName, Message, filter) {
	if (!filter.test($(TaxboxName).val())) {
		// Filter non-digits from input value.
		$(TaxboxName).val($(TaxboxName).val().replace(/\D/g, ''));
		return NotValidate(TaxboxName, LabelName, Message);
	}
	else {
		return Validate(TaxboxName, LabelName, Message);
	}
}

function Validate(TaxboxName, LabelName, Message) {
	$(TaxboxName).css('border', '1px solid green');
	$(TaxboxName).css('background-color', '#fcfcfd');
	if (LabelName != "")
		$(LabelName).text("");
	else {
		$(TaxboxName).removeClass('PlaceholderColor');
		$(TaxboxName).attr('placeholder', '');
	}

	if ($(TaxboxName).hasClass("select-search")) {
		TaxboxName = $(TaxboxName).next();

		$(TaxboxName).css('border', '1px solid green');
		$(TaxboxName).css('background-color', '#fcfcfd');
		if (LabelName != "")
			$(LabelName).text("");
		else {
			$(TaxboxName).removeClass('PlaceholderColor');
			$(TaxboxName).attr('placeholder', '');
		}
	}
	return true;
}

function NumberValidate(TaxboxName, LabelName, Message) {
	$(TaxboxName).css('border', '1px solid green');
	$(TaxboxName).css('background-color', '#fcfcfd');
	if (LabelName != "")
		$(LabelName).text("");
	else {
		$(TaxboxName).removeClass('PlaceholderColor');
		$(TaxboxName).attr('placeholder', '');
	}
	return true;
}

function NotValidate(TaxboxName, LabelName, Message) {

	var visible = $('body').find(TaxboxName).length;
	if (visible > 0) {
		$(TaxboxName).css('border', '1px solid red');
		$(TaxboxName).css('background-color', '#FDEEEE');
		if (LabelName != "")
			$(LabelName).text(Message);
		else {
			$(TaxboxName).addClass('PlaceholderColor');
			$(TaxboxName).attr('placeholder', Message);
		}

		if ($(TaxboxName).hasClass("select-search")) {
			TaxboxName = $(TaxboxName).next();

			$(TaxboxName).css('border', '1px solid red');
			$(TaxboxName).css('background-color', '#FDEEEE');
			if (LabelName != "")
				$(LabelName).text(Message);
			else {
				$(TaxboxName).addClass('PlaceholderColor');
				$(TaxboxName).attr('placeholder', Message);
			}
		}
		return false;
	}
	return true;
}

//Filter Validation
function IsValidate(TaxboxName, LabelName, FieldName, filter) {
	return ApplayClass((filter.test($(TaxboxName).val())), TaxboxName, LabelName, FieldName);
}

function IsValidateNull(TaxboxName, LabelName, Message, filter) {
	var Value = $(TaxboxName).val();
	return ApplayClass(((Value == "") || ((Value != "") && (filter.test(Value)))), TaxboxName, LabelName, Message);
}


function IsSelect(TaxboxName, LabelName, Message) {
	var Value = $(TaxboxName).val();
	return ApplayClass((Value != null && Value != "" && Value != "0"), TaxboxName, LabelName, Message);
}

function IsSelectOther(DropDown, LabelName, Message, TaxboxName) {
	var ddl = $(DropDown).val();
	var IsValid = IsSelect(DropDown, LabelName, Message);
	if (IsValid && ddl == "1000")
		return IsRequired(TaxboxName, LabelName, Message);
	return IsValid;
}

function IsMatch(TaxboxName1, LabelName, Message, TaxboxName2) {
	return ApplayClass(($(TaxboxName1).val() == $(TaxboxName2).val()), TaxboxName2, LabelName, Message);
}

//Requride Validation
function IsRequired(TaxboxName, LabelName, Message) {
	return ApplayClass(($(TaxboxName).val() != ""), TaxboxName, LabelName, Message);
}

//Requride NULL Validation
function IsNullRequired(TaxboxName, LabelName, Message) {
	return ApplayClass(true, TaxboxName, LabelName, Message);
}

function IsURL(TaxboxName, LabelName, Message) {
	var filter = /[-a-zA-Z0-9@:%_\+.~#?&//=]{2,256}\.[a-z]{2,4}\b(\/[-a-zA-Z0-9@:%_\+.~#?&//=]*)?/gi;;
	return IsValidate(TaxboxName, LabelName, Message, filter);
}

function IsNullURL(TaxboxName, LabelName, Message) {
	var filter = /[-a-zA-Z0-9@:%_\+.~#?&//=]{2,256}\.[a-z]{2,4}\b(\/[-a-zA-Z0-9@:%_\+.~#?&//=]*)?/gi;;
	return IsValidateNull(TaxboxName, LabelName, Message, filter);
}

function IsNullNumber(TaxboxName, LabelName, Message) {
	var filter = /^\d+(\.\d+)?$/;
	return IsValidateNull(TaxboxName, LabelName, Message, filter);
}

function IsNumber(TaxboxName, LabelName, Message) {

	var filter = /^\-?\d+(\.\d+)?$/;
	return IsValidate(TaxboxName, LabelName, Message, filter);
}

function IsNegativeNumber(TaxboxName, LabelName, Message) {
	var filter = /^-\d{2}(\.\d+)?$/;
	return IsValidate(TaxboxName, LabelName, Message, filter);
}

function IsNullNegativeNumber(TaxboxName, LabelName, Message) {
	var filter = /^-\d{2}(\.\d+)?$/;
	return IsValidateNull(TaxboxName, LabelName, Message, filter);
}

function IsPositiveNumber(TaxboxName, LabelName, Message) {
	var filter = /^\+?\d{2}(\.\d+)?$/;
	return IsValidate(TaxboxName, LabelName, Message, filter);
}

function IsNullPositiveNumber(TaxboxName, LabelName, Message) {
	var filter = /^\+?\d{2}(\.\d+)?$/;
	return IsValidateNull(TaxboxName, LabelName, Message, filter);
}

function IsMaxNumber(TaxboxName, LabelName, Message, maxValue) {
	if (IsNumber(TaxboxName, LabelName, Message)) {
		var value = Math.ceil($(TaxboxName).val());
		return ApplayClass((value <= parseInt(maxValue)), TaxboxName, LabelName, Message);
	}
	else
		return ApplayClass(false, TaxboxName, LabelName, Message);
}

function IsNullMaxNumber(TaxboxName, LabelName, Message, maxValue) {

	if (IsNumber(TaxboxName, LabelName, Message)) {
		var value = Math.ceil($(TaxboxName).val());
		return ApplayClass((value <= parseInt(maxValue)), TaxboxName, LabelName, Message);
	}
	else
		return IsNullNumber(TaxboxName, LabelName, Message);
}

function IsMinNumber(TaxboxName, LabelName, Message, minValue) {
	if (IsNumber(TaxboxName, LabelName, Message)) {
		var value = Math.ceil($(TaxboxName).val());
		return ApplayClass((value >= parseInt(minValue)), TaxboxName, LabelName, Message);
	}
	else
		return ApplayClass(false, TaxboxName, LabelName, Message);
}

function IsNullMinNumber(TaxboxName, LabelName, Message, minValue) {
	if (IsNumber(TaxboxName, LabelName, Message)) {
		var value = Math.ceil($(TaxboxName).val());
		return ApplayClass((value >= parseInt(minValue)), TaxboxName, LabelName, Message);
	}
	else
		return IsNullNumber(TaxboxName, LabelName, Message);
}

function IsNumberBetween(TaxboxName, LabelName, Message, minValue, maxValue) {
	if (IsNumber(TaxboxName, LabelName, Message)) {
		var value = Math.ceil($(TaxboxName).val());
		return ApplayClass(((value >= parseInt(minValue)) && (value <= parseInt(maxValue))), TaxboxName, LabelName, Message);
	}
	else
		return ApplayClass(false, TaxboxName, LabelName, Message);
}

function IsNullNumberBetween(TaxboxName, LabelName, Message, minValue, maxValue) {
	if (IsNumber(TaxboxName, LabelName, Message)) {
		var value = Math.ceil($(TaxboxName).val());
		return ApplayClass(((value >= parseInt(minValue)) && (value <= parseInt(maxValue))), TaxboxName, LabelName, Message);
	}
	else
		return IsNullNumber(TaxboxName, LabelName, Message);
}

function IsMobileNumber(TaxboxName, LabelName, Message) {

	var filter = /^([6-9]{1}[0-9]{9})$/;
	return IsValidate(TaxboxName, LabelName, Message, filter);
}

function IsNullMobileNumber(TaxboxName, LabelName, Message) {
	var filter = /^([6-9]{1}[0-9]{9})$/;
	return IsValidateNull(TaxboxName, LabelName, Message, filter);
}

function IsEmail(TaxboxName, LabelName, Message) {
	var filter = /^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$/;
	return IsValidate(TaxboxName, LabelName, Message, filter);
}

function IsNullEmail(TaxboxName, LabelName, Message) {
	var filter = /^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$/;
	return IsValidateNull(TaxboxName, LabelName, Message, filter);

}

function IsDate(TaxboxName, LabelName, Message) {
	var filter = /^\d{2}-\d{2}-\d{4}$/;
	return IsValidate(TaxboxName, LabelName, Message, filter);
}

function IsNullDate(TaxboxName, LabelName, Message) {
	var filter = /^\d{2}-\d{2}-\d{4}$/;
	return IsValidateNull(TaxboxName, LabelName, Message, filter);
}

function IsTime(TaxboxName, LabelName, Message) {
	var filter = /^\d{1,2}([:.]?\d{1,2})?([ ]?(([a|p]m)|([A|P]M)))?$/;
	return IsValidate(TaxboxName, LabelName, Message, filter);
}

function IsNullTime(TaxboxName, LabelName, Message) {
	var filter = /^\d{1,2}([:.]?\d{1,2})?([ ]?[a|p]m)?$/;
	return IsValidateNull(TaxboxName, LabelName, Message, filter);
}

function IsCharacter(TaxboxName, LabelName, Message) {
	var filter = /^[ A-Za-z/]*$/;
	return IsValidateNull(TaxboxName, LabelName, Message, filter);
}

function IsNullCharacter(TaxboxName, LabelName, Message) {
	var filter = /^[ A-Za-z/]*$/;
	return IsValidateNull(TaxboxName, LabelName, Message, filter);
}

function IsGST(TaxboxName, LabelName, Message) {
	var filter = /^([0-9]){2}([a-zA-Z]){5}([0-9]){4}([a-zA-Z]){1}([0-9]){1}([a-zA-Z]){1}([0-9]|[a-zA-Z]){1}?$/;
	return IsValidate(TaxboxName, LabelName, Message, filter);
}

function IsNullGST(TaxboxName, LabelName, Message) {
	var filter = /^([0-9]){2}([a-zA-Z]){5}([0-9]){4}([a-zA-Z]){1}([0-9]){1}([a-zA-Z]){1}([0-9]|[a-zA-Z]){1}?$/;
	return IsValidateNull(TaxboxName, LabelName, Message, filter);
}

function IsPANNo(TaxboxName, LabelName, Message) {
	var filter = /^([a-zA-Z]){5}([0-9]){4}([a-zA-Z]){1}?$/;
	return IsValidate(TaxboxName, LabelName, Message, filter);
}

function IsNullPANNo(TaxboxName, LabelName, Message) {
	var filter = /^([a-zA-Z]){5}([0-9]){4}([a-zA-Z]){1}?$/;
	return IsValidateNull(TaxboxName, LabelName, Message, filter);
}

function IsBirthday(TaxboxName, LabelName, Message) {
	if (IsDate(TaxboxName, LabelName, Message)) {
		var Value = $(TaxboxName).val().split("-");
		var TodayDate = new Date();
		return ApplayClass((parseInt(TodayDate.getFullYear() - 1) > Value[2]), TaxboxName, LabelName, Message);
	}
	else
		return ApplayClass(false, TaxboxName, LabelName, Message);

}

function IsNullBirthday(TaxboxName, LabelName, Message) {
	if (IsNullDate(TaxboxName, LabelName, Message)) {
		var Value = $(TaxboxName).val().split("-");
		var TodayDate = new Date();
		return ApplayClass((parseInt(TodayDate.getFullYear() - 1) > Value[2]), TaxboxName, LabelName, Message);
	}
	else
		return ApplayClass(false, TaxboxName, LabelName, Message);
}

function IsCustomDate(TaxboxName, LabelName, Message, Year) {
	if (IsDate(TaxboxName, LabelName, Message)) {
		var Value = $(TaxboxName).val().split("-");
		var TodayDate = new Date();
		return ApplayClass((parseInt(TodayDate.getFullYear() - Year) > Value[2]), TaxboxName, LabelName, Message);
	}
	else
		return ApplayClass(false, TaxboxName, LabelName, Message);
}

function IsNullCustomDate(TaxboxName, LabelName, Message, Year) {
	if (IsNullDate(TaxboxName, LabelName, Message) && $(TaxboxName).val() != "") {
		var Value = $(TaxboxName).val().split("-");
		var TodayDate = new Date();
		return ApplayClass((parseInt(TodayDate.getFullYear() - Year) > Value[2]), TaxboxName, LabelName, Message);
	}
	else {
		return ApplayClass(true, TaxboxName, LabelName, Message);
	}
}

function IsBeforeDate(TaxboxName, LabelName, Message) {
	if (IsDate(TaxboxName, LabelName, Message)) {
		var today = todayDate();
		var texboxDate = $(TaxboxName).val();
		today = today.split("-");
		texboxDate = texboxDate.split("-");
		var d1 = new Date(today[2], today[1], today[0]);
		var d2 = new Date(texboxDate[2], texboxDate[1], texboxDate[0]);

		return ApplayClass((d1 >= d2), TaxboxName, LabelName, Message);
	}
	else
		return ApplayClass(false, TaxboxName, LabelName, Message);

}

function IsNullBeforeDate(TaxboxName, LabelName, Message) {
	if (IsDate(TaxboxName, LabelName, Message)) {
		var today = todayDate();
		var texboxDate = $(TaxboxName).val();
		today = today.split("-");
		texboxDate = texboxDate.split("-");
		var d1 = new Date(today[2], today[1], today[0]);
		var d2 = new Date(texboxDate[2], texboxDate[1], texboxDate[0]);

		return ApplayClass((d1 >= d2), TaxboxName, LabelName, Message);
	}
	else
		return ApplayClass(true, TaxboxName, LabelName, Message);
}

function IsAfterDate(TaxboxName, LabelName, Message) {
	if (IsDate(TaxboxName, LabelName, Message)) {
		var today = todayDate();
		var texboxDate = $(TaxboxName).val();
		today = today.split("-");
		texboxDate = texboxDate.split("-");
		var d1 = new Date(today[2], today[1], today[0]);
		var d2 = new Date(texboxDate[2], texboxDate[1], texboxDate[0]);

		return ApplayClass((d1 <= d2), TaxboxName, LabelName, Message);
	}
	else
		return ApplayClass(false, TaxboxName, LabelName, Message);

}

function IsNullAfterDate(TaxboxName, LabelName, Message) {
	if (IsDate(TaxboxName, LabelName, Message) && $(TaxboxName).val() != "") {
		var today = todayDate();
		var texboxDate = $(TaxboxName).val();
		today = today.split("-");
		texboxDate = texboxDate.split("-");
		var d1 = new Date(today[2], today[1], today[0]);
		var d2 = new Date(texboxDate[2], texboxDate[1], texboxDate[0]);

		return ApplayClass((d1 <= d2), TaxboxName, LabelName, Message);
	}
	else
		return ApplayClass(true, TaxboxName, LabelName, Message);
}

function IsLessDate(TaxboxName1, TaxboxName2, LabelName, Message) {
	if (IsDate(TaxboxName1, LabelName, Message) && IsDate(TaxboxName2, LabelName, Message)) {
		var texbox1 = $(TaxboxName1).val();
		var texbox2 = $(TaxboxName2).val();
		texbox1 = texbox1.split("-");
		texbox2 = texbox2.split("-");
		var d1 = new Date(texbox1[2], texbox1[1], texbox1[0]);
		var d2 = new Date(texbox2[2], texbox2[1], texbox2[0]);

		return ApplayClass((d1 <= d2), TaxboxName2, LabelName, Message);
	}
	else {
		return ApplayClass(false, TaxboxName2, LabelName, Message);
	}
}

function IsBigDate(TaxboxName1, TaxboxName2, LabelName, Message) {
	if (IsDate(TaxboxName1, LabelName, Message) && IsDate(TaxboxName2, LabelName, Message)) {
		var texbox1 = $(TaxboxName1).val();
		var texbox2 = $(TaxboxName2).val();
		texbox1 = texbox1.split("-");
		texbox2 = texbox2.split("-");
		var d1 = new Date(texbox1[2], texbox1[1], texbox1[0]);
		var d2 = new Date(texbox2[2], texbox2[1], texbox2[0]);

		return ApplayClass((d1 > d2), TaxboxName2, LabelName, Message);
	}
	else {
		return ApplayClass(false, TaxboxName2, LabelName, Message);
	}
}

function IsPhoneFaxNumber(TaxboxName, LabelName, Message) {
	var filter = /[+/-0-9 ]/;
	return IsValidateNull(TaxboxName, LabelName, Message, filter);
}

function IsNullPhoneFaxNumber(TaxboxName, LabelName, Message) {
	var filter = /[+/-0-9 ]/;
	return IsValidateNull(TaxboxName, LabelName, Message, filter);
}

function IsContactNumber(TaxboxName, LabelName, Message) {
	var filter = /^\d[0-9]{5,15}$/;
	return IsValidateNull(TaxboxName, LabelName, Message, filter);
}

function IsNullContactNumber(TaxboxName, LabelName, Message) {
	var filter = /^\d[0-9]{5,15}$/;
	return IsValidateNull(TaxboxName, LabelName, Message, filter);
}

function IsPincode(TaxboxName, LabelName, Message) {
	var filter = /^\d[0-9]{5}$/;
	return IsValidate(TaxboxName, LabelName, Message, filter);
}


function IsNullPincode(TaxboxName, LabelName, Message) {
	var filter = /^\d[0-9]{5}$/;
	return IsValidateNull(TaxboxName, LabelName, Message, filter);
}

function IsCheckLength(TaxboxName, LabelName, Message, Length) {
	return ApplayClass(($(TaxboxName).val().length > Length), TaxboxName, LabelName, Message);
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

function parseDate(str) {
	var s = str.split(" "),
		d = str[0].split("-"),
		t = str[1].replace(/:/g, "");
	return d[2] + d[1] + d[0] + t;
}

function IsFileName(TaxboxName, LabelName, Message) {
	var filter = /^[a-zA-Z0-9/(/)._-]+$/;
	return IsValidate(TaxboxName, LabelName, Message, filter);
}

function IsNullFileName(TaxboxName, LabelName, Message) {
	var filter = /^[a-zA-Z0-9._-]+$/;
	return IsValidateNull(TaxboxName, LabelName, Message, filter);
}

function IsCheckLengthMin(TaxboxName, LabelName, Message, Length) {
	return ApplayClass(($(TaxboxName).val().length < Length), TaxboxName, LabelName, Message);
}


$(".intnumber").keydown(function (e) {
	// Allow: backspace, delete, tab, escape, enter and .
	if ($.inArray(e.keyCode, [8, 9, 27, 13, 109]) !== -1 ||
		// Allow: Ctrl+A, Command+A
		(e.keyCode === 65 && (e.ctrlKey === true || e.metaKey === true)) ||
		// Allow: home, end, left, right, down, up
		(e.keyCode >= 35 && e.keyCode <= 40) ||
		(e.ctrlKey && e.keyCode == 86) ||
        (e.ctrlKey && e.keyCode == 67) ||
		(e.ctrlKey && e.keyCode == 88)) {
		// let it happen, don't do anything
		return;
	}
	// Ensure that it is a number and stop the keypress
	if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
		e.preventDefault();
	}
});

$(".flotnumber").keydown(function (e) {
	if ($(this).val().indexOf('.') > 0 && e.keyCode == 110)
		e.preventDefault();

	// Allow: backspace, delete, tab, escape, enter and .
	if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 110, 190, 109]) !== -1 ||
		// Allow: Ctrl+A, Command+A
		(e.keyCode === 65 && (e.ctrlKey === true || e.metaKey === true)) ||
		// Allow: home, end, left, right, down, up
		(e.keyCode >= 35 && e.keyCode <= 40) ||
		(e.ctrlKey && e.keyCode == 86) ||
        (e.ctrlKey && e.keyCode == 88) ||
        (e.ctrlKey && e.keyCode == 67)) {
		// let it happen, don't do anything
		return;
	}

	// Ensure that it is a number and stop the keypress
	if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
		e.preventDefault();
	}
});

$(".flotnumber2").keydown(function (e) {
	if ($(this).val().indexOf('.') > 0 && e.keyCode == 110)
		e.preventDefault();

	// Allow: backspace, delete, tab, escape, enter and .
	if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 110, 190, 109]) !== -1 ||
		// Allow: Ctrl+A, Command+A
		(e.keyCode === 65 && (e.ctrlKey === true || e.metaKey === true)) ||
		// Allow: home, end, left, right, down, up
		(e.keyCode >= 35 && e.keyCode <= 40) ||
		(e.ctrlKey && e.keyCode == 86) ||
        (e.ctrlKey && e.keyCode == 67) ||
		(e.ctrlKey && e.keyCode == 88)) {
		// let it happen, don't do anything
		return;
	}

	// Ensure that it is a number and stop the keypress
	if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
		e.preventDefault();
	}

	var num = $(this).val().split(".");
	if (num.length > 1) {
		if (num[1].length >= 2)
			e.preventDefault();
	}
});

$(".flotnumber4").keydown(function (e) {
	if ($(this).val().indexOf('.') > 0 && e.keyCode == 110)
		e.preventDefault();

	// Allow: backspace, delete, tab, escape, enter and .
	if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 110, 190, 109]) !== -1 ||
		// Allow: Ctrl+A, Command+A
		(e.keyCode === 65 && (e.ctrlKey === true || e.metaKey === true)) ||
		// Allow: home, end, left, right, down, up
		(e.keyCode >= 35 && e.keyCode <= 40) ||
		(e.ctrlKey && e.keyCode == 86) ||
		(e.ctrlKey && e.keyCode == 88)) {
		// let it happen, don't do anything
		return;
	}

	// Ensure that it is a number and stop the keypress
	if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
		e.preventDefault();
	}

	var num = $(this).val().split(".");
	if (num.length > 1) {
		if (num[1].length >= 4)
			e.preventDefault();
	}
});

$(".charecter").keydown(function (e) {
	if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 110, 190, 109]) !== -1 ||
		// Allow: Ctrl+A, Command+A
		(e.keyCode === 65 && (e.ctrlKey === true || e.metaKey === true)) ||
		// Allow: home, end, left, right, down, up
		(e.keyCode >= 35 && e.keyCode <= 40) ||
		(e.ctrlKey && e.keyCode == 86) ||
		(e.ctrlKey && e.keyCode == 88)) {
		// let it happen, don't do anything
		return;
	}

	// Allow controls such as backspace
	var arr = [8, 9, 13, 16, 17, 20, 35, 36, 37, 38, 39, 40, 45, 46];

	// Allow letters
	for (var i = 65; i <= 90; i++) {
		arr.push(i);
	}

	// Prevent default if not in array
	if (jQuery.inArray(e.which, arr) === -1) {
		e.preventDefault();
	}
});

$(".min2digitnumber").each(function () {
	Setmin2digitnumber($(this));
});

$(".min2digitnumber").change(function () {
	Setmin2digitnumber($(this));
});

function Setmin2digitnumber(TextBox) {
	var Val = TextBox.val();
	if (Val != "" && Val.length == 1) {
		TextBox.val("0" + Val);
	}
} 