using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace EverlandApi.Core.Models
{
    public class ApiValidationError : ApiError
    {
        public class Field
        {
            [Required]
            public string Name { get; private set; }

            [Required]
            public IEnumerable<string> Reasons { get; private set; }

            public Field(string name, ModelStateEntry entry)
            {
                Name = name;
                Reasons = entry.Errors.Select(e => e.ErrorMessage);
            }
        }

        public IEnumerable<Field> InvalidFields { get; private set; }

        public ApiValidationError(ModelStateDictionary modelState)
            : base(
                "An error occurred while validating the request.",
                ApiErrorCode.InvalidRequest
            )
        {
            InvalidFields = modelState.Keys
                .Select(key => new Field(key, modelState[key]));
        }
    }
}
