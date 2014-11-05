using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rimango.ImageField.Services
{
    using System.Drawing;
    using System.Drawing.Drawing2D;

    using Rimango.ImageField.Helper;
    using Rimango.ImageField.Settings;

    public class ImageService : IImageService
    {
        public bool ValidateImage(Image img, Dimensions validateDimensions, ValidationType type)
        {
            bool result = false;

            // No Validation when width or hight to 0
            if (validateDimensions.Width == 0 || validateDimensions.Height == 0) 
                return true;


            double indicatorHeight = validateDimensions.Height - img.Height;
            double indicatorWidth = validateDimensions.Width - img.Width;

            switch (type)
            {
                case ValidationType.Exact:
                    if (indicatorHeight == 0 && indicatorWidth == 0) 
                        result = true;
                    break;
                case ValidationType.Max:
                    if (indicatorHeight >= 0 && indicatorWidth >= 0) 
                        result = true;
                    break;
                case ValidationType.Min:
                    if (indicatorHeight <= 0 && indicatorWidth <= 0) 
                        result = true;
                    break;
            }

            return result;

        }

        public Bitmap Resize(Image img, Dimensions resizeDimensions, ResizeType type)
        {

            Dimensions newDimensions = null;
            var imgDimensions = new Dimensions(img.Width, img.Height);

            switch (type)
            {
                case ResizeType.IgnoreRatio:
                    newDimensions = resizeDimensions;
                    break;
                case ResizeType.KeepRatioOnMax:
                    newDimensions = TransformationHelper.GetTransformDimensionsOnMax(imgDimensions, resizeDimensions);
                    break;
                case ResizeType.KeepRatioOnHeight:
                    newDimensions = TransformationHelper.GetTransformDimensionsOnHeight(
                        imgDimensions,
                        resizeDimensions);
                    break;
                case ResizeType.KeepRatioOnWidth:
                    newDimensions = TransformationHelper.GetTransformDimensinonsOnWidth(imgDimensions, resizeDimensions);
                    break;
            }

            var target = Resize(img, newDimensions);

            return target;
        }

        public Bitmap Crop(Image img, Point leftUperEdge, int width, int height)
        {
            var bmpImage = new Bitmap(img);
            var bmpCrop = bmpImage.Clone(new Rectangle(leftUperEdge.X, leftUperEdge.Y, width, height), bmpImage.PixelFormat);

            return bmpCrop;
        }

        public Bitmap Crop(Image img, Point leftUperEdge, Dimensions cropDimensions)
        {
            return this.Crop(img, leftUperEdge, cropDimensions.Width, cropDimensions.Height);
        }

        private Bitmap Resize(Image img, Dimensions resizeDimensions)
        {
            var target = new Bitmap(resizeDimensions.Width, resizeDimensions.Height);
            using (var graphics = Graphics.FromImage(target))
            {
                graphics.CompositingQuality = CompositingQuality.HighSpeed;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.DrawImage(img, 0, 0, resizeDimensions.Width, resizeDimensions.Height);
            }

            return target;
        }
    }
}