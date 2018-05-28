using System;
using System.Windows;

namespace Osmo.Core.Reader
{
    class VersionSizeDescriptor : SkinningReader
    {
        private Size suggestedSDSize;
        private Size maxSize;
        private Size minSize;

        public Size SuggestedSDSize => suggestedSDSize;
        public Size MaximumSize => maxSize;
        public Size MinimumSize => minSize;

        public VersionSizeDescriptor(string sizeContent)
        {
            string[] content = sizeContent.Split(':');
            if (content.Length > 1)
            {
                SetVersion(content[0]);
                SetSize(content[1]);
            }
            else
            {
                SetSize(content[0]);
            }
        }

        private void SetSize(string sizeDefinition)
        {
            if (sizeDefinition.StartsWith("max", StringComparison.InvariantCultureIgnoreCase))
            {
                sizeDefinition = sizeDefinition.Replace("max", "");
                maxSize = StringToSize(sizeDefinition);
            }
            else if (sizeDefinition.StartsWith("min", StringComparison.InvariantCultureIgnoreCase))
            {
                sizeDefinition = sizeDefinition.Replace("min", "");
                minSize = StringToSize(sizeDefinition);
            }
            else
            {
                suggestedSDSize = StringToSize(sizeDefinition);
            }
        }

        private Size StringToSize(string rawSize)
        {
            string[] content = rawSize.Split('x');

            if (content.Length > 1)
            {
                return new Size(Parser.TryParse(content[0], 0), Parser.TryParse(content[1], 0));
            }
            else
            {
                return new Size(Parser.TryParse(content[0], 0), 0);
            }
        }
    }
}
