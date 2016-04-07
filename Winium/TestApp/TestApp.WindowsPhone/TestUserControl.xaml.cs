using Windows.UI.Xaml.Controls;

namespace TestApp
{
    public sealed partial class TestUserControl : UserControl
    {
        public TestUserControl()
        {
            this.InitializeComponent();
            this.ClrProperty = "Test";
        }

        public string ClrProperty { get; set; }
    }
}
