using AwesomeSauceCompanyLtd.Services;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Threading.Tasks;

namespace AwesomeSauceCompanyLtd.Infrastructure
{
    public class UserModelBinder : IModelBinder
    {
        private readonly IUsers _users;

        public UserModelBinder(IUsers users)
        {
            _users = users;
        }

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var key = bindingContext.ModelMetadata.Name + "Id";

            var value = bindingContext.ValueProvider.GetValue(key);
            if (value == ValueProviderResult.None)
            {
                return;
            }

            var userIdValue = value.FirstValue;
            if (string.IsNullOrEmpty(userIdValue))
            {
                return;
            }

            if (int.TryParse(userIdValue, out var userId))
            {
                var user = await _users.WhereIdIs(userId);
                bindingContext.Result = ModelBindingResult.Success(user);
                return;
            }

            bindingContext.ModelState.TryAddModelError(key, "User Id must be a number");
        }
    }
}
