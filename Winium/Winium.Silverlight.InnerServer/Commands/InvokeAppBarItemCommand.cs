namespace Winium.Silverlight.InnerServer.Commands
{
    using System;
    using System.Windows;
    using Mobile.Common;
    using Mobile.Common.Exceptions;
    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Shell;
    using Public;

    internal enum BarItemType
    {
        IconButton,
        MenuItem
    }

    internal class InvokeAppBarItemCommand : CommandBase
    {
        public override string DoImpl()
        {
            var index = this.Parameters["index"].ToObject<int>();
            BarItemType itemType;

            if (!Enum.TryParse(this.Parameters["itemType"].ToString(), true, out itemType))
            {
                throw new AutomationException("Unknown BarItemType type", ResponseStatus.InvalidSelector);
            }

            if (itemType == BarItemType.IconButton)
            {
                var button = GetAppBarIconButton(index);
                button.RaiseClick();
            }
            else if (itemType == BarItemType.MenuItem)
            {
                var item = GetAppBarMenuItem(index);
                item.RaiseClick();
            }

            return this.JsonResponse(ResponseStatus.Success, true);
        }

        private static IApplicationBar GetAppBar()
        {
            var page = ((PhoneApplicationFrame)Application.Current.RootVisual).Content as PhoneApplicationPage;
            if (page != null && page.ApplicationBar != null)
            {
                return page.ApplicationBar;
            }

            throw new AutomationException("Could not find AppBar", ResponseStatus.NoSuchElement);
        }

        private static ClickableApplicationBarMenuItem GetAppBarMenuItem(int index)
        {
            var appBar = GetAppBar();
            var menuItem = appBar.MenuItems[index] as ClickableApplicationBarMenuItem;
            if (menuItem == null)
            {
                throw new AutomationException("Could not find ClickableApplicationBarMenuItem", ResponseStatus.NoSuchElement);
            }

            return menuItem;
        }

        private static ClickableApplicationBarIconButton GetAppBarIconButton(int index)
        {
            var appBar = GetAppBar();
            var button = appBar.Buttons[index] as ClickableApplicationBarIconButton;
            if (button == null)
            {
                throw new AutomationException("Could not find ClickableApplicationBarIconButton", ResponseStatus.NoSuchElement);
            }
            
            return button;
        }
    }
}
