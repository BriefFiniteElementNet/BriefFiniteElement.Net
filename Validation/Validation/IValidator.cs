﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet.Validation
{
    public interface IValidator
    {
        ValidationResult[] DoPopularValidation();
        ValidationResult[] DoAllValidation();
    }
}
