using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using Shaman.Dom;
using NBoilerpipe.Parser;
using NBoilerpipe.Document;
using NBoilerpipe.Labels;
using NBoilerpipe.Util;
using Sharpen;
using System.Globalization;
using Shaman.Runtime;

namespace NBoilerpipe
{

    public class NBoilerpipeContentHandler : IContentHandler
    {
        enum Event
        {
            START_TAG,
            END_TAG,
            CHARACTERS,
            WHITESPACE
        }

        readonly IDictionary<string, TagAction> tagActions = DefaultTagActionMap.INSTANCE;
        string title = null;

        internal static readonly ValueString ANCHOR_TEXT_START = "\ue00a";
        internal static readonly ValueString ANCHOR_TEXT_END = "\ue00b";
        internal List<ValueString> tokenBuilder = new List<ValueString>();
        internal StringBuilder textBuilder = new StringBuilder();
        internal int inBody = 0;
        internal int inAnchor = 0;
        internal int inIgnorableElement = 0;
        internal int tagLevel = 0;
        internal int blockTagLevel = -1;
        internal bool sbLastWasWhitespace = false;

        int textElementIdx = 0;
        readonly IList<TextBlock> textBlocks = new AList<TextBlock>();
        string lastStartTag = null;
        string lastEndTag = null;
        NBoilerpipeContentHandler.Event lastEvent;
        int offsetBlocks = 0;
        BitSet currentContainedTextElements = new BitSet();
        bool flush = false;
        bool inAnchorText = false;
        internal List<List<LabelAction>> labelStacks = new List<List<LabelAction>>();
        internal List<int?> fontSizeStack = new List<int?>();

        //static readonly Sharpen.Pattern PAT_VALID_WORD_CHARACTER = Sharpen.Pattern
        //    .Compile("[\\p{L}\\p{Nd}\\p{Nl}\\p{No}]");


        public void StartElement(HtmlNode node)
        {
            nodeStack.Push(node);
            labelStacks.AddItem(null);
            TagAction ta = tagActions.Get(node.TagName);
            if (ta != null)
            {
                if (ta.ChangesTagLevel())
                {
                    tagLevel++;
                }
                flush = ta.Start(this, node.TagName, node.Attributes) | flush;
            }
            else
            {
                tagLevel++;
                flush = true;
            }
            lastEvent = NBoilerpipeContentHandler.Event.START_TAG;
            lastStartTag = node.TagName;
        }

        public void EndElement(HtmlNode node)
        {
            TagAction ta = tagActions.Get(node.TagName);
            if (ta != null)
            {
                flush = ta.End(this, node.TagName) | flush;
            }
            else
            {
                flush = true;
            }
            if (ta == null || ta.ChangesTagLevel())
            {
                tagLevel--;
            }
            if (flush)
            {
                FlushBlock();
            }
            lastEvent = NBoilerpipeContentHandler.Event.END_TAG;
            lastEndTag = node.TagName;
            labelStacks.RemoveLast();
            nodeStack.Pop();
        }

