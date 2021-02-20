using System;
using BOL;
using Utility;
using System.Web;

/// <summary>
/// Summary description for LoginUtilities
/// </summary>
public static class LoginUtilities
{
	public static string SessionId = "UsersId";

	public static void CheckSession()
	{
		CheckSession(true);
	}


	public static void CheckSession(bool IsRedirect)
	{
		if (HttpContext.Current.Request.QueryString["UserName"] != null &&
			HttpContext.Current.Request.QueryString["Password"] != null)
		{
			LoginUtilities.LoginIfValid(HttpContext.Current.Request.QueryString["UserName"].ToString(), HttpContext.Current.Request.QueryString["Password"].ToString(), true);
		}

		if (HttpContext.Current.Session[SessionId] == null)
		{
			if (HttpContext.Current.Request.Cookies[CS._CurrentLoginTime] != null && HttpContext.Current.Request.Cookies[CS._PrevVisit] != null)
			{
				string cl_user = HttpContext.Current.Request.Cookies[CS._CurrentLoginTime].Value;
				string ltocu = HttpContext.Current.Request.Cookies[CS._PrevVisit].Value;

				var dtCookie = new LoginCookie() { CookieClUser = cl_user, CookieLtocu = ltocu }.Select(new LoginCookie() { CookieExpireTime = IndianDateTime.Now });
				if (dtCookie.Rows.Count > 0 && Convert.ToDateTime(dtCookie.Rows[0][CS.CookieExpireTime]) > IndianDateTime.Now)
				{
					if (!IsValidUsersId(CC.DecryptCookies(cl_user, ltocu)))
					{
						if (IsRedirect)
							HttpContext.Current.Response.Redirect("Logout.aspx?" + CS.rurl.Encrypt() + "=" + HttpContext.Current.Request.Url.ToString().Encrypt());
					}
					else
						WriteSession(int.Parse(CC.DecryptCookies(cl_user, ltocu)));
				}
				else
				{
					if (IsRedirect)
						HttpContext.Current.Response.Redirect("Login.aspx?" + CS.rurl.Encrypt() + "=" + HttpContext.Current.Request.Url.ToString().Encrypt());
				}
			}
			else
			{
				if (IsRedirect)
					HttpContext.Current.Response.Redirect("Login.aspx?" + CS.rurl.Encrypt() + "=" + HttpContext.Current.Request.Url.ToString().Encrypt());
			}
		}

		CU.GetMasterPageLabel("lblMstUsersId").Text = CU.GetUsersId().ToString();
	}

	public static bool CheckCookies()
	{
		if (HttpContext.Current.Session[SessionId] == null)
		{
			if (HttpContext.Current.Request.Cookies[CS._CurrentLoginTime] != null && HttpContext.Current.Request.Cookies[CS._PrevVisit] != null)
			{
				string cl_user = HttpContext.Current.Request.Cookies[CS._CurrentLoginTime].Value;
				string ltocu = HttpContext.Current.Request.Cookies[CS._PrevVisit].Value;
				var dtCookie = new LoginCookie() { CookieClUser = cl_user, CookieLtocu = ltocu }.Select(new LoginCookie() { CookieExpireTime = IndianDateTime.Now });
				if (dtCookie.Rows.Count > 0 && Convert.ToDateTime(dtCookie.Rows[0][CS.CookieExpireTime]) > IndianDateTime.Now)
				{
					if (!IsValidUsersId(CC.DecryptCookies(cl_user, ltocu)))
					{
						HttpContext.Current.Response.Redirect("Logout.aspx?" + CS.rurl.Encrypt() + "=" + HttpContext.Current.Request.Url.ToString().Encrypt());
					}
					else
					{
						WriteSession(int.Parse(CC.DecryptCookies(cl_user, ltocu)));
						return true;
					}
				}
			}
			return false;
		}
		else
		{
			return true;
		}
	}


