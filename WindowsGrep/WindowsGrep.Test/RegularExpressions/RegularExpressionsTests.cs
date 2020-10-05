using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WindowsGrep.Common;
using WindowsGrep.Engine;

namespace WindowsGrep.Test.RegularExpressions
{
    public class RegularExpressionsTests : TestBase
    {
        #region Member Variables..
        private string _TestDataRelativePath = @"RegularExpressions\TestData";
        #endregion Member Variables..

        #region Properties..
        #endregion Properties..

        #region Setup
        [SetUp]
        public void Setup()
        {
            TestDataDirectory = Path.Combine(TestConfigurationManager.ProjectDirectory, _TestDataRelativePath);
        }
        #endregion Setup

        #region Tests..
        #region RegularExpressions_One
        [Test]
        public void RegularExpressions_One()
        {
       
        }
        #endregion RegularExpressions_One 
        #endregion Tests..
    }
}
