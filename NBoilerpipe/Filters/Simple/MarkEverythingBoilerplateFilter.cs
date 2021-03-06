﻿using NBoilerpipe.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBoilerpipe.Filters.Simple
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
     * Marks all blocks as boilerplate.
     * 
     * @author Christian Kohlschütter
     */
    public class MarkEverythingBoilerplateFilter : BoilerpipeFilter
    {
        public static readonly MarkEverythingBoilerplateFilter INSTANCE = new MarkEverythingBoilerplateFilter();
        private MarkEverythingBoilerplateFilter()
        {
        }

        public bool Process(TextDocument doc)
        {

            var changes = false;

            foreach (TextBlock tb in doc.GetTextBlocks())
            {
                if (tb.IsContent())
                {
                    tb.SetIsContent(false);
                    changes = true;
                }
            }

            return changes;

        }
    }

}
