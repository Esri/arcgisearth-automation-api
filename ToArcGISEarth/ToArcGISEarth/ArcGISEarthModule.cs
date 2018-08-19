using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;

namespace ToArcGISEarth
{
    internal class ArcGISEarthModule : Module
    {
        private static ArcGISEarthModule _this = null;

        public static ArcGISEarthModule Current
        {
            get
            {
                return _this ?? (_this = (ArcGISEarthModule)FrameworkApplication.FindModule("ToArcGISEarth_Module"));
            }
        }

        protected override bool CanUnload()
        {
            return true;
        }
    }
}
