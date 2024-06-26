# ArcGIS Pro to ArcGIS Earth
Use ArcGIS Pro 3.0 or later versions to control and communicate with ArcGIS Earth.

### Build
Build and compile the add-in using the provided source code. Modify the code where necessary in your own program. If your ArcGIS Pro installation directory is not `C:\Program Files\ArcGIS\Pro`, you need to modify the value of `ProInstallDir` in the `ArcGISProToArcGISEarth.csproj` file to match your custom installation directory.

### Use
1. Double-click `ArcGISProToArcGISEarth.esriAddinX` to install.
2. Start ArcGIS Pro 3.0 or later versions. The add-in will appear on the Add-In tab if it’s installed successfully.
3. Start ArcGIS Earth and ensure the Automation API is enabled. See the API [Administrator configuration](http://doc.arcgis.com/en/arcgis-earth/automation-api/use-api.htm#GUID-341A72C0-C868-4733-B868-57389BACD9F6) for more details.
4. Make sure a global scene viewing mode is open in ArcGIS Pro.
5. Select sync functions by clicking the Synchronization button on the ArcGIS Pro Add-In tab.
6. Enjoy the tool!
