using System;
using System.Collections.Generic;
using System.Text;
using urakawa.metadata.daisy;
using urakawa.metadata;
using urakawa.xuk;

namespace urakawa.daisy
{
    public static class DiagramContentModelHelper
    {
        // -------------------------------------------------

        //public const string NS_PREFIX_XLINK = "xlink";
        //public const string NS_URL_XLINK = "http://www.w3.org/1999/xlink";


        //public const string XLINK_Href = NS_PREFIX_XLINK + ":href";

        // -------------------------------------------------

        public const string NS_PREFIX_TOBI = "tobi";
        public const string NS_URL_TOBI = "http://www.daisy.org/tobi";


        public const string TOBI_Audio = NS_PREFIX_TOBI + ":audio";

        // -------------------------------------------------


        // -------------------------------------------------

        //http://www.daisy.org/z3998/2012/z3998-2012.html
        public const string NS_PREFIX_ZAI = "z";
        public const string NS_URL_ZAI = "http://www.daisy.org/ns/z3998/authoring/";


        public const string P = "p";

#if true || SUPPORT_ANNOTATION_ELEMENT
        public const string Annotation = "annotation";
        public const string Ref = "ref";
        public const string By = "by";
        public const string Role = "role";
#endif //SUPPORT_ANNOTATION_ELEMENT

        public const string Object = "object";
        public const string Src = "src";
        public const string SrcType = "srctype";

        public const string Meta = "meta";
        public const string Name = "name"; // legacy
        public const string Property = "property";
        public const string Content = "content";
        public const string Rel = "rel";
        public const string Resource = "resource";
        public const string About = "about";

        // -------------------------------------------------

        // Special Urakawa SDK "Not Acknowledged" entity to use for metadata property and content,
        // when actual metadata has empty property/content pair with only optional attributes
        // (e.g. rel / resource)
        public const string NA = "N/A";

        // used in metadata of AltContent to identify one of the XML element names listed below
        public const string DiagramElementName = "diagram-element-name";
        public const string DiagramElementName_OBSOLETE = "description-name";

        // -------------------------------------------------

        //http://www.daisy.org/z3998/2012/auth/features/description/1.0/
        public const string NS_PREFIX_DIAGRAM = "d";
        public const string NS_PREFIX_DIAGRAM_METADATA = "diagram";
        public const string NS_URL_DIAGRAM = "http://www.daisy.org/ns/z3998/authoring/features/description/";


        //http://www.daisy.org/z3998/2012/auth/features/description/1.0/#z3998.feature.description.body.summary
        public const string D_Summary = NS_PREFIX_DIAGRAM + ":summary";

        //http://www.daisy.org/z3998/2012/auth/features/description/1.0/#z3998.feature.description.body.longdesc
        public const string D_LondDesc = NS_PREFIX_DIAGRAM + ":longdesc";

        //http://www.daisy.org/z3998/2012/auth/features/description/1.0/#z3998.feature.description.body.simplifiedLanguageDescription
        public const string D_SimplifiedLanguageDescription = NS_PREFIX_DIAGRAM + ":simplifiedLanguageDescription";

        //http://www.daisy.org/z3998/2012/auth/features/description/1.0/#z3998.feature.description.body.tactile
        public const string D_Tactile = NS_PREFIX_DIAGRAM + ":tactile";

        //http://www.daisy.org/z3998/2012/auth/features/description/1.0/#z3998.feature.description.tour
        public const string D_Tour = NS_PREFIX_DIAGRAM + ":tour";

        //http://www.daisy.org/z3998/2012/auth/features/description/1.0/#z3998.feature.description.body.simplifiedImage
        public const string D_SimplifiedImage = NS_PREFIX_DIAGRAM + ":simplifiedImage";

        //http://www.daisy.org/z3998/2012/auth/features/description/1.0/#z3998.feature.description
        public const string D_Description = NS_PREFIX_DIAGRAM + ":description";

        //http://www.daisy.org/z3998/2012/auth/features/description/1.0/#z3998.feature.description.body
        public const string D_Body = NS_PREFIX_DIAGRAM + ":body";

        //http://www.daisy.org/z3998/2012/auth/features/description/1.0/#z3998.feature.description.head
        public const string D_Head = NS_PREFIX_DIAGRAM + ":head";

