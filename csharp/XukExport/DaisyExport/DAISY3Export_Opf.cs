using System.Collections.Generic;
using System.Xml;
using System.IO;

namespace DaisyExport
{
    partial class DAISY3Export
    {

        private void CreateOpfDocument()
        {
            XmlDocument opfDocument = CreateStub_OpfDocument();

            XmlNode manifestNode = getFirstChildElementsWithName(opfDocument, true, "manifest", null); //opfDocument.GetElementsByTagName("manifest")[0];
            const string mediaType_Smil = "application/smil";
            const string mediaType_Wav = "audio/x-wav";
            const string mediaType_Mpg = "audio/mpeg";
            const string mediaType_Opf = "text/xml";
            const string mediaType_Ncx = "application/x-dtbncx+xml";
            const string mediaType_Dtbook = "application/x-dtbook+xml";
            const string mediaType_Image_Jpg = "image/jpeg";
            const string mediaType_Image_Png = "image/png";

            XmlNode itemNode = opfDocument.CreateElement(null, "item", manifestNode.NamespaceURI);
            manifestNode.AppendChild(itemNode);
            CommonFunctions.CreateAppendXmlAttribute(opfDocument, itemNode, "href", m_Filename_Ncx);
            CommonFunctions.CreateAppendXmlAttribute(opfDocument, itemNode, "id", GetNextID(ID_OpfPrefix));
            CommonFunctions.CreateAppendXmlAttribute(opfDocument, itemNode, "media-type", mediaType_Ncx);

            itemNode = opfDocument.CreateElement(null, "item", manifestNode.NamespaceURI);
            manifestNode.AppendChild(itemNode);
            CommonFunctions.CreateAppendXmlAttribute(opfDocument, itemNode, "href", m_Filename_Content);
            CommonFunctions.CreateAppendXmlAttribute(opfDocument, itemNode, "id", GetNextID(ID_OpfPrefix));
            CommonFunctions.CreateAppendXmlAttribute(opfDocument, itemNode, "media-type", mediaType_Dtbook);

            itemNode = opfDocument.CreateElement(null, "item", manifestNode.NamespaceURI);
            manifestNode.AppendChild(itemNode);
            CommonFunctions.CreateAppendXmlAttribute(opfDocument, itemNode, "href", m_Filename_Opf);
            CommonFunctions.CreateAppendXmlAttribute(opfDocument, itemNode, "id", GetNextID(ID_OpfPrefix));
            CommonFunctions.CreateAppendXmlAttribute(opfDocument, itemNode, "media-type", mediaType_Opf);

            // add smil files to manifest
            List<string> smilIDListInPlayOrder = new List<string>();

            foreach (string smilFileName in m_FilesList_Smil)
            {
                itemNode = opfDocument.CreateElement(null, "item", manifestNode.NamespaceURI);
                manifestNode.AppendChild(itemNode);
                CommonFunctions.CreateAppendXmlAttribute(opfDocument, itemNode, "href", smilFileName);
                string strID = GetNextID(ID_OpfPrefix);
                CommonFunctions.CreateAppendXmlAttribute(opfDocument, itemNode, "id", strID);
                CommonFunctions.CreateAppendXmlAttribute(opfDocument, itemNode, "media-type", mediaType_Smil);

                smilIDListInPlayOrder.Add(strID);
            }

            foreach (string audioFileName in m_FilesList_Audio)
            {
                itemNode = opfDocument.CreateElement(null, "item", manifestNode.NamespaceURI);
                manifestNode.AppendChild(itemNode);
                CommonFunctions.CreateAppendXmlAttribute(opfDocument, itemNode, "href", audioFileName);
                string strID = GetNextID(ID_OpfPrefix);
                CommonFunctions.CreateAppendXmlAttribute(opfDocument, itemNode, "id", strID);

                if (string.Compare(Path.GetExtension(audioFileName), ".wav", true) == 0)
                {
                    CommonFunctions.CreateAppendXmlAttribute(opfDocument, itemNode, "media-type", mediaType_Wav);
                }
                else
                {
                    CommonFunctions.CreateAppendXmlAttribute(opfDocument, itemNode, "media-type", mediaType_Mpg);
                }
            }

            foreach (string imageFileName in m_FilesList_Image)
            {
                itemNode = opfDocument.CreateElement(null, "item", manifestNode.NamespaceURI);
                manifestNode.AppendChild(itemNode);
                CommonFunctions.CreateAppendXmlAttribute(opfDocument, itemNode, "href", imageFileName);
                string strID = GetNextID(ID_OpfPrefix);
                CommonFunctions.CreateAppendXmlAttribute(opfDocument, itemNode, "id", strID);

                if (string.Compare(Path.GetExtension(imageFileName), ".png", true) == 0)
                {
                    CommonFunctions.CreateAppendXmlAttribute(opfDocument, itemNode, "media-type", mediaType_Image_Png);
                }
                else
                {
                    CommonFunctions.CreateAppendXmlAttribute(opfDocument, itemNode, "media-type", mediaType_Image_Jpg);
                }
            }



            // create spine
            XmlNode spineNode = getFirstChildElementsWithName(opfDocument, true, "spine", null); //opfDocument.GetElementsByTagName("spine")[0];

            foreach (string strSmilID in smilIDListInPlayOrder)
            {
                XmlNode itemRefNode = opfDocument.CreateElement(null, "itemref", spineNode.NamespaceURI);
                spineNode.AppendChild(itemRefNode);
                CommonFunctions.CreateAppendXmlAttribute(opfDocument, itemRefNode, "idref", strSmilID);
            }

            AddMetadata_Opf(opfDocument);
            CommonFunctions.WriteXmlDocumentToFile(opfDocument,
                Path.Combine(m_OutputDirectory, m_Filename_Opf));
        }

