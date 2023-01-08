using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.ComponentModel.DataAnnotations;


namespace StaffManagement.Models.CustomAttributes
{
    public class RequiredIfActionMethodAttribute : ValidationAttribute, IClientModelValidator
    {

        public string GetErrorMessage() => CustomErrorMessage;

        public string RequiredActionMethod { get; set; }

        public string CurrentActionMethod { get; set; }

        public string CustomErrorMessage { get; set; } = "";



        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {



            var httpContextAccessor = (IHttpContextAccessor)validationContext.GetService(typeof(IHttpContextAccessor));

            CurrentActionMethod = httpContextAccessor.HttpContext.Request.RouteValues["action"].ToString();




            if ((value == null || value.ToString() == "") && CurrentActionMethod == RequiredActionMethod)
            {



                return new ValidationResult(GetErrorMessage());


            }










            return ValidationResult.Success;
        }

        //public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        //{
        //    var rule = new ModelClientValidationRule
        //    {
        //        ErrorMessage = this.ErrorMessage,
        //        ValidationType = "requiredifactionmethod"
        //    };

        //    rule.ValidationParameters["actionmethod"] = this.ActionMethod;

        //    yield return rule;
        //}

        public void AddValidation(ClientModelValidationContext context)
        {


            CurrentActionMethod = context.ActionContext.HttpContext.Request.RouteValues["action"].ToString();


            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            context.Attributes.Add("data-val", "true");
            context.Attributes.Add("data-val-requiredifactionmethod", GetErrorMessage());
            context.Attributes.Add("data-val-requiredactionmethod", RequiredActionMethod);
            context.Attributes.Add("data-val-currentactionmethod", CurrentActionMethod);
        }

    }
}
