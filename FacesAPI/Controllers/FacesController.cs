using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FacesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacesController : ControllerBase
    {   
        [HttpPost]
        public async Task<Tuple<List<byte[]>, Guid>> ReadFaces(Guid orderId)
        {
            using var ms = new MemoryStream(2048);
            await Request.Body.CopyToAsync(ms);
            var faces = GetFaces(ms.ToArray());
            return new Tuple<List<byte[]>, Guid>(faces, orderId);
        }

        private List<byte[]> GetFaces(byte[] image)
        {
            var faceList = new List<byte[]>
            {
                image,
                image
            };
            //Mat src = Cv2.ImDecode(image, ImreadModes.Color);
            //src.SaveImage("faces.jpg", new ImageEncodingParam(ImwriteFlags.JpegProgressive, 255));
            //var cascadeFile = Path.Combine(Directory.GetCurrentDirectory(), "CascadeFile", "haarcascade_frontalface_default.xml");
            //var faceCascade = new CascadeClassifier();
            //faceCascade.Load(cascadeFile);
            //var faces = faceCascade.DetectMultiScale(src, 1.1, 6, HaarDetectionTypes.DoRoughSearch, new Size(60, 60));
            //int j = 0;
            //foreach(var face in faces)
            //{
            //    var faceImg = new Mat(src, face);
            //    faceList.Add(faceImg.ToBytes(".jpg"));
            //    faceImg.SaveImage("face" + j + ".jpg", new ImageEncodingParam(ImwriteFlags.JpegProgressive, 255));
            //    j++;
            //}
            return faceList;
        }
    }
}
