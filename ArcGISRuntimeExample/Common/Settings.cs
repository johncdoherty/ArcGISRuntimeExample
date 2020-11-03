using Esri.ArcGISRuntime.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcGISRuntimeExample.Common
{
    public static class Settings
    {

        /// <summary>
        /// Indicates that one or more filters changed.
        /// </summary>
        public static event EventHandler<EventArgs> IncidentFilterChanged;

        /// <summary>
        /// Indicates if the cluster count values should be displayed in the overlay graphics layer.
        /// </summary>
        public static bool ShowClusterCounts = true;

        /// <summary>
        /// Refresh interval of the incident layer. The layer updates are independent of cluster updates - syncing these intervals may make sense.
        /// </summary>
        public static int LayerRefreshInSeconds = 60;

        /// <summary>
        /// Not thoroughly tested
        /// Inidcates if the cluster data should be updated asynchronously on an interval in the background.
        /// This can limit performance, depending on many factors including size of dataset and interval of updates.
        /// This is only necessary if data is updateing in the underlying feature layer.
        /// </summary>
        public static bool AllowBackgroundClusterFeatureUpdates = false;

        /// <summary>
        /// If <seealso cref="AllowBackgroundClusterFeatureUpdates"/> is set to true, this interval indicates the number of seconds that the cluster points will be refreshed.
        /// This is only necessary if data is updateing in the underlying feature layer.
        /// </summary>
        public static int BackgroundClusterFeatureUpdateInSeconds = 60;

        /// <summary>
        /// Current map scale of the new map.
        /// </summary>
        public static double MapScale { get; set; }

        /// <summary>
        /// The current this.MapView.UnitsPerPixel value.
        /// </summary>
        public static double MapUnitsPerPixel { get; set; }

        /// <summary>
        /// The current visible map extent.
        /// </summary>
        public static Envelope MapExtent { get; set; }
    }
}