        public void HandleText(HtmlTextNode node)
        {
            if (IsTag(node.Text))
                node.Text = "";

            var ch = node.Text;
            int start = 0;
            int length = ch.Length;

            textElementIdx++;

            if (flush)
            {
                FlushBlock();
                flush = false;
            }
            if (inIgnorableElement != 0)
            {
                return;
            }

            char c;
            bool startWhitespace = false;
            bool endWhitespace = false;
            if (length == 0)
            {
                return;
            }
            int end = start + length;
            //for (int i = start; i < end; i++)
            //{
            //    if (IsWhiteSpace(ch[i]))
            //    {
            //        ch[i] = ' ';
            //    }
            //}
            while (start < end)
            {
                c = ch[start];
                if (char.IsWhiteSpace(c))
                {
                    startWhitespace = true;
                    start++;
                    length--;
                }
                else
                {
                    break;
                }
            }
            while (length > 0)
            {
                c = ch[start + length - 1];
                if (char.IsWhiteSpace(c))
                {
                    endWhitespace = true;
                    length--;
                }
                else
                {
                    break;
                }
            }
            if (length == 0)
            {
                if (startWhitespace || endWhitespace)
                {
                    if (!sbLastWasWhitespace)
                    {
                        textBuilder.Append(' ');
                    }
                    sbLastWasWhitespace = true;
                }
                else
                {
                    sbLastWasWhitespace = false;
                }
                lastEvent = NBoilerpipeContentHandler.Event.WHITESPACE;
                return;
            }
            if (startWhitespace)
            {
                if (!sbLastWasWhitespace)
                {
                    textBuilder.Append(' ');
                }
            }
            if (blockTagLevel == -1)
            {
                blockTagLevel = tagLevel;
            }
            if (blockFirstNode == null) blockFirstNode = nodeStack.Peek();

            textBuilder.Append(ch, start, length);
            tokenBuilder.Add(ch.AsValueString().Substring(start, length));
            if (endWhitespace)
            {
                textBuilder.Append(' ');
            }
            sbLastWasWhitespace = endWhitespace;
            lastEvent = NBoilerpipeContentHandler.Event.CHARACTERS;
            currentContainedTextElements.Add(textElementIdx);
        }

        bool IsTag(String text)
        {
            return (Regex.IsMatch(text, "</?[a-z][a-z0-9]*[^<>]*>", RegexOptions.IgnoreCase));
        }

        bool IsWhiteSpace(char ch)
        {
            if (ch == '\u00A0') return false;
            return char.IsWhiteSpace(ch);
        }

        private HtmlNode blockFirstNode;

        public void FlushBlock()
        {
            var firstNode = this.blockFirstNode;
            this.blockFirstNode = null;
            if (inBody == 0)
            {
                if (inBody == 0 && "TITLE".Equals(lastStartTag, StringComparison.OrdinalIgnoreCase))
                    SetTitle(tokenBuilder.ToString().Trim());
                textBuilder.Length = 0;
                tokenBuilder.Clear();
                return;
            }

            int length = tokenBuilder.Count;
            if (length == 0)
            {
                return;
            }
            else if (length == 1)
            {
                if (sbLastWasWhitespace)
                {
                    textBuilder.Length = 0;
                    tokenBuilder.Clear();
                    return;
                }
            }

            
            int numWords = 0;
            int numLinkedWords = 0;
            int numWrappedLines = 0;
            int currentLineLength = -1; // don't count the first space
            int maxLineLength = 80;
            int numTokens = 0;
            int numWordsCurrentLine = 0;

            foreach (ValueString token in tokenBuilder)
            {
                if (token == ANCHOR_TEXT_START)
                {
                    inAnchorText = true;
                }
                else
                {
                    if (token == ANCHOR_TEXT_END)
                    {
                        inAnchorText = false;
                    }
                    else
                    {
                        if (IsWord(token))
                        {
                            numTokens++;
                            numWords++;
                            numWordsCurrentLine++;

                            if (inAnchorText)
                            {
                                numLinkedWords++;
                            }
                            int tokenLength = token.Length;
                            currentLineLength += tokenLength + 1;
                            if (currentLineLength > maxLineLength)
                            {
                                numWrappedLines++;
                                currentLineLength = tokenLength;
                                numWordsCurrentLine = 1;
                            }
                        }
                        else
                        {
                            numTokens++;
                        }
                    }
                }
            }
            if (numTokens == 0)
            {
                return;
            }
            int numWordsInWrappedLines;
            if (numWrappedLines == 0)
            {
                numWordsInWrappedLines = numWords;
                numWrappedLines = 1;
            }
            else
            {
                numWordsInWrappedLines = numWords - numWordsCurrentLine;
            }
            TextBlock tb = new TextBlock(textBuilder.ToString().Trim(), currentContainedTextElements
                , numWords, numLinkedWords, numWordsInWrappedLines, numWrappedLines, offsetBlocks, GetCommonAncestor(firstNode, nodeStack.Peek())
                );
            currentContainedTextElements = new BitSet();
            offsetBlocks++;
            textBuilder.Length = 0;
            tokenBuilder.Clear();
            tb.SetTagLevel(blockTagLevel);
            AddTextBlock(tb);
            blockTagLevel = -1;
        }

