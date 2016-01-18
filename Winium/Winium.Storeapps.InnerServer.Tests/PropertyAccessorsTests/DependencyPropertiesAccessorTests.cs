namespace Winium.StoreApps.InnerServer.Tests.PropertyAccessorsTests
{
    #region

    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    using Winium.StoreApps.InnerServer.Element.Helpers;

    #endregion

    [TestClass]
    public class DependencyPropertiesAccessorTests
    {
        #region Public Methods and Operators

        [TestMethod]
        public void ShouldReturnDependencyPropertyDefinedAsField()
        {
            UiDispatch.Invoke(
                () =>
                    {
                        var obj = new TestControl { Some = "Test Value" };

                        object value;
                        DependencyPropertiesAccessor.TryGetDependencyProperty(obj, "Some", out value);

                        Assert.AreEqual(obj.GetValue(TestControl.SomeProperty), value);
                    }).Wait();
        }

        [TestMethod]
        public void ShouldReturnDependencyPropertyDefinedAsProperty()
        {
            UiDispatch.Invoke(
                () =>
                    {
                        var obj = new TextBox { IsReadOnly = true };

                        object value;
                        DependencyPropertiesAccessor.TryGetDependencyProperty(obj, "IsReadOnly", out value);

                        Assert.AreEqual(obj.GetValue(TextBox.IsReadOnlyProperty), value);
                    }).Wait();
        }

        [TestMethod]
        public void ShouldReturnInheritedDependencyProperty()
        {
            UiDispatch.Invoke(
                () =>
                    {
                        var obj = new TestControl { Content = "Test Value" };

                        object value;
                        DependencyPropertiesAccessor.TryGetDependencyProperty(obj, "Content", out value);

                        Assert.AreEqual(obj.GetValue(ContentControl.ContentProperty), value);
                    }).Wait();
        }

        #endregion

        private sealed class TestControl : Button
        {
            #region Static Fields

            public static readonly DependencyProperty SomeProperty = DependencyProperty.Register(
                "Some",
                typeof(string),
                typeof(TestControl),
                new PropertyMetadata(null));

            #endregion

            #region Public Properties

            public string Some
            {
                set
                {
                    this.SetValue(SomeProperty, value);
                }
            }

            #endregion
        }
    }
}
