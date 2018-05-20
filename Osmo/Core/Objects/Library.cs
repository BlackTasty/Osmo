using System.IO;

namespace Osmo.Core.Objects
{
    class Library
    {
        private string expectedPath;
        private string expectedName;
        private bool isRequired = true;
        private string customMessage;

        internal string Message { get { return customMessage; } }

        public Library(string expectedPath, string expectedName)
        {
            this.expectedName = expectedName;
            this.expectedPath = expectedPath;
        }

        public Library(string expectedPath, string expectedName, bool isRequired, string customMessage) : this(expectedPath, expectedName)
        {
            this.isRequired = isRequired;
            this.customMessage = customMessage;
        }

        internal void Exists(out bool exists, out bool isRequired, out string message)
        {
            exists = File.Exists(Path.Combine(expectedPath, expectedName));
            isRequired = this.isRequired;
            message = customMessage;
        }

        public override string ToString()
        {
            return expectedName;
        }
    }
}
