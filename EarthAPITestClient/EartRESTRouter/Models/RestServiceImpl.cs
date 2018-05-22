using System;
using System.IO;
using System.ServiceModel.Web;
using EarthAPIUtils;

namespace ArcGISEarth.WCFNamedPipeIPC
{
    public class RestServiceImpl : IRestServiceImpl
    {
        private static EarthNamedpipeAPIUtils _utils = null;

        RestServiceImpl()
        {
            if(_utils == null)
            {
                _utils = new EarthNamedpipeAPIUtils();
                _utils.Connect();
            }
        }


        public Stream GetSnapshotFile()
        {
            return null;
        }

        public Stream GetSnapshot()
        {
            string TmpFolderPath = System.IO.Path.GetTempPath();
            TmpFolderPath += "tmp.jpg";
            _utils.GetSnapshot(TmpFolderPath);
            WebOperationContext.Current.OutgoingResponse.ContentType = "image/jpg";
            try
            {
                FileStream f = new FileStream(TmpFolderPath, FileMode.Open);
                int length = (int)f.Length;
                WebOperationContext.Current.OutgoingResponse.ContentLength = length;
                byte[] buffer = new byte[length];
                int sum = 0;
                int count;
                while ((count = f.Read(buffer, sum, length - sum)) > 0)
                {
                    sum += count;
                }
                f.Close();
                MemoryStream ms = new MemoryStream(buffer);
                ms.Position = 0;
                return ms;
            }
            catch (Exception)
            {
            }
            return null;
        }

        public string GetCamera()
        {
            return _utils.GetCamera();
        }

        public string FlyTo(string json)
        {
            return _utils.FlyTo(json);
        }

        public string UpdateCamera(string json)
        {
            return _utils.SetCamera(json);
        }

        public string AddLayerSync(EarthLayerDescription lyr)
        {
            EarthLayerDescriptionImpl earthLayerDescriptionImpl = new EarthLayerDescriptionImpl(lyr);
            return _utils.AddLayerSync(earthLayerDescriptionImpl.ToJson());
        }

        public string GetLayerInformation(string layerId)
        {
            string json = "{ \"id\":\"" + layerId + "\"}";
            return _utils.GetLayerInformation(json);
        }

        public string GetLayersInformation(/*EarthLayerDescription lyr*/)
        {
            //EarthLayerDescriptionImpl earthLayerDescriptionImpl = lyr as EarthLayerDescriptionImpl;
            return _utils.GetLayersInformation(/*earthLayerDescriptionImpl.ToJson()*/"null");
        }

        public string ImportLayers(EarthLayerDescription lyr)
        {
            EarthLayerDescriptionImpl earthLayerDescriptionImpl = lyr as EarthLayerDescriptionImpl;
            return _utils.ImportLayers(earthLayerDescriptionImpl.ToJson());
        }

        public string ClearLayers(EarthLayerDescription lyr)
        {
            EarthLayerDescriptionImpl earthLayerDescriptionImpl = lyr as EarthLayerDescriptionImpl;
            return _utils.ClearLayers(earthLayerDescriptionImpl.ToJson());
        }

        public string RemoveLayer(string layerId)
        {
            string json = "{ \"id\":\"" + layerId + "\"}";
            return _utils.RemoveLayer(json);
        }
    }
}
