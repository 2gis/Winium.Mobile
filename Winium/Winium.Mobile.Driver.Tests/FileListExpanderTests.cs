namespace Winium.Mobile.Driver.Tests
{
    #region

    using System.Collections.Generic;
    using System.IO.Abstractions.TestingHelpers;
    using System.Linq;

    using NUnit.Framework;

    using Winium.Mobile.Driver.CommandHelpers;

    #endregion

    [TestFixture]
    public class FileExpanderTests
    {
        #region Fields

        private FilesCapabilityExpander expander;

        #endregion

        #region Public Methods and Operators

        [SetUp]
        public void BeforeTest()
        {
            var fileSystem =
                new MockFileSystem(
                    new Dictionary<string, MockFileData>
                        {
                            { @"c:\root.txt", new MockFileData(string.Empty) }, 
                            { @"c:\demo\nested1.txt", new MockFileData(string.Empty) }, 
                            { @"c:\demo\nested2.txt", new MockFileData(string.Empty) },
                            { @"c:\demo\subfolder\file.txt", new MockFileData(string.Empty) },
                        });

            this.expander = new FilesCapabilityExpander(fileSystem);
        }

        [Test]
        public void TestFileExpanderFileToDirectory()
        {
            var d = new Dictionary<string, string> { { @"c:\root.txt", "downloads/" } };
            var rv = this.expander.ExpandFiles(d);
            Assert.AreEqual("downloads\\root.txt", rv.First().Value);
        }

        [Test]
        public void TestFileExpanderFileToFile()
        {
            var d = new Dictionary<string, string> { { @"c:\root.txt", "downloads/new.txt" } };
            var rv = this.expander.ExpandFiles(d);
            Assert.AreEqual("downloads\\new.txt", rv.First().Value);
        }

        [Test]
        public void TestFileExpanderDirectoryToDirectory()
        {
            var d = new Dictionary<string, string> { { @"c:\demo/", "downloads" } };
            var rv = this.expander.ExpandFiles(d);
            var expected = new List<string>
                               {
                                   @"downloads\nested1.txt",
                                   @"downloads\nested2.txt",
                                   @"downloads\subfolder\file.txt",
                               };
            CollectionAssert.AreEqual(expected, rv.Select(x => x.Value));
        }

        [Test]
        public void TestFileExpanderDirectoryToRootDirectory()
        {
            var d = new Dictionary<string, string> { { @"c:\demo/", string.Empty } };
            var rv = this.expander.ExpandFiles(d);
            var expected = new List<string>
                               {
                                   @"nested1.txt",
                                   @"nested2.txt",
                                   @"subfolder\file.txt",
                               };
            CollectionAssert.AreEqual(expected, rv.Select(x => x.Value));
        }

        #endregion
    }
}
