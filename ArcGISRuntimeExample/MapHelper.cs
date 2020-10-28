using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcGISRuntimeExample
{
    public static class MapHelper
    {
        /// <summary>
        /// Creates a feature layer from a service URL, applying the given request mode.
        /// </summary>
        /// <param name="serviceUri"></param>
        /// <param name="requestMode"></param>
        /// <returns></returns>
        public static FeatureLayer CreateServiceFeatureLayer(Uri serviceUri, FeatureRequestMode requestMode)
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
