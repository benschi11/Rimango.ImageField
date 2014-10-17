using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Rimango.ImageField.Settings
{
    public class ImageFieldSettings
    {
        [Range(0, 2048)]
        public int MaxWidth { get; set; }
        [Range(0, 2048)]
        public int MaxHeight { get; set; }
        public ResizeActions ResizeAction { get; set; }
        public bool Required { get; set; }
        public bool AlternateText { get; set; }
        public string Hint { get; set; }
        public string MediaFolder { get; set; }
        public string FileName { get; set; }
    }

    public enum ResizeActions
    {
        // Ensures the original image is in the boundaries
        Validate,
        // Apply binary transformation so that the new image complies with boundaries
        Resize,
        // Don't alter the image, the html code will define the size to render
        Scale,
        // If the image is out of bounds, the image is cropped
        Crop,
        // The User get an Dialog to crop the image themself
        UserCrop
    }
}