using ArcGISRuntimeExample.Common;
using Esri.ArcGISRuntime;
using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.Tasks.Geoprocessing;
using Esri.ArcGISRuntime.UI;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Color = System.Drawing.Color;

namespace ArcGISRuntimeExample.Models
{
    public class IncidentModel
    {
        #region Constants
        // Feature layer example
        private const string _featureServiceURL = "https://services1.arcgis.com/fif0ERoHeJunwZ8D/arcgis/rest/services/Denver_Accidents/FeatureServer/0";//"http://sampleserver3.arcgisonline.com/ArcGIS/rest/services/SanFrancisco/311Incidents/FeatureServer/0";
        // Size of symbol to cluster based on
        // TODO: Update accordingly
        private double _symbolTolerance = 10;
        #endregion

        #region Members
        private List<int> _currentSelection;
        private object _clusterLock = new object();
        private System.Timers.Timer _featureUpdateTimer;
        // Semaphore with a value of 1. This means that only 1 thread can be granted access at a time.
        private SemaphoreSlim _featureSemaphore = new SemaphoreSlim(1, 1);
        private bool _isSelectionInitialized = false;
        private bool _isClusteringInitialized = false;
        protected string _previousDefinitionExpression = string.Empty;
        /// <summary>
        /// Locking mechanism for Monitor.TryEnter behavior.
        /// </summary>
        private volatile int _isUpdateFeaturesRunning = 0;
        /// <summary>
        /// Locking mechanism for Monitor.TryEnter behavior.
        /// </summary>
        private volatile int _isClusterRunning = 0;
        /// <summary>
        /// Tracks the previous map scale units.
        /// </summary>
        private double _previousUnitsPerPixel = 0.0;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public IncidentModel(GraphicsOverlay clusterOverlay)
        {
            InitializeClustering(clusterOverlay);
            Initialize();
        }
        #endregion

        #region Properties

        /// <summary>
        /// The feature service feature layer of the feature records
        /// </summary>
        public FeatureLayer Layer
        {
            get;
            set;
        }

        /// <summary>
        /// Graphics to show cluster counts.
        /// </summary>
        public GraphicsOverlay ClusterOverly { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Generates a where clause based on filters that are set.
        /// </summary>
        /// <returns></returns>
        private  string GetFilterString()
        {
            string retVal = "1=1"; // All records
            return retVal;
        }

        /// <summary>
        /// Initializes the object. Base class subscribes to events.
        /// </summary>
        private void Initialize()
        {
            
            this.Layer = MapHelper.CreateServiceFeatureLayer(new Uri(_featureServiceURL), FeatureRequestMode.OnInteractionNoCache);
            this.Layer.RefreshInterval = TimeSpan.FromSeconds(Settings.LayerRefreshInSeconds);
            this.UpdateDefinitionExpression();

            // Listen for changes to feature filters and update definition expression accordingly   
            Settings.IncidentFilterChanged += (s, e) => UpdateDefinitionExpression();

        }

        /// <summary>
        /// Initialized feature clustering capabilities (counts of overlapping features).
        /// </summary>
        /// <param name="clusteringOverlay">The graphics layer to initialize clustering for.</param>
        private void InitializeClustering(GraphicsOverlay clusteringOverlay)
        {
            this.ClusterOverly = clusteringOverlay ?? throw new ArgumentNullException(nameof(clusteringOverlay));
            this.ClusterOverly.IsVisible = true;
            this._isClusteringInitialized = true;
        }


        /// <summary>
        /// Helper to check if layer is in a state to query.
        /// </summary>
        /// <returns></returns>
        private bool IsLayerInitialized()
        {
            return this.Layer != null && this.Layer.LoadStatus == Esri.ArcGISRuntime.LoadStatus.Loaded && this.Layer.DefinitionExpression.IsNotNullOrEmpty();
        }


        /// <summary>
        /// A local collection of semaphore locked features in the layer's feature table.
        /// </summary>
        private List<MapPoint> FeaturePoints { get; set; }
        public async Task<List<MapPoint>> GetFeaturePoints()
        {
            await _featureSemaphore.WaitAsync();
            try
            {
                return FeaturePoints.ToList();
            }
            catch
            {
                return new List<MapPoint>();
            }
            finally
            {
                _featureSemaphore.Release();
            }
        }

        /// <summary>
        /// Updates the local Features collection with the current features available in the feature table.
        /// </summary>
        private async Task UpdateFeaturePoints()
        {
            if (this.IsLayerInitialized() && _isClusteringEnabled)
            {

                // Re-entrant, but only allow one to process at a time, tell others to try back later
                // Acts same as Monitor.TryEnter
                if (Interlocked.CompareExchange(ref _isUpdateFeaturesRunning, 1, 0) != 0)
                {
                    return;
                }

                List<MapPoint> points = null;
                try
                {
                    points = await GetFeaturesUsingService();

                }
                catch (Exception ex)
                {
                    //Logger.Warn(ex);
                    return;
                }
                finally
                {
                    // Release the lock
                    Interlocked.Exchange(ref _isUpdateFeaturesRunning, 0);
                }
                if (points != null)
                {
                    await _featureSemaphore.WaitAsync();
                    try
                    {
                        this.FeaturePoints = points;
                    }
                    finally
                    {
                        _featureSemaphore.Release();
                    }
                }

            }
        }

        /// <summary>
        /// Retrieves a feature list from the map service feature layer. Has known issues with exception regularly occurring.
        /// </summary>
        /// <returns></returns>
        private async Task<List<MapPoint>> GetFeaturesUsingService()
        {
            var serviceFeatureTable = this.Layer.FeatureTable as ServiceFeatureTable;

            FeatureQueryResult featuresResult;
            List<MapPoint> points = null;
            // Special Scenario (identified in v100.3): too frequent layer refreshes cause SQL Constraint exception, but succeeds
            // regularly at the RefreshInterval of the layer
            // Retry a max of 10 times for success
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    var queryParams = new QueryParameters()
                    {
                        WhereClause = this.Layer.DefinitionExpression
                    };

                    featuresResult = await serviceFeatureTable.QueryFeaturesAsync(queryParams, QueryFeatureFields.IdsOnly).ConfigureAwait(false);
                    points = featuresResult.Select(p => p.Geometry as MapPoint).ToList();
                    break;
                }
                catch (ArcGISException)
                {
                    // Wait 1 sec
                    await Task.Delay(1000);
                }
            }
            return points;
        }

