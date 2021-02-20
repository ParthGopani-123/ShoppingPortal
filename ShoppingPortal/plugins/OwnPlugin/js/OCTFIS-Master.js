//GetSMSBalance();
CallNotificationFunction();

function CallNotificationFunction() {
	GetNotification();
	setInterval(function () {
		GetNotification();
	}, 300000);

	$(window).resize(function () {
		SetNotificationScroll();
	});
	$(".ViewNotificationStatus").click(function () {
		UpdateViewNotificationStatus();
	});

	$("#ViewNotification").click(function () {
		GetNotificationList();
	});
}

function UpdateViewNotificationStatus() {
	$(".notificationcount").text("");
	$.ajax({
		url: "GetAjaxData.aspx",
		type: "get",
		async: true,
		data: { ajaxDataType: 3 }, //UpdateViewNotificationStatus
		success: function (data) {
		}
	});
}

function UpdateNotificationRead(NotificationId) {
	$.ajax({
		url: "GetAjaxData.aspx",
		type: "get",
		data: { ajaxDataType: 4, NotificationId: NotificationId }, //UpdateViewNotificationStatus
		async: true,
		success: function (data) {
			$(".Notification" + NotificationId).removeClass("NotificationActive");
		}
	});
}

function ViewNotificationText(NotificationId, NotificationText) {
	ShowModel('notification-modal');
	$('.lblNotifDescription').html(NotificationText);

	$.ajax({
		url: "GetAjaxData.aspx",
		type: "get",
		data: { ajaxDataType: 4, NotificationId: NotificationId }, //UpdateViewNotificationStatus
		async: true,
		success: function (data) {
		}
	});
}

function GetNotification() {
	$.ajax({
		url: "GetAjaxData.aspx",
		type: "get",
		data: { ajaxDataType: 1 }, // GetNotification
		async: true,
		success: function (data) {
			var notification = data.split('####')[0];
			if (notification != "0") {
				notification.split('##~##').forEach(function (item) {
					var setdata = item.split("#~#");
					if (setdata[0] != "")
						notifyMe(setdata[0], setdata[1], setdata[2]);
				});
			}

			var notificationcount = data.split('####')[1];
			if (notificationcount != "0") {
				$(".notificationcount").text(data.split('####')[1]);
			}
			else
				$(".notificationcount").text("");


			$('.ulnewsticker').html(data.split('####')[2]);
			$('.newsticker').newsTicker();
		}
	});
}

function GetNotificationList() {
	if ($(".checkopennotification").attr("aria-expanded") == "false") {
		addRegionLoader("divNotification");
		$.ajax({
			url: "GetNotificaionList.aspx",
			type: "get",
			data: {},
			success: function (Response) {
				var data = Response.split("#OCTFIS#");
				$('#divNotification').html(data[1]);
				$(".notificationcount").text("");

				SetNotificationScroll();
				removeRegionLoader("divNotification");

				//Open Link then not open Notification popup
				$(".divViewNotificationText a").click(function (e) {
					e.stopPropagation();
				});
			}
		});
	}
}

function GetSMSBalance() {
	//alert();
	$.ajax({
		url: "GetSMSBalance.aspx",
		type: "get",
		data: {},
		success: function (Response) {
			$('#divSMSBalance').html(Response.split("#OCTFIS#")[1]);

			var RemainingPer = parseInt($(".lblSMSRemainingPer").text());
			if (RemainingPer <= 10)
				$(".spansmsindicator").addClass("dot");
			else
				$(".spansmsindicator").removeClass("dot");

			var BalanceColor = "#1abe9e";
			if (RemainingPer < 30)
				BalanceColor = "#f35369";
			else if (RemainingPer < 70)
				BalanceColor = "#0773ef";
			$(".fabalnceindicator").css("color", BalanceColor);
		}
	});
}

function SetNotificationScroll() {
	$(".media-list").css("max-height", (document.documentElement.clientHeight - 200) + "px");
	$(".media-list").mCustomScrollbar({
		theme: "minimal-dark"
	});
}

function notifyMe(title, time, Imageicon) {
	// Let's check if the browser supports notifications
	if (!("Notification" in window)) {
		alert(title + " Time=" + time);
	}

	// Let's check if the user is okay to get some notification
	else if (Notification.permission === "granted") {
		var getImageicon = Imageicon;
		if (getImageicon == "") {
			getImageicon = "images/zibma.jpg";
		}

		// If it's okay let's create a notification
		var options = {
			body: time,
			icon: getImageicon
		};
		var notification = new Notification(title, options);
	}

	// Otherwise, we need to ask the user for permission
	// Note, Chrome does not implement the permission static property
	// So we have to check for NOT 'denied' instead of 'default'
	else if (Notification.permission !== 'denied') {
		Notification.requestPermission(function (permission) {
			// Whatever the user answers, we make sure we store the information
			if (!('permission' in Notification)) {
				Notification.permission = permission;
			}

			// If the user is okay, let's create a notification
			if (permission === "granted") {
				var options = {
					body: time,
					icon: getImageicon
				};
				var notification = new Notification(title, options);
			}
		});
	}

	// At last, if the user already denied any notification, and you
	// want to be respectful there is no need to bother them any more.
}

