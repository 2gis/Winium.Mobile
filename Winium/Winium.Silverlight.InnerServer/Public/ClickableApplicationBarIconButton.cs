namespace Winium.Silverlight.InnerServer.Public
{
    using System;

    using Microsoft.Phone.Shell;

    public class ClickableApplicationBarIconButton : ApplicationBarIconButton
    {
        public ClickableApplicationBarIconButton()
            : base()
        {
        }

        public ClickableApplicationBarIconButton(Uri iconUri)
            : base(iconUri)
        {
        }

        public new event EventHandler Click;

        public void RaiseClick()
        {
            if (this.Click != null)
            {
                this.Click(this, EventArgs.Empty);
            }
        }
    }
}