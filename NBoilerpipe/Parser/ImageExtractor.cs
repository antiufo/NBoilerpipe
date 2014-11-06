using NBoilerpipe;
using NBoilerpipe.Parser;
using Shaman.Dom;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBoilerpipe.Parser
{


    /**
     * Extracts the images that are enclosed by extracted content. 
     * 
     * @author Christian Kohlschütter
     */
    public class ImageExtractor
    {
        public static readonly ImageExtractor INSTANCE = new ImageExtractor();

        /**
         * Returns the singleton instance of {@link ImageExtractor}.
         * 
         * @return
         */
        public static ImageExtractor getInstance()
        {
            return INSTANCE;
        }

        private ImageExtractor()
        {
        }

        ///**
        // * Processes the given {@link TextDocument} and the original HTML text (as a
        // * String).
        // * 
        // * @param doc
        // *            The processed {@link TextDocument}.
        // * @param origHTML
        // *            The original HTML document.
        // * @return A List of enclosed {@link Image}s
        // * @throws BoilerpipeProcessingException
        // */
        //public List<Image> process( TextDocument doc, string origHTML)  {
        //    return process(doc, new InputSource(
        //            new StringReader(origHTML)));
        //}

        ///**
        // * Processes the given {@link TextDocument} and the original HTML text (as an
        // * {@link InputSource}).
        // * 
        // * @param doc
        // *            The processed {@link TextDocument}.
        // * @param origHTML
        // *            The original HTML document.
        // * @return A List of enclosed {@link Image}s
        // * @throws BoilerpipeProcessingException
        // */
        //public List<Image> process( HtmlDocument doc,
        //         InputSource inpsrc) {
        //     Implementation implementation = new Implementation();
        //    implementation.process(doc, inpsrc);

        //    return implementation.linksHighlight;
        //}

        ///**
        // * Fetches the given {@link URL} using {@link HTMLFetcher} and processes the
        // * retrieved HTML using the specified {@link BoilerpipeExtractor}.
        // * 
        // * @param doc
        // *            The processed {@link TextDocument}.
        // * @param is
        // *            The original HTML document.
        // * @return A List of enclosed {@link Image}s
        // * @throws BoilerpipeProcessingException
        // */
        //public List<Image> process( Uri url,  BoilerpipeExtractor extractor) {
        // HTMLDocument htmlDoc = HTMLFetcher.fetch(url);

        // TextDocument doc = new BoilerpipeSAXInput(htmlDoc.toInputSource())
        //            .getTextDocument();
        //    extractor.process(doc);

        // InputSource ls = htmlDoc.toInputSource();

        //    return process(doc, ls);
        //}


        private class Implementation : NBoilerpipeContentHandler
        {
            List<Image> linksHighlight = new List<Image>();
            private List<Image> linksBuffer = new List<Image>();

            private int inIgnorableElement = 0;
            private int characterElementIdx = 0;
            private List<bool> contentBitSet = new List<bool>();

            private bool inHighlight = false;

            //Implementation() {
            //    super(new HTMLConfiguration());
            //    setContentHandler(this);
            //}

            //void process( TextDocument doc,  InputSource ls)
            // {
            //    for (TextBlock block : doc.getTextBlocks()) {
            //        if (block.isContent()) {
            //             BitSet bs = block.getContainedTextElements();
            //            if (bs != null) {
            //                contentBitSet.or(bs);
            //            }
            //        }
            //    }

            //        parse(ls);

            //}

            public void endDocument()
            {
            }

            public void endPrefixMapping(String prefix)
            {
            }

            public void ignorableWhitespace(char[] ch, int start, int length)
            {
            }

            public void processingInstruction(String target, String data)
            {
            }

            //public void setDocumentLocator(Locator locator) {
            //}

            public void skippedEntity(String name)
            {
            }

            public void startDocument()
            {
            }

            public void startElement(String uri, String localName, String qName,
                    HtmlAttributeCollection atts)
            {
                //TagAction ta = CommonTagActions. .get(localName);
                //if (ta != null) {
                //    ta.beforeStart(this, localName);
                //}

                //try {
                //    if (inIgnorableElement == 0) {
                //        if(inHighlight && "IMG".equalsIgnoreCase(localName)) {
                //            String src = atts.getValue("src");
                //            if(src != null && src.length() > 0) {
                //                linksBuffer.add(new Image(src, atts.getValue("width"), atts.getValue("height"), atts.getValue("alt")));
                //            }
                //        }
                //    }
                //} finally {
                //    if (ta != null) {
                //        ta.afterStart(this, localName);
                //    }
                //}
            }

            public void endElement(String uri, String localName, String qName)
            {
                //TagAction ta = TAG_ACTIONS.get(localName);
                //if (ta != null) {
                //    ta.beforeEnd(this, localName);
                //}

                //try {
                //    if (inIgnorableElement == 0) {
                //        //
                //    }
                //} finally {
                //    if (ta != null) {
                //        ta.afterEnd(this, localName);
                //    }
                //}
            }

            public void characters(char[] ch, int start, int length)
            {
                characterElementIdx++;
                if (inIgnorableElement == 0)
                {

                    var highlight = contentBitSet[characterElementIdx];
                    if (!highlight)
                    {
                        if (length == 0)
                        {
                            return;
                        }
                        var justWhitespace = true;
                        for (int i = start; i < start + length; i++)
                        {
                            if (!char.IsWhiteSpace(ch[i]))
                            {
                                justWhitespace = false;
                                break;
                            }
                        }
                        if (justWhitespace)
                        {
                            return;
                        }
                    }

                    inHighlight = highlight;
                    if (inHighlight)
                    {
                        linksHighlight.AddRange(linksBuffer);
                        linksBuffer.Clear();
                    }
                }
            }

            public void startPrefixMapping(String prefix, String uri)
            {
            }

        }


        private static TagAction TA_IGNORABLE_ELEMENT;/* = new TagAction() {
		void beforeStart(Implementation instance, String localName) {
			instance.inIgnorableElement++;
		}

		void afterEnd(Implementation instance, String localName) {
			instance.inIgnorableElement--;
		}
	};*/

        //private static Dictionary<String, TagAction> TAG_ACTIONS = new Dictionary<String, TagAction>();
        //static Implementation(){
        //    TAG_ACTIONS.put("STYLE", TA_IGNORABLE_ELEMENT);
        //    TAG_ACTIONS.put("SCRIPT", TA_IGNORABLE_ELEMENT);
        //    TAG_ACTIONS.put("OPTION", TA_IGNORABLE_ELEMENT);
        //    TAG_ACTIONS.put("NOSCRIPT", TA_IGNORABLE_ELEMENT);
        //    TAG_ACTIONS.put("OBJECT", TA_IGNORABLE_ELEMENT);
        //    TAG_ACTIONS.put("EMBED", TA_IGNORABLE_ELEMENT);
        //    TAG_ACTIONS.put("APPLET", TA_IGNORABLE_ELEMENT);
        //    TAG_ACTIONS.put("LINK", TA_IGNORABLE_ELEMENT);

        //    TAG_ACTIONS.put("HEAD", TA_IGNORABLE_ELEMENT);
        //}

        private abstract class TagAction : NBoilerpipe.Parser.TagAction
        {

            public TagAction()
            {

            }
            public bool Start(NBoilerpipeContentHandler instance, string localName, HtmlAttributeCollection atts) { return false; }

            /// <exception cref="Sharpen.SAXException"></exception>
            public bool End(NBoilerpipeContentHandler instance, string localName) { return false; }

            public bool ChangesTagLevel() { return false; }

        }
    }

}
