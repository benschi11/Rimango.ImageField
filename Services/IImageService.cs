using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rimango.ImageField.Services
{
    using System.Drawing;

    using Orchard;

    using Rimango.ImageField.Settings;

    public interface IImageService : IDependency
    {
        bool ValidateImage(Image img, Dimensions validateDimensions, ValidationType type);

        Bitmap Resize(Image img, Dimensions resizeDimensions, ResizeType type);

        Bitmap Crop(Image img, Point leftUperEdge, int width, int height);

        Bitmap Crop(Image img, Point leftUperEdge, Dimensions cropDimensions);
    }

    public enum ValidationType
    {
        Max,
        Min,
        Exact
    }

    public enum ResizeType
    {
        KeepRatioOnWidth,
        KeepRatioOnHeight,
        IgnoreRatio,
        KeepRatioOnMax
    }
}
