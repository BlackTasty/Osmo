using Microsoft.VisualStudio.TestTools.UnitTesting;
using Osmo.UI;
using Osmo.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Osmo.Tests
{
    [TestClass]
    public class SliderballCreatorTests
    {
        private static string testImagePath = @"D:\Program Files (x86)\osu!\Skins\Its Toothless! - HTTYD Skin\sliderb0.png";
        //private static SliderballCreator window = SliderballCreator.Instance;
        private static SBCreatorViewModel vm = new SBCreatorViewModel();

        [TestMethod]
        public void AnimationTest()
        {
            vm.SourceImagePath = testImagePath;
            vm.StartAnimation();
            Thread.Sleep(5000);
            vm.StopAnimation();
        }

        [TestMethod]
        public void AnimationWithWindowTest()
        {

        }
    }
}
