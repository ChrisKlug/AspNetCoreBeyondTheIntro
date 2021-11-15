using AwesomeSauceCompanyLtd.Services.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using System;

namespace AwesomeSauceCompanyLtd.Infrastructure
{
    public class UserModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            _ = context ?? throw new ArgumentNullException(nameof(context));

            if (context.Metadata.ModelType == typeof(User))
            {
                // BinderTypeModelBinder acts as a factory providing DI
                return new BinderTypeModelBinder(typeof(UserModelBinder));
            }

            return null;
        }
    }
}