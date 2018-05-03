using System;
using System.Diagnostics;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Osmo.Core;
using Osmo.Core.Objects;

namespace Osmo.Tests
{
    [TestClass]
    public class SkinManagerTests
    {
        private static string testDirectory = AppDomain.CurrentDomain.BaseDirectory + "\\Osmo Test Files\\";
        private static string testFilePath = testDirectory + "\\Test file.txt";
        private static string testFileContent = "Hello dear reader!";

        private int changedTicks = 0;


        [TestMethod]
        public void EventTest()
        {
            SkinManager manager = new SkinManager(testDirectory);
            manager.SkinChanged += new EventHandler<SkinChangedEventArgs>((sender, e) =>
            {
                switch (changedTicks)
                {
                    case 0: //Create
                        Assert.Equals(e.Path, testFilePath);
                        break;
                    case 1: //Changed
                        Assert.Equals(testFileContent, File.ReadAllText(testFilePath));
                        break;
                    case 3: //Remove
                        Assert.IsFalse(File.Exists(testFilePath + "_RENAME"));
                        break;
                }
                changedTicks++;
            });
            manager.SkinRenamed += new EventHandler<SkinRenamedEventArgs>((sender, e) =>
            {
                if (changedTicks == 2)
                    Assert.Equals(e.Path, testFilePath + "_RENAME");
                changedTicks++;
            });
            
            File.WriteAllText(testFilePath, "");
            File.WriteAllText(testFilePath, testFileContent);
            File.Move(testFilePath, testFilePath + "_RENAME");
            File.Delete(testFilePath + "_RENAME");
        }
    }
}
