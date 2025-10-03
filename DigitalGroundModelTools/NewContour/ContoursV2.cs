using System.Globalization;
using System.Text;
using TriangleNet.Data;
using static HeepWare.Contouring.BoundBox3D;

namespace HeepWare.Contouring
{
    public class ContoursV2
    {
        private bool Debug = false;
        // Major and minor contour intervals set to default values
        internal double minorInterval = 5;
        internal double majorInterval = 10;

        // Reference to the triangle list pass to the constructor         
        private int NumberOfContours = 0;

        // Contour high and low elevations
        internal double lowContourElevation = 0.0;
        internal double highContourElevation = 0.0;
        //// internal table relating contour elevation to is contour object
        //internal ArrayList tableList = new ArrayList(10);
        internal Dictionary<ContourID, ContourV2> contourDictionary = new Dictionary<ContourID, ContourV2>();

        internal List<LinearSegmentV2> allElevSegments = new List<LinearSegmentV2>();
        internal List<LineString> allLineStrings = new List<LineString>();

        //New triangle  data
        internal ICollection<Triangle> triangles = null;
        //Bounding Box for triangle data which will be contours bbox also
        internal BoundingBox3D boundingBox;

        // Debug variables below
        //internal LogFile logFile = null;
        internal bool debug = false;
        internal bool logFlag = false;

        internal static int currentcolorindex = 0;
        internal static int GetColorIndex()
        {
            if (currentcolorindex > 254) currentcolorindex = 0;

            return currentcolorindex++;
        }

        internal string Elevation2String(double value)
        {
            return String.Format(CultureInfo.InvariantCulture, "{0:00.00}", value);
        }

        // Get and Set Minor contour intervals
        public double getMinorInterval()
        {
            return minorInterval;
        }

        public double setMinorInterval(double newInterval)
        {
            minorInterval = newInterval;
            return minorInterval;
        }

        public List<LineString> GetAllLineStrings()
        {
            return allLineStrings;
        }

        public List<ContourID> GetContourkeys()
        {
            return contourDictionary.Keys.ToList();
        }

        public ContourV2 GetContour(string key)
        {
            ContourID contourID = new ContourID(key);
            return contourDictionary[contourID];
        }

        public ContourV2 GetContour(ContourID key)
        {
            return contourDictionary[key];
        }

        public ContourV2 GetContour4Elevation(double elev)
        {
            ContourID contourID = new ContourID(elev);
            return contourDictionary[contourID];
        }

        // Get and Set Major contour intervals
        public double getMajorInterval()
        {
            return majorInterval;
        }

        public double setMajorInterval(double newInterval)
        {
            majorInterval = newInterval;
            return majorInterval;
        }


        // Constructor 
        public ContoursV2(double major, double minor, ICollection<Triangle> triangles_)
        {
            if (debug) Console.WriteLine("Contours:(double minor, double major, TriangleList triList):Entered");
            minorInterval = minor;
            majorInterval = major;
            triangles = triangles_;

            boundingBox = ComputeBounds(triangles);

            lowContourElevation = (int)(boundingBox.MinZ / minorInterval);

            InitializeContours();

            if (debug) Console.WriteLine("Contours:(double minor, double major, TriangleList triList):Excited");
        }

        private bool InitializeContours()
        {
            // Note: lowContourElevation is computed in the method
            // computeNumberOfContours

            bool result = false;
            NumberOfContours = computNumberOfContours();

            // create an array of contours for storing the contour line segments
            if (NumberOfContours > 1)    // one represents no contours
            {
                for (int i = 0; i < NumberOfContours; i++)
                {
                    double elevation = lowContourElevation + i * minorInterval;
                    ContourID contourId = new ContourID(elevation);

                    ContourV2 contour = new ContourV2(contourId);

                    if (debug) Console.WriteLine("contour[" + i + "] " + elevation);
                    contourDictionary.Add(contourId, contour);
                }
            }
            else
            {
                Console.Write("error: Number of contours computes to Zero or less\n");
                return result;
            }

            if (debug) Console.WriteLine("List of contours =" + this);

            allElevSegments = ComputeContourSegments();

            //Sort based on z or elevation
            //Contours are in lowest to highest elevation order
            allElevSegments = allElevSegments.OrderBy(x => x.p1.Z).ToList(); 

            List<ContourID> keys = contourDictionary.Keys.ToList();
            StringBuilder sball = new StringBuilder();

            for (int i = 0; i < keys.Count; i++)
            {
                List<LinearSegmentV2>? contourSegments = allElevSegments.Where(x => x.contourId == keys[i].GetId()).ToList();
                if (contourSegments != null && contourSegments.Count > 0)
                {
                    //convert two point line segments into line string array of points
                    List<LineString>? lineStrings = Segments2LineString(contourSegments);

                    //For debugging output to wave front file
                    if (Debug) sball.AppendLine(ContourLineStrings2WaveFront( lineStrings));

                    contourDictionary[keys[i]].SetLineStrings(lineStrings);
                    allLineStrings.AddRange(lineStrings);
                }
            } 

            if(Debug)
            {
                string elevstr1 = "allContours";
                string lines = OBJOutPut.MergeOBJContourData(sball.ToString());
                System.IO.File.WriteAllText(string.Format(@"OutputObj\{0}_contour_" + elevstr1 + ".obj", "V2Test"), lines);
            }
            return result;
        }

