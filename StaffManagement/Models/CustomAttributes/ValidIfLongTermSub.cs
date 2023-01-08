using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StaffManagement.Models.CustomAttributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class ValidIfLongTermSub : ValidationAttribute, IClientModelValidator
    {







        public string GetErrorMessage() => $"Long-Term Subs must have a Substitute set.";


        public void AddValidation
(ClientModelValidationContext context)
        {
            context.Attributes.Add("data-val", "true");
            context.Attributes.Add("data-val-longtermsub",
    "Invalid country. Valid values are USA, UK, and India.");
        }


        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {


            var httpContextAccessor = (IHttpContextAccessor)validationContext.GetService(typeof(IHttpContextAccessor));
            var request = httpContextAccessor.HttpContext.Request;

            var container = validationContext.ObjectInstance;

            if (container.GetType() == typeof(AccountModel))
            {


                if (
                               (((AccountModel)container).AccountTypeId == (int)EnumAccountTypeDescription.LongTermSub)
                               &&
                               (request.Form["linkedemployee"].ToString() == ""))

                {
                    return new ValidationResult(GetErrorMessage());
                }







            }


            return ValidationResult.Success;
        }

        public class AttributeUtils
        {
            public static bool MergeAttribute(
                IDictionary<string, string> attributes,
                string key,
                string value)
            {
                if (attributes.ContainsKey(key))
                {
                    return false;
                }
                attributes.Add(key, value);
                return true;
            }
        }


        public class ValidIfLongTermSubAttributeAdapter : AttributeAdapterBase<ValidIfLongTermSub>
        {
            public ValidIfLongTermSubAttributeAdapter(ValidIfLongTermSub attribute, IStringLocalizer stringLocalizer)
                : base(attribute, stringLocalizer)
            {

            }

            public override void AddValidation(ClientModelValidationContext context)
            {
                if (context == null)
                {
                    throw new ArgumentNullException(nameof(context));
                }

                AttributeUtils.MergeAttribute(context.Attributes, "data-val", "true");
                AttributeUtils.MergeAttribute(context.Attributes, "data-val-longtermsub", GetErrorMessage(context));
            }

            public override string GetErrorMessage(ModelValidationContextBase validationContext)
            {
                if (validationContext == null)
                {
                    throw new ArgumentNullException(nameof(validationContext));
                }

                return GetErrorMessage(validationContext.ModelMetadata, validationContext.ModelMetadata.GetDisplayName());
            }



        }

        public class ValidIfLongTermSubAdapterProvider : IValidationAttributeAdapterProvider
        {

            private readonly IValidationAttributeAdapterProvider _baseProvider = new ValidationAttributeAdapterProvider();

            public IAttributeAdapter GetAttributeAdapter(ValidationAttribute attribute, IStringLocalizer stringLocalizer)
            {

                if (attribute is ValidIfLongTermSub)
                {
                    return new ValidIfLongTermSubAttributeAdapter(attribute as ValidIfLongTermSub, stringLocalizer);
                }
                else
                {
                    return _baseProvider.GetAttributeAdapter(attribute, stringLocalizer);
                }
            }
        }


    }
}
