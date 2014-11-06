/*
 * This code is derived from boilerpipe
 * 
 */

using System;
using System.IO;
using NBoilerpipe;
using NBoilerpipe.Document;
using NBoilerpipe.Extractors;
using NBoilerpipe.Parser;
using Sharpen;
using Shaman.Dom;

namespace NBoilerpipe.Extractors
{
    /// <summary>The base class of Extractors.</summary>
    /// <remarks>
    /// The base class of Extractors. Also provides some helper methods to quickly
    /// retrieve the text that remained after processing.
    /// </remarks>
    /// <author>Christian Kohlsch√ºtter</author>
    public abstract class ExtractorBase : BoilerpipeExtractor
    {
        /// <summary>Extracts text from the HTML code given as a String.</summary>
        /// <remarks>Extracts text from the HTML code given as a String.</remarks>
        /// <param name="html">The HTML code as a String.</param>
        /// <returns>The extracted text.</returns>
        /// <exception cref="NBoilerpipe.BoilerpipeProcessingException">NBoilerpipe.BoilerpipeProcessingException
        /// 	</exception>
        public virtual string GetText(string html)
        {

            var parser = new NBoilerpipeHtmlParser(new NBoilerpipeContentHandler());
            parser.Parse(html);
            return GetText(parser.ToTextDocument());

        }

        public string GetText(HtmlNode document)
        {
            var parser = new NBoilerpipeHtmlParser(new NBoilerpipeContentHandler());
            parser.Traverse(document);
            return GetText(parser.ToTextDocument());
        }

        public TextDocument GetDocument(HtmlNode document)
        {
            var parser = new NBoilerpipeHtmlParser(new NBoilerpipeContentHandler());
            parser.Traverse(document);
            return parser.ToTextDocument();
        }

        /// <summary>
        /// Extracts text from the given
        /// <see cref="NBoilerpipe.Document.TextDocument">NBoilerpipe.Document.TextDocument</see>
        /// object.
        /// </summary>
        /// <param name="doc">
        /// The
        /// <see cref="NBoilerpipe.Document.TextDocument">NBoilerpipe.Document.TextDocument</see>
        /// .
        /// </param>
        /// <returns>The extracted text.</returns>
        /// <exception cref="NBoilerpipe.BoilerpipeProcessingException">NBoilerpipe.BoilerpipeProcessingException
        /// 	</exception>
        public virtual string GetText(TextDocument doc)
        {
            Process(doc);
            return doc.GetContent();
        }

        public abstract bool Process(TextDocument arg1);
    }
}
