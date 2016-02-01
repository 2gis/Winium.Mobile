namespace Winium.StoreApps.InnerServer.Tests
{
    #region

    using System.Text;
    using System.Xml;

    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

    using Windows.Data.Xml.Dom;
    using Windows.UI.Xaml.Controls;

    using Winium.StoreApps.InnerServer.Commands;
    using Winium.StoreApps.InnerServer.Element;

    #endregion

    [TestClass]
    public class PageSourceCommandTests
    {
        #region Public Methods and Operators

        [TestMethod]
        public void ShouldEncodeTagName()
        {
            UiDispatch.Invoke(
                () =>
                    {
                        var winiumElement = new WiniumElement(new TestControl());

                        var sb = new StringBuilder();
                        using (var writer = XmlWriter.Create(sb))
                        {
                            PageSourceCommand.WriteElementToXml(writer, winiumElement);
                        }

                        var doc = new XmlDocument();
                        doc.LoadXml(sb.ToString());
                        var element = doc.DocumentElement;

                        Assert.AreEqual(XmlConvert.EncodeLocalName(winiumElement.ClassName), element.LocalName);
                        Assert.AreEqual(winiumElement.ClassName, element.GetAttribute("class_name"));
                    }).Wait();
        }

        #endregion

        public class TestControl : TextBox
        {
        }
    }
}
