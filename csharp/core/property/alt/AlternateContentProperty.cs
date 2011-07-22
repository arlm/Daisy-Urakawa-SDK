using System;
using System.Xml;
using urakawa.metadata;
using urakawa.xuk;
using urakawa.progress;

namespace urakawa.property.alt
{
    public class AlternateContentProperty : Property
    {
        public override string GetTypeNameFormatted()
        {
            return XukStrings.AlternateContentProperty;
        }


        public AlternateContentProperty()
        {
            m_AlternateContents = new ObjectListProvider<AlternateContent>(this, true);
            m_Metadata = new ObjectListProvider<Metadata>(this, true);
        }

        private ObjectListProvider<Metadata> m_Metadata;
        public ObjectListProvider<Metadata> Metadatas
        {
            get
            {
                return m_Metadata;
            }
        }

        private ObjectListProvider<AlternateContent> m_AlternateContents;
        public ObjectListProvider<AlternateContent> AlternateContents
        {
            get
            {
                return m_AlternateContents;
            }
        }

        protected override void XukInAttributes(XmlReader source)
        {
            //nothing new here
            base.XukInAttributes(source);
        }

        protected override void XukOutAttributes(XmlWriter destination, Uri baseUri)
        {
            //nothing new here
            base.XukOutAttributes(destination, baseUri);
        }

        protected override void XukOutChildren(XmlWriter destination, Uri baseUri, IProgressHandler handler)
        {
            base.XukOutChildren(destination, baseUri, handler);

            destination.WriteStartElement(XukStrings.Metadatas, XukAble.XUK_NS);
            foreach (Metadata md in m_Metadata.ContentsAs_Enumerable)
            {
                md.XukOut(destination, baseUri, handler);
            }
            destination.WriteEndElement();

            destination.WriteStartElement(XukStrings.AlternateContents, XukAble.XUK_NS);
            foreach (AlternateContent ac in m_AlternateContents.ContentsAs_Enumerable)
            {
                ac.XukOut(destination, baseUri, handler);
            }
            destination.WriteEndElement();
        }

        private void XukInMetadata(XmlReader source, IProgressHandler handler)
        {
            if (source.IsEmptyElement) return;
            while (source.Read())
            {
                if (source.NodeType == XmlNodeType.Element)
                {
                    Metadata md = Presentation.MetadataFactory.CreateMetadata();
                    md.XukIn(source, handler);
                    m_Metadata.Insert(m_Metadata.Count, md);
                }
                else if (source.NodeType == XmlNodeType.EndElement)
                {
                    break;
                }
                if (source.EOF)
                {
                    throw new exception.XukException("Unexpectedly reached EOF");
                }
            }
        }

        protected override void XukInChild(XmlReader source, IProgressHandler handler)
        {
            bool readItem = false;
            if (source.NamespaceURI == XukAble.XUK_NS)
            {
                readItem = true;
                if (source.LocalName == XukStrings.Metadatas)
                {
                    XukInMetadata(source, handler);
                }
                else if (source.LocalName == XukStrings.AlternateContents)
                {
                    AlternateContent ac = Presentation.AlternateContentFactory.CreateAlternateContent();
                    ac.XukIn(source, handler);
                    m_AlternateContents.Insert(m_AlternateContents.Count, ac);
                }
                else
                {
                    readItem = false;
                }
            }
            if (!(readItem || source.IsEmptyElement))
            {
                source.ReadSubtree().Close(); //Read past unknown child 
            }
        }
    }
}
