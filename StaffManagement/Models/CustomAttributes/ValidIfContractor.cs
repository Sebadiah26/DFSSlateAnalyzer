using System.ComponentModel.DataAnnotations;

namespace StaffManagement.Models.CustomAttributes
{
    public class ValidIfContractor : ValidationAttribute
    {

        public string GetErrorMessage() => $"Contractors/Other must have a Start Date and End Date set.";

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            var container = validationContext.ObjectInstance;

            if (container.GetType() == typeof(AccountModel))
            {


                if ((((AccountModel)container).AccountTypeId == (int)EnumAccountTypeDescription.Contractor || ((AccountModel)container).AccountTypeId == (int)EnumAccountTypeDescription.LongTermSub))
                {
                    // Is Contractor or Long-term sub so check  Start and End Dates.

                    if (((AccountModel)container).JobRecord.StartDate == null || ((AccountModel)container).JobRecord.EndDate == null)

                    {
                        return new ValidationResult(GetErrorMessage());

                    }
                }









            }


            return ValidationResult.Success;
        }
    }
}
