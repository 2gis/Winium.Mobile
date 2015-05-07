namespace Winium.StoreApps.InnerServer.Commands
{
    #region

    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;

    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Media;

    using Winium.StoreApps.Common;
    using Winium.StoreApps.InnerServer.Commands.Helpers;

    #endregion

    internal class PageSourceCommand : CommandBase
    {
        #region Public Properties

        public string ElementId { get; set; }

        #endregion

        #region Public Methods and Operators

        public override string DoImpl()
        {
            string source;
            var settings = new XmlWriterSettings { Indent = true, Encoding = new UTF8Encoding(false)};

            using (var writer = new MemoryStream())
            {
                var xmlWriter = XmlWriter.Create(writer, settings);
                this.WriteElementsToXml(xmlWriter);
                xmlWriter.Flush();

                var buffer = writer.ToArray();
                source = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
            }

            return this.JsonResponse(ResponseStatus.Success, source);
        }

        #endregion

        private void WriteElementsToXml(XmlWriter writer)
        {
            writer.WriteStartElement("root");
            this.WriteElementToXml(writer, this.Automator.VisualRoot as FrameworkElement);
            var popups = VisualTreeHelper.GetOpenPopups(Window.Current);
            foreach (var popupChild in popups.Select(popup => popup.Child))
            {
                this.WriteElementToXml(writer, popupChild as FrameworkElement);
            }

            writer.WriteEndElement();
        }

        #region Methods

        private void WriteElementToXml(XmlWriter writer, FrameworkElement item)
        {
            if (item == null)
            {
                return;
            }

            writer.WriteStartElement(item.GetType().ToString());
            var rect = item.GetRect(this.Automator.VisualRoot);
            var attributes = new Dictionary<string, string>
                                 {
                                     { "name", item.AutomationName() }, 
                                     { "id", item.AutomationId() }, 
                                     { "xname", item.Name }, 
                                     {
                                         "visible", 
                                         item.IsUserVisible(this.Automator.VisualRoot)
                                         .ToString()
                                         .ToLowerInvariant()
                                     }, 
                                     { "value", item.GetText() }, 
                                     {
                                         "x", 
                                         rect.X.ToString(CultureInfo.InvariantCulture)
                                     }, 
                                     {
                                         "y", 
                                         rect.Y.ToString(CultureInfo.InvariantCulture)
                                     },
                                     {
                                         "width", 
                                         rect.Width.ToString(CultureInfo.InvariantCulture)
                                     }, 
                                     {
                                         "height", 
                                         rect.Height.ToString(CultureInfo.InvariantCulture)
                                     }
                                 };
            foreach (var attribute in attributes)
            {
                writer.WriteAttributeString(attribute.Key, attribute.Value);
            }

            var childrenCount = VisualTreeHelper.GetChildrenCount(item);
            for (var i = 0; i < childrenCount; ++i)
            {
                var child = VisualTreeHelper.GetChild(item, i);
                this.WriteElementToXml(writer, child as FrameworkElement);
            }

            writer.WriteEndElement();
        }

        #endregion
    }
}
