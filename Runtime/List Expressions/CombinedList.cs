/*
 *  Copyright 2026 KamKamKee <kamkamkeedev@gmail.com>
 *
 *  This Source Code Form is subject to the terms of the Mozilla Public
 *  License, v. 2.0. If a copy of the MPL was not distributed with this
 *  file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *  
 */

using System;
using System.Collections.Generic;

namespace EExtensions
{
    namespace CombinedList
    {
        public class CombinedList<T1, T2>
        {
            private List<T1> list1;
            private List<T2> list2;

            public CombinedList(List<T1> list1, List<T2> list2)
            {
                this.list1 = list1;
                this.list2 = list2;
            }

            public List<TOut> Choose<TOut>(Func<List<T1>, List<T2>, List<TOut>> func)
            {
                return func(list1, list2);
            }
        }
    }
}
