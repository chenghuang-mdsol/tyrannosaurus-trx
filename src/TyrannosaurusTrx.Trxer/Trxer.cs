﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;

namespace TyrannosaurusTrx.Trxer
{
    public class Program
    {
        /// <summary>
        /// Embedded Resource name
        /// </summary>
        private const string XSLT_FILE = "Trxer.xslt";
        /// <summary>
        /// Trxer output format
        /// </summary>
        private const string OUTPUT_FILE_EXT = ".html";

        //static void Main(string[] args)
        //{
        //    if (args.Any() == false)
        //    {
        //        Console.WriteLine("No trx file,  Trxer.exe <filename>");
        //        return;
        //    }
        //    Console.WriteLine("Trx File\n{0}", args[0]);
        //    Transform(args[0], PrepareXsl());
        //}


        public static void Transform(XmlReader trxFile, XmlWriter xmlOutput)
        {
            var xsl = PrepareXsl();
            XslCompiledTransform x = new XslCompiledTransform(true);
            x.Load(xsl, new XsltSettings(true, true), null);
            x.Transform(trxFile, xmlOutput);
            //x.Transform(fileName, fileName + OUTPUT_FILE_EXT);
        }

        public static string TransformXml(XDocument trxFile)
        {
            var xmlReader = XmlReader.Create(new StringReader(trxFile.ToString()));
            //MemoryStream stream = new MemoryStream();
            var sb = new StringBuilder();
            var settings = new XmlWriterSettings() { 
                
            };
            var xmlWriter = XmlWriter.Create(sb, settings);
            Transform(xmlReader, xmlWriter);

            //xmlWriter.Flush();
            //stream.Position = 0;

            //StreamReader reader = new StreamReader(stream);
            string text = sb.ToString();
            return text;
        }

        /// <summary>
        /// Loads xslt form embedded resource
        /// </summary>
        /// <returns>Xsl document</returns>
        public static XmlDocument PrepareXsl()
        {
            XmlDocument xslDoc = new XmlDocument();
            Console.WriteLine("Loading xslt template...");
            xslDoc.Load(ResourceReader.StreamFromResource(XSLT_FILE));
            //MergeCss(xslDoc);
            //MergeJavaScript(xslDoc);
            return xslDoc;
        }

        /// <summary>
        /// Merges all javascript linked to page into Trxer html report itself
        /// </summary>
        /// <param name="xslDoc">Xsl document</param>
        private static void MergeJavaScript(XmlDocument xslDoc)
        {
            Console.WriteLine("Loading javascript...");
            XmlNode scriptEl = xslDoc.GetElementsByTagName("script")[0];
            XmlAttribute scriptSrc = scriptEl.Attributes["src"];
            string script = ResourceReader.LoadTextFromResource(scriptSrc.Value);
            scriptEl.Attributes.Remove(scriptSrc);
            scriptEl.InnerText = script;
        }

        /// <summary>
        /// Merges all css linked to page ito Trxer html report itself
        /// </summary>
        /// <param name="xslDoc">Xsl document</param>
        private static void MergeCss(XmlDocument xslDoc)
        {
            Console.WriteLine("Loading css...");
            XmlNode headNode = xslDoc.GetElementsByTagName("head")[0];
            XmlNodeList linkNodes = xslDoc.GetElementsByTagName("link");
            List<XmlNode> toChangeList = linkNodes.Cast<XmlNode>().ToList();

            foreach (XmlNode xmlElement in toChangeList)
            {
                XmlElement styleEl = xslDoc.CreateElement("style");
                styleEl.InnerText = ResourceReader.LoadTextFromResource(xmlElement.Attributes["href"].Value);
                headNode.ReplaceChild(styleEl, xmlElement);
            }
        }
    }
}