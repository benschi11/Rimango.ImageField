using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Rimango.ImageField.Settings;

namespace Rimango.ImageField.ViewModels
{
    public class ImageFieldViewModel
    {
        public ImageFieldViewModel()
        {
            Coordinates = new Coordinates();
        }
        public ImageFieldSettings Settings { get; set; }
        public Fields.ImageField Field { get; set; }
        public string AlternateText { get; set; }
        public bool Removed { get; set; }

        public Coordinates Coordinates { get; set; }

        public int CropedWidth { get; set; }
        public int CropedHeight { get; set; }

    }

    public class Coordinates {
        public int x { get; set; }
        public int y { get; set; }
    }
}