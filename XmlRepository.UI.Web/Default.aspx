<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="XmlRepository.UI.Web.Default" %>

<!DOCTYPE html>

<html>
    <head runat="server">
		<meta http-equiv="content-language" content="de-DE" />

		<meta name="author" content="Golo Roden, Peter Bucher" />
		<meta name="description" content="xmlrepository.ch" />
		<meta name="keywords" content=".NET, C#, XML, repository, data, speichern, save, store, serialisieren, serialize" />
		<meta name="robots" content="index, follow" />

		<link rel="icon" type="image/x-icon" href="/favicon.ico" />
		<link rel="shortcut icon" type="image/x-icon" href="/favicon.ico" />
		
        <link rel="stylesheet" type="text/css" href="/Styles/Layout.css" />
		<link rel="stylesheet" type="text/css" href="/Styles/Base.css" />

        <script src="/Scripts/jquery-1.4.2.min.js" type="text/javascript"></script>

        <title>xmlrepository.ch | Startseite</title>
        
        <script type="text/javascript">
            var _gaq = _gaq || [];
            _gaq.push(['_setAccount', 'UA-1251716-30']);
            _gaq.push(['_trackPageview']);

            (function () {
                var ga = document.createElement('script'); ga.type = 'text/javascript'; ga.async = true;
                ga.src = ('https:' == document.location.protocol ? 'https://ssl' : 'http://www') + '.google-analytics.com/ga.js';
                var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(ga, s);
            })();
        </script>
    </head>

    <body>
        <form runat="server">
            <div id="content">
                <h1>xmlrepository.ch</h1><br />
                <p>
                    &lt;xmlrepository.ch&gt;<br />
                    &nbsp;&nbsp;&nbsp;&nbsp;&lt;description&gt;&lt;![CDATA[<br />
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="highlight">Store PONOs (plain old .NET objects) as XML in memory or to disk.</span><br />
                    &nbsp;&nbsp;&nbsp;&nbsp;]]&gt;&lt;/description&gt;<br />
                    &nbsp;&nbsp;&nbsp;&nbsp;&lt;download&gt;<br />
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;download type="<a href="">sources</a>" mimeType="application/zip" /&gt;<br />
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;download type="<a href="">binary</a>" mimeType="application/zip" /&gt;<br />
                    &nbsp;&nbsp;&nbsp;&nbsp;&lt;/download&gt;<br />
                    &nbsp;&nbsp;&nbsp;&nbsp;&lt;copyright&gt;&lt;![CDATA[<br />
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&copy; Copyright 2011 <a href="http://www.peterbucher.ch">Peter Bucher</a> und <a href="http://www.goloroden.de">Golo Roden</a>. Alle Rechte vorbehalten.<br />
                    &nbsp;&nbsp;&nbsp;&nbsp;]]&gt;&lt;/copyright&gt;<br />
                    &lt;/xmlrepository.ch&gt;
                </p>
            </div>
        </form>

        <script type="text/javascript">
            $(document).ready(function () {
                center();
                center();
            });

            $(window).bind("resize", center);

            function center() {
                var content = $("#content");

                var contentHeight = content.outerHeight();
                var contentWidth = content.outerWidth();

                var windowHeight = $(window).height();
                var windowWidth = $(window).width();

                content.css({
                    position: "absolute",
                    top: ((windowHeight - contentHeight) / 2) + "px",
                    left: ((windowWidth - contentWidth) / 2) + "px"
                });
            }
        </script>
    </body>
</html>