	public static bool IsValidLogin(string UserName, string Password, ref int UsersId)
	{
		var dtLogin = new Logins()
		{
			Username = UserName,
		}.Select(new Logins() { UsersId = 0, Password = "", PwdSalt = "" });

		if (dtLogin.Rows.Count > 0)
		{
			int UsersIdTemp = dtLogin.Rows[0][CS.UsersId].zToInt().Value;
			var UserCount = new Users()
			{
				UsersId = UsersIdTemp,
				eStatus = (int)eStatus.Active,
			}.SelectCount();

			if (UserCount == 0)
				return false;

			if (ComparePassword(Password, dtLogin.Rows[0]["Password"].ToString(), dtLogin.Rows[0]["PwdSalt"].ToString()))
			{
				UsersId = UsersIdTemp;
				return true;
			}
		}

		return false;
	}

	public static bool LoginIfValid(string UserName, string Password, bool RememberMe)
	{
		int UsersId = 0;
		if (IsValidLogin(UserName, Password, ref UsersId))
		{
			Login(UsersId, RememberMe);
			return true;
		}
		return false;
	}


	public static void Login(int UsersId, bool RememberMe)
	{
		LoginUtilities.WriteSession(UsersId);
		LoginUtilities.WriteCookies(RememberMe);

		Redirect(eLoginType.Manual, UsersId);
	}

	public static void Redirect(eLoginType LoginType, int UsersId)
	{
		HttpBrowserCapabilities browser = HttpContext.Current.Request.Browser;
		LoginUtilities.LoginEntry(UsersId, LoginType, HttpContext.Current.Request.UserAgent, ref browser);

		if (HttpContext.Current.Request.QueryString[CS.rurl.Encrypt()] == null)
			HttpContext.Current.Response.Redirect("Default.aspx");
		else
			HttpContext.Current.Response.Redirect(HttpContext.Current.Request.QueryString[CS.rurl.Encrypt()].ToString().Decrypt());
	}


	public static bool IsValidUserName(ref int? UsersId, string UserName)
	{
		UsersId = null;
		var dtLogin = new Logins() { Username = UserName }.Select(new Logins() { UsersId = 0 });
		if (dtLogin.Rows.Count > 0)
		{
			var UserCount = new Users()
			{
				UsersId = dtLogin.Rows[0][CS.UsersId].zToInt(),
				eStatus = (int)eStatus.Active,
			}.SelectCount();

			if (UserCount == 0)
				return false;

			UsersId = dtLogin.Rows[0][CS.UsersId].zToInt();

			return true;
		}

		return false;
	}

	public static bool IsValidPassword(int UsersId, string Password)
	{
		var drLogin = new Logins() { UsersId = UsersId }.Select(new Logins() { Password = "", PwdSalt = "" }).Rows[0];
		return ComparePassword(Password, drLogin[CS.Password].ToString(), drLogin[CS.PwdSalt].ToString());
	}

	private static bool ComparePassword(string Password, string DBPassword, string PwdSalt)
	{
		DBPassword = GetDBPassword(DBPassword, PwdSalt);
		return (DBPassword == Password);
	}

	public static string GetDBPassword(string DBPassword, string PwdSalt)
	{
		CC.DecryptPassword(ref DBPassword);
		if (!string.IsNullOrEmpty(PwdSalt) && DBPassword.StartsWith(PwdSalt))
			DBPassword = DBPassword.Substring(PwdSalt.Length, DBPassword.Length - PwdSalt.Length);

		return DBPassword;
	}