        /// <summary>
        /// Updates the definitiion expression of the layer based on the app's filter criteria.
        /// This is only necessary if the app is applying custom filters, and if so the clusters need to be calculated accordingly.
        /// </summary>
        private bool UpdateDefinitionExpression()
        {
            // Generate the query expression using the set filters. 
            // This should only get called when something has changed
            var newDefinitionExpression = GetFilterString();
            // Just in case, prevent the layer's filter from being updated if the query hasn't changed
            if (_previousDefinitionExpression != newDefinitionExpression)
            {
                this.Layer.DefinitionExpression = newDefinitionExpression;
                _previousDefinitionExpression = newDefinitionExpression;

                // Run synchronously - async call inside
                UpdateClustersAsync(Settings.MapUnitsPerPixel, true);
                return true;
            }
            return false;

        }

        #region Clustering Functionality

        /// <summary>
        /// Vertical offset to apply to cluster labels.
        /// </summary>
        public double ClusterOffsetX
        {
            get;
            set;
        } = 13;
        /// <summary>
        /// Vertical offset to apply to cluster labels.
        /// </summary>
        public double ClusterOffsetY
        {
            get;
            set;
        } = 15;

        private bool _isClusteringEnabled = false;
        public bool IsClusteringEnabled
        {
            get
            {
                return _isClusteringEnabled;
            }
            set
            {
                if (_isClusteringEnabled != value &&
                    value == true)
                {
                    _isClusteringEnabled = value;
                    // Update the clusters in the background and updating the features
                    
                    // Run synchronously, runs on background thread
                    UpdateClustersAsync(Settings.MapUnitsPerPixel, true);

                    this.ClusterOverly.IsVisible = true;
                }
                else
                {
                    _isClusteringEnabled = value;
                    this.ClusterOverly.IsVisible = false;
                }
            }
        }


        /// <summary>
        /// Clears the cluster graphics
        /// </summary>
        public void ClearClusters()
        {
            this.ClusterOverly?.Graphics?.Clear();
        }

