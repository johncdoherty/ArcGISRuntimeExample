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
using ArcGISRuntimeExample.Common;
using ArcGISRuntimeClustering.Models;

namespace ArcGISRuntimeExample.Models
{

    public class DispatchesModel : BaseGraphicsLayerModel
    {
        #region Constants
        private const int InvalidDispatchId = int.MinValue;
        // Feature layer example
        private const string _featureServiceURL = "https://services1.arcgis.com/fif0ERoHeJunwZ8D/arcgis/rest/services/Denver_Accidents/FeatureServer/0";//"http://sampleserver3.arcgisonline.com/ArcGIS/rest/services/SanFrancisco/311Incidents/FeatureServer/0";
        #endregion

        #region Members

        public byte _red { get; set; }
        public byte _green { get; set; }
        public byte _blue { get; set; }
        public int _size { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public DispatchesModel()
        {
        }
        #endregion

        #region Properties

        /// <summary>
        /// Property bound in UI to update the Red color of symbols.
        /// </summary>
        public byte Red
        {
            get
            {
                return _red;
            }
            set
            {
                _red = value;

                // Apply to selected graphics only if there are any, otherwie apply to all
                if (this.FeatureGraphicsLayer.SelectedGraphics.Count() > 0)
                {
                    this.FeatureGraphicsLayer?.SelectedGraphics?.ForEach(p => ((SimpleMarkerSymbol)p.Symbol).Color = Color.FromArgb(this.Red, this.Green, this.Blue));
                }
                else
                {
                    this.FeatureGraphicsLayer?.Graphics?.ForEach(p => ((SimpleMarkerSymbol)p.Symbol).Color = Color.FromArgb(this.Red, this.Green, this.Blue));
                }
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Property bound in UI to update the Green color of symbols.
        /// </summary>
        public byte Green
        {
            get
            {
                return _green;
            }
            set
            {
                _green = value;

                // Apply to selected graphics only if there are any, otherwie apply to all
                if (this.FeatureGraphicsLayer.SelectedGraphics.Count() > 0)
                {
                    this.FeatureGraphicsLayer?.SelectedGraphics?.ForEach(p => ((SimpleMarkerSymbol)p.Symbol).Color = Color.FromArgb(this.Red, this.Green, this.Blue));
                }
                else
                {
                    this.FeatureGraphicsLayer?.Graphics?.ForEach(p => ((SimpleMarkerSymbol)p.Symbol).Color = Color.FromArgb(this.Red, this.Green, this.Blue));
                }
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Property bound in UI to update the Blue color of symbols.
        /// </summary>
        public byte Blue
        {
            get
            {
                return _blue;
            }
            set
            {
                _blue = value;

                // Apply to selected graphics only if there are any, otherwie apply to all
                if (this.FeatureGraphicsLayer.SelectedGraphics.Count() > 0)
                {
                    this.FeatureGraphicsLayer?.SelectedGraphics?.ForEach(p => ((SimpleMarkerSymbol)p.Symbol).Color = Color.FromArgb(this.Red, this.Green, this.Blue));
                }
                else
                {
                    this.FeatureGraphicsLayer?.Graphics?.ForEach(p => ((SimpleMarkerSymbol)p.Symbol).Color = Color.FromArgb(this.Red, this.Green, this.Blue));
                }
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Property bound in UI to update the size of symbols.
        /// </summary>
        public int Size
        {
            get
            {
                return _size;
            }
            set
            {
                _size = value;

                // Apply to selected graphics only if there are any, otherwie apply to all
                if (this.FeatureGraphicsLayer?.SelectedGraphics.Count() > 0)
                {
                    this.FeatureGraphicsLayer?.SelectedGraphics?.ForEach(p => ((SimpleMarkerSymbol)p.Symbol).Size = _size);
                }
                else
                {
                    this.FeatureGraphicsLayer?.Graphics?.ForEach(p => ((SimpleMarkerSymbol)p.Symbol).Size = _size);
                }
                this.NotifyPropertyChanged();
            }
        }

        #endregion

        #region Methods

        public override async Task Initialize()
        {
            await base.Initialize(_featureServiceURL);
        }

        #endregion

    }
}
