<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="GetOtherNewsRss._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript">
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
        </div>
        <asp:Button ID="Button1" runat="server" Text="Button" OnClick="Button1_Click" />
        <asp:Timer runat="server" ID="timer1" OnTick="Button1_Click" Interval="30000"></asp:Timer>

        <h3>Read RSS Feed</h3>
        <div id="divRSS">
            <asp:Panel ID="panelRss" runat="server"></asp:Panel>
        </div>
        <div style="height: 90%; overflow: auto;">

            <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
            <asp:UpdatePanel ID="updpanel" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="Button1" />
                    <asp:AsyncPostBackTrigger ControlID="Timer1" EventName="Tick" />
                </Triggers>
                <ContentTemplate>
                    更新時間：<asp:Label ID="lbrss" runat="server"></asp:Label>
                    <asp:ListView ID="gvRss" runat="server" Style="width: 90%" DataSourceID="SqlDataSource1">
                        <EmptyDataTemplate>
                            <h3>未傳回資料。</h3>
                        </EmptyDataTemplate>
                        <ItemTemplate>
                            <table width="100%" border="0" cellpadding="0" cellspacing="5">
                                <tr>
                                    <td>
                                        <h3 style="color: #3e7cff"><%# Eval("RM_Title") %></h3>
                                    </td>
                                    <td width="200px">
                                        <%# Eval("RM_PubDate") %>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <hr />
                                        <%# Eval("RM_Description") %>
                                    </td>
                                </tr>
                                <tr>
                                    <td>&nbsp;</td>
                                    <td align="right">
                                        <a href='<%# Eval("RM_Link") %>' target="_blank">Read more......</a>
                                    </td>
                                </tr>
                            </table>
                        </ItemTemplate>
                    </asp:ListView>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>

        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:DefaultDataSource %>" SelectCommand="
                select * from RSS_message where RM_Time<=getDate() order by RM_ID desc
            ">
        </asp:SqlDataSource>
</form>
</body>

</html>
