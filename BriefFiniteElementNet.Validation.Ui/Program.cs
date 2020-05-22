using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml;
using HtmlTags;

namespace BriefFiniteElementNet.Validation.Ui
{
    internal static class Program
    {
        static void Main(string[] args)
        {

            //TestTri();
            TestBar();
            //TestTelepaty();
            //TestTetra();
            //return;
            //BarElementTester.ValidateConsoleUniformLoad();
            //BarElementTester.TestEndReleaseStyiffness();

            //GithubIssues.Issue29.Run();
        }

        static void TestTelepaty()
        {
            Validation.EqualDofValidator.Test1();

            //var resss = new BarElementTester().DoValidation();
        }

        static void TestBar()
        {
            //Console.WriteLine("Bar Element Test - Start");


            ExportToHtmFile("c:\\temp\\validation.html", new BarElementTester());
            //BarElementTester.TestEndReleaseStyiffness();

            //var resss = new BarElementTester().DoValidation();
        }

        static void TestTetra()
        {
            Console.WriteLine("Tetrahedron Test - Start");


            ExportToHtmFile("c:\\temp\\validation.html", new TetrahedronElementTester());


            //var resss = new BarElementTester().DoValidation();
        }

        static void TestTri()
        {
            Console.WriteLine("Trianlge Element Test - Start");


            ExportToHtmFile("c:\\temp\\tri-validation.html", new TriangleElementTester());


            //var resss = new BarElementTester().DoValidation();
        }

        public static void ExportToHtmFile(string fileName, params IValidator[] validators)
        {
            bool all = false;

            while(true)
            {
                Console.Write("Please enter the type of validations to run (All/Populars)[a/p]:p");
                Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                var k = Console.ReadKey();

                if (k.Key == ConsoleKey.A )
                {
                    all = true;
                    break;
                }

                if (k.Key == ConsoleKey.P || k.Key == ConsoleKey.Enter)
                {
                    all = false;
                    break;
                }
            }
            

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



            var ctx = body.Add("div").Id("ctxtbl").AddClass("jumbotron");

            ctx.Add("div").Add("p").Text("Table of contents:");

            body = body.Add("div").AddClass("container");

            foreach (var validator in validators)
            {
                var valReses = all ?
                    validator.DoAllValidation() :
                    validator.DoPopularValidation();

                foreach (var valRese in valReses)
                {
                    var id = Guid.NewGuid().ToString("N").Substring(0, 5);

                    var validationSpan = body.Add("div").Id(id).AddClasses("card");


                    validationSpan.Add("div").AddClasses("card-header").Text("Validation");//title


                    var blc = validationSpan.Add("div").AddClasses("card-block");

                    blc.Add("h4").AddClasses("card-title").Text(valRese.Title);
                    var panelBody = blc.Add("div").AddClasses("card-text");


                    panelBody.Children.Add(valRese.Span);

                    if (valRese.ValidationFailed.HasValue)
                    {
                        var fail = valRese.ValidationFailed.Value;

                        var sp2 = validationSpan.Add("div").AddClass("alert").AddClass(fail? "alert-danger" : "alert-success").Attr("role", "alert");

                        sp2.Text(fail ? "Validation Failed!!" : "Validation Success!!");
                        //body.Children.Add(sp2);
                    }
                        


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
