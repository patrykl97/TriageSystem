using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TriageSystem.Validations
{
    public class PPSAttribute : ValidationAttribute
    {


        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                string pps = value.ToString();
                string regex = "[0-9]{7}[A-Za-z]{1}[AaHhWw]?";
                if (!Regex.IsMatch(pps, regex, RegexOptions.IgnoreCase))
                {
                    return new ValidationResult(GetErrorMessage());
                }
            }

            return ValidationResult.Success;
        }

        private string GetErrorMessage()
        {
            return "PPS number is invalid!";
        }
    }

}