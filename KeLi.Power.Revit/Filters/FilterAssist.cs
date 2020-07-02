/*
 * MIT License
 *
 * Copyright(c) 2019 KeLi
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

/*
             ,---------------------------------------------------,              ,---------,
        ,----------------------------------------------------------,          ,"        ,"|
      ,"                                                         ,"|        ,"        ,"  |
     +----------------------------------------------------------+  |      ,"        ,"    |
     |  .----------------------------------------------------.  |  |     +---------+      |
     |  | C:\>FILE -INFO                                     |  |  |     | -==----'|      |
     |  |                                                    |  |  |     |         |      |
     |  |                                                    |  |  |/----|`---=    |      |
     |  |              Author: KeLi                          |  |  |     |         |      |
     |  |              Email: kelistudy@163.com              |  |  |     |         |      |
     |  |              Creation Time: 10/30/2019 07:08:41 PM |  |  |     |         |      |
     |  | C:\>_                                              |  |  |     | -==----'|      |
     |  |                                                    |  |  |   ,/|==== ooo |      ;
     |  |                                                    |  |  |  // |(((( [66]|    ,"
     |  `----------------------------------------------------'  |," .;'| |((((     |  ,"
     +----------------------------------------------------------+  ;;  | |         |,"
        /_)_________________________________________________(_/  //'   | +---------+
           ___________________________/___  `,
          /  oooooooooooooooo  .o.  oooo /,   \,"-----------
         / ==ooooooooooooooo==.o.  ooo= //   ,`\--{)B     ,"
        /_==__==========__==_ooo__ooo=_/'   /___________,"
*/

using System;
using System.Collections.Generic;
using System.Linq;

using Autodesk.Revit.DB;

namespace KeLi.Power.Revit.Filters
{
    /// <summary>
    ///     Filter assist.
    /// </summary>
    public static class FilterAssist
    {
        /// <summary>
        ///     Checkouts all elements in the document.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="type"></param>
        /// <param name="viewId"></param>
        /// <returns></returns>
        public static List<Element> Checkout(this Document doc, FilterType type, ElementId viewId = null)
        {
            if (doc is null)
                throw new ArgumentNullException(nameof(doc));

            var filter = new FilteredElementCollector(doc);

            if (viewId != null)
                filter = new FilteredElementCollector(doc, viewId);

            switch (type)
            {
                case FilterType.Instance:
                    return filter.WhereElementIsNotElementType().ToList();

                case FilterType.Type:
                    return filter.WhereElementIsElementType().ToList();

                case FilterType.All:
                    return filter.ToList();

                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        /// <summary>
        ///     Gets the specified type of the element list.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="viewId"></param>
        /// <returns></returns>
        public static List<T> GetTypeList<T>(this Document doc, ElementId viewId = null) where T : Element
        {
            if (doc is null)
                throw new ArgumentNullException(nameof(doc));

            var filter = new FilteredElementCollector(doc);

            if (viewId != null)
                filter = new FilteredElementCollector(doc, viewId);

            return filter.OfClass(typeof(T)).WhereElementIsElementType().Cast<T>().ToList();
        }

        /// <summary>
        ///     Gets the specified type and category of the element list.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="category"></param>
        /// <param name="viewId"></param>
        /// <returns></returns>
        public static List<T> GetTypeList<T>(this Document doc, BuiltInCategory category, ElementId viewId = null) where T : Element
        {
            if (doc is null)
                throw new ArgumentNullException(nameof(doc));

            var filter = new FilteredElementCollector(doc);

            if (viewId != null)
                filter = new FilteredElementCollector(doc, viewId);

            return filter.OfCategory(category).WhereElementIsElementType().Cast<T>().ToList();
        }

        /// <summary>
        ///     Gets the specified type of the element list.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="viewId"></param>
        /// <returns></returns>
        public static List<T> GetInstanceList<T>(this Document doc, ElementId viewId = null) where T : Element
        {
            if (doc is null)
                throw new ArgumentNullException(nameof(doc));

            var filter = new FilteredElementCollector(doc);

            if (viewId != null)
                filter = new FilteredElementCollector(doc, viewId);

            return filter.OfClass(typeof(T)).WhereElementIsNotElementType().Cast<T>().ToList();
        }

        /// <summary>
        ///     Gets the specified type and category of the element list.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="category"></param>
        /// <param name="viewId"></param>
        /// <returns></returns>
        public static List<T> GetInstanceList<T>(this Document doc, BuiltInCategory category, ElementId viewId = null) where T : Element
        {
            if (doc is null)
                throw new ArgumentNullException(nameof(doc));

            var filter = new FilteredElementCollector(doc);

            if (viewId != null)
                filter = new FilteredElementCollector(doc, viewId);

            return filter.OfCategory(category).WhereElementIsNotElementType().Cast<T>().ToList();
        }
    }
}