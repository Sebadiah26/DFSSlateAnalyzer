using System.ComponentModel.DataAnnotations;

namespace StaffManagement.Models.CustomAttributes
{
    public class DerivedField : ValidationAttribute
    {

        // public string GetErrorMessage() => $"Contractors must have a Start Date set.";

        //protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        //{
        //    var account = (AccountModel)validationContext.ObjectInstance;

        //    if (account.JobRecord.IsPHRST != null && account.JobRecord.IsPHRST == false && account.JobRecord.StartDate == null)
        //    {
        //        return new ValidationResult(GetErrorMessage());
        //    }

        //    return ValidationResult.Success;
        //}
    }
}
