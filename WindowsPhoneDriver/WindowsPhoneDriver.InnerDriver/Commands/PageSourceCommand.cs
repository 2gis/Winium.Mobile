namespace WindowsPhoneDriver.InnerDriver.Commands
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Windows;
    using System.Windows.Media;
    using System.Xml;

    using WindowsPhoneDriver.Common;

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
            var coordinates = item.GetCoordinates(this.Automator.VisualRoot);
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
                                         coordinates.X.ToString(CultureInfo.InvariantCulture)
                                     }, 
                                     {
                                         "y", 
                                         coordinates.Y.ToString(CultureInfo.InvariantCulture)
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