	public static bool ChangePassword(int UsersId, string NewPassword)
	{
		var drLogin = new Logins() { UsersId = UsersId }.Select(new Logins() { LoginId = 0 }).Rows[0];

		string salt = GetSalt();
		NewPassword = salt + NewPassword;
		CC.EncryptPassword(ref NewPassword);

		new Logins()
		{
			LoginId = drLogin[CS.LoginId].zToInt(),
			Password = NewPassword,
			PwdSalt = salt,
		}.UpdateAsync();

		new PasswordHistory()
		{
			LoginId = drLogin[CS.LoginId].zToInt(),
			Password = NewPassword,
			PwdSalt = salt,
			CreateDate = IndianDateTime.Now,
		}.InsertAsync();

		try //Logout All Other Device
		{
			var dtLoginCooke = new LoginCookie() { SessionId = UsersId }.Select(new LoginCookie() { LoginCookieId = 0 });
			foreach (System.Data.DataRow drLoginCooke in dtLoginCooke.Rows)
				new LoginCookie() { LoginCookieId = drLoginCooke[CS.LoginCookieId].zToInt() }.Delete();

			WriteCookies(HttpContext.Current.Request.Cookies[CS._RememberCheckBox] != null && HttpContext.Current.Request.Cookies[CS._RememberCheckBox].Value == CS._Checked);
		}
		catch { }

		return true;
	}


	public static string GetSalt()
	{
		byte[] arrSalt = new byte[20];
		new System.Security.Cryptography.RNGCryptoServiceProvider().GetBytes(arrSalt);
		string salt = Convert.ToBase64String(arrSalt);
		if (salt.Length > 10)
			salt = salt.Substring(0, 10);

		return salt;
	}


	public static void WriteSession(int UsersId)
	{
		HttpContext.Current.Session[SessionId] = UsersId.ToString();
		HttpContext.Current.Session.Timeout = 1440;
	}

	public static void WriteCookies(bool RememberUser)
	{
		if (RememberUser)
		{
			string val1 = HttpContext.Current.Session[SessionId].ToString();
			string val2 = "";
			DateTime dt = IndianDateTime.Now.AddDays(20);
			CC.EncryptCookies(ref val1, ref val2, dt);

			HttpContext.Current.Response.Cookies[CS._CurrentLoginTime].Value = val1;
			HttpContext.Current.Response.Cookies[CS._CurrentLoginTime].Expires = dt;

			HttpContext.Current.Response.Cookies[CS._PrevVisit].Value = val2;
			HttpContext.Current.Response.Cookies[CS._PrevVisit].Expires = dt;

			HttpContext.Current.Response.Cookies[CS._RememberCheckBox].Value = CS._Checked;
			HttpContext.Current.Response.Cookies[CS._RememberCheckBox].Expires = IndianDateTime.Now.AddDays(20);

			new LoginCookie()
			{
				SessionId = Convert.ToInt32(HttpContext.Current.Session[SessionId]),
				CookieClUser = val1,
				CookieLtocu = val2,
				CookieCreateTime = IndianDateTime.Now,
				CookieExpireTime = dt,
			}.InsertAsync();
		}
		else
		{
			HttpContext.Current.Response.Cookies[CS._RememberCheckBox].Value = CS._UnChecked;
			HttpContext.Current.Response.Cookies[CS._RememberCheckBox].Expires = IndianDateTime.Now.AddDays(20);
		}
	}

	public static string GetSessionId()
	{
		if (HttpContext.Current.Session[SessionId] == null)
			return "";
		else
			return HttpContext.Current.Session[SessionId].ToString();
	}

	public static int GetUsersId()
	{
		if (HttpContext.Current.Session[SessionId] == null)
			return 0;
		else
			return Convert.ToInt32(HttpContext.Current.Session[SessionId]);
	}


	public static void CreateLogin(int UsersId, string UserName, string Password)
	{
		string salt = GetSalt();
		Password = salt + Password;
		CC.EncryptPassword(ref Password);

		new Logins()
		{
			CurrentLogin = IndianDateTime.Now,
			LastLogin = IndianDateTime.Now,
			Username = UserName.Trim().ToLower(),
			Password = Password.Trim(),
			UsersId = UsersId,
			PwdSalt = salt,
			eStatus = (int)eStatus.Active,
			FailedPasswordAttemptCount = 0,
		}.InsertAsync();
	}


