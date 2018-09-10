using System;

namespace Board2Make.Model
{
    public class ValidatedPath
    {
        public string path { get; set; }
        public string ValidationResult => validate(path);
        public bool isValid => ValidationResult == null;
        public bool hasError => !isValid;

        public ValidatedPath(Func<string, string> validator)
        {
            this.validate = validator;
        }

        private Func<string, string> validate;
    }
}
