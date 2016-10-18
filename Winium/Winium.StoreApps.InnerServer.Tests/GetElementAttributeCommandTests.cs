namespace Winium.StoreApps.InnerServer.Tests
{
    #region

    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

    using Windows.UI.Xaml.Automation;
    using Windows.UI.Xaml.Controls;

    using Mobile.Common.CommandSettings;
    using Winium.StoreApps.InnerServer.Commands;
    using Winium.StoreApps.InnerServer.Element;

    #endregion

    [TestClass]
    public class GetElementAttributeCommandTests
    {
        #region Public Methods and Operators

        [DataTestMethod]
        [DataRow(ElementAttributeAccessModifier.AutomationProperties, "TestId", null, null,
            DisplayName = "Automation Properties Only")]
        [DataRow(ElementAttributeAccessModifier.DependencyProperties, "TestId", "Test Text", null,
            DisplayName = "Automation and Dependency Properties Only")]
        [DataRow(ElementAttributeAccessModifier.ClrProperties, "TestId", "Test Text", "Test Value",
            DisplayName = "Automation, Dependency and CLR Properties")]
        public void ShouldLimitPropertiesAccess(
            ElementAttributeAccessModifier scope,
            string expectedId,
            string expectedText,
            string expectedClr)
        {
            UiDispatch.Invoke(
                () =>
                    {
                        var element = CreateTestElement();
                        var winiumElement = new WiniumElement(element);

                        var auotmationId = GetElementAttributeCommand.GetPropertyCascade(
                            winiumElement,
                            "AutomationProperties.AutomationId",
                            scope);

                        var text = GetElementAttributeCommand.GetPropertyCascade(winiumElement, "Text", scope);

                        var clr = GetElementAttributeCommand.GetPropertyCascade(winiumElement, "ClrProperty", scope);

                        Assert.AreEqual(expectedId, auotmationId);
                        Assert.AreEqual(expectedText, text);
                        Assert.AreEqual(expectedClr, clr);
                    }).Wait();
        }

        #endregion

        #region Methods

        private static TextBox CreateTestElement()
        {
            var element = new TestControl();
            element.SetValue(AutomationProperties.AutomationIdProperty, "TestId");
            element.SetValue(TextBox.TextProperty, "Test Text");
            element.ClrProperty = "Test Value";

            return element;
        }

        #endregion

        private sealed class TestControl : TextBox
        {
            #region Public Properties

            public string ClrProperty { get; set; }

            #endregion
        }
    }
}
