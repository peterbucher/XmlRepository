﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="XmlRepository.UI.Web.Default" %>

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

        <title>xmlrepository.ch</title>
        
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
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="highlight">XML-basiertes Repository für flache .NET-Objekte, welches das Dateisystem und<br />
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;In-Memory unterstützt.</span><br />
                    &nbsp;&nbsp;&nbsp;&nbsp;]]&gt;&lt;/description&gt;<br /><br />
                    &nbsp;&nbsp;&nbsp;&nbsp;&lt;example&gt;&lt;![CDATA[<br />
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;XmlRepository.DefaultQueryProperty = "Id";<br />
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;XmlRepository.DataProvider = new XmlFileProvider("~/App_Data/");<br /><br />
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;using(var repository = XmlRepository.GetInstance&lt;Foo&gt;) {<br />
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;var foos = repository.LoadAllBy(f => f.Id > 42);<br />
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;repository.SaveOnSubmit(new Foo { ... });<br />
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;repository.DeleteOnSubmit(f =&gt; f.Id == 23);<br />
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}<br />
                    &nbsp;&nbsp;&nbsp;&nbsp;]]&gt;&lt;/example&gt;<br /><br />
                    &nbsp;&nbsp;&nbsp;&nbsp;&lt;downloads&gt;<br />
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;download type="<a href="/Downloads/XmlRepository-201101091814.zip">assembly-net3.5</a>" mime="application/zip" /&gt;<br />
                    &nbsp;&nbsp;&nbsp;&nbsp;&lt;/downloads&gt;<br /><br />
                    &nbsp;&nbsp;&nbsp;&nbsp;&lt;documentation /&gt;<br /><br />
                    &nbsp;&nbsp;&nbsp;&nbsp;&lt;copyright&gt;&lt;![CDATA[<br />
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&copy; Copyright 2009-<%= DateTime.Now.Year %> <a href="http://www.peterbucher.ch">Peter Bucher</a> und <a href="http://www.goloroden.de">Golo Roden</a>. Alle Rechte vorbehalten.<br />
                    &nbsp;&nbsp;&nbsp;&nbsp;]]&gt;&lt;/copyright&gt;<br />
                    &lt;/xmlrepository.ch&gt;
                </p>
            </div>
        </form>

        <script type="text/javascript">
            $(document).ready(function () {
                // Duplicate call to center() is required, at least for Google Chrome (other
                // browsers were not tested yet).
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