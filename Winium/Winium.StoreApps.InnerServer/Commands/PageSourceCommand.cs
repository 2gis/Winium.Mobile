namespace Winium.StoreApps.InnerServer.Commands
{
    #region

    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Xml;

    using Winium.StoreApps.Common;
    using Winium.StoreApps.InnerServer.Element;

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
            var settings = new XmlWriterSettings { Indent = true, Encoding = new UTF8Encoding(false) };

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

        #region Methods

        private void WriteElementToXml(XmlWriter writer, WiniumElement element)
        {
            if (element == null)
            {
                return;
            }

            writer.WriteStartElement(element.ClassName);
            var rect = element.GetRect(this.Automator.VisualRoot);
            var attributes = new Dictionary<string, string>
                                 {
                                     { "name", element.AutomationName }, 
                                     { "id", element.AutomationId }, 
                                     { "xname", element.XName }, 
                                     {
                                         "visible", 
                                         element.IsUserVisible(this.Automator.VisualRoot)
                                         .ToString()
                                         .ToLowerInvariant()
                                     }, 
                                     { "value", element.GetText() }, 
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
                                         element.GetCoordinatesInView(this.Automator.VisualRoot).ToString(CultureInfo.InvariantCulture)
                                     }
                                 };
            foreach (var attribute in attributes)
            {
                writer.WriteAttributeString(attribute.Key, attribute.Value);
            }

            foreach (var child in element.Find(TreeScope.Children, x => true))
            {
                this.WriteElementToXml(writer, child);
            }

            writer.WriteEndElement();
        }

        private void WriteElementsToXml(XmlWriter writer)
        {
            writer.WriteStartElement("root");

            foreach (var element in WiniumVirtualRoot.Current.Find(TreeScope.Children, x=> true))
            {
                this.WriteElementToXml(writer, element);
            }

            writer.WriteEndElement();
        }

        #endregion
    }
}