        /// <summary>
        /// Updates the overlapping cluster count graphics for the feature layer using the thread pool.
        /// </summary>
        /// <param name="unitsPerPixel">The number of units associated with each pixel (from MapView>UnitsPerPixel).</param>
        /// <param name="refreshData">Indicates if the local data cache should also be updated.</param>
        public async Task UpdateClustersAsync(double unitsPerPixel, bool refreshData = true)
        {
            if (!(_isClusteringInitialized && this.IsLayerInitialized()) || _isClusterRunning == 1 || !_isClusteringEnabled)
            {
                return;
            }
            // Ensure that processing is done with thread pool
            await Task.Run(async () =>
            {
                await UpdateClusters(unitsPerPixel, refreshData);
            });
        }

        /// <summary>
        /// Updates the overlapping cluster count graphics for the feature layer.
        /// </summary>
        /// <param name="unitsPerPixel">The number of units associated with each pixel (from MapView>UnitsPerPixel).</param>
        /// <param name="refreshData">Indicates if the local data cache should also be updated.</param>
        public async Task UpdateClusters(double unitsPerPixel, bool refreshData = true)
        {
            if (!(_isClusteringInitialized && this.IsLayerInitialized()) || _isClusterRunning == 1 || !Settings.ShowClusterCounts || !_isClusteringEnabled)
            {
                return;
            }

            // Only allow one process to run this operation and return if already in progress
            // Necessary, since method is invoked from several areas, including timer
            // This behavior works asynchronously, whereas Monitor.TryEnter does not
            if (Interlocked.CompareExchange(ref _isClusterRunning, 1, 0) != 0)
                return;

            try
            {
                if (unitsPerPixel != this._previousUnitsPerPixel)
                {
                    this.ClearClusters();
                }

                if (refreshData)
                {
                    await UpdateFeaturePoints().ConfigureAwait(false);
                }

                // Create a dictionary of all points with a value indicating if they have been clustered yet
                Dictionary<MapPoint, bool> clusterPoints = new Dictionary<MapPoint, bool>();
                // Read/Write access to the Features object needs to be locked, so it doesn't change while accessing
                await _featureSemaphore.WaitAsync();
                try
                {
                    this.FeaturePoints?.ForEach(point =>
                    {
                        clusterPoints.Add(point.ToWebMercator() as MapPoint, false);
                    });
                }
                finally
                {
                    _featureSemaphore.Release();
                }

                // Generate the cluster groups
                var clusters = CreateClusters(clusterPoints, unitsPerPixel * _symbolTolerance);

                // Update the graphic counts for the clusters
                UpdateClusterGraphics(clusters);
                _previousUnitsPerPixel = unitsPerPixel;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                Interlocked.Exchange(ref _isClusterRunning, 0);
            }

        }

        // TODO: Optimizations may be required.
        // Considerations:
        // - How much data is being clustered and what are the performance expectations?
        // - Is data dynamic - is data being updated in the data source? How often? Will it be updating in the background - should clustering of all points (not just map extent) occur at that time? 
        // - Is it better to filter out based on map extent and recalculate every pan/zoom? or recalculate all only on zoom changes?
        // - Are there faster storage data structures available? Perhaps something that can be clipped by the map extent
        // - Is updating existing geometries/values be faster than recreating every time?
        // - Is there a faster algorithm for calculating overlaps?
        // - Is it faster to cluster and render on less or more buckets of data? - Example: maybe less clusters (with more data per cluster) and larger symbology is faster to calculate and render?
        /// <summary>
        /// Creates the clusters from the dictionary of points.
        /// </summary>
        private List<Cluster> CreateClusters(Dictionary<MapPoint, bool> clusterPoints, double mapTolerance)
        {
            List<Cluster> clusters = new List<Cluster>();
            for (int i = 0; i < clusterPoints.Count; i++)
            {
                var kvp = clusterPoints.ElementAt(i);
                if (!kvp.Value && kvp.Key.IsWithin(Settings.MapExtent)) // TODO: This IsWithin call is optional - filters out points that are not within current map extent
                {
                    clusterPoints[kvp.Key] = true;
                    var overlaps = GetOverlappingPoints(clusterPoints, kvp.Key, mapTolerance);
                    if (overlaps != null && overlaps.Count > 0)
                    {

                        clusters.Add(
                            new Cluster
                            {
                                TargetPoint = kvp.Key,
                                PointsInRange = overlaps,
                                Count = overlaps.Count + 1 // Account for the current point too
                            });
                    }
                }
            }
            return clusters;
        }

