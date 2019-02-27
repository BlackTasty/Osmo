using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Osmo.Core.Configuration;

namespace Osmo.Tests
{
    [TestClass]
    public class ProfileManagerTests
    {
        private static ProfileManager profileManager;
        private static string testProfileName = "UnitTestProfile";

        public ProfileManagerTests()
        {
            AppConfiguration configuration = new AppConfiguration()
            {
                OsuDirectory = @"\."
            };
            configuration.Save();
            profileManager = new ProfileManager(configuration);
        }

        [TestMethod]
        public void Test_All()
        {
            Test_ProfileManager();
            Test_ProfileLifecycle();
        }

        [TestMethod]
        public void Test_ProfileManager()
        {
            ReadConfigurations();
            SwapConfiguration();
            SwapInvalidConfiguration();
        }

        #region Profile Manager
        [TestMethod]
        public void ReadConfigurations()
        {

            Console.WriteLine("===== TEST: READ CONFIGURATION =====");
            PrintProfileManagerDetails(profileManager);

            Console.WriteLine("Assert: Is current configuration valid?");
            Assert.IsTrue(profileManager.Profile.IsValid);
            Console.WriteLine("Yes!");

            Console.WriteLine("Assert: Is current configuration the default one?");
            Assert.IsTrue(profileManager.IsDefault);
            Console.WriteLine("Yes!");

            foreach (var config in profileManager.Profiles)
            {
                Console.WriteLine("Assert: Is configuration {0} valid?", config.ProfileName);
                Assert.IsTrue(config.IsValid);
                Console.WriteLine("Yes!");
            }
        }

        [TestMethod]
        public void SwapConfiguration()
        {
            Console.WriteLine("===== TEST: SWAP CONFIGURATION =====");
            Console.WriteLine("--- Current loaded configuration ---");
            PrintConfiguration(profileManager.Profile);

            Console.WriteLine("Assert: Is current configuration \"Harry\"?");
            Assert.IsTrue(profileManager.ChangeActiveProfile("Harry"));

            Console.WriteLine("--- Current loaded configuration ---");
            PrintConfiguration(profileManager.Profile);

            Assert.AreEqual(profileManager.Profile.ProfileName, "Harry");
            Console.WriteLine("Correct configuration loaded. Success!");

            Assert.IsTrue(profileManager.ChangeActiveProfile(""));

            Console.WriteLine("--- Current loaded configuration ---");
            PrintConfiguration(profileManager.Profile);
            Assert.IsTrue(profileManager.IsDefault);
            Console.WriteLine("Default configuration loaded. Success!");
        }

        [TestMethod]
        public void SwapInvalidConfiguration()
        {
            Console.WriteLine("===== TEST: SWAP INVALID CONFIGURATION =====");
            AppConfiguration current = profileManager.Profile;

            Console.WriteLine("--- Current loaded configuration ---");
            PrintConfiguration(profileManager.Profile);

            Assert.IsFalse(profileManager.ChangeActiveProfile("Invalid input which should not exist"));

            Console.WriteLine("--- Current loaded configuration ---");
            PrintConfiguration(profileManager.Profile);

            Assert.AreEqual(current, profileManager.Profile);
            Console.WriteLine("Configuration is unchanged. Success!");
        }
        #endregion

        [TestMethod]
        public void Test_ProfileLifecycle()
        {
            AddProfile();
            EditProfile();
            RemoveProfile();
        }

        #region Profile Lifecycle Tests
        [TestMethod]
        public void AddProfile()
        {
            Console.WriteLine("===== TEST: ADD PROFILE =====");

            AppConfiguration newProfile = new AppConfiguration(testProfileName)
            {
                OsuDirectory = "a:\\TEST",
                Language = Core.Objects.Language.German,
                DisclaimerRead = true,
            };

            newProfile.Save();
            
            Console.WriteLine("--- Configuration details ---");
            PrintConfiguration(newProfile);

            int profiles = profileManager.Profiles.Count;
            profileManager.AddProfile(newProfile);

            Assert.AreEqual(profiles + 1, profileManager.Profiles.Count);
        }

        [TestMethod]
        public void EditProfile()
        {
            Console.WriteLine("===== TEST: EDIT PROFILE =====");
            AppConfiguration testConfig = profileManager.GetProfileByName(testProfileName);

            Console.WriteLine("--- Unchanged test configuration ---");
            PrintConfiguration(testConfig);

            testConfig = profileManager.GetProfileByName(testProfileName);
            Assert.IsNotNull(testConfig);
            string newPath = "t:\\his is\\a pretty\\and pathy.test";

            testConfig.Language = Core.Objects.Language.Default;
            testConfig.OsuDirectory = newPath;
            testConfig.Save();

            testConfig = profileManager.GetProfileByName(testProfileName);

            Assert.AreEqual(newPath, testConfig.OsuDirectory);
            Assert.AreEqual(Core.Objects.Language.Default, testConfig.Language);
        }

        [TestMethod]
        public void RenameProfile()
        {
            Console.WriteLine("===== TEST: RENAME PROFILE =====");
            AppConfiguration testConfig = profileManager.GetProfileByName(testProfileName);

            Console.WriteLine("--- Unchanged test configuration ---");
            PrintConfiguration(testConfig);

            string newProfileName = "RenamedTestProfile";
            testConfig.ProfileName = newProfileName;

            testConfig = profileManager.GetProfileByName(newProfileName);
            Assert.AreEqual(newProfileName, testConfig.ProfileName);

            Console.WriteLine("--- Renamed test configuration ---");
            PrintConfiguration(testConfig);
        }

        [TestMethod]
        public void RemoveProfile()
        {
            Console.WriteLine("===== TEST: REMOVE PROFILE =====");
            Assert.IsTrue(profileManager.RemoveProfile(testProfileName));
        }
        #endregion

        #region Helper methods
        private void PrintProfileManagerDetails(ProfileManager profileManager)
        {
            Console.WriteLine("--- Profile Manager data ---");
            Console.WriteLine("\nDefault configuration:");
            PrintConfiguration(profileManager.Profile);

            Console.WriteLine("\n\n--- Profile Manager profiles ---");
            foreach (var config in profileManager.Profiles)
            {
                Console.WriteLine("\nProfile: {0}", config.ProfileName);
                PrintConfiguration(config);
            }
        }

        private void PrintConfiguration(AppConfiguration configuration)
        {
            Console.WriteLine("\tFilePath: {0}", configuration.FilePath);
            Console.WriteLine("\tIsValid: {0}", configuration.IsValid);
            Console.WriteLine("\tBackupBeforeMixing: {0}", configuration.BackupBeforeMixing);
            Console.WriteLine("\tBackupDirectory: {0}", configuration.BackupDirectory);
            Console.WriteLine("\tDisclaimerRead: {0}", configuration.DisclaimerRead);
            Console.WriteLine("\tLanguage: {0}", configuration.Language);
            Console.WriteLine("\tOsuDirectory: {0}", configuration.OsuDirectory);
            if (!profileManager.IsDefault)
            {
                Console.WriteLine("\tProfileName: {0}", configuration.ProfileName);
            }
            else
            {
                Console.WriteLine("\tProfilePath: {0}", configuration.ProfilePath);
            }
            Console.WriteLine("\tReopenLastSkin: {0}", configuration.ReopenLastSkin);
            Console.WriteLine("\tTemplateDirectory: {0}", configuration.TemplateDirectory);
            Console.WriteLine("\tUnsavedChanges: {0}", configuration.UnsavedChanges);
        }
        #endregion
    }
}
