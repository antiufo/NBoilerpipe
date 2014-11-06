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
     * Marks nested list-item blocks after the end of the main content.
     * 
     * @author Christian Kohlschütter
     */
    public class ListAtEndFilter : BoilerpipeFilter
    {
        public static readonly ListAtEndFilter INSTANCE = new ListAtEndFilter();

        private ListAtEndFilter()
        {
        }

        public bool Process(TextDocument doc)
        {

            var changes = false;

            int tagLevel = int.MaxValue;
            foreach (var tb in doc.GetTextBlocks())
            {
                if (tb.IsContent()
                        && tb.HasLabel(DefaultLabels.VERY_LIKELY_CONTENT))
                {
                    tagLevel = tb.GetTagLevel();
                }
                else
                {
                    if (tb.GetTagLevel() > tagLevel
                            && tb.HasLabel(DefaultLabels.MIGHT_BE_CONTENT)
                            && tb.HasLabel(DefaultLabels.LI)
                            && tb.GetLinkDensity() == 0
                            )
                    {
                        tb.SetIsContent(true);
                        changes = true;
                    }
                    else
                    {
                        tagLevel = int.MaxValue;
                    }
                }
            }

            return changes;

        }
    }

}
