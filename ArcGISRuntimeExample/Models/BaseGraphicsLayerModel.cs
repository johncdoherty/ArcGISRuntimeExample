
using ArcGISRuntimeClustering.Models;
using ArcGISRuntimeExample.Common;
using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcGISRuntimeExample.Models
{
    public abstract class BaseGraphicsLayerModel : BaseModel
    {


        public Guid ID { get; protected set; } = Guid.NewGuid();

        public GraphicsOverlay FeatureGraphicsLayer { get; set; } = new GraphicsOverlay();

        /// <summary>
        /// Initializes the object
        /// </summary>
        public abstract Task Initialize();

        /// <summary>
        /// Initializes the object
        /// </summary>
        protected virtual async Task Initialize(string url)
        {
            Logger.Debug($"Initializing Dispatch feature layer from URL [{url}]");
            var featureServiceLayer = CreateServiceFeatureLayer(new Uri(url), FeatureRequestMode.OnInteractionNoCache);

            await PopulateFeatures(featureServiceLayer);
        }

        /// <summary>
        /// Populates the graphics layer using the input feature layer
        /// </summary>
        /// <param name="featureLayer"></param>
        /// <returns></returns>
        private async Task PopulateFeatures(FeatureLayer featureLayer)
        {
            var whereClause = string.Empty;
            var query =
                    new Esri.ArcGISRuntime.Data.QueryParameters()
                    {
                    };
            var result = await featureLayer.FeatureTable.QueryFeaturesAsync(new QueryParameters() { WhereClause = "1=1"}).ConfigureAwait(false);

            var featureGraphics = new List<Graphic>();
            result.ForEach(p =>
            {
                var point = p.Geometry as MapPoint;
                if (point != null && !point.IsEmpty)
                {
                    var attributes = new Dictionary<string, object>(p.Attributes);
                    attributes["GUID"] = Guid.NewGuid();
                    attributes["Name"] = "Dispatch " + p.Attributes["FID"].ToString();
                    var graphic = new CustomGraphic(Convert.ToInt32(p.Attributes["FID"]),p.Geometry, attributes, new SimpleMarkerSymbol(SimpleMarkerSymbolStyle.Circle, Color.Red, 8));
                    FeatureGraphicsLayer.Graphics.Add(graphic);

                }
            });
        }


        /// <summary>
        /// Creates a feature layer from a service URL, applying the given request mode.
        /// </summary>
        /// <param name="serviceUri"></param>
        /// <param name="requestMode"></param>
        /// <returns></returns>
        protected FeatureLayer CreateServiceFeatureLayer(Uri serviceUri, FeatureRequestMode requestMode)
        {
            // Create feature table for the pools feature service.
            ServiceFeatureTable featureTable = new ServiceFeatureTable(serviceUri)
            {
                // Define the request mode.
                FeatureRequestMode = requestMode
            };

            // Create FeatureLayer that uses the created table.
            var featureLayer = new FeatureLayer(featureTable);
            return featureLayer;
        }

    }
}
