using ArcGISRuntimeClustering.Models;
using ArcGISRuntimeExample.Common;
using ArcGISRuntimeExample.Models;
using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.UI;
using Esri.ArcGISRuntime.UI.Controls;
using Esri.Sunstate.DynamicDispatch.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ArcGISRuntimeExample.ViewModels
{
    /// <summary>
    /// Provides map data to an application
    /// </summary>
    public class MapViewModel : BaseViewModel
    {
        public MapViewModel()
        {
            // Wire up handler to reset HasLabels flag anytime Labels collection changes
            this.Labels.CollectionChanged += (s, e) => this.NotifyPropertyChanged("HasLabels");
        }

        /// <summary>
        /// The cluster count graphics overlay
        /// </summary>
        //private GraphicsOverlay _clusterGraphicsOverlay = new GraphicsOverlay();
        private Map _map = new Map(Basemap.CreateNavigationVector());

        private DispatchesModel _dispatches;
        private TrucksModel _trucks;

        private MapView _mapView;

        // Boolean to prevent concurrent identify calls.
        private bool _isIdentifying = false;

        // Store the next identify cell task.
        private Action _nextIdentifyAction = null;

        private int _previousCallout = -1;

        private bool _isSelectEnabled = false;
        private bool _isMoveLabelsEnabled = false;
        private bool _isClickLablesEnabled = true;
        private double _labelLocationX = 0;
        private double _labelLocationY = 0;
        private bool _isInitialized = false;

        /// <summary>
        /// Gets or sets the map
        /// </summary>
        public Map Map
        {
            get { return _map; }
            set { _map = value; NotifyPropertyChanged(); }
        }

        /// <summary>
        /// Reference to the MapView object itself.
        /// Not an ideal thing to have a control reference, but was needed for references to contained properties, 
        /// such as MapScale bindings in the UI.
        /// </summary>
        public MapView MapView
        {
            get { return this._mapView; }
        }

        public DispatchesModel Dispatches
        {
            get { return _dispatches; }
            set { _dispatches = value; base.NotifyPropertyChanged(); }
        }

        // TODO: Model that could be used to encapsulate graphics and business logic related to trucks
        public TrucksModel Trucks
        {
            get { return _trucks; }
            set { _trucks = value; base.NotifyPropertyChanged(); }
        }

        /// <summary>
        /// UI property binding
        /// </summary>
        public bool IsSelectEnabled
        {
            get
            {
                return _isSelectEnabled;
            }
            set
            {
                _isSelectEnabled = value;
                if (!_isSelectEnabled)
                {
                    this.Dispatches.FeatureGraphicsLayer.ClearSelection();
                }
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// UI property binding
        /// </summary>
        public bool IsMoveLabelsEnabled
        {
            get
            {
                return _isMoveLabelsEnabled;
            }
            set
            {
                _isMoveLabelsEnabled = value;
                if (!_isMoveLabelsEnabled)
                {
                    this.MapView.DismissCallout();
                }
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// UI property binding
        /// </summary>
        public bool IsClickLabelsEnabled
        {
            get
            {
                return _isClickLablesEnabled;
            }
            set
            {
                _isClickLablesEnabled = value;
                if (!_isClickLablesEnabled)
                {
                    this.Labels.Clear();
                }
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// creates the layers and sets the initial properties for filtering to the region and date and zooms to the extent of the region/user's extent
        /// </summary>
        /// <param name="mapView">This parameter is from the usercontrol directly.  Needed for making viewport changes.</param>
        public async Task<bool> InitializeMap(MapView mapView)
        {
            bool result = false;
            if (this.Map != null)
            {
                //this.Map.LoadStatusChanged += async (s, e) =>
                //{
                //    if (e.Status == LoadStatus.FailedToLoad)
                //    {
                //    }
                //    else if (e.Status == LoadStatus.Loaded)
                //    {
                        
                //    }
                //};


                this._mapView = mapView;

                // Update stored values of map scale and units for clustering calculations
                this.MapView.ViewpointChanged += (s, e) =>
                {
                    if (_isInitialized)
                    {
                        Settings.MapScale = this._mapView.MapScale;
                        Settings.MapUnitsPerPixel = this._mapView.UnitsPerPixel;
                        Settings.MapExtent = this.MapView.GetCurrentViewpoint(ViewpointType.BoundingGeometry).TargetGeometry.ToWebMercator() as Envelope;
                    }
                };

                // Standard map configuration
                this._mapView.InteractionOptions = new Esri.ArcGISRuntime.UI.MapViewInteractionOptions
                {
                    IsRotateEnabled = false,
                    WheelZoomDirection = Esri.ArcGISRuntime.UI.WheelZoomDirection.Default
                };


                // Handle navigation completed to clear labels
                this.MapView.NavigationCompleted += async (o, e) =>
                {
                    if (this.Labels.Count > 0)
                    {
                        this.Labels.Clear();
                    }
                };

                // Create and initialize the dispatch features
                this.Dispatches = new DispatchesModel();
                await this.Dispatches.Initialize();

                // Add the dispatches graphics overlay to the map
                this.MapView.GraphicsOverlays.Add(this.Dispatches.FeatureGraphicsLayer);

                // Zoom in to points
                await this.MapView.SetViewpointGeometryAsync(this.Dispatches.FeatureGraphicsLayer.Extent, 10);


                // Handle map click event
                this.MapView.GeoViewTapped += async (s, e) =>
                {
                    try
                    {
                        if (this.Labels.Count > 0)
                        {
                            this.Labels.Clear();
                        }
                        this.MapView.DismissCallout();
                        _dispatches.FeatureGraphicsLayer.ClearSelection();
                        
                        double tolerance = 10d; // Use larger tolerance for touch
                        int maximumResults = 10; // Only return one graphic  
                        bool onlyReturnPopups = false; // Return more than popups

                        // Use the following method to identify graphics in a specific graphics overlay
                        IdentifyGraphicsOverlayResult identifyResults = await this.MapView.IdentifyGraphicsOverlayAsync(
                            this.Dispatches.FeatureGraphicsLayer,
                            e.Position,
                            tolerance,
                            onlyReturnPopups,
                            maximumResults);
                        if (identifyResults.Graphics.Count > 0)
                        {
                            if (this.IsSelectEnabled)
                            {
                                // Select the features based on query parameters defined above.
                                _dispatches.FeatureGraphicsLayer.SelectGraphics(identifyResults.Graphics);
                            }
                            if (this.IsClickLabelsEnabled)
                            {
                                this.LabelLocationX = e.Position.X;
                                this.LabelLocationY = e.Position.Y;
                                identifyResults.Graphics.ForEach(p => this.Labels.Add(((CustomGraphic)p).ID.ToString()));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString(), "Error");
                    }

                };

                // Handle mouse move to show callout at given location
                this.MapView.MouseMove += async (s, e) =>
                {
                    if (this.IsMoveLabelsEnabled)
                    {
                        // Get the curent mouse position.
                        Point position = e.GetPosition(this.MapView);

                        // Identify the raster cell at that position.
                        ShowCalloutTopDispatches(position);
                    }
                };

                result = true;
            }
            _isInitialized = true;
            return result;
        }


        /// <summary>
        /// Shows a callout with information about the graphics/features at that location
        /// </summary>
        /// <param name="position"></param>
        private async void ShowCalloutTopDispatches(Point position)
        {
            // Check if a cell is already being identified
            if (_isIdentifying)
            {
                _nextIdentifyAction = () => ShowCalloutTopDispatches(position);
                return;
            }

            // Set variable to true to prevent concurrent identify calls.
            _isIdentifying = true;

            try
            {
                // Get the result for where the user hovered on the raster layer.
                var identifyResult = await this.MapView.IdentifyGraphicsOverlayAsync(this.Dispatches.FeatureGraphicsLayer, position, 1, false, 1);

                var graphic = identifyResult.Graphics.FirstOrDefault() as CustomGraphic;
                // If no cell was identified, dismiss the callout.
                if (graphic != null && graphic.ID != _previousCallout)
                {
                    this.MapView.DismissCallout();
                    _previousCallout = -1;
                }
                else if (graphic == null)
                {
                    this.MapView.DismissCallout();
                    _previousCallout = -1;
                    return;
                }
                else if (graphic != null && graphic.ID == _previousCallout)
                {
                    return;
                }
                _previousCallout = graphic.ID;
                // Create a StringBuilder to display information to the user.
                var stringBuilder = new StringBuilder();

                // Get the identified raster cell.
                GeoElement cell = graphic;

                // Loop through the attributes (key/value pairs).
                foreach (KeyValuePair<string, object> keyValuePair in cell.Attributes)
                {
                    // Add the key/value pair to the string builder.
                    stringBuilder.AppendLine($"{keyValuePair.Key}: {keyValuePair.Value}");
                }

                var mapPoint = this.MapView.ScreenToLocation(position).ToWgs84() as MapPoint;
                // Get the x and y values of the cell.
                double x = position.X;
                double y = position.Y;

                // Add the X & Y coordinates where the user clicked raster cell to the string builder.
                stringBuilder.AppendLine($"X: {Math.Round(mapPoint.X, 4)}\nY: {Math.Round(mapPoint.Y, 4)}");

                // Create a callout using the string.
                var definition = new CalloutDefinition(stringBuilder.ToString());

                // TODO: To show a custom view for the callout, use this.MapView.ShowCalloutAt(graphic.Geometry as MapPoint, this.MyView);
                // Display the call out in the map view.
                this.MapView.ShowCalloutAt(graphic.Geometry as MapPoint, definition);

                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().ToString());
            }
            finally
            {
                _isIdentifying = false;
            }

            // Check if there is a new position to identify.
            if (_nextIdentifyAction != null)
            {
                Action action = _nextIdentifyAction;

                // Clear the queued identify action.
                _nextIdentifyAction = null;

                // Run the next action.
                action();
            }
        }

        /// <summary>
        /// Labels to show in radial panel when clicking on features/graphics
        /// </summary>
        public ObservableCollection<string> Labels
        {
            get;
            private set;
        } = new ObservableCollection<string>();

        /// <summary>
        /// Indicates if labels exist
        /// </summary>
        public bool HasLabels
        {
            get
            {
                return this.Labels.Count > 0;
            }
        }

        /// <summary>
        /// Click location
        /// </summary>
        public double LabelLocationX
        {
            get
            {
                return this._labelLocationX;
            }
            set
            {
                this._labelLocationX = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Click location
        /// </summary>
        public double LabelLocationY
        {
            get
            {
                return this._labelLocationY;
            }
            set
            {
                this._labelLocationY = value;
                this.NotifyPropertyChanged();
            }
        }
    }
}