        private void AddMetadata_Opf(XmlDocument opfDocument)
        {
            XmlNode dc_metadataNode = getFirstChildElementsWithName(opfDocument, true, "dc-metadata", null); //opfDocument.GetElementsByTagName("dc-metadata")[0];
            XmlNode x_metadataNode = getFirstChildElementsWithName(opfDocument, true, "x-metadata", null); //opfDocument.GetElementsByTagName("x-metadata")[0];

            foreach (urakawa.metadata.Metadata m in m_Presentation.Metadatas.ContentsAs_YieldEnumerable)
            {
                XmlNode metadataNodeCreated = null;
                if (m.NameContentAttribute.Name.StartsWith("dc:"))
                {
                    if (m.NameContentAttribute.Name == "dc:format")
                    {
                        metadataNodeCreated = AddMetadataAsInnerText(opfDocument, dc_metadataNode, m.NameContentAttribute.Name, "ANSI/NISO Z39.86-2005");
                    }
                    else
                    {
                        metadataNodeCreated = AddMetadataAsInnerText(opfDocument, dc_metadataNode, m.NameContentAttribute.Name, m.NameContentAttribute.Value);
                    }
                }
                else
                {
                    if (m.NameContentAttribute.Name == "dtb:totaltime")
                    {
                        metadataNodeCreated = AddMetadataAsAttributes(opfDocument, x_metadataNode, m.NameContentAttribute.Name, m_TotalTime.ToString());
                    }
                    else
                    {
                        metadataNodeCreated = AddMetadataAsAttributes(opfDocument, x_metadataNode, m.NameContentAttribute.Name, m.NameContentAttribute.Value);
                    }
                }
                // add other metadata attributes if any
                foreach (urakawa.metadata.MetadataAttribute ma in m.OtherAttributes.ContentsAs_YieldEnumerable)
                {
                    CommonFunctions.CreateAppendXmlAttribute(opfDocument, metadataNodeCreated, ma.Name, ma.Value);
                }

            } // end of metadata for each loop

            // add total time
            XmlNode totalTimeNode = getFirstChildElementsWithName(opfDocument, true, "dtb:totaltime", null);
            if (totalTimeNode == null)
            {
                AddMetadataAsAttributes(opfDocument, x_metadataNode, "dtb:totaltime", m_TotalTime.ToString());
            }
            //XmlNodeList totalTimeNodesList = opfDocument.GetElementsByTagName("dtb:totaltime");
            //if (totalTimeNodesList == null || (totalTimeNodesList != null && totalTimeNodesList.Count == 0))
            //{
            //    AddMetadataAsAttributes(opfDocument, x_metadataNode, "dtb:totaltime", m_TotalTime.ToString());
            //}

            // add uid to dc:identifier
            //XmlNodeList identifierList = opfDocument.GetElementsByTagName("dc:Identifier");
            XmlNode identifierNode = null;
            bool isUidReAssigned = false;
            foreach (XmlNode identifierMetaNode in getChildrenElementsWithName(opfDocument, true, "dc:Identifier", null, false))
            {
                if (identifierNode == null) identifierNode = identifierMetaNode;

                if (identifierMetaNode.Attributes.GetNamedItem("uid") != null)
                {
                    identifierMetaNode.Attributes.GetNamedItem("uid").Value = "uid";
                    isUidReAssigned = true;
                }
            }
            if (!isUidReAssigned)
            {
                CommonFunctions.CreateAppendXmlAttribute(opfDocument, identifierNode, "id", "uid");
            }
        }

