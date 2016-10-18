namespace Winium.StoreApps.InnerServer.Commands
{
    #region

    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Xml;

    using Winium.Mobile.Common;
    using Winium.StoreApps.InnerServer.Element;

    #endregion

    internal class PageSourceCommand : CommandBase
    {
        #region Public Properties

        public string ElementId { get; set; }

        #endregion

        #region Methods

        internal static void WriteElementToXml(XmlWriter writer, WiniumElement element)
        {
            if (element == null)
            {
                return;
            }

            var className = element.ClassName;
            var tagName = XmlConvert.EncodeNmToken(className);
            writer.WriteStartElement(tagName);
            var rect = element.GetRect();
            var attributes = new Dictionary<string, string>
                                 {
                                     { "name", element.AutomationName }, 
                                     { "id", element.AutomationId }, 
                                     { "class_name", className }, 
                                     { "xname", element.XName }, 
                                     {
                                         "visible", 
                                         element.IsUserVisible().ToString().ToLowerInvariant()
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
                                         element.GetCoordinatesInView()
                                         .ToString(CultureInfo.InvariantCulture)
                                     }
                                 };
            foreach (var attribute in attributes)
            {
                writer.WriteAttributeString(attribute.Key, attribute.Value);
            }

            foreach (var child in element.Find(TreeScope.Children, x => true))
            {
                WriteElementToXml(writer, child);
            }

            writer.WriteEndElement();
        }

        protected override string DoImpl()
        {
            string source;
            var settings = new XmlWriterSettings { Indent = true, Encoding = new UTF8Encoding(false) };

            using (var writer = new MemoryStream())
            {
                var xmlWriter = XmlWriter.Create(writer, settings);
                WriteElementsToXml(xmlWriter);
                xmlWriter.Flush();

                var buffer = writer.ToArray();
                source = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
            }

            return this.JsonResponse(ResponseStatus.Success, source);
        }

        private static void WriteElementsToXml(XmlWriter writer)
        {
            writer.WriteStartElement("root");

            foreach (var element in WiniumVirtualRoot.Current.Find(TreeScope.Children, x => true))
            {
                WriteElementToXml(writer, element);
            }

            writer.WriteEndElement();
        }

        #endregion
    }
}
