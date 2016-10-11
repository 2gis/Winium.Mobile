namespace Winium.Silverlight.InnerServer.Public
{
    using System;
    using Microsoft.Phone.Shell;

    public class ClickableApplicationBarMenuItem : ApplicationBarMenuItem
    {
        public ClickableApplicationBarMenuItem()
            : base()
        {
        }

        public ClickableApplicationBarMenuItem(string text)
            : base(text)
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
