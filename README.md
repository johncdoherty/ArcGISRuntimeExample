# ArcGISRuntimeClustering

Simple example showing how various graphics, selection, and labeling functionality can be accomplished in ArcGIS Runtime 100.x. This example reads in features from an existing feature service as a data source to populate a graphics overlay in a model, which serves as a wrapper to encapsulate related functinality. This functionality could be modified to be populated using a direct database table connection.

Primary Functionality:
- Illustrates how a graphics overlay can be used to render table records using latitude/longitude fields
- Illustartes how a graphics object can be extended to embed custom properties
- Illustrates a way to display rich dynamic labels in WPF using a radial panel when clicking on features. This uses standard WPF objects and drawing them at the click location
- Illustrates how built-in ArcGIS callouts can be displayed when moving the mouse
  - Note: This can display a custom user control by using `MapView.ShowCalloutAt(mapPoint, myView);`
- Illustrates how selections can be handled on a graphics overlay
- Illustrates how symbol properties can be updated dynamically
  - Note: the properties of a symbol or renderer can also be bound directly in the UI, since ArcGIS Runtime objects implement INotifyPropertyChanges


Note: This solution is an iterative change to the [ArcGISRuntimeClustering](https://github.com/johncdoherty/ArcGISRuntimeClustering) repo, therefore that functionality is included in this solution but not enabled

![screenshot](./screenshot.png)


