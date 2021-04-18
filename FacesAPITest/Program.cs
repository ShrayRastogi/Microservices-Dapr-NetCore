using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace FacesAPITest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var imagePath = @"oscars-2017.jpg";
            var orderId = Guid.NewGuid();
            var urlAddress = "http://localhost:6000/api/faces?orderId=" + orderId;
            var uri = new Uri(urlAddress);
            var bytes = ImageUtility.ConvertToBytes(imagePath);
            Tuple<List<byte[]>, Guid> faceList = null;
            var byteContent = new ByteArrayContent(bytes);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            using var httpClient = new HttpClient();
            using var res = await httpClient.PostAsync(uri, byteContent);
            string apiResponse = await res.Content.ReadAsStringAsync();
            faceList = JsonConvert.DeserializeObject<Tuple<List<byte[]>, Guid>>(apiResponse);
            if (faceList.Item1.Count > 0)
            {
                for (int i = 0; i < faceList.Item1.Count; i++)
                {
                    ImageUtility.FromBytesToArray(faceList.Item1[i], "face" + i);
                }
            }
        }
    }
}
