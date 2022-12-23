using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using System.ComponentModel.DataAnnotations;

namespace Quarter.Attributes.ValidationAttributes
{
    public class MaxFileSizeAttribute:ValidationAttribute
    {
        private readonly int _maxFileSize;

        public MaxFileSizeAttribute(int maxFileSize)
        {
            this._maxFileSize = maxFileSize;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            List<IFormFile> files = new List<IFormFile>();

            if(value is List<IFormFile>)
            {
                files = value as List<IFormFile>;
            }
            else if(value is IFormFile)
            {
                files.Add(value as IFormFile);
            }
            foreach (var file in files)
            {
                if(file.Length > (_maxFileSize * 1024 * 1024))
                {
                    return new ValidationResult("File size must be smaller than" + _maxFileSize + "MB");
                }
            }


            return ValidationResult.Success;
        }
    }
}
