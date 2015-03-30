namespace Winium.StoreApps.Inspector
{
    #region

    using System;
    using System.Linq;
    using System.Text;
    using System.Xml;

    #endregion

    public static class XPathHelpers
    {
        #region Public Methods and Operators

        public static string FindXPath(XmlNode node)
        {
            var builder = new StringBuilder();
            while (node != null)
            {
                switch (node.NodeType)
                {
                    case XmlNodeType.Attribute:
                        builder.Insert(0, "/@" + node.Name);
                        node = ((XmlAttribute)node).OwnerElement;
                        break;
                    case XmlNodeType.Element:
                        var index = FindElementIndex((XmlElement)node);
                        builder.Insert(0, "/" + node.Name + "[" + index + "]");
                        node = node.ParentNode;
                        break;
                    case XmlNodeType.Document:
                        return builder.ToString();
                    default:
                        throw new ArgumentException("Only elements and attributes are supported");
                }
            }
            return "*";
            //            throw new ArgumentException("Node was not in a document");
        }

        #endregion

        #region Methods

        private static int FindElementIndex(XmlElement element)
        {
            var parentNode = element.ParentNode;
            if (parentNode is XmlDocument)
            {
                return 1;
            }

            var parent = (XmlElement)parentNode;
            var index = 1;
            if (parent != null)
            {
                foreach (var candidate in parent.ChildNodes.Cast<XmlNode>().Where(candidate => candidate is XmlElement && candidate.Name == element.Name))
                {
                    if (candidate == element)
                    {
                        return index;
                    }

                    index++;
                }
            }

            throw new ArgumentException("Couldn't find element within parent");
        }

        #endregion
    }
}
