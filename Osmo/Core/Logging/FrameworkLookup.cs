using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Osmo.Core.Logging
{
    class FrameworkLookup
    {
        private const string NET_SUBKEY = @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\";

        public static string GetFrameworks
        {
            get
            {
                string frameworks = "Installed .NET Framework versions: ";
                foreach (string str in GetFrameworksOld)
                {
                    frameworks += "\n\t" + str;
                }

                frameworks += "\n\t" + GetFrameworks4_5;
                return frameworks + "\n\n";
            }
        }

        static List<string> GetFrameworksOld
        {
            get
            {
                List<string> frameworks = new List<string>();
                using (RegistryKey ndpKey = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, "")
                                                .OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\"))
                {
                    foreach (string versionKeyName in ndpKey.GetSubKeyNames())
                    {
                        if (versionKeyName.StartsWith("v"))
                        {
                            RegistryKey versionKey = ndpKey.OpenSubKey(versionKeyName);
                            string name = (string)versionKey.GetValue("Version", "");
                            string sp = versionKey.GetValue("SP", "").ToString();
                            string install = versionKey.GetValue("Install", "").ToString();
                            if (install == string.Empty) //no install info, must be later.
                                frameworks.Add(".NET Framework Version: " + versionKeyName + "  " + name);
                            else
                            {
                                if (sp != "" && install == "1")
                                {
                                    frameworks.Add(".NET Framework Version: " + versionKeyName + "  " + name + "  SP" + sp);
                                }

                            }
                            if (name != "")
                            {
                                continue;
                            }
                            foreach (string subKeyName in versionKey.GetSubKeyNames())
                            {
                                RegistryKey subKey = versionKey.OpenSubKey(subKeyName);
                                name = (string)subKey.GetValue("Version", "");
                                if (name != "")
                                    sp = subKey.GetValue("SP", "").ToString();
                                install = subKey.GetValue("Install", "").ToString();
                                if (install == string.Empty) //no install info, must be later.
                                    frameworks.Add(versionKeyName + "  " + name);
                                else
                                {
                                    if (sp != "" && install == "1")
                                    {
                                        frameworks.Add("  " + subKeyName + "  " + name + "  SP" + sp);
                                    }
                                    else if (install == "1")
                                    {
                                        frameworks.Add("  " + subKeyName + "  " + name);
                                    }
                                }
                            }
                        }
                    }
                }
                return frameworks;
            }
        }

        static string GetFrameworks4_5
        {
            get
            {
                List<string> frameworks = new List<string>();

                using (RegistryKey ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(NET_SUBKEY))
                {
                    return ndpKey != null && ndpKey.GetValue("Release") != null ?
                        ".NET Framework Version: " + CheckFor45PlusVersion((int)ndpKey.GetValue("Release")) :
                        ".NET Framework Version 4.5 or later not installed!";
                }
            }
        }

        static string CheckFor45PlusVersion(int releaseKey)
        {
            if (releaseKey >= 394802)
                return "4.6.2 or later";
            if (releaseKey >= 394254)
            {
                return "4.6.1";
            }
            if (releaseKey >= 393295)
            {
                return "4.6";
            }
            if ((releaseKey >= 379893))
            {
                return "4.5.2";
            }
            if ((releaseKey >= 378675))
            {
                return "4.5.1";
            }
            if ((releaseKey >= 378389))
            {
                return "4.5";
            }
            // This code should never execute. A non-null release key should mean
            // that 4.5 or later is installed.
            return "4.5 or later version not installed!";
        }
    }
}
