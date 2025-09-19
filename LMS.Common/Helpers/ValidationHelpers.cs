using FluentValidation;

namespace LMS.Common.Helpers
{
    public static class ValidationHelpers
    {
        public static async Task<List<object>> ValidateAsync<T>(T model, params Type[] validatorTypes)
        {
            var errors = new List<object>();

            foreach (var validatorType in validatorTypes)
            {
                if (Activator.CreateInstance(validatorType) is IValidator<T> validator)
                {
                    var result = await validator.ValidateAsync(model);
                    if (!result.IsValid)
                    {
                        errors.AddRange(result.Errors.Select(e => new
                        {
                            Property = e.PropertyName,
                            Error = e.ErrorMessage
                        }));
                    }
                }
            }

            return errors;
        }
    }

}