        // -------------------------------------------------
        //http://www.daisy.org/z3998/2012/vocab/descriptions/

        //public const string DIAGRAM_Credentials = "diagram:credentials"; // Metadata property attribute

        public const string DIAGRAM_Purpose = "diagram:purpose"; // Metadata property attribute
        public const string DIAGRAM_DescriptionQuality = "diagram:descriptionQuality"; // Metadata property attribute

        public const string DIAGRAM_TargetAge = "diagram:targetAge"; // Metadata property attribute
        public const string DIAGRAM_targetGrade = "diagram:targetGrade"; // Metadata property attribute

        public const string DIAGRAM_CurrentVersion = "diagram:currentVersion"; // Metadata rel attribute
        public const string DIAGRAM_ThisVersion = "diagram:thisVersion"; // Metadata rel attribute
        public const string DIAGRAM_PreviousVersion = "diagram:previousVersion"; // Metadata rel attribute
        public const string DIAGRAM_AlternateVersion = "diagram:alternateVersion"; // Metadata rel attribute

        public const string DIAGRAM_Repository = "diagram:repository"; // Metadata rel attribute
        public const string DIAGRAM_QueryConcept = "diagram:queryConcept"; // Metadata property attribute

        // NOT IN THE SPEC, BUT USED IN THE XML SAMPLE!?
        public const string DIAGRAM_Credentials = "diagram:credentials"; // Metadata property attribute

        // -------------------------------------------------

        // -------------------------------------------------

        private static List<string> m_DIAGRAM_ElementNames = null;
        public static List<string> DIAGRAM_ElementNames
        {
            get
            {
                if (m_DIAGRAM_ElementNames == null)
                {
                    m_DIAGRAM_ElementNames = new List<string>();

                    m_DIAGRAM_ElementNames.Add(D_Summary);
                    m_DIAGRAM_ElementNames.Add(D_LondDesc);
                    m_DIAGRAM_ElementNames.Add(D_SimplifiedLanguageDescription);
                    m_DIAGRAM_ElementNames.Add(D_Tactile);
                    m_DIAGRAM_ElementNames.Add(D_SimplifiedImage);

                    //// not technically in the DIAGRAM namespace ;)
                    //m_DIAGRAM_ElementNames.Add(Object);

#if true || SUPPORT_ANNOTATION_ELEMENT
                    // ? ... not technically in the DIAGRAM namespace, but useful in DIAGRAM nonetheless.
                    m_DIAGRAM_ElementNames.Add(Annotation);
#endif //SUPPORT_ANNOTATION_ELEMENT
                }
                return m_DIAGRAM_ElementNames;
            }
        }

        private static List<string> m_DIAGRAM_ElementAttributes = null;
        public static List<string> DIAGRAM_ElementAttributes
        {
            get
            {
                if (m_DIAGRAM_ElementAttributes == null)
                {
                    m_DIAGRAM_ElementAttributes = new List<string>();

                    m_DIAGRAM_ElementAttributes.Add(XmlReaderWriterHelper.XmlId);
                    m_DIAGRAM_ElementAttributes.Add(XmlReaderWriterHelper.XmlLang);

                    //// see m_DIAGRAM_ElementNames.Add(Object);
                    //m_DIAGRAM_ElementAttributes.Add(Src);
                    //m_DIAGRAM_ElementAttributes.Add(SrcType);

#if true || SUPPORT_ANNOTATION_ELEMENT
                    // ? see m_DIAGRAM_ElementNames.Add(Annotation);
                    m_DIAGRAM_ElementAttributes.Add(Ref);
                    m_DIAGRAM_ElementAttributes.Add(By);
                    m_DIAGRAM_ElementAttributes.Add(Role);
#endif //SUPPORT_ANNOTATION_ELEMENT
                }
                return m_DIAGRAM_ElementAttributes;
            }
        }