	public static void LoginEntry(int UsersId, eLoginType LoginType, string userAg, ref System.Web.HttpBrowserCapabilities browser)
	{
		string ParentOS = string.Empty, OSName = string.Empty;
		LoginUtilities.GetOSName(userAg, ref ParentOS, ref OSName);

		var drLogin = new Logins() { UsersId = UsersId }.Select(new Logins() { LoginId = 0, CurrentLogin = IndianDateTime.Now }).Rows[0];

		new Logins()
		{
			LoginId = drLogin[CS.LoginId].zToInt(),
			LastLogin = drLogin[CS.CurrentLogin].zIsNullOrEmpty() ? (DateTime?)null : Convert.ToDateTime(drLogin[CS.CurrentLogin]),
			CurrentLogin = IndianDateTime.Now,
		}.UpdateAsync();

		new LoginLog()
		{
			LoginId = drLogin[CS.LoginId].zToInt(),
			LoginTime = IndianDateTime.Now,
			BrowserName = GetBrowserName(userAg, ParentOS, ref browser),
			OSName = OSName,
			ParentOS = ParentOS,
			IP = GetIP(),
			LoginType = (int)LoginType,
			UserAgent = userAg,
		}.InsertAsync();
	}


	public static void GetOSName(string userAg, ref string ParentOS, ref string OSName)
	{
		ParentOS = "Unknown OS";
		OSName = "Unknown OS";

		if (userAg.Contains("Windows NT"))
		{
			ParentOS = "Windows OS";
			#region Windows
			if (userAg.Contains("Windows NT 5.1"))
				OSName = "Windows XP";
			else if (userAg.Contains("Windows NT 6.0"))
				OSName = "Windows Vista";
			else if (userAg.Contains("Windows NT 6.1"))
				OSName = "Windows 7";
			else if (userAg.Contains("Windows NT 6.2"))
				OSName = "Windows 8";
			else if (userAg.Contains("Windows NT 6.3"))
				OSName = "Windows 8.1";
			else if (userAg.Contains("Windows NT 10.0"))
				OSName = "Windows 10";
			#endregion
		}
		else if (userAg.Contains("Android") || (userAg.Contains(" Adr ") && userAg.Contains("UCBrowser") && userAg.Contains("Mobile")))     //"UCWEB/2.0" && "en-US;" 
		{
			ParentOS = "Android OS";
			#region Android
			try
			{
				if (userAg.Contains("Android"))
				{
					userAg = userAg.Substring(userAg.IndexOf("Android"));
					userAg = userAg.Substring(0, userAg.IndexOf(";"));
					OSName = userAg;
				}
				else
				{
					userAg = userAg.Substring(userAg.IndexOf("Android"));
					userAg = userAg.Substring(0, userAg.IndexOf(";"));
					OSName = userAg;
				}
			}
			catch
			{
				OSName = "Android OS";
			}
			#endregion
		}
		else if (userAg.Contains("Linux x86_64"))
		{
			ParentOS = "Android OS";
			OSName = "Android OS";
		}
		else if (userAg.Contains("iPhone"))
		{
			ParentOS = "iPhone OS";
			#region iPhone
			try
			{
				userAg = userAg.Substring(userAg.IndexOf("iPhone"));
				userAg = userAg.Substring(userAg.LastIndexOf("iPhone"));
				userAg = userAg.Substring(0, userAg.IndexOf(")"));
				if (userAg.Contains("like"))
					userAg = userAg.Substring(0, userAg.IndexOf("like") - 1);
				OSName = userAg;
			}
			catch
			{
				OSName = "iPhone OS";
			}
			#endregion
		}
		else if (userAg.Contains("Windows Phone"))
		{
			ParentOS = "Windows Phone OS";
			#region Windows Phone
			try
			{
				userAg = userAg.Substring(userAg.IndexOf("Windows Phone"));
				userAg = userAg.Substring(0, userAg.IndexOf(";"));
				OSName = userAg;
			}
			catch
			{
				OSName = "Windows Phone OS";
			}
			#endregion
		}
		else if (userAg.Contains("Linux"))
		{
			ParentOS = "Linux";
			OSName = "Linux OS";
		}
		else if (userAg.Contains("Mac OS"))
		{
			ParentOS = "Mac OS";
			#region Mac OS
			try
			{
				userAg = userAg.Substring(userAg.IndexOf("Mac OS"));
				userAg = userAg.Substring(0, userAg.IndexOf(")"));
				OSName = userAg;
			}
			catch
			{
				OSName = "Mac OS";
			}
			#endregion
		}
		else if (userAg.Contains("BB10"))
		{
			ParentOS = "BlackBerry OS";
			#region BB10
			OSName = "BB10";
			#endregion
		}
		else if (userAg.Contains("BlackBerry"))
		{
			ParentOS = "BlackBerry OS";
			#region BlackBerry OS
			try
			{
				userAg = userAg.Substring(userAg.IndexOf("BlackBerry"));
				userAg = userAg.Substring(0, userAg.IndexOf(")"));
				userAg = userAg.Split(new char[] { ';' })[2].Trim();
			}
			catch
			{
				OSName = "BlackBerry OS";
			}
			#endregion
		}
		else
		{
			ParentOS = "Unknown OS";
			OSName = "Unknown OS";
		}
	}

