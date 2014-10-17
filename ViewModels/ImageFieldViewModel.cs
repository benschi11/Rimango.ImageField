using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Rimango.ImageField.Settings;

namespace Rimango.ImageField.ViewModels
{
    public class ImageFieldViewModel
    {
        public ImageFieldSettings Settings { get; set; }
        public Fields.ImageField Field { get; set; }
        public string AlternateText { get; set; }
        public bool Removed { get; set; }
    }
}