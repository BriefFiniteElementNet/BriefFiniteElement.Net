﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BriefFiniteElementNet.Validation
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ValidationCaseAttribute : Attribute
    {
        public string Title;

        public Type[] Elements; //elements that are validated

        public ValidationCaseAttribute(string title, params Type[] elements)
        {
            Title = title;
            Elements = elements;
        }
    }

    public interface IValidationCase
    {
        //string GetTitle();

        ValidationResult Validate();
    }
}
