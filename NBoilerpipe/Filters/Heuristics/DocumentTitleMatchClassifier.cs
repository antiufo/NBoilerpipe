/*
 * This code is derived from boilerpipe
 * 
 */

using System.Collections.Generic;
using NBoilerpipe;
using NBoilerpipe.Document;
using NBoilerpipe.Labels;
using Sharpen;
using System.Text.RegularExpressions;

namespace NBoilerpipe.Filters.Heuristics
{
    /// <summary>
    /// Marks
    /// <see cref="NBoilerpipe.Document.TextBlock">NBoilerpipe.Document.TextBlock</see>
    /// s which contain parts of the HTML
    /// <code>&lt;TITLE&gt;</code> tag, using some heuristics which are quite
    /// specific to the news domain.
    /// </summary>
    /// <author>Christian Kohlschvºtter</author>
    public sealed class DocumentTitleMatchClassifier : BoilerpipeFilter
    {
        private readonly ICollection<string> potentialTitles;

        public DocumentTitleMatchClassifier(string title)
        {
            if (title == null)
            {
                this.potentialTitles = null;
            }
            else
            {
                title = title.Replace('\u00a0', ' ');
                title = title.Replace("'", string.Empty);
                title = title.Trim().ToLowerInvariant();
                if (title.Length == 0)
                {
                    this.potentialTitles = null;
                }
                else
                {
                    this.potentialTitles = new HashSet<string>();
                    potentialTitles.AddItem(title);
                    string p;
                    p = GetLongestPart(title, "[ ]*[\\|»|-][ ]*");
                    if (p != null)
                    {
                        potentialTitles.AddItem(p);
                    }
                    p = GetLongestPart(title, "[ ]*[\\|»|:][ ]*");
                    if (p != null)
                    {
                        potentialTitles.AddItem(p);
                    }
                    p = GetLongestPart(title, "[ ]*[\\|»|:\\(\\)][ ]*");
                    if (p != null)
                    {
                        potentialTitles.AddItem(p);
                    }
                    p = GetLongestPart(title, "[ ]*[\\|»|:\\(\\)\\-][ ]*");
                    if (p != null)
                    {
                        potentialTitles.AddItem(p);
                    }
                    p = GetLongestPart(title, "[ ]*[\\|»|,|:\\(\\)\\-][ ]*");
                    if (p != null)
                    {
                        potentialTitles.AddItem(p);
                    }
                    p = GetLongestPart(title, "[ ]*[\\|»|,|:\\(\\)\\-\u00a0][ ]*");
                    if (p != null)
                    {
                        potentialTitles.AddItem(p);
                    }

                    addPotentialTitles(potentialTitles, title, "[ ]+[\\|][ ]+", 4);
                    addPotentialTitles(potentialTitles, title, "[ ]+[\\-][ ]+", 4);

                    potentialTitles.Add(Regex.Replace(title, " - [^\\-]+$", ""));
                    potentialTitles.Add(Regex.Replace(title, "^[^\\-]+ - ", ""));
                }
            }
        }

        public ICollection<string> GetPotentialTitles()
        {
            return potentialTitles;
        }
        private void addPotentialTitles(ICollection<string> potentialTitles, string title, string pattern, int minWords)
        {
            string[] parts = title.Split(pattern);
            if (parts.Length == 1)
            {
                return;
            }
            for (int i = 0; i < parts.Length; i++)
            {
                var p = parts[i];
                if (p.Contains(".com"))
                {
                    continue;
                }
                int numWords = p.Split("[\b ]+").Length;
                if (numWords >= minWords)
                {
                    potentialTitles.Add(p);
                }
            }
        }

        private string GetLongestPart(string title, string pattern)
        {
            string[] parts = title.Split(pattern);
            if (parts.Length == 1)
            {
                return null;
            }
            int longestNumWords = 0;
            string longestPart = string.Empty;
            for (int i = 0; i < parts.Length; i++)
            {
                string p = parts[i];
                if (p.Contains(".com"))
                {
                    continue;
                }
                int numWords = p.Split("[\b ]+").Length;
                if (numWords > longestNumWords || p.Length > longestPart.Length)
                {
                    longestNumWords = numWords;
                    longestPart = p;
                }
            }
            if (longestPart.Length == 0)
            {
                return null;
            }
            else
            {
                return longestPart.Trim();
            }
        }


        private static Regex PAT_REMOVE_CHARACTERS = new Regex("[\\?\\!\\.\\-\\:]+");


        /// <exception cref="NBoilerpipe.BoilerpipeProcessingException"></exception>
        public bool Process(TextDocument doc)
        {
            if (potentialTitles == null)
            {
                return false;
            }
            bool changes = false;

            foreach (TextBlock tb in doc.GetTextBlocks())
            {
                var text = tb.GetText();

                text = text.Replace('\u00a0', ' ');
                text = text.Trim();

                if (potentialTitles.Contains(text))
                {
                    tb.AddLabel(DefaultLabels.TITLE);
                    changes = true;
                    break;
                }

                text = PAT_REMOVE_CHARACTERS.Replace(text, string.Empty).Trim();
                if (potentialTitles.Contains(text))
                {
                    tb.AddLabel(DefaultLabels.TITLE);
                    changes = true;
                    break;
                }
            }
            return changes;
        }
    }
}
