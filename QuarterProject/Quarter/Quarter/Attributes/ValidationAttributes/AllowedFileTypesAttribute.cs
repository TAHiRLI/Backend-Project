using System.ComponentModel.DataAnnotations;

namespace Quarter.Attributes.ValidationAttributes
{
    public class AllowedFileTypesAttribute:ValidationAttribute
    {
        string[] _fileTypes;

        public AllowedFileTypesAttribute(params string[] fileTypes)
        {
            _fileTypes = fileTypes;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {

            List<IFormFile> files = new List<IFormFile>();

            if (value is List<IFormFile>)
            {
                files = value as List<IFormFile>;
            }
            else if (value is IFormFile)
            {
                files.Add(value as IFormFile);
            }
            foreach (var file in files)
            {
                if (!_fileTypes.Contains(file.ContentType))
                {
                    return new ValidationResult("File type must be" + string.Join(", ", _fileTypes));
                }
            }

            return ValidationResult.Success;
        }


    }
}
