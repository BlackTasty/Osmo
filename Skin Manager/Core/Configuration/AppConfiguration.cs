using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Osmo.Core.Configuration
{
    class AppConfiguration : ConfigurationFile
    {
        #region Singleton implementation
        private static AppConfiguration instance;
        public static AppConfiguration GetInstance()
        {
            if (instance == null)
                instance = new AppConfiguration();

            return instance;
        }
        #endregion

        public string OsuDirectory { get; set; }

        private AppConfiguration() : base("settings")
        {

        }

        public void Save()
        {
            #region Properties
            Content = new string[]
            {
                "OsuDirectory:" + OsuDirectory
            };
            #endregion
        }
    }
}
