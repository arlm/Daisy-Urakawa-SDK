using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Xsl;
using System.Xml;
using System.Reflection;

namespace XukToZed
{
    public class XukToZed
    {
        System.Xml.Xsl.XslCompiledTransform theTransformer = new XslCompiledTransform(true);
        public System.Xml.Xsl.XsltArgumentList TransformationArguments = new XsltArgumentList();
        private string strOutputDir = ".";
        private string strContextFolder = ".";

        public XukToZed()//string pathToStylesheet)
        {
            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                string pathToStylesheet = "XukToZed.xslt";
                XslResolver theResolver = new XslResolver(this.GetType());
                theTransformer.Load(pathToStylesheet, null, theResolver);
            }
            catch (Exception loadException)
            {
                System.Diagnostics.Debug.WriteLine(loadException.ToString());
                throw (loadException);
            }
        }

        public string OuputDir
        {
            get { return strOutputDir; }
            set { strOutputDir = value; }
        }

        public string contextFolderName
        {
            get { return strContextFolder; }
            set { strContextFolder = value; }
        }
       
        public void WriteZed(XmlReader input)
        {
            System.IO.StringWriter dataHolder = new System.IO.StringWriter();
            XmlWriter results = XmlWriter.Create((System.IO.TextWriter)dataHolder);

            string strDcDate = (string)TransformationArguments.GetParam("dcDate","");
            if (strDcDate == null)
            {
                strDcDate = DateTime.Now.ToString("yyyy-MM-dd");
                TransformationArguments.AddParam("dcDate", "", strDcDate);
            }

            try
            {
                theTransformer.Transform(input, TransformationArguments ,results);
            }
            catch(Exception eAnything)
            { 
                results = null;
            }
            input.Close(); //Since the stream has been read to the end, we might as well close it.

            #region this region only needed for debugging, will be removed 
            //TODO: the actual removal!
            string[] strSmilFiles = System.IO.Directory.GetFiles(strOutputDir, "*.smil");
            foreach(string aSmilFile in strSmilFiles)
            {
                System.IO.File.Delete(aSmilFile);
            }
            #endregion

            XmlDocument resDoc = new XmlDocument();
            resDoc.LoadXml(dataHolder.ToString());

            XmlWriterSettings fileSettings = new XmlWriterSettings();
            fileSettings.Indent = true;

            //TODO:Remove following line
            resDoc.Save(strOutputDir + "/raw.xml");

 
            XmlNamespaceManager xPathNSManager = new XmlNamespaceManager((XmlNameTable)new NameTable());
            xPathNSManager.AddNamespace("smil", "http://www.w3.org/2001/SMIL20/Language");
            xPathNSManager.AddNamespace("opf", "http://openebook.org/namespaces/oeb-package/1.0/");
            xPathNSManager.AddNamespace("ncx", "http://www.daisy.org/z3986/2005/ncx/");


            XmlNode ncxTree = resDoc.DocumentElement.SelectSingleNode("//ncx:ncx", xPathNSManager);
            string ncxFilename = (string)TransformationArguments.GetParam("ncxFilename", "");
            if (ncxFilename == "")
                ncxFilename = "navigation.ncx";
            XmlWriter ncxFile = XmlWriter.Create(strOutputDir+ "/" + ncxFilename, fileSettings);
            ncxFile.WriteNode(ncxTree.CreateNavigator(), false);
            ncxFile.Close();
            ncxTree.ParentNode.RemoveChild(ncxTree); //remove the written bit


            #region Calculating running time, setting on smil file nodes as required

            TimeSpan prevDuration = new TimeSpan();
            try
            {
                string tmpXpathStatement = "//*[self::smil:smil or self::smil:audio]";
                XmlNodeList lstAudAndSmil = resDoc.DocumentElement.SelectNodes(tmpXpathStatement, xPathNSManager);
                
                for (int i = 0; i < lstAudAndSmil.Count;i++)
                {
                    XmlElement curElement = (XmlElement)lstAudAndSmil[i];
                    switch (curElement.LocalName)
                    {
                        case "smil":
                            XmlElement ndElapsed = (XmlElement)curElement.SelectSingleNode(".//smil:meta[@name='dtb:totalElapsedTime']", xPathNSManager);
                            ndElapsed.SetAttribute("content",prevDuration.ToString().TrimEnd(".0".ToCharArray()));
                            break;
                        case "audio":
                            try
                            {
                                prevDuration = prevDuration.Subtract(TimeSpan.Parse(curElement.GetAttribute("clipBegin")));
                            }
                            catch { } 
                            try
                            {
                                prevDuration = prevDuration.Add(TimeSpan.Parse((curElement.GetAttribute("clipEnd"))));
                            }
                            catch { }
                            break;
                        default:

                            break;

                    }
                }
            }
            catch(Exception eAnything)
            {
                //TODO: Error forwarding
                System.Diagnostics.Debug.WriteLine(eAnything.ToString());
            }

            //TODO:Remove following line
            //resDoc.Save(strOutputDir + "/raw.xml");
            #endregion 

            XmlElement metaDtbTotalDuration = (XmlElement)resDoc.SelectSingleNode("//opf:meta[@name='dtb:totalTime']",xPathNSManager);
            metaDtbTotalDuration.SetAttribute("content", prevDuration.ToString().TrimEnd(".0".ToCharArray()));

            XmlNode opfTree = resDoc.DocumentElement.SelectSingleNode("//opf:package", xPathNSManager);
            string opfFilename = (string)TransformationArguments.GetParam("packageFilename","");
            if (opfFilename == "")
                opfFilename = "package.opf";
            XmlWriter opfFile = XmlWriter.Create(strOutputDir + "/" + opfFilename, fileSettings);
            opfFile.WriteNode(opfTree.CreateNavigator(), false);
            opfFile.Close();
            opfTree.ParentNode.RemoveChild(opfTree); //remove the written bit

            XmlNodeList smilTrees = resDoc.DocumentElement.SelectNodes("//smil:smil", xPathNSManager);
            for (int i = smilTrees.Count - 1; i > -1; i--)
            {
                XmlElement newRoot = (XmlElement)smilTrees[i];
                XmlWriter smilFile = XmlWriter.Create(strOutputDir + "/" + newRoot.GetAttribute("filename") + ".smil",fileSettings);
                newRoot.RemoveAttribute("filename");
                smilFile.WriteNode(newRoot.CreateNavigator(),false);
                smilFile.Close();
                newRoot.ParentNode.RemoveChild(newRoot);
            }

            //TODO:Remove following line
            //resDoc.Save(strOutputDir + "/raw.xml");

            XmlNodeList filesToCopy = resDoc.DocumentElement.SelectNodes("filenames/file",xPathNSManager);
            foreach(XmlNode fileNode in filesToCopy)
            {
                string strSourceFileName = strContextFolder + "\\" + fileNode.InnerText;
                strSourceFileName = strSourceFileName.Replace("/","\\");

                string strDestFileName = fileNode.InnerText.Substring((fileNode.InnerText.LastIndexOf("/") > 0) ? fileNode.InnerText.LastIndexOf("/")+1 : 0);
                strDestFileName = OuputDir + "\\" + strDestFileName;
                strDestFileName = strDestFileName.Replace("/","\\");
                try
                {
                    System.IO.File.Copy(strSourceFileName,strDestFileName, true);
                }
                catch (Exception eAnything)
                {
                    System.Diagnostics.Debug.WriteLine(eAnything.ToString());
                }
            }
        }
    }
}
