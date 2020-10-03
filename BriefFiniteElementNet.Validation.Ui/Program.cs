using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using HtmlTags;
using BriefFiniteElementNet.Validation;

namespace BriefFiniteElementNet.Validation.Ui
{
    internal static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            RunAllValidations();

            Console.ReadKey();
        }

        static void RunAllValidations()
        {
            var caseList = new List<IValidationCase>();

            {//load cases

                var asm = typeof(IValidator).Assembly;
                var types = asm.GetTypes();

                foreach (var type in types)
                {

                    if (!ImplementsInterface(type, typeof(IValidationCase)))
                        continue;

                    var attribs = type.GetCustomAttributes(typeof(ValidationCaseAttribute));

                    if (!attribs.Any())
                        continue;

                    var cse = (IValidationCase)Activator.CreateInstance(type);

                    caseList.Add(cse);
                }
            }

            ExportToHtmFile(Path.GetTempFileName() + ".html", caseList.ToArray());
        }

        public static bool ImplementsInterface(this Type type, Type @interface)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (@interface == null)
            {
                throw new ArgumentNullException(nameof(@interface));
            }

            var interfaces = type.GetInterfaces();
            if (@interface.IsGenericTypeDefinition)
            {
                foreach (var item in interfaces)
                {
                    if (item.IsConstructedGenericType && item.GetGenericTypeDefinition() == @interface)
                    {
                        return true;
                    }
                }
            }
            else
            {
                foreach (var item in interfaces)
                {
                    if (item == @interface)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static void ExportToHtmFile(string fileName, params IValidationCase[] validators)
        {/*
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
            }*/
            

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
                var valRese = validator.Validate();

                //foreach (var valRese in valReses)
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

            var fl = System.IO.Path.GetTempFileName() + ".html";// "c:\\temp\\val-res.html";

            var xmlDoc = new XmlDocument();

            xmlDoc.LoadXml(doc.ToString());

            xmlDoc.Save(fl);

            Process.Start(fl);
        }
    }
}