        /// <summary>
        /// Performs Euclidian distance calculation of points to determine if they are within the given map tolerance.
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="mapTolerance"></param>
        /// <returns></returns>
        private bool CalculateDistance(MapPoint point1, MapPoint point2, double mapTolerance)
        {
            // Same coordinate.
            if (point1 == point2)
                return true;

            double xDelta = point1.X - point2.X;
            double yDelta = point1.Y - point2.Y;

            double distance = Math.Sqrt(xDelta * xDelta + yDelta * yDelta);

            return (distance <= mapTolerance);
        }

        /// <summary>
        /// Generates a list of overlapping points for the input point, and updating the input dictionary accordingly
        /// with the status of whether the point has been clustered already.
        /// </summary>
        /// <param name="points">Dictionary tracking the points and the state of if they have been clustered.</param>
        /// <param name="firstPoint"></param>
        /// <param name="mapTolerance"></param>
        /// <returns></returns>
        private List<MapPoint> GetOverlappingPoints(Dictionary<MapPoint, bool> points, MapPoint firstPoint, double mapTolerance)
        {
            List<MapPoint> overlappingPoints = new List<MapPoint>();

            for (int i = 0; i < points.Count; i++)
            {
                var kvp = points.ElementAt(i);
                if (!kvp.Value)
                {

                    if (CalculateDistance(kvp.Key, firstPoint, mapTolerance))
                    {
                        points[kvp.Key] = true;
                        overlappingPoints.Add(kvp.Key);
                    }
                }
            }
            return overlappingPoints.Count > 0 ? overlappingPoints : null;
        }

        /// <summary>
        /// Starts a timer to refresh features and associated UI graphics.
        /// </summary>
        public void StartTimedFeatureUpdates()
        {
            // Refresh the clusters at the interval the layer refreshes at - this automatically updates the Features list too
            _featureUpdateTimer = new System.Timers.Timer(Settings.BackgroundClusterFeatureUpdateInSeconds * 1000);
            _featureUpdateTimer.Elapsed += async (s, e) =>
            {
                if (Settings.AllowBackgroundClusterFeatureUpdates)
                {
                    // Update the features
                    await UpdateFeaturePoints();
                }

                // Update the clusters (requesting not to update features again)
                if (Settings.ShowClusterCounts)
                {
                    await UpdateClusters(Settings.MapUnitsPerPixel, false);
                }
            };
            _featureUpdateTimer.Start();
        }

        /// <summary>
        /// Stop feature refresh timer.
        /// </summary>
        public void StopTimedFeatureUpdates()
        {
            _featureUpdateTimer.Stop();
            _featureUpdateTimer.Dispose();
        }

        /// <summary>
        /// Clears and replaces the cluster count graphics.
        /// </summary>
        /// <param name="clusters">The clusters used to create the graphics.</param>
        private void UpdateClusterGraphics(List<Cluster> clusters)
        {
            // TODO: Consider not clearing graphics and binding visibility and size
            this.ClusterOverly.Graphics.Clear();
            foreach (var p in clusters)
            {
                if (p.Count > 1)
                {
                    // TODO: Use custom symbol
                    var textSymbol =
                        new TextSymbol(
                            p.Count.ToString(), // Set the text to the count vallue
                            Color.Black,
                            15, // TODO: Change symbol size based on number of points clustered (p.PointsInRange)
                            Esri.ArcGISRuntime.Symbology.HorizontalAlignment.Center,
                            Esri.ArcGISRuntime.Symbology.VerticalAlignment.Middle)
                        {
                            OutlineColor = Color.Black,
                            OutlineWidth = 2,
                            OffsetX = this.ClusterOffsetX,
                            OffsetY = this.ClusterOffsetY,
                            HaloColor = Color.White,
                            HaloWidth = 1,
                            FontWeight = Esri.ArcGISRuntime.Symbology.FontWeight.Bold
                        } as Symbol;
                    this.ClusterOverly.Graphics.Add(new Graphic(p.TargetPoint, textSymbol));
                }
            }
        }

        #endregion

        #endregion

    }
}
