namespace Osmo.Core.Configuration
{
    partial class ConfigurationFile
    {
        /// <summary>
        /// Returns the name and the value of a property
        /// </summary>
        /// <param name="value">Raw property</param>
        /// <returns>Array order is: [0] = Property name, [1] = Value</returns>
        protected string[] GetPropertyPair(string value)
        {
            int splitIndex = value.IndexOf(':');

            if (splitIndex > -1)
            {
                string[] output = new string[]
                {
                    value.Remove(splitIndex),
                    value.Remove(0, splitIndex + 1)
                };

                return output;
            }
            else
                return null;
        }
    }
}
