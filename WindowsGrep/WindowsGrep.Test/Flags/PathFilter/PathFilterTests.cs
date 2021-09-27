using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WindowsGrep.Common;
using WindowsGrep.Test;

public class PathFilterTests : TestBase
{
    #region Member Variables..
    private string _FlagDescriptorShort;
    private string _FlagDescriptorLong;
    private string _TestDataRelativePath = @"Flags\PathFilter\TestData";
    #endregion Member Variables..

    #region Properties..
    #endregion Properties..

    #region Setup
    [SetUp]
    public void Setup()
    {
        TestDataDirectory = Path.Combine(TestConfigurationManager.ProjectDirectory, _TestDataRelativePath);

        List<string> DescriptionCollection = ConsoleFlag.FileTypeFilter.GetCustomAttribute<DescriptionCollectionAttribute>()?.Value.OrderBy(x => x.Length).ToList();
        _FlagDescriptorShort = DescriptionCollection[0];
        _FlagDescriptorLong = DescriptionCollection[1];
    }
    #endregion Setup

    #region Tests..
    #endregion Tests..
}