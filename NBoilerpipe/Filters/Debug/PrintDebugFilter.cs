using NBoilerpipe.Document;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBoilerpipe.Filters.Debug
{
    /**
   * boilerpipe
   *
   * Copyright (c) 2012 Christian Kohlschütter
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
     * Prints debug information about the current state of the TextDocument. (=
     * calls {@link TextDocument#debugString()}.
     * 
     * @author Christian Kohlschütter
     */
    public class PrintDebugFilter : BoilerpipeFilter
    {
        /**
         * Returns the default instance for {@link PrintDebugFilter},
         * which dumps debug information to <code>System.out</code>
         */
        public static readonly PrintDebugFilter INSTANCE = new PrintDebugFilter();

        /**
         * Returns the default instance for {@link PrintDebugFilter},
         * which dumps debug information to <code>System.out</code>
         */
        public static PrintDebugFilter getInstance()
        {
            return INSTANCE;
        }



        public bool Process(TextDocument doc)
        {
            Console.WriteLine(doc.DebugString());
            return false;
        }
    }

}
