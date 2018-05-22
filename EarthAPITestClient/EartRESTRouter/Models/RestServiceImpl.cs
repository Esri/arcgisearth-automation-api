using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Threading.Tasks;

using System.ServiceModel.Web;

namespace ArcGISEarth.WCFNamedPipeIPC
{
    public class RestServiceImpl : IRestServiceImpl
    {
        RestServiceImpl()
        {
            // init utils
        }


        public Stream GetSnapshotFile()
        {
            return null;
        }

        public Stream GetSnapshot()
        {
            string TmpFolderPath = System.IO.Path.GetTempPath();
            TmpFolderPath += "tmp.jpg";
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
            //            Camera ca = this._viewServices.Camera;
            //            return JsonHelper.CameraToJson(ca);
            return null;
        }

        public string GetOperationalLayers()
        {
            return null;
        }

        public string RemoveOperationalLayers()
        {
            return null;
        }

//        public string AddLayers(EarthLayerDescription lyr)
//        {
//            if (lyr != null)
//            {
//                EarthLayerDescriptionImpl lyrImpl = lyr as EarthLayerDescriptionImpl;
//                return lyrImpl.Add2APP(sv);
//            }
//            return null;
//        }


        public string AddOpertionalLayer(LayerShellContract lyr)
        {
            //            if (lyr != null)
            //            {
            //                List<string> sources = new List<string>();
            //                sources.Add(lyr.url);
            //                System.Threading.Tasks.Task<bool> sucess = sv.AddFiles(sources.ToArray());
            //                if (sucess.Result == true)
            //                    return lyr.url;
            //            }
            //            return "lyr=null";
            return null;
        }


        public string FlyTo(string json)
        {
            return null;
        }

        public string UpdateCamera(string json)
        {
            //            Camera camera = JsonHelper.CameraFromJson(json);
            //            if (camera != null)
            //            {
            //                try
            //                {
            //                    this._viewServices.SetViewAsync(camera, new TimeSpan(0, 0, 5));
            //                }
            //                catch (Exception)
            //                {
            //                    return "error";
            //                }
            //            }
            //            return json;
            return null;
        }

        public string AddLayerSync(EarthLayerDescription lyr)
        {
            throw new NotImplementedException();
        }

        public string GetLayerInformation(EarthLayerDescription lyr)
        {
            throw new NotImplementedException();
        }

        public string GetLayersInformation(EarthLayerDescription lyr)
        {
            throw new NotImplementedException();
        }

        public string ImportLayers(EarthLayerDescription lyr)
        {
            throw new NotImplementedException();
        }

        public string ClearLayers(EarthLayerDescription lyr)
        {
            throw new NotImplementedException();
        }

        public string RemoveLayer(EarthLayerDescription lyr)
        {
            throw new NotImplementedException();
        }
    }
}
