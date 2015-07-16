namespace Winium.StoreApps.InnerServer.Element
{
    #region

    using System;
    using System.Collections.Generic;

    using Windows.UI.Xaml;

    #endregion

    public enum TreeScope
    {
        Children = 2, 

        Descendants = 4, 
    }

    internal interface ISearchable
    {
        #region Public Methods and Operators

        IEnumerable<WiniumElement> Find(TreeScope scope, Predicate<FrameworkElement> predicate);

        #endregion
    }
}
