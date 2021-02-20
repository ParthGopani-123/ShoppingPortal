using System;
using System.Net;
using System.Text;  // for class Encoding
using System.IO;

public partial class _Default : CompressorPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Response.Redirect("Home.aspx");

        //var request = (HttpWebRequest)WebRequest.Create("http://localhost:4937/API/APIShopingPortal.aspx?API=2");

        //var postData = "";
        //postData += "&Name=jatin lathiya";
        //var data = Encoding.ASCII.GetBytes(postData);

        //request.Method = "POST";
        //request.ContentType = "application/x-www-form-urlencoded";
        //request.ContentLength = data.Length;

        //using (var stream = request.GetRequestStream())
        //{
        //    stream.Write(data, 0, data.Length);
        //}

        //var response = (HttpWebResponse)request.GetResponse();

        //var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

    }
}
