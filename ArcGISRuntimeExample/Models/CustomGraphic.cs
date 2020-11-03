using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcGISRuntimeClustering.Models
{
    /// <summary>
    /// Extends a regular graphic so extra attributes can be attached
    /// </summary>
    public class CustomGraphic : Graphic
    {

        public CustomGraphic(
            int id, 
            Geometry geometry,
            IEnumerable<KeyValuePair<string, Object>> attributes,
            Symbol symbol) : base(geometry, attributes, symbol)
        {
            this.ID = id;

        }
        public int ID { get; private set; }
    }
}