	public static string GetBrowserName(string userAg, string ParentOS, ref System.Web.HttpBrowserCapabilities browser)
	{
		string BrowserName = "Unknown";
		if (browser.MajorVersion != 0)
		{
			BrowserName = browser.Browser + " " + browser.Version;
		}
		else
		{
			if (ParentOS == "Android OS")
			{
				#region Android Browser
				if (userAg.Contains("UCBrowser"))
				{
					#region UCBrowser
					try
					{
						userAg = userAg.Substring(userAg.IndexOf("UCBrowser"));
						userAg = userAg.Substring(userAg.IndexOf("/") + 1);
						userAg = userAg.Substring(0, userAg.IndexOf(" "));
						BrowserName = "UCBrowser " + userAg;   //UCBrowser + Version
					}
					catch
					{
						BrowserName = "UCBrowser";
					}
					#endregion
				}
				else if (userAg.Contains("Opera"))
				{
					#region Opera
					try
					{
						userAg = userAg.Substring(userAg.IndexOf("Opera"));
						userAg = userAg.Substring(userAg.IndexOf("/") + 1);
						userAg = userAg.Substring(0, userAg.IndexOf(" "));
						BrowserName = "Opera " + userAg;   //Opera + Version
					}
					catch
					{
						BrowserName = "Opera";
					}
					#endregion
				}
				else if (userAg.Contains("Linux x86_64"))
				{
					BrowserName = "Android Browser (Desktop View)";
				}
				else
				{
					BrowserName = "Android Browser";
				}
				#endregion
			}
			else if (ParentOS == "iPhone OS")
			{
				#region iPhone Browser
				if (userAg.Contains("Version"))
				{
					#region Safari
					try
					{
						userAg = userAg.Substring(userAg.IndexOf("Version"));
						userAg = userAg.Substring(userAg.IndexOf("/") + 1);
						userAg = userAg.Substring(0, userAg.IndexOf(" "));
						BrowserName = "Safari " + userAg;   //Safari + Version
					}
					catch
					{
						BrowserName = "Safari";
					}
					#endregion
				}
				else if (userAg.Contains("CriOS"))
				{
					#region Chrome
					try
					{
						userAg = userAg.Substring(userAg.IndexOf("CriOS"));
						userAg = userAg.Substring(userAg.IndexOf("/") + 1);
						userAg = userAg.Substring(0, userAg.IndexOf(" "));
						BrowserName = "Chrome " + userAg;   //Chrome + Version
					}
					catch
					{
						BrowserName = "Chrome";
					}
					#endregion
				}
				else if (userAg.Contains("UCBrowser"))
				{
					#region UCBrowser
					try
					{
						userAg = userAg.Substring(userAg.IndexOf("UCBrowser"));
						userAg = userAg.Substring(userAg.IndexOf("/") + 1);
						userAg = userAg.Substring(0, userAg.IndexOf(" "));
						BrowserName = "UCBrowser " + userAg;   //UCBrowser + Version
					}
					catch
					{
						BrowserName = "UCBrowser";
					}
					#endregion
				}
				else if (userAg.Contains("OPiOS"))
				{
					#region Opera
					try
					{
						userAg = userAg.Substring(userAg.IndexOf("OPiOS"));
						userAg = userAg.Substring(userAg.IndexOf("/") + 1);
						userAg = userAg.Substring(0, userAg.IndexOf(" "));
						BrowserName = "Opera " + userAg;   //Opera + Version
					}
					catch
					{
						BrowserName = "Opera";
					}
					#endregion
				}
				else
				{
					BrowserName = browser.Browser + " " + browser.Version;
				}
				#endregion
			}
			else
			{
				BrowserName = "Unknown";
			}
		}
		return BrowserName;
	}

