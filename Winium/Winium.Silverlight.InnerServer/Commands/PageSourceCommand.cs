namespace Winium.Silverlight.InnerServer.Commands
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Xml;
    using Mobile.Common;
    using FindByHelpers;

    internal class PageSourceCommand : CommandBase
    {
        #region Public Properties

        public string ElementId { get; set; }

        #endregion

        #region Public Methods and Operators

        public override string DoImpl()
        {
            string source;
            var settings = new XmlWriterSettings { Indent = true, Encoding = new UTF8Encoding(false) };

            using (var writer = new MemoryStream())
            {
                var xmlWriter = XmlWriter.Create(writer, settings);
                this.WriteElementToXml(xmlWriter, this.Automator.VisualRoot as FrameworkElement);
                xmlWriter.Flush();

                var buffer = writer.ToArray();
                source = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
            }

            return this.JsonResponse(ResponseStatus.Success, source);
        }

        #endregion

        #region Methods

        private void WriteElementToXml(XmlWriter writer, FrameworkElement item)
        {
            if (item == null)
            {
                return;
            }

            writer.WriteStartElement(item.GetType().ToString());
            var coordinates = item.GetCoordinatesInView(this.Automator.VisualRoot);
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
                                     { "x", rect.X.ToString(CultureInfo.InvariantCulture) }, 
                                     { "y", rect.Y.ToString(CultureInfo.InvariantCulture) }, 
                                     {
                                         "width", 
                                         rect.Width.ToString(CultureInfo.InvariantCulture)
                                     }, 
                                     {
                                         "height", 
                                         rect.Height.ToString(CultureInfo.InvariantCulture)
                                     }, 
                                     {
                                         "clickable_point",
                                         coordinates.ToString(CultureInfo.InvariantCulture)
                                     }
                                 };
            foreach (var attribute in attributes)
            {
                writer.WriteAttributeString(attribute.Key, attribute.Value);
            }

            var children = Finder.GetChildren(item);
            if (item == this.Automator.VisualRoot)
            {
                children = children.Concat(Finder.GetPopupsChildren());
            }

            foreach (var child in children)
            {
                this.WriteElementToXml(writer, child as FrameworkElement);
            }

            writer.WriteEndElement();
        }

        #endregion
    }
}
