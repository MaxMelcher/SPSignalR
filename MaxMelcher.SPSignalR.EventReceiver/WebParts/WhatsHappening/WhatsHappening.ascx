<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WhatsHappening.ascx.cs" Inherits="MaxMelcher.SPSignalR.EventReceiver.WebParts.WhatsHappening.WhatsHappening" %>


<script src="/_layouts/15/MaxMelcher.SPSignalR.EventReceiverExample/jquery-1.6.4.min.js"></script>
<script src="/_layouts/15/MaxMelcher.SPSignalR.EventReceiverExample/jquery.signalR-1.1.2.min.js"></script>
<script src="/signalr/hubs" type="text/javascript"></script>

<h2>What's happening?</h2>

<div id="container">
</div>

<script type="text/javascript">
    $.connection.hub.logging = true;

    var myHub = $.connection.docLibHub;



    myHub.client.Notify = function (title, url, type) {
        var eventtype;
        if (type == 0) {
            eventtype = "added";
        }
        else if (type == 1) {
            eventtype = "updated";
        } else
            eventtype = "deleted";

        $('#container').append('<li><a href="' + url + '">' + title + '</a>: ' + eventtype + '</li>');
    };

    $.connection.hub.error(function () {
        alert("An error occured");
    });

    $.connection.hub.start()
                    .done(function () {

                    })
                    .fail(function () {
                        alert("Could not Connect!");
                    });
</script>