	public static string GetIP()
	{
		string VisitorsIPAddr = string.Empty;
		if (HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
		{
			VisitorsIPAddr = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
			if (string.IsNullOrEmpty(VisitorsIPAddr))
				VisitorsIPAddr = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
		}
		else if (HttpContext.Current.Request.UserHostAddress.Length != 0)
		{
			VisitorsIPAddr = HttpContext.Current.Request.UserHostAddress;
		}
		return VisitorsIPAddr;
	}


	#region Logout

	public static void RemoveSession()
	{
		HttpContext.Current.Session[SessionId] = null;
		HttpContext.Current.Session.Abandon();
		HttpContext.Current.Session.Clear();

		HttpContext.Current.Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
		HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
		HttpContext.Current.Response.Cache.SetNoStore();
	}

	public static void RemoveCookies()
	{
		if (HttpContext.Current.Request.Cookies[CS._CurrentLoginTime] != null && HttpContext.Current.Request.Cookies[CS._PrevVisit] != null)
		{
			new LoginCookie()
			{
				CookieClUser = HttpContext.Current.Request.Cookies[CS._CurrentLoginTime].Value,
				CookieLtocu = HttpContext.Current.Request.Cookies[CS._PrevVisit].Value
			}.Delete();
		}

		string RememberCheckBox = CS._UnChecked;
		if (HttpContext.Current.Request.Cookies[CS._RememberCheckBox] != null)
			RememberCheckBox = HttpContext.Current.Request.Cookies[CS._RememberCheckBox].Value;

		HttpCookie aCookie = new HttpCookie("tmp");
		int i = 0;
		int limit = HttpContext.Current.Request.Cookies.Count - 1;
		for (i = 0; i <= limit; i++)
		{
			aCookie = HttpContext.Current.Request.Cookies[i];
			aCookie.Expires = IndianDateTime.Now.AddDays(-1);
			HttpContext.Current.Response.Cookies.Add(aCookie);
		}

		HttpContext.Current.Response.Cookies[CS._RememberCheckBox].Value = RememberCheckBox;
		HttpContext.Current.Response.Cookies[CS._RememberCheckBox].Expires = IndianDateTime.Now.AddDays(20);
	}

	#endregion


	public static bool IsValidUsersId(string strUsersId)
	{
		if (!strUsersId.zIsNumber())
			return false;
		else
		{
			return IsValidUsersId(int.Parse(strUsersId));
		}
	}

	public static bool IsValidUsersId(int UsersId)
	{
		if (new Users() { UsersId = UsersId, eStatus = (int)eStatus.Active }.SelectCount() == 0)
			return false;
		else
			return true;
	}
}
