using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml;
using HtmlTags;

namespace BriefFiniteElementNet.Validation.Ui
{
    class Program
    {
        static void Main(string[] args)
        {
            TestBar();
        }

        static void TestBar()
        {
            Console.WriteLine("Bar Element Test - Start");

            ExportToHtmFile("c:\\temp\\validation.html", new BarElementTester());

            //var resss = new BarElementTester().DoValidation();
        }

        public static void ExportToHtmFile(string fileName, params IValidator[] validators)
        {
            var doc = new HtmlTags.HtmlDocument();
            
            doc.Head
                .Add("description").Attr("content", "Some validations of BriefFiniteEelement.NET library")
                .Parent.Add("keywords").Attr("content",
                    "finite element, C#, FEA, FEM, BriefFiniteEelement.NET, BriefFiniteEelementDOTNET")
                .Parent.Add("viewport").Attr("content", "width=device-width, initial-scale=1")
                
                .Parent.Add("link").Attr("rel", "stylesheet").Attr("crossorigin", "anonymous").Attr("href", "https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0/css/bootstrap.min.css").Attr("integrity","sha384-Gn5384xqQ1aoWXA+058RXPxPg6fy4IWvTNh0E263XmFcJlSAwiGgFAW/dAiS6JXm").Text(" ")
                .Parent.Add("script").Attr("crossorigin", "anonymous").Attr("src", "https://code.jquery.com/jquery-3.2.1.slim.min.js").Attr("integrity","sha384-KJ3o2DKtIkvYIK3UENzmM7KCkRr/rE9/Qpg6aAZGJwFDMVNA/GpGFF93hXpG5KkN").Text(" ")
                .Parent.Add("script").Attr("crossorigin", "anonymous").Attr("src","https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.12.9/umd/popper.min.js").Attr("integrity","sha384-ApNbgh9B+Y1QKtv3Rn7W3mgPxhU9K/ScQsAP7hUibX39j7fakFPskvXusvfa0b4Q").Text(" ")
                .Parent.Add("script").Attr("crossorigin", "anonymous").Attr("src", "https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0/js/bootstrap.min.js").Attr("integrity","sha384-JZR6Spejh4U02d8jOt6vLEHfe/JQGiRRSQQxSfFWpi1MquVdAyjUar5+76PVCmYl").Text(" ")
                ;

            //doc.ReferenceJavaScriptFile("https://code.jquery.com/jquery-3.2.1.slim.min.js");
            ;
            /*
            doc.AddScript("javascript","https://code.jquery.com/jquery-3.2.1.slim.min.js");
            doc.AddJavaScript("https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.12.9/umd/popper.min.js");
            doc.AddJavaScript("https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0/js/bootstrap.min.js");

            doc.AddStyle("https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0/css/bootstrap.min.css");
            
            */
            doc.Title = "BriefFiniteElement Validtion Cases";

            var body = doc.Body;

            body.Add("noscript")
                .Text(
                    "This page needs javascript to be enabled in your webbrowser, otherwise page will be a little illegible");

            body.Style("padding", "60px");

            body.Add("script").Attr("type", "text/javascript")
                .Text(
                    @"$(document).ready(function(){jQuery?void(0):alert('jquery not loaded')});");



            var ctx = body.Add("div").Id("ctxtbl");


            foreach (var validator in validators)
            {
                var valReses = validator.DoValidation();

                foreach (var valRese in valReses)
                {
                    var id = Guid.NewGuid().ToString("N").Substring(0, 5);

                    valRese.Span.Id(id);

                    body.Children.Add(valRese.Span);
                    ctx.Add("a").Attr("href", "#" + id).Text(valRese.Title);
                    ctx.Add("br");
                }
            }

            var fl = "c:\\temp\\val-res.html";

            var xmlDoc = new XmlDocument();

            xmlDoc.LoadXml(doc.ToString());

            xmlDoc.Save(fl);

            
            Process.Start(fl);
        }
    }
}