        public static IEnumerable<ValueString> GetWords(string str)
        {

            var currentWordStart = 0;
            var currentWordLength = 0;
            bool prevCharWasSep = true;
            for (int i = 0; i < str.Length; i++)
            {
                bool curCharIsSep;
                char c = str[i];

                if (char.IsLetterOrDigit(c) || CharUnicodeInfo.GetUnicodeCategory(c) == UnicodeCategory.NonSpacingMark)

                {
                    if (prevCharWasSep)
                    {
                        currentWordLength = 0;
                        prevCharWasSep = false;
                    }
                    if (currentWordLength == 0) currentWordStart = i;
                    currentWordLength++;

                    curCharIsSep = CharUnicodeInfo.GetUnicodeCategory(c) == UnicodeCategory.OtherLetter;
                }
                else
                {
                    curCharIsSep = true;
                }
                if (curCharIsSep)
                {
                    if (currentWordLength != 0)
                    {
                        yield return new ValueString(str, currentWordStart, currentWordLength);
                        currentWordLength = 0;
                    }
                    prevCharWasSep = true;
                }
            }
            if (currentWordLength != 0)
            {
                yield return new ValueString(str, currentWordStart, currentWordLength);
            }

        }


        public static HtmlNode GetCommonAncestor(HtmlNode a, HtmlNode b)
        {
            if (a == null) return b;
            if (b == null) return a;
            if (a.ParentNode == b) return b;
            if (a == b) return a;
            var ancestors = a.AncestorsAndSelf().ToList();
            return b.AncestorsAndSelf().FirstOrDefault(x => ancestors.Contains(x));
        }

        private Stack<HtmlNode> nodeStack = new Stack<HtmlNode>();


        static bool IsWord(ValueString token)
        {
            ////   L, Nd, Nl, No
            for (int i = 0; i < token.Length; i++)
            {
                var k = token[i];
                
                if (char.IsLetterOrDigit(k)) return true;
                if (k < 256) continue;
                var m = CharUnicodeInfo.GetUnicodeCategory(k);
                

                if (m == UnicodeCategory.OtherNumber || m == UnicodeCategory.LetterNumber) return true;
            }
            return false;
        }

        public TextDocument ToTextDocument()
        {
            return new TextDocument(title, textBlocks);
        }

        protected void AddTextBlock(TextBlock tb)
        {
            foreach (int l in fontSizeStack)
            {
                tb.AddLabels("font-" + l);
                break;
            }

            foreach (List<LabelAction> labels in labelStacks)
            {
                if (labels != null)
                {
                    foreach (LabelAction label in labels)
                    {
                        label.AddTo(tb);
                    }
                }
            }
            textBlocks.Add(tb);
        }


        public void AddWhitespaceIfNecessary()
        {
            if (!sbLastWasWhitespace)
            {
                textBuilder.Append(' ');
                sbLastWasWhitespace = true;
            }
        }

        public void AddLabelAction(LabelAction la)
        {
            List<LabelAction> labelStack = labelStacks.GetLast();
            if (labelStack == null)
            {
                labelStack = new List<LabelAction>();
                labelStacks.RemoveLast();
                labelStacks.AddItem(labelStack);
            }
            labelStack.AddItem(la);
        }

        public void SetTitle(string s)
        {
            if (s == null || s.Length == 0)
            {
                return;
            }
            title = s;
        }


    }
}
