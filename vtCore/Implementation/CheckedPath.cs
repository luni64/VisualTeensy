using System;
using System.IO;

namespace vtCore
{
    public class CheckedPath : IEquatable<CheckedPath>
    {
        public CheckedPath(string checkedFile, bool optional = true)
        {
            this.checkedFile = checkedFile;
            this.optional = optional;
            this._path = path;
            error = null;
        }
        public string path
        {
            get => _path;
            set
            {
                if (value != _path)
                {
                    _path = value;
                    error = check();
                }
            }
        }
        public string shortPath => Helpers.getShortPath(path);
        public bool isOk => error == null;
        public string error { get; private set; }

        string _path;
        readonly string checkedFile;
        readonly bool optional;

        string check()
        {
            if (String.IsNullOrWhiteSpace(path))
            {
                return optional ? null : "Folder required";
            }


            if (Directory.Exists(path))
            {
                if (checkedFile == null || File.Exists(Path.Combine(path, checkedFile)))
                {
                    return null;
                }
                return $"{checkedFile} not found in the specified folder";
            }
            return "Folder doesn't exist";
        }

        public bool Equals(CheckedPath other)
        {
            return (path == other.path) && (checkedFile == other.checkedFile);
        }
    }
}