        private string ContourLineStrings2WaveFront( List<LineString> lineStrings)
        {
            StringBuilder sb = new StringBuilder();
            if (lineStrings != null && lineStrings.Count > 0)
            { 
                int pointcount = 0;
                //add these line string to the contour
                for (int tt = 0; tt < lineStrings.Count; tt++)
                {
                    LineString linestring = lineStrings[tt];
                    for (int p = 0; p < linestring.Count; p++)
                    {
                        sb.AppendLine(string.Format("v {0} {1} {2}", linestring[p].X, linestring[p].Y, linestring[p].Z));
                    }
                    sb.AppendLine();

                    for (int l = 1; l < linestring.Count; l++)
                    {
                        sb.AppendLine(string.Format("l {0} {1}", l + pointcount, (l + pointcount + 1)));
                    }
                    sb.AppendLine();
                    pointcount = linestring.Count;
                } 
            }
            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private List<LinearSegmentV2> ComputeContourSegments()
        {

            int minElevIndex = 0;
            int maxElevIndex = 0;

            List<ContourID> keys = contourDictionary.Keys.ToList();

            for (int t = 0; t < triangles.Count; t++)
            {
                Triangle tri = triangles.ElementAt(t);

                //Get triangle points
                List<Vertex> vertices = tri.GetVertices();
                //Sort the triangle points by z or elevation
                List<Vertex> sortedTriPntst = vertices.OrderBy(x => x.Z).ToList();

                GetContourBoundsForTriangle(sortedTriPntst, out minElevIndex, out maxElevIndex);

                for (int i = minElevIndex; i <= maxElevIndex; i++)
                {
                    List<LinearSegmentV2>? seg = ComputeSegmentGivenZ(sortedTriPntst, keys[i].elev, keys[i].GetId());
                    if (seg != null)
                    {
                        allElevSegments.AddRange(seg);
                    }
                }
            }
            return allElevSegments;
        }

        private void GetContourBoundsForTriangle(List<Vertex> sortedTriPntst, out int minElevindex, out int maxElevIndex)
        {
            minElevindex = -1;
            maxElevIndex = -1;

            List<ContourID> keys = contourDictionary.Keys.ToList();
            int i = 0;
            for (; i < keys.Count; i++)
            {
                if (keys[i].elev >= sortedTriPntst[0].Z)
                {
                    if (i == 0)
                    {
                        minElevindex = 0;
                        break;
                    }
                    else
                    {
                        minElevindex = i - 1;
                        break;
                    }
                }
                else if (i == keys.Count - 1)
                {
                    //triangle min and max greater than max contour elev 
                    minElevindex = keys.Count - 1;
                    maxElevIndex = keys.Count - 1;
                    return;
                }
            }

            for (; i < keys.Count; i++)
            {
                if (keys[i].elev >= sortedTriPntst[2].Z)
                {
                    maxElevIndex = i;
                    break;
                }
                else if (sortedTriPntst[2].Z >= keys[i].elev)
                {
                    maxElevIndex = keys.Count - 1;
                    break;
                }
            }
            if (minElevindex == -1 || maxElevIndex == -1)
            {
                throw new Exception("error in GetContourBoundsForTriangle");
            }
        }




        private List<LinearSegmentV2>? ComputeSegmentGivenZ(List<Vertex> sortedTriPntst, double contourElev, Guid contourid)
        {
            Vertex[] contourPnts = new Vertex[2];
            List<LinearSegmentV2> segments = new List<LinearSegmentV2>();
            //  beginContour is set to the first contour elevation greater that the lowest z of this
            //  triangle
            if (sortedTriPntst[0].Z <= contourElev && sortedTriPntst[2].Z >= contourElev)
            {
                /* test zmax against First contour to break the loop early */
                // if elevation(the current contour elevation greater than the highest elevation
                // of this triangle
                //if (elevation < pList[0].Z)
                //{
                //    if (debug) Console.WriteLine("Triangle # " + i + " elevation:" + elevation + " < pList[0].z:" + pList[0].Z);
                //    continue;
                //}
                //else if (elevation > pList[2].Z)
                //{
                //    Console.WriteLine("elevation : " + elevation + " pList[2].z : " + pList[2].Z);
                //    Console.WriteLine("elevation > pList[2].z  break happened");
                //    System.Environment.Exit(-1);
                //    break;
                //}

                if (sortedTriPntst[0].Z == contourElev)
                {
                    //if the lowest elevation in the triangle is equal the contour elevation
                    //then there can be only one point and more segment can be computed
                    return null;
                }
                /* check boundary conditions if all three triangle points have equal elevations */
                else if (sortedTriPntst[0].Z == contourElev && sortedTriPntst[1].Z == contourElev && sortedTriPntst[2].Z == contourElev)
                {
                    //if (debug) Console.WriteLine("Triangle # " + t + " Flat triangle found Triangle : " + triangle.ToString());

                    segments.Add(new LinearSegmentV2(sortedTriPntst[0], sortedTriPntst[1], contourid));
                    segments.Add(new LinearSegmentV2(sortedTriPntst[1], sortedTriPntst[2], contourid));
                    segments.Add(new LinearSegmentV2(sortedTriPntst[2], sortedTriPntst[0], contourid));
                    return segments;
                }
                ///* check boundary conditions if two triangle points have equal elevation */
                else if ((sortedTriPntst[0].Z == contourElev && sortedTriPntst[1].Z == contourElev))
                {
                    // flat triangle edge found 
                    segments.Add(new LinearSegmentV2(sortedTriPntst[0], sortedTriPntst[1], contourid));
                    return segments;
                }
                else if (sortedTriPntst[0].Z == contourElev && sortedTriPntst[2].Z == contourElev)
                {  // flat triangle edge found 
                    segments.Add(new LinearSegmentV2(sortedTriPntst[0], sortedTriPntst[2], contourid));
                    return segments;
                }
                else if (sortedTriPntst[1].Z == contourElev && sortedTriPntst[2].Z == contourElev)
                { // flat triangle edge found 
                    segments.Add(new LinearSegmentV2(sortedTriPntst[1], sortedTriPntst[2], contourid));
                    return segments;
                }

                // else /* normal triangle */
                else
                {
                    //if (debug) Console.WriteLine("Triangle # " + t + " Normal found Triangle : " + triangle.ToString());
                    /* check pnt1 and pnt2 */
                    if ((sortedTriPntst[0].Z < contourElev) && (contourElev <= sortedTriPntst[1].Z))
                    {
                        contourPnts[0] = MathLib.InterpXY(sortedTriPntst[1], sortedTriPntst[0], contourElev);
                        if (MathLib.isBadVertex(contourPnts[0]) == true)
                        {
                            Console.Write("Error:interpXY failed from ContourTriangle\n");
                            Console.Write("pList[0] = " + sortedTriPntst[0].X + "  " + sortedTriPntst[0].Y + "  " + sortedTriPntst[0].Z + "\n");
                            Console.Write("pList[1] = " + sortedTriPntst[1].X + "  " + sortedTriPntst[1].Y + "  " + sortedTriPntst[1].Z + "\n");
                            throw new Exception(string.Format("Error:interpXY failed from ContourTriangle {0} {1}", sortedTriPntst[0].ToString(), sortedTriPntst[1].ToString()));
                        }
                        contourPnts[1] = MathLib.InterpXY(sortedTriPntst[2], sortedTriPntst[0], contourElev);
                        if (MathLib.isBadVertex(contourPnts[0]) == true)
                        {
                            Console.Write("Error:interpXY failed from ContourTriangle\n");
                            Console.Write("pList[1] = " + sortedTriPntst[1].X + "  " + sortedTriPntst[1].Y + "  " + sortedTriPntst[1].Z + "\n");
                            Console.Write("pList[2] = " + sortedTriPntst[2].X + "  " + sortedTriPntst[2].Y + "  " + sortedTriPntst[2].Z + "\n");
                            throw new Exception(string.Format("Error:interpXY failed from ContourTriangle {0} {1}", sortedTriPntst[1].ToString(), sortedTriPntst[2].ToString()));
                        }
                    }
                    /* check pnt1 and pnt3 */
                    else if ((sortedTriPntst[1].Z <= contourElev) && (contourElev <= sortedTriPntst[2].Z) && (sortedTriPntst[0].Z <= contourElev))
                    {
                        contourPnts[0] = MathLib.InterpXY(sortedTriPntst[2], sortedTriPntst[0], contourElev);
                        if (MathLib.isBadVertex(contourPnts[0]) == true)
                        {
                            Console.Write("Error:interpXY failed from ContourTriangle\n");
                            Console.Write("pList[0] = " + sortedTriPntst[0].X + "  " + sortedTriPntst[0].Y + "  " + sortedTriPntst[0].Z + "\n");
                            Console.Write("pList[1] = " + sortedTriPntst[1].X + "  " + sortedTriPntst[1].Y + "  " + sortedTriPntst[1].Z + "\n");
                            Console.Write("pList[2] = " + sortedTriPntst[2].X + "  " + sortedTriPntst[2].Y + "  " + sortedTriPntst[2].Z + "\n");
                            throw new Exception(string.Format("Error:interpXY failed from ContourTriangle {0} {1} {2}",
                                                sortedTriPntst[0].ToString(), sortedTriPntst[1].ToString(), sortedTriPntst[2].ToString()));
                        }
                        contourPnts[1] = MathLib.InterpXY(sortedTriPntst[2], sortedTriPntst[1], contourElev);

                        if (MathLib.isBadVertex(contourPnts[1]) == true)
                        {
                            Console.Write("Error:interpXY failed from ContourTriangle\n");
                            Console.Write("pList[0] = " + sortedTriPntst[0].X + "  " + sortedTriPntst[0].Y + "  " + sortedTriPntst[0].Z + "\n");
                            Console.Write("pList[1] = " + sortedTriPntst[1].X + "  " + sortedTriPntst[1].Y + "  " + sortedTriPntst[1].Z + "\n");
                            Console.Write("pList[2] = " + sortedTriPntst[2].X + "  " + sortedTriPntst[2].Y + "  " + sortedTriPntst[2].Z + "\n");
                            throw new Exception(string.Format("Error:interpXY failed from ContourTriangle {0} {1} {2}",
                                                sortedTriPntst[0].ToString(), sortedTriPntst[1].ToString(), sortedTriPntst[2].ToString()));
                        }
                    }
                    else
                    {
                        Console.Write("Error: contour segment not found or added\n");
                        Console.Write("Elevation : " + contourElev + "\n");
                        Console.Write("pList[0].z : " + sortedTriPntst[0].Z + "\n");
                        Console.Write("pList[1].z : " + sortedTriPntst[1].Z + "\n");
                        Console.Write("pList[2].z : " + sortedTriPntst[2].Z + "\n");
                        return null;
                    }

                    /* all good now add segment to contour list */
                    if (debug) Console.WriteLine("elevation :" + contourElev + " p1 :" + contourPnts[0] + " p2 :" + contourPnts[1]);

                    if (contourPnts[0].GetHashCode() != contourPnts[1].GetHashCode())
                    {
                        segments.Add(new LinearSegmentV2(contourPnts[0], contourPnts[1], contourid));
                    }

                    if (debug) Console.Write("segment count : " + segments.Count + "\n");
                }
            } // end of for loop

            return segments;
        }


        public List<LineString>? Segments2LineString(List<LinearSegmentV2> contourSegmentlist)
        {
            List<LineString> linestrings = new List<LineString>();
            if (contourSegmentlist == null || contourSegmentlist.Count < 1) return null;

            //need to process all the segments in the list
            // There can be more than one contour at the same elevation
            int interations = 0;
            while (contourSegmentlist.Count > 0 && interations < 50)
            {
                LineString linestring = new LineString();

                contourSegmentlist = contourSegmentlist.OrderBy(x => x.p1.GetHashCode()).ThenBy(x => x.p2.GetHashCode()).ToList();

                if (debug)
                {
                    for (int s = 0; s < contourSegmentlist.Count; s++) Console.WriteLine("{0}  {1}", s, contourSegmentlist[s]);
                }

                //Start the line string by adding the first segment points
                linestring.Add(contourSegmentlist[0].p1);
                linestring.Add(contourSegmentlist[0].p2);
                contourSegmentlist.Remove(contourSegmentlist[0]);
                for (int i = contourSegmentlist.Count - 1; i >= 0; i--)
                {
                    LinearSegmentV2? linSeg = contourSegmentlist.Where(x => (x.p1.GetHashCode() == linestring[linestring.Count - 1].GetHashCode()) ||
                                                          (x.p2.GetHashCode() == linestring[linestring.Count - 1].GetHashCode())).FirstOrDefault();
                    if (linSeg != null)
                    {
                        if (linSeg.p1.GetHashCode() == linestring[linestring.Count - 1].GetHashCode())
                        {
                            linestring.Add(linSeg.p2);
                        }
                        else if (linSeg.p2.GetHashCode() == linestring[linestring.Count - 1].GetHashCode())
                        {
                            linestring.Add(linSeg.p1);
                        }
                        contourSegmentlist.Remove(linSeg);
                    }
                }
                linestrings.Add(linestring);
            }
            String lstxt = "Line string count " + linestrings.Count;
            String segltxt = "Segemnt List count " + contourSegmentlist.Count;
            return linestrings;
        }



        // compute the distance between two points
        private double length(Vertex p1, Vertex p2)
        {
            double result = -1.0;
            if (p1 != null && p2 != null)
            {
                double x = p1.X - p2.X;
                double y = p1.Y - p2.Y;
                result = Math.Sqrt(x * x + y * y);
            }
            return result;
        }

        public bool makeContourLabels(double labelDistance)
        {
            if (true) Console.WriteLine("Contours:makeContourLabels:Entered ");
            bool result = false;

            //for (int i = 0; i < contour.Length; i++)
            //{
            //    result = contour[i].makeLabels(labelDistance);
            //    if (result == false)
            //    {
            //        Console.WriteLine("Contours:makeContourLabels:Error: on contour[" + i + "] elevation : " + contour[i].elevation);
            //        break;
            //    }
            //}

            //if (true) Console.WriteLine("Contours:makeContourLabels:Exicted ");

            return result;
        }



        private int computNumberOfContours()
        {
            if (debug) Console.WriteLine("Contours:computNumberOfContours:Entered");

            int NumberOfContours = 0;
            double lowElevation = boundingBox.MinZ;
            double highElevation = boundingBox.MaxZ;

            lowContourElevation = (int)(lowElevation / minorInterval);
            lowContourElevation = (lowContourElevation * minorInterval); // + minorInterval ;

            highContourElevation = (int)(highElevation / minorInterval) + 1;
            highContourElevation = (highContourElevation * minorInterval); // -minorInterval;

            if ((highContourElevation - minorInterval) <= highElevation)
            {
                highContourElevation = highContourElevation - minorInterval;
            }

            NumberOfContours = (int)((highContourElevation - lowContourElevation) / minorInterval) + 1;

            if (debug)
            {
                Console.WriteLine("Min Triangle list elevation : " + lowElevation);
                Console.WriteLine("Max Triangle list elevation : " + highElevation);
                Console.WriteLine("NumberOfContours : " + NumberOfContours);
                Console.WriteLine("minorInterval : " + minorInterval);
                Console.WriteLine("majorInterval : " + majorInterval);
            }

            if (debug) Console.WriteLine("Contours:computNumberOfContours:Exited");
            return NumberOfContours;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tris"></param>
        /// <returns></returns>
        internal BoundingBox3D ComputeBounds(ICollection<Triangle> tris)
        {
            BoundingBox3D boundingBox3D = new BoundingBox3D();

            Vertex tmp;
            for (int n = 0; n < tris.Count; n++)
            {
                for (int i = 0; i < 3; i++)
                {
                    tmp = tris.ElementAt(n).GetVertex(i);

                    boundingBox3D.MinX = Math.Min(boundingBox3D.MinX, tmp.X);
                    boundingBox3D.MinY = Math.Min(boundingBox3D.MinY, tmp.Y);
                    boundingBox3D.MinZ = Math.Min(boundingBox3D.MinZ, tmp.Z);

                    boundingBox3D.MaxX = Math.Max(boundingBox3D.MaxX, tmp.X);
                    boundingBox3D.MaxY = Math.Max(boundingBox3D.MaxY, tmp.Y);
                    boundingBox3D.MaxZ = Math.Max(boundingBox3D.MaxZ, tmp.Z);
                }
            }

            return boundingBox3D;
        }
    }
}
