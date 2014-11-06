using NBoilerpipe.Document;
using NBoilerpipe.Labels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBoilerpipe.Filters.Heuristics
{
    /**
     * boilerpipe
     *
     * Copyright (c) 2009 Christian Kohlschütter
     *
     * The author licenses this file to You under the Apache License, Version 2.0
     * (the "License"); you may not use this file except in compliance with
     * the License.  You may obtain a copy of the License at
     *
     *     http://www.apache.org/licenses/LICENSE-2.0
     *
     * Unless required by applicable law or agreed to in writing, software
     * distributed under the License is distributed on an "AS IS" BASIS,
     * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
     * See the License for the specific language governing permissions and
     * limitations under the License.
     */

    /**
     * Marks all blocks as content that:
     * <ol>
     * <li>are on the same tag-level as very likely main content (usually the level of the largest block)</li>
     * <li>have a significant number of words, currently: at least 100</li>  
     * </ol>
     * 
     * @author Christian Kohlschütter
     */
    public class LargeBlockSameTagLevelToContentFilter : BoilerpipeFilter
    {
        public static readonly LargeBlockSameTagLevelToContentFilter INSTANCE = new LargeBlockSameTagLevelToContentFilter();
        private LargeBlockSameTagLevelToContentFilter()
        {
        }

        public bool Process(TextDocument doc)
        {

            var changes = false;

            int tagLevel = -1;
            foreach (var tb in doc.GetTextBlocks())
            {
                if (tb.IsContent() && tb.HasLabel(DefaultLabels.VERY_LIKELY_CONTENT))
                {
                    tagLevel = tb.GetTagLevel();
                    break;
                }
            }

            if (tagLevel == -1)
            {
                return false;
            }

            foreach (var tb in doc.GetTextBlocks())
            {
                if (!tb.IsContent())
                {

                    if (tb.GetNumWords() >= 100 && tb.GetTagLevel() == tagLevel)
                    {
                        tb.SetIsContent(true);
                        changes = true;
                    }
                }
            }

            return changes;

        }
    }

}
