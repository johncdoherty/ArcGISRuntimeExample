using Esri.ArcGISRuntime.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcGISRuntimeExample
{
    /// <summary>
    /// Holds information about each individual cluster
    /// </summary>
    public class Cluster
    {
        /// <summary>
        /// The location of the point to render (centroid, offset centroid, etc.)
        /// </summary>
        public MapPoint TargetPoint { get; set; }

        /// <summary>
        /// Discrete points represented in the cluster.
        /// </summary>
        public List<MapPoint> PointsInRange { get; set; }

        /// <summary>
        /// The number of points in the cluster.
        /// </summary>
        public int Count { get; set; }
    }
}
