using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCommerce.BLL.Exceptions;

public class CustomValidationException : Exception
{
    public Dictionary<string, string[]> Errors { get; }

    public CustomValidationException(Dictionary<string, string[]> errors)
        : base("One or more validation errors occurred.")
    {
        Errors = errors;
    }
    public CustomValidationException(string key, string error)
        : base("One or more validation errors occurred.")
    {
        Errors = new Dictionary<string, string[]>
        {
            {key ,new [] {error} }
        };
    }
}
