<%@ Application Language="C#" %>

<script RunAt="server">

    void Application_Start(object sender, EventArgs e)
    {
        CU.AssignValues();

        string Message = "";
        VU.UpdateVersion(ref Message);
        //new System.Threading.Thread(new System.Threading.ThreadStart(CrateNotification)).Start();
    }

    private void CrateNotification()
    {
        //while (true)
        //{
        //	try
        //	{
        //		int i = 0;
        //		while (i < 60 * 5)
        //		{
        //			System.Threading.Thread.Sleep(1000);
        //			i++;
        //		}
        //	}
        //	catch { }
        //}
    }

    //void Application_End(object sender, EventArgs e)
    //{
    //}

    //void Application_Error(object sender, EventArgs e)
    //{
    //    // Code that runs when an unhandled error occurs
    //}

    //void Session_Start(object sender, EventArgs e)
    //{
    //    // Code that runs when a new session is started
    //}

    //void Session_End(object sender, EventArgs e)
    //{
    //    // Code that runs when a session ends. 
    //    // Note: The Session_End event is raised only when the sessionstate mode
    //    // is set to InProc in the Web.config file. If session mode is set to StateServer 
    //    // or SQLServer, the event is not raised.
    //}

</script>

