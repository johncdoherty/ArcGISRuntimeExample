using Esri.ArcGISRuntime;
using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Symbology;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArcGISRuntimeExample.Models
{
    public class TrucksModel : BaseGraphicsLayerModel
    {

        private int _currentStyleId = -1;

        /// <summary>
        /// Constructor.
        /// </summary>
        public TrucksModel()
        {
            //Initialize();
        }

        #region Properties

        ///// <summary>
        ///// Working status layer
        ///// </summary>
        //public FeatureLayer LayerStatus
        //{
        //    get;
        //    set;
        //}

        ///// <summary>
        ///// Zoomed out truck layer.
        ///// </summary>
        //public FeatureLayer LayerDetailed
        //{
        //    get;
        //    set;
        //}

        ///// <summary>
        ///// Zoomed in truck layer.
        ///// </summary>
        //public FeatureLayer LayerSimple
        //{
        //    get;
        //    set;
        //}

        ///// <summary>
        ///// Override the base Opacity and update the user setting accordingly
        ///// </summary>
        //public override double Opacity { get => base.Opacity; set { base.Opacity = value; AppSettings.Instance.User.TruckLayersOpacity = value; } }

        ///// <summary>
        ///// Indicates if the group of layers are identifiable
        ///// </summary>
        //public override Boolean IsIdentifiable
        //{
        //    get
        //    {
        //        return AppSettings.Instance.User.TruckhLayersIsIdentifiable;
        //    }
        //    set
        //    {
        //        AppSettings.Instance.User.TruckhLayersIsIdentifiable = value;
        //        this.UpdateLayerIsIdentifiable(value);
        //    }
        //}

        #endregion

        #region Methods

        public override async Task Initialize()
        {
            //await base.Initialize();
        }

        /// <summary>
        /// Generates a where clause based on the collaboration filters that are set.
        /// </summary>
        /// <returns></returns>
        //protected override string GetFilterString()
        //{
        //    string retVal = string.Empty;

        //    //if (AppSettings.Instance.CurrentRegionCode.IsNotNullOrEmpty())
        //    //{
        //    //    retVal = "RegionCode = " + AppSettings.Instance.CurrentRegionCode.ToSingleQuote(true);
        //    //}

        //    //if (CollaborationHelper.Instance.TruckTypes.Count > 0)
        //    //{
        //    //    retVal += " AND TruckType " + CollaborationHelper.Instance.TruckTypes.ToCommaDelimitedStringValues(true).ToInClause();
        //    //}

        //    //if (CollaborationHelper.Instance.TruckCodePartial.IsNotNullOrEmpty())
        //    //{
        //    //    retVal += " AND TruckCode " + CollaborationHelper.Instance.TruckCodePartial.ToLikeClause(true);
        //    //}

        //    //if (CollaborationHelper.Instance.DriverNamePartial.IsNotNullOrEmpty())
        //    //{
        //    //    retVal += " AND DriverName " + CollaborationHelper.Instance.DriverNamePartial.ToLikeClause(true);
        //    //}

        //    //if (CollaborationHelper.Instance.BranchDriverTruckFilter.Count() > 0)
        //    //{
        //    //    retVal += " AND BranchCode " + CollaborationHelper.Instance.BranchDriverTruckFilter.Where(p => p.Value).Select(p => p.Key).ToCommaDelimitedStringValues(true).ToInClause();
        //    //}

        //    return retVal;
        //}

        /// <summary>
        /// Initializes the object. Base class subscribes to events.
        /// </summary>
        //protected override void Initialize()
        //{
        //    Logger.Debug("*** Begin Initializing Trucks ***");

        //    var url1 = MapViewModel.AGSPrefix($"{AppSettings.Instance.MapServiceName}/MapServer/0");
        //    var url2 = MapViewModel.AGSPrefix($"{AppSettings.Instance.MapServiceName}/MapServer/1");
        //    var url3 = MapViewModel.AGSPrefix($"{AppSettings.Instance.MapServiceName}/MapServer/2");

        //    Logger.Debug($"Initializing Trucks Status feature layer from URL [{url1}]");
        //    this.LayerStatus = CreateServiceFeatureLayer(new Uri(url1), FeatureRequestMode.OnInteractionNoCache);

        //    Logger.Debug($"Initializing Trucks Detailed feature layer from URL [{url2}]");

        //    this.LayerDetailed = CreateServiceFeatureLayer(new Uri(url2), FeatureRequestMode.OnInteractionNoCache);

        //    Logger.Debug($"Initializing Trucks Simple feature layer from URL [{url3}]");
        //    this.LayerSimple = CreateServiceFeatureLayer(new Uri(url3), FeatureRequestMode.OnInteractionNoCache);

        //    var refreshInterval = TimeSpan.FromSeconds(AppSettings.Instance.TrucksLayerRefreshInSeconds);
        //    Logger.Debug($"Initializing Trucks refresh interval [{refreshInterval.TotalSeconds} seconds]");
        //    this.LayerSimple.RefreshInterval = LayerStatus.RefreshInterval = LayerDetailed.RefreshInterval = refreshInterval;

        //    Logger.Debug("Updating layer names...");
        //    this.LayerSimple.Name = "Trucks";
        //    this.LayerDetailed.Name = "Trucks";
        //    this.LayerStatus.Name = "Truck Status";
        //    Logger.Debug("Updating layer visibility...");

        //    // Per Sunstate UAT request, always have layers visible upon initialization
        //    AppSettings.Instance.User.TruckLayersIsVisible = true;
        //    this.LayerSimple.IsVisible = AppSettings.Instance.User.TruckLayersIsVisible;
        //    this.LayerDetailed.IsVisible = AppSettings.Instance.User.TruckLayersIsVisible;
        //    this.LayerStatus.IsVisible = AppSettings.Instance.User.TruckStatusIsVisible;

        //    this.LayerSimple.PropertyChanged += (s, e) =>
        //    {
        //        if (e.PropertyName == nameof(FeatureLayer.IsVisible))
        //        {
        //            var value = this.LayerSimple.IsVisible;
        //            if (this.LayerDetailed.IsVisible != value)
        //            {
        //                this.LayerDetailed.IsVisible = value;
        //            }
        //            AppSettings.Instance.User.TruckLayersIsVisible = value;
        //        }
        //    };
        //    this.LayerDetailed.PropertyChanged += (s, e) =>
        //    {
        //        if (e.PropertyName == nameof(FeatureLayer.IsVisible))
        //        {
        //            var value = this.LayerDetailed.IsVisible;
        //            if (this.LayerSimple.IsVisible != value)
        //            {
        //                this.LayerSimple.IsVisible = value;
        //            }
        //            AppSettings.Instance.User.TruckLayersIsVisible = value;
        //        }
        //    };
        //    this.LayerStatus.PropertyChanged += (s, e) =>
        //    {
        //        if (e.PropertyName == nameof(FeatureLayer.IsVisible))
        //        {
        //            AppSettings.Instance.User.TruckStatusIsVisible = this.LayerStatus.IsVisible;
        //        }
        //    };

        //    Logger.Debug($"Updating layer oacity [{AppSettings.Instance.User.TruckLayersOpacity}]...");
        //    this.Layers.ForEach(p => p.Layer.Opacity = AppSettings.Instance.User.TruckLayersOpacity);

        //    Logger.Debug("Initializing layers collection...");
        //    this.Layers.Add(new LayerContentInfo(LayerStatus, MapLayerType.Trucks, this));
        //    base.Layers.Add(new LayerContentInfo(LayerDetailed, MapLayerType.Trucks, this));
        //    base.Layers.Add(new LayerContentInfo(LayerSimple, MapLayerType.Trucks, this));

        //    Logger.Debug("Updating model opacity...");
        //    // Populate with user setting
        //    base.Opacity = AppSettings.Instance.User.TruckLayersOpacity;

        //    Logger.Debug("Updating definition expression...");
        //    this.UpdateDefinitionExpression();
        //    Logger.Debug($"Definition expression set to [{this._previousDefinitionExpression}]");

        //    // Listen for changes to dispatch filters and update definition expression accordingly   
        //    Logger.Debug("Subscribing to truck type filter changes...");
        //    CollaborationHelper.Instance.TruckTypeFilterChanged += (s, e) => UpdateDefinitionExpression();

        //    Logger.Debug("Subscribing to Truck Code filter changes...");
        //    CollaborationHelper.Instance.TruckCodePartialChanged += (s, e) => UpdateDefinitionExpression();

        //    Logger.Debug("Subscribing to Driver Name filter changes...");
        //    CollaborationHelper.Instance.DriverNamePartialChanged += (s, e) => UpdateDefinitionExpression();

        //    Logger.Debug("Subscribing to Driver Name filter changes...");
        //    CollaborationHelper.Instance.BranchDriverTruckFilterChanged += (s, e) => UpdateDefinitionExpression();

        //    //
        //    Logger.Debug("Initializing base...");
        //    base.Initialize();

        //    Logger.Debug($"Initializing Dispatch identfy capabilities...");
        //    this.IsIdentifiable = AppSettings.Instance.User.TruckhLayersIsIdentifiable;
        //    this.UpdateLayerIsIdentifiable(this.IsIdentifiable);

        //    Logger.Debug("*** End Initializing Trucks ***");
        //}

        /// <summary>
        /// Initializes the override/customization of symbols based on the dd.stylx file.
        /// </summary>
        /// <param name="styleId">The ID key embedded and defined in the stylx file. </param>
        //internal void InitializeSymbolStyle(int styleId = 0)
        //{
        //    try
        //    {
        //        if (this.LayerSimple.LoadStatus == LoadStatus.Loaded)
        //        {
        //            Logger.Debug($"Trucks Simple layer LoadStatus: [{LayerSimple.LoadStatus}]");
        //            if (_currentStyleId == styleId)
        //            {
        //                Logger.Debug("Trucks Simple layer style already set");
        //                return;
        //            }
        //            _currentStyleId = styleId;
        //            var renderer = this.LayerSimple.Renderer as UniqueValueRenderer;
        //            if (renderer == null)
        //            {
        //                Logger.Debug("Truck Simple layer initial renderer is null. Trying again in 3 seconds...");
        //                Task.Delay(3000);
        //                renderer = this.LayerSimple.Renderer as UniqueValueRenderer;

        //                if (renderer == null)
        //                {
        //                    Logger.Debug("Trucks Simple layer initial renderer is still null. Trying LayerInfo approach...");
        //                    var table = this.LayerSimple?.FeatureTable as ArcGISFeatureTable;
        //                    renderer = table?.LayerInfo?.DrawingInfo?.Renderer as UniqueValueRenderer;
        //                }
        //            }
        //            if (renderer != null)
        //            {
        //                var styles = StyleHelper.TruckStyles;
        //                if (styles != null)
        //                {
        //                    renderer.UniqueValues.ForEach(p =>
        //                    {
        //                        var value = p.Values.FirstOrDefault();
        //                        if (value != null)
        //                        {
        //                            var searchKey = "T" + value.ToString().Trim() + styleId.ToString();
        //                            var style = styles.FirstOrDefault(s => s.Key == searchKey);
        //                            if (style != null)
        //                            {
        //                                p.Symbol = style.Symbol;
        //                                p.Label = style.Name;
        //                            }

        //                        }
        //                    });
        //                    this.LayerSimple.Renderer = renderer;
        //                    NotifyStyleChanged();
        //                }
        //            }
        //            else
        //            {
        //                Logger.Error($"Trucks Simple layer renderer is null");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Error(ex);
        //    }
        //}

        ///// <summary>
        ///// Gets the extent of truck features.
        ///// </summary>
        ///// <param name="truckCodes">IDs to get extent for.</param>
        ///// <returns></returns>
        //public async Task<Envelope> GetTruckFeatureExtent(List<string> truckCodes)
        //{
        //    if (truckCodes.Count > 0 &&
        //        this.LayerDetailed.LoadStatus == ArcGISRuntime.LoadStatus.Loaded &&
        //        this.LayerDetailed.DefinitionExpression.IsNotNullOrEmpty())
        //    {
        //        var whereClause = this.LayerDetailed.DefinitionExpression;
        //        whereClause += " AND TruckCode " + truckCodes.ToCommaDelimitedStringValues(true).ToInClause();
        //        var query =
        //            new ArcGISRuntime.Data.QueryParameters()
        //            {
        //                WhereClause = whereClause
        //            };
        //        return await this.LayerDetailed.FeatureTable.QueryExtentAsync(query);
        //    }
        //    return null;
        //}

        ///// <summary>
        ///// Gets the extent of truck features.
        ///// </summary>
        ///// <param name="driverCodes">IDs to get extent for.</param>
        ///// <returns></returns>
        //public async Task<Envelope> GetDriverFeatureExtent(List<string> driverCodes)
        //{
        //    if (driverCodes.Count > 0 &&
        //        this.LayerDetailed.LoadStatus == ArcGISRuntime.LoadStatus.Loaded &&
        //        this.LayerDetailed.DefinitionExpression.IsNotNullOrEmpty())
        //    {
        //        var whereClause = this.LayerDetailed.DefinitionExpression;
        //        whereClause += " AND DriverCode " + driverCodes.ToCommaDelimitedStringValues(true).ToInClause();
        //        var query =
        //                new ArcGISRuntime.Data.QueryParameters()
        //                {
        //                    WhereClause = whereClause
        //                };
        //        return await this.LayerDetailed.FeatureTable.QueryExtentAsync(query);
        //    }
        //    return null;
        //}

        ///// <summary>
        ///// Gets a point collection representation of the given truck features.
        ///// </summary>
        ///// <param name="dispatchIDs">IDs to get points of.</param>
        ///// <returns></returns>
        //public async Task<List<MapPoint>> GetTruckFeaturePointCollection(List<string> truckCodes)
        //{
        //    if (truckCodes.Count > 0 &&
        //        this.LayerDetailed.LoadStatus == ArcGISRuntime.LoadStatus.Loaded &&
        //        this.LayerDetailed.DefinitionExpression.IsNotNullOrEmpty())
        //    {

        //        var whereClause = this.LayerDetailed.DefinitionExpression;
        //        whereClause += " AND TruckCode " + truckCodes.ToCommaDelimitedStringValues(true).ToInClause();
        //        var query =
        //                new ArcGISRuntime.Data.QueryParameters()
        //                {
        //                    WhereClause = whereClause
        //                };
        //        var result = await this.LayerDetailed.FeatureTable.QueryFeaturesAsync(query);

        //        var pointCollection = new List<MapPoint>();
        //        result.ForEach(p =>
        //        {
        //            var point = p.Geometry as MapPoint;
        //            if (point != null && !point.IsEmpty)
        //            {
        //                pointCollection.Add(point);
        //            }
        //        });
        //        return pointCollection;
        //    }
        //    return null;
        //}

        ///// <summary>
        ///// Gets a point collection representation of the given drivers' truck features.
        ///// </summary>
        ///// <param name="dispatchIDs">IDs to get points of.</param>
        ///// <returns></returns>
        //public async Task<List<MapPoint>> GetDriverFeaturePointCollection(List<string> truckCodes)
        //{
        //    if (truckCodes.Count > 0 &&
        //        this.LayerDetailed.LoadStatus == ArcGISRuntime.LoadStatus.Loaded &&
        //        this.LayerDetailed.DefinitionExpression.IsNotNullOrEmpty())
        //    {
        //        var whereClause = this.LayerDetailed.DefinitionExpression;
        //        whereClause += " AND DriverCode " + truckCodes.ToCommaDelimitedStringValues(true).ToInClause();
        //        var query =
        //                new ArcGISRuntime.Data.QueryParameters()
        //                {
        //                    WhereClause = whereClause
        //                };
        //        var result = await this.LayerDetailed.FeatureTable.QueryFeaturesAsync(query);

        //        var pointCollection = new List<MapPoint>();
        //        result.ForEach(p =>
        //        {
        //            var point = p.Geometry as MapPoint;
        //            if (point != null && !point.IsEmpty)
        //            {
        //                pointCollection.Add(point);
        //            }
        //        });
        //        return pointCollection;
        //    }
        //    return null;
        //}

        ///// <summary>
        ///// Updates state of sub layer IsIdentifiable state.
        ///// </summary>
        ///// <param name="value">Value to set.</param>
        //protected override void UpdateLayerIsIdentifiable(bool value)
        //{
        //    this.Layers.ForEach(p =>
        //    {
        //        // Special case: Do not identify the truck status layer
        //        if (p.Layer == this.LayerStatus)
        //        {
        //            p.IsIdentifiable = false;
        //        }
        //        else
        //        {
        //            p.IsIdentifiable = value;
        //        }
        //    });
        //}

        #endregion

    }
}