        private XmlNode AddMetadataAsInnerText(XmlDocument doc, XmlNode metadataParentNode, string name, string content)
        {
            XmlNode node = null;

            if (name.Contains(":"))
            {
                // split the metadata name and make first alphabet upper, required for daisy 3.0
                string splittedName = name.Split(':')[1];
                splittedName = splittedName.Substring(0, 1).ToUpper() + splittedName.Remove(0, 1);

                node = doc.CreateElement(name.Split(':')[0], splittedName, metadataParentNode.Attributes.GetNamedItem("xmlns:dc").Value);
            }
            else
            {
                node = doc.CreateElement(null, name, metadataParentNode.NamespaceURI);
            }
            metadataParentNode.AppendChild(node);
            node.AppendChild(
                doc.CreateTextNode(content));

            return node;
        }

        private XmlDocument CreateStub_OpfDocument()
        {
            XmlDocument document = new XmlDocument();
            document.XmlResolver = null;

            document.CreateXmlDeclaration("1.0", "utf-8", null);
            document.AppendChild(document.CreateDocumentType("package",
                "+//ISBN 0-9673008-1-9//DTD OEB 1.2 Package//EN",
                "http://openebook.org/dtds/oeb-1.2/oebpkg12.dtd",
                null));

            XmlNode rootNode = document.CreateElement(null,
                "package",
                "http://openebook.org/namespaces/oeb-package/1.0/");

            document.AppendChild(rootNode);


            CommonFunctions.CreateAppendXmlAttribute(document, rootNode, "unique-identifier", "uid");

            XmlNode metadataNode = document.CreateElement(null, "metadata", rootNode.NamespaceURI);
            rootNode.AppendChild(metadataNode);

            XmlNode dcMetadataNode = document.CreateElement(null, "dc-metadata", rootNode.NamespaceURI);
            metadataNode.AppendChild(dcMetadataNode);
            CommonFunctions.CreateAppendXmlAttribute(document, dcMetadataNode, "xmlns:dc", "http://purl.org/dc/elements/1.1/");
            CommonFunctions.CreateAppendXmlAttribute(document, dcMetadataNode, "xmlns:oebpackage", "http://openebook.org/namespaces/oeb-package/1.0/");

            XmlNode xMetadataNode = document.CreateElement(null, "x-metadata", rootNode.NamespaceURI);
            metadataNode.AppendChild(xMetadataNode);

            XmlNode manifestNode = document.CreateElement(null, "manifest", rootNode.NamespaceURI);
            rootNode.AppendChild(manifestNode);

            XmlNode spineNode = document.CreateElement(null, "spine", rootNode.NamespaceURI);
            rootNode.AppendChild(spineNode);


            return document;
        }

    }
}