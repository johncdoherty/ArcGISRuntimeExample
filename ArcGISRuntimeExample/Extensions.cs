using Esri.ArcGISRuntime.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcGISRuntimeExample
{

    public static class IEnumerableExtensions
    {
        public static void ForEach<T>(
            this IEnumerable<T> source,
            Action<T> action)
        {
            foreach (T element in source)
                action(element);
        }
    }

    public static class GeometryExtensions
    {

        public static Geometry ToWgs84(this Geometry geometry)
        {
            return GeometryEngine.Project(geometry, SpatialReferences.Wgs84);
        }

        public static Geometry ToWebMercator(this Geometry geometry)
        {
            return GeometryEngine.Project(geometry, SpatialReferences.WebMercator);
        }

        private static Geometry Project(this Geometry geometry, SpatialReference spatialReference)
        {
            if (geometry != null && geometry.IsEmpty && geometry.SpatialReference != null)
            {
                return null;
            }
            // Create a geographic transformation step for transform WKID 108055, WGS_1984_To_MSK_1942
            GeographicTransformationStep geoStep = new GeographicTransformationStep(spatialReference.Wkid);

            //// Create the transformation
            //GeographicTransformation geoTransform = new GeographicTransformation(geoStep);

            // Project to a coordinate system 
            return GeometryEngine.Project(geometry, spatialReference);

        }

        public static bool IsWithin(this MapPoint geometry, Envelope envelope)
        {
            return GeometryEngine.Within(geometry, envelope);
        }
    }

    public static class EnvelopeExtensions
    {

        /// <summary>
        /// Creates an envelope with a slightly larger area if current area is zero.
        /// </summary>
        /// <param name="envelope">Envelope to resize.</param>
        /// <returns>New larger envelope or current envelope if already > zero area.</returns>
        public static Envelope ToNonZeroAreaEnvelope(this Envelope envelope)
        {
            if (envelope != null &&
                !envelope.IsEmpty &&
                envelope.XMax == envelope.XMin &&
                envelope.YMin == envelope.YMax)
            {
                return new Envelope(
                    envelope.XMin * 0.9999,
                    envelope.YMin * 0.9999,
                    envelope.XMax * 1.0001,
                    envelope.YMax * 1.0001,
                    envelope.SpatialReference);
            }
            return envelope;
        }


    }

    /// <summary>
    /// Extensions for <see cref="System.String"/>
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// A nicer way of calling <see cref="System.String.IsNullOrEmpty(string)"/>
        /// </summary>
        /// <param name="value">The string to test.</param>
        /// <returns>true if the value parameter is null or an empty string (""); otherwise, false.</returns>
        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        /// <summary>
        /// A nicer way of calling the inverse of <see cref="System.String.IsNullOrEmpty(string)"/>
        /// </summary>
        /// <param name="value">The string to test.</param>
        /// <returns>true if the value parameter is not null or an empty string (""); otherwise, false.</returns>
        public static bool IsNotNullOrEmpty(this string value)
        {
            return !value.IsNullOrEmpty();
        }

        /// <summary>
        /// A nicer way of calling <see cref="System.String.Format(string, object[])"/>
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <returns>A copy of format in which the format items have been replaced by the string representation of the corresponding objects in args.</returns>
        public static string FormatWith(this string format, params object[] args)
        {
            return string.Format(format, args);
        }

        /// <summary>
        /// Allows for using strings in null coalescing operations
        /// </summary>
        /// <param name="value">The string value to check</param>
        /// <returns>Null if <paramref name="value"/> is empty or the original value of <paramref name="value"/>.</returns>
        public static string NullIfEmpty(this string value)
        {
            if (value == string.Empty)
                return null;

            return value;
        }
    }
    }