        private static List<string> m_DIAGRAM_MetadataProperties = null;
        public static List<string> DIAGRAM_MetadataProperties
        {
            get
            {
                if (m_DIAGRAM_MetadataProperties == null)
                {
                    m_DIAGRAM_MetadataProperties = new List<string>();

                    m_DIAGRAM_MetadataProperties.Add(DIAGRAM_QueryConcept);
                    m_DIAGRAM_MetadataProperties.Add(DIAGRAM_Purpose);
                    m_DIAGRAM_MetadataProperties.Add(DIAGRAM_TargetAge);
                    m_DIAGRAM_MetadataProperties.Add(DIAGRAM_targetGrade);
                    m_DIAGRAM_MetadataProperties.Add(DIAGRAM_DescriptionQuality);
                    //m_DIAGRAM_MetadataProperties.Add(DIAGRAM_Credentials);

                    m_DIAGRAM_MetadataProperties.Add(SupportedMetadata_Z39862005.DC_AccessRights);

                    foreach (MetadataDefinition def in SupportedMetadata_Z39862005.DefinitionSet.Definitions)
                    {
                        //string str = def.Name.ToLower();

                        bool contains = false;
                        foreach (string str in m_DIAGRAM_MetadataProperties)
                        {
                            if (str.Equals(def.Name, StringComparison.OrdinalIgnoreCase))
                            {
                                contains = true;
                                break;
                            }
                        }

                        if (def.Name.StartsWith(SupportedMetadata_Z39862005.NS_PREFIX_DUBLIN_CORE + ":", StringComparison.OrdinalIgnoreCase)
                            && !contains)
                        {
                            m_DIAGRAM_MetadataProperties.Add(def.Name);
                        }

                        if (def.Synonyms != null)
                        {
                            foreach (string syn in def.Synonyms)
                            {
                                contains = false;
                                foreach (string str in m_DIAGRAM_MetadataProperties)
                                {
                                    if (str.Equals(syn, StringComparison.OrdinalIgnoreCase))
                                    {
                                        contains = true;
                                        break;
                                    }
                                }

                                if (syn.StartsWith(SupportedMetadata_Z39862005.NS_PREFIX_DUBLIN_CORE + ":", StringComparison.OrdinalIgnoreCase)
                                    && !contains)
                                {
                                    m_DIAGRAM_MetadataProperties.Add(syn);
                                }
                            }
                        }
                    }
                }
                return m_DIAGRAM_MetadataProperties;
            }
        }

        private static List<string> m_DIAGRAM_MetadataRelAttributeValues = null;
        public static List<string> DIAGRAM_MetadataRelAttributeValues
        {
            get
            {
                if (m_DIAGRAM_MetadataRelAttributeValues == null)
                {
                    m_DIAGRAM_MetadataRelAttributeValues = new List<string>();

                    m_DIAGRAM_MetadataRelAttributeValues.Add(DIAGRAM_PreviousVersion);
                    m_DIAGRAM_MetadataRelAttributeValues.Add(DIAGRAM_CurrentVersion);
                    m_DIAGRAM_MetadataRelAttributeValues.Add(DIAGRAM_ThisVersion);
                    m_DIAGRAM_MetadataRelAttributeValues.Add(DIAGRAM_AlternateVersion);

                    m_DIAGRAM_MetadataRelAttributeValues.Add(DIAGRAM_Repository);
                }
                return m_DIAGRAM_MetadataRelAttributeValues;
            }
        }


        private static List<string> m_DIAGRAM_MetadataAdditionalAttributeNames = null;
        public static List<string> DIAGRAM_MetadataAdditionalAttributeNames
        {
            get
            {
                if (m_DIAGRAM_MetadataAdditionalAttributeNames == null)
                {
                    m_DIAGRAM_MetadataAdditionalAttributeNames = new List<string>();

                    m_DIAGRAM_MetadataAdditionalAttributeNames.Add(XmlReaderWriterHelper.XmlId);
                    m_DIAGRAM_MetadataAdditionalAttributeNames.Add(About);
                    m_DIAGRAM_MetadataAdditionalAttributeNames.Add(Rel);
                    m_DIAGRAM_MetadataAdditionalAttributeNames.Add(Resource);
                }
                return m_DIAGRAM_MetadataAdditionalAttributeNames;
            }
        }













        public static string StripNSPrefix(string str)
        {
            //str.Split(':')[1]

            if (str.Length < 2) return str;

            int index = str.IndexOf(':');

            if (index == -1) return str;

            if (index == str.Length - 1) return string.Empty;

            return str.Substring(index + 1);
        }
    }
}