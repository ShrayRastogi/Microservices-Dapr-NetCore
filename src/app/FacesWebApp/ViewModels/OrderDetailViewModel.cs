using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FacesWebApp.ViewModels
{
    public class OrderDetailViewModel
    {
        public int OrderDetailId { get; set; }
        public byte[] FaceData { get; set; }
        public string ImageString { get; set; }
    }
}
