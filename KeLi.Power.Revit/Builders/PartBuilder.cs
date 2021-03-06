﻿/*
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
     |  |              Creation Time: 04/29/2020 03:48:41 PM |  |  |     |         |      |
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

using KeLi.Power.Revit.Widgets;

using static KeLi.Power.Revit.Builders.TextureAngle;

namespace KeLi.Power.Revit.Builders
{
    /// <summary>
    ///     Part builder.
    /// </summary>
    public static class PartBuilder
    {
        /// <summary>
        ///     Divide part list for element.
        /// </summary>
        /// <param name="elm"></param>
        /// <param name="origin"></param>
        /// <param name="baseX"></param>
        /// <param name="step"></param>
        /// <param name="initAngle"></param>
        /// <param name="radius"></param>
        public static void DividePartList(this Element elm, XYZ origin, XYZ baseX, double[] step, TextureAngle initAngle, double radius = 5000)
        {
            if (elm is null)
                throw new ArgumentNullException(nameof(elm));

            if (origin is null)
                throw new ArgumentNullException(nameof(origin));

            if (baseX is null)
                throw new ArgumentNullException(nameof(baseX));

            if (step[0] < 0)
                throw new ArgumentException(nameof(step));

            if (step[1] < 0)
                throw new ArgumentException(nameof(step));

            if (radius < 0)
                throw new ArgumentException(nameof(radius));

            if (initAngle == Rotation90 || initAngle == Rotation270)
                step = new[] { step[1], step[0] };

            var plane = Plane.CreateByNormalAndOrigin(XYZ.BasisZ, origin);
            var baseY = plane.Normal.CrossProduct(baseX);
            var lines = new List<Curve>();

            var xAxis = Line.CreateBound(origin - radius * baseX, origin + radius * baseX);
            var yAxis = Line.CreateBound(origin - radius * baseY, origin + radius * baseY);

            lines.Add(xAxis);
            lines.Add(yAxis);

            var yp0 = yAxis.GetEndPoint(0);
            var yp1 = yAxis.GetEndPoint(1);

            var xNum = Convert.ToInt32(Math.Ceiling(radius / step[0]));
            var yNum = Convert.ToInt32(Math.Ceiling(radius / step[1]));

            // Draws lines on x direction 
            for (var i = 0; i < xNum; i++)
            {
                var offset = (i + 1) * step[0] * baseX;

                // On Right.
                lines.Add(Line.CreateBound(yp0 + offset, yp1 + offset));

                // On Left.
                lines.Add(Line.CreateBound(yp0 - offset, yp1 - offset));
            }

            var xp0 = xAxis.GetEndPoint(0);
            var xp1 = xAxis.GetEndPoint(1);

            // Draws lines on y direction 
            for (var i = 0; i < yNum; i++)
            {
                var offset = (i + 1) * step[1] * baseY;

                // Above.
                lines.Add(Line.CreateBound(xp0 + offset, xp1 + offset));

                // Below.
                lines.Add(Line.CreateBound(xp0 - offset, xp1 - offset));
            }

            var doc = elm.Document;
            var sketchPlane = SketchPlane.Create(doc, plane);
            var ids = PartUtils.GetAssociatedParts(doc, elm.Id, true, false).ToList();

            if (ids.Count > 0)
            {
                var partMaker = PartUtils.GetAssociatedPartMaker(doc, ids[0]);

                doc.Delete(partMaker.Id);
            }

            ids = PartUtils.GetAssociatedParts(doc, elm.Id, true, true).ToList();

            if (ids.Count <= 0)
            {
                PartUtils.CreateParts(doc, new List<ElementId> { elm.Id });

                doc.Regenerate();

                ids = PartUtils.GetAssociatedParts(doc, elm.Id, true, true).ToList();
            }

            if (ids.Count == 1)
                PartUtils.DivideParts(doc, ids, new List<ElementId>(), lines, sketchPlane.Id);
        }

        /// <summary>
        ///     Divides part list for wall.
        /// </summary>
        /// <param name="wall"></param>
        /// <param name="step"></param>
        /// <param name="materialNames"></param>
        /// <param name="lineHeight"></param>
        /// <param name="isHorizontal"></param>
        /// <param name="eps"></param>
        public static void DividePartList(this Wall wall, double[] step, string[] materialNames, double lineHeight, bool isHorizontal = true, double eps = 1e-3)
        {
            if (wall is null)
                throw new ArgumentNullException(nameof(wall));

            if (step is null)
                throw new ArgumentNullException(nameof(step));

            if (materialNames is null)
                throw new ArgumentNullException(nameof(materialNames));

            var line = (wall.Location as LocationCurve)?.Curve as Line;
            var lines = new List<Curve>();

            if (isHorizontal)
            {
                var p0 = line.GetEndPoint(0);
                var p1 = line.GetEndPoint(1);

                lines.Add(Line.CreateBound(p0, p1));

                for (var i = 0; i < step.Length; i++)
                {
                    var sum = step.Take(i + 1).Sum();
                    var tmpP0 = new XYZ(p0.X, p0.Y, p0.Z + sum);
                    var tmpP1 = new XYZ(p1.X, p1.Y, p1.Z + sum);
                    var tmpLine = Line.CreateBound(tmpP0, tmpP1);

                    lines.Add(tmpLine);
                }
            }

            else
            {
                var p0 = line.GetEndPoint(0);
                var p1 = new XYZ(p0.X, p0.Y, lineHeight);

                lines.Add(Line.CreateBound(p0, p1));

                for (var i = 0; i < step.Length; i++)
                {
                    var sum = step.Take(i + 1).Sum();

                    var tmpP0 = p0 + sum * line.Direction;
                    var tmpP1 = p1 + sum * line.Direction;
                    var tmpLine = Line.CreateBound(tmpP0, tmpP1);

                    lines.Add(tmpLine);
                }
            }

            var opt = new Options { ComputeReferences = true, DetailLevel = ViewDetailLevel.Coarse };
            var ge = wall.get_Geometry(opt);
            var solid = ge.FirstOrDefault(f => f is Solid) as Solid;

            if (solid != null)
            {
                var faces = solid.Faces.Cast<Face>().Where(w => w is PlanarFace).Cast<PlanarFace>().ToList();
                var face = faces.FirstOrDefault(f => f.FaceNormal.AngleTo(wall.Orientation) < eps);
                var doc = wall.Document;
                var plane = SketchPlane.Create(doc, face.Reference);
                var ids = PartUtils.GetAssociatedParts(doc, wall.Id, true, false).ToList();

                if (ids.Count > 0)
                {
                    var partMaker = PartUtils.GetAssociatedPartMaker(doc, ids[0]);

                    doc.Delete(partMaker.Id);
                }

                ids = PartUtils.GetAssociatedParts(doc, wall.Id, true, true).ToList();

                if (ids.Count <= 0)
                {
                    PartUtils.CreateParts(doc, new List<ElementId> { wall.Id });

                    doc.Regenerate();
                    ids = PartUtils.GetAssociatedParts(doc, wall.Id, true, true).ToList();
                }

                if (ids.Count == 1)
                    PartUtils.DivideParts(doc, ids, new List<ElementId>(), lines, plane.Id);

                var materials = doc.GetInstanceList<Material>();

                for (var i = 0; i < ids.Count; i++)
                {
                    var part = doc.GetElement(ids[i]) as Part;

                    if (part != null)
                    {
                        var parm = part.get_Parameter(BuiltInParameter.DPART_MATERIAL_BY_ORIGINAL);

                        parm.Set(0);
                    }

                    var materialParm = part.get_Parameter(BuiltInParameter.DPART_MATERIAL_ID_PARAM);
                    var materialId = materials.FirstOrDefault(f => f.Name == materialNames[i])?.Id;

                    materialParm.Set(materialId);
                }
            }
        }
    }

    /// <summary>
    ///     Texture angle.
    /// </summary>
    public enum TextureAngle
    {
        /// <summary>
        ///     Initializes angle.
        /// </summary>
        Rotation0 = 0,

        /// <summary>
        ///     Rotates 90 angle.
        /// </summary>
        Rotation90 = 90,

        /// <summary>
        ///     Rotates 180 angle.
        /// </summary>
        Rotation180 = 180,

        /// <summary>
        ///     Rotates 270 angle.
        /// </summary>
        Rotation270 = 270
    }
}