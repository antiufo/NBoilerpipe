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
     * Marks trailing headlines ({@link TextBlock}s that have the label {@link DefaultLabels#HEADING})
     * as boilerplate. Trailing means they are marked content and are below any other content block.
     * 
     * @author Christian Kohlschütter
     */
    public class TrailingHeadlineToBoilerplateFilter : BoilerpipeFilter
    {
        public static readonly TrailingHeadlineToBoilerplateFilter INSTANCE = new TrailingHeadlineToBoilerplateFilter();

        /**
         * Returns the singleton instance for ExpandTitleToContentFilter.
         */
        public static TrailingHeadlineToBoilerplateFilter getInstance()
        {
            return INSTANCE;
        }

        public bool Process(TextDocument doc)
        {
            var changes = false;

            var list = doc.GetTextBlocks();


            for (int i = list.Count - 1; i >= 0; i--)
            {
                var tb = list[i];
                if (tb.IsContent())
                {
                    if (tb.HasLabel(DefaultLabels.HEADING))
                    {
                        tb.SetIsContent(false);
                        changes = true;
                    }
                    else
                    {
                        break;
                    }
                }
            }


            return changes;
        }

    }

}
