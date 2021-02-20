<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="VersonUpdate_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Version Update - Shopping Portal</title>
    <link rel="icon" href="OCTFISIcon.ico" />
    <link rel="apple-touch-icon" href="OCTFISIcon.ico" />
    <link href="css.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <div class="col12 m0auto">
        <center>
            <div class="col8 m0auto p10 border">
                <div class="col12">
                    <div class="col col6 mt10">
                        <h2 class="text-center">
                            Current Version:
                            <asp:Label ID="lblCurentVerson" runat="server" Text=""></asp:Label></h2>
                    </div>
                    <div class="col col6 mt10">
                        <h2 class="text-center">
                            Latest Version:
                            <asp:Label ID="lblLatestVersion" runat="server" Text=""></asp:Label>
                            <br />
                            <asp:Label ID="lblLastVersionUpdateDate" CssClass="LastVersionUpdateDate" runat="server"
                                Text=""></asp:Label>
                        </h2>
                    </div>
                </div>
                <div id="divSucess" runat="server" class="col12">
                    <div class="col10 m0auto done">
                        <asp:Label ID="lblSucess" runat="server" Text="Your System Update Successfully."></asp:Label>
                        <a href="" class="close"></a>
                    </div>
                </div>
                <div id="divError" runat="server" class="col12">
                    <div class="col10 m0auto fail">
                        <asp:Label ID="lblError" runat="server" Text="Your System Update failed."></asp:Label>
                        <a href="" class="close"></a>
                    </div>
                </div>
                <div id="divUpdateNow" runat="server">
                    <div class="col12">
                        <div class="col6 text-center m0auto p5">
                            <asp:LinkButton ID="btnUpdateVerson" class="button button-exlarge button-green" OnClick="btnUpdateVerson_Click"
                                OnClientClick="loadButton();" runat="server"> <h3>
                                Update Now</h3></asp:LinkButton>
                        </div>
                    </div>
                    <div class="col12">
                        <div class="col6 text-center m0auto p5">
                            <img src="updatenow.jpg" class="col5" />
                        </div>
                    </div>
                </div>
                <div id="divUptodate" runat="server">
                    <div class="col12">
                        <div class="col6 text-center m0auto p5">
                            <a id="A1" class="button button-exlarge button-green">
                                <h3>
                                    Your system is currenttly up to date</h3>
                            </a>
                        </div>
                    </div>
                    <div class="col12">
                        <div class="col6 text-center m0auto p5">
                            <img src="uptodate.jpg" class="col5" />
                        </div>
                    </div>
                </div>
            </div>
        </center>
    </div>
    </form>

    <script type="text/javascript">
	function loadButton()
	{
		var element = document.getElementById('<%= btnUpdateVerson.ClientID %>');
		element.classList.add("load-button");
		return false;
	}
    </script>

</body>
</html>
