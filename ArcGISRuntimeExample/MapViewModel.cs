using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Location;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Security;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.Tasks;
using Esri.ArcGISRuntime.UI;
using Esri.ArcGISRuntime.UI.Controls;
using Esri.ArcGISRuntime;

namespace ArcGISRuntimeExample
{
    /// <summary>
    /// Provides map data to an application
    /// </summary>
    public class MapViewModel : INotifyPropertyChanged
    {
        public MapViewModel()
        {

        }

        /// <summary>
        /// The cluster count graphics overlay
        /// </summary>
        private GraphicsOverlay _clusterGraphicsOverlay = new GraphicsOverlay();
        private Map _map = new Map(Basemap.CreateNavigationVector());

        private IncidentModel _incidentModel = null;
        private MapView _mapView;
        private double _previousMapScale;

        /// <summary>
        /// Gets or sets the map
        /// </summary>
        public Map Map
        {
            get { return _map; }
            set { _map = value; OnPropertyChanged(); }
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
                    Settings.MapScale = this._mapView.MapScale;
                    Settings.MapUnitsPerPixel = this._mapView.UnitsPerPixel;
                    Settings.MapExtent = this.MapView.GetCurrentViewpoint(ViewpointType.BoundingGeometry).TargetGeometry.ToWebMercator() as Envelope;
                };

                // Standard map configuration
                this._mapView.InteractionOptions = new Esri.ArcGISRuntime.UI.MapViewInteractionOptions
                {
                    IsRotateEnabled = false,
                    WheelZoomDirection = Esri.ArcGISRuntime.UI.WheelZoomDirection.Default
                };

                // Add a cluster graphics layer to display cluster count values of the IncidentModel.Layer feature layer values.
                if (Settings.ShowClusterCounts)
                {
                    this._incidentModel = new IncidentModel(_clusterGraphicsOverlay);
                }

                // Listen for layer loaded state changes
                this._incidentModel.Layer.LoadStatusChanged += async (s, e) =>
                {
                    if (e.Status == LoadStatus.FailedToLoad)
                    {
                        throw new Exception("Layer failed to load");
                    }
                    else if (e.Status == LoadStatus.Loaded)
                    {
                        // Upon load, update clustering
                        if (this.Map.LoadStatus == LoadStatus.Loaded)
                        {
                            if (Settings.ShowClusterCounts)
                            {
                                // Update the clusters based on the new zoom extent and update the associated data
                                await this._incidentModel.UpdateClustersAsync(this.MapView.UnitsPerPixel, true);
                            }
                            else
                            {
                                this._clusterGraphicsOverlay.Graphics.Clear();
                            }

                        }
                    }
                };

                //this.Map.Basemap.BaseLayers.Clear();
                //this.Map.Basemap.BaseLayers.Add(new BingMapsLayer("<YOUR KEY>", BingMapsLayerStyle.Road));

                // Add the feature layer ot the map
                this.Map.OperationalLayers.Add(this._incidentModel.Layer);

                if (Settings.ShowClusterCounts)
                {
                    // Add the cluster  graphics layer to the map - always present, not always containing graphics
                    this.MapView.GraphicsOverlays.Add(this._clusterGraphicsOverlay);

                    // Handle navigation changes, and dynamically update clustering
                    this.MapView.NavigationCompleted += async (o, e) =>
                    {
                        try
                        {
                            // Handle cluster updates when navigation changes
                            if (//this.MapView.MapScale != this._previousMapScale && // Optional- only update clusters when zooming 
                                this.Map.LoadStatus == LoadStatus.Loaded &&
                                this._incidentModel.Layer.LoadStatus == LoadStatus.Loaded)
                            {
                                
                                // Update the clusters based on the new zoom extent and do not update the associated data
                                await this._incidentModel.UpdateClustersAsync(this.MapView.UnitsPerPixel, false);

                            }
                            this._previousMapScale = this.MapView.MapScale;

                        }
                        catch (Exception ex)
                        {
                            throw;
                        }
                    };

                    // Handle layer status changes, so clustering is only visible when active
                    this.MapView.LayerViewStateChanged += (s, e) =>
                    {
                        if (e.Layer == this._incidentModel.Layer)
                        {
                            if (e.LayerViewState.Status == LayerViewStatus.Active)
                            {
                                this._incidentModel.IsClusteringEnabled = true;
                            }
                            else
                            {
                                this._incidentModel.IsClusteringEnabled = false;
                            }
                        }
                    };

                    // Start background timer to update cluster data
                    if (Settings.AllowBackgroundClusterFeatureUpdates)
                    {
                        this._incidentModel.StartTimedFeatureUpdates();
                    }

                }

                result = true;
            }
            return result;
        }

        

        /// <summary>
        /// Raises the <see cref="MapViewModel.PropertyChanged" /> event
        /// </summary>
        /// <param name="propertyName">The name of the property that has changed</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var propertyChangedHandler = PropertyChanged;
            if (propertyChangedHandler != null)
                propertyChangedHandler(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
