using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Rimango.ImageField.Settings;

namespace Rimango.ImageField.Helper
{
    public class TransformationHelper
    {
        public static Dimensions GetTransformDimensionsOnMax(Dimensions actualDimensions, Dimensions maxDimensions) {
            // apply transformation
            int newWidth = maxDimensions.Width > 0 && actualDimensions.Width > maxDimensions.Width
                ? maxDimensions.Width
                : actualDimensions.Width;
            float widthFactor = actualDimensions.Width / (float)newWidth;
            int newHeight = maxDimensions.Height > 0 && actualDimensions.Height > maxDimensions.Height
                ? maxDimensions.Height
                : actualDimensions.Height;
            float heightFactor = actualDimensions.Height / (float)newHeight;

            double epsilon =  Math.Pow(10.0, -10) ;
            if (Math.Abs(widthFactor - heightFactor) > epsilon)
            {
                if (widthFactor > heightFactor)
                {
                    newHeight = Convert.ToInt32(actualDimensions.Height / widthFactor);
                }
                else
                {
                    newWidth = Convert.ToInt32(actualDimensions.Width / heightFactor);
                }
            }

            return new Dimensions(newWidth,newHeight);
        }

        public static Dimensions GetTransformDimensinonsOnWidth(Dimensions actualDimensions, Dimensions maxDimensions)
        {
            int newWidth = maxDimensions.Width > 0 && actualDimensions.Width > maxDimensions.Width
                ? maxDimensions.Width
                : actualDimensions.Width;
            float widthFactor = actualDimensions.Width / (float)newWidth;

            int newHeight = Convert.ToInt32(actualDimensions.Height / widthFactor);

            return new Dimensions(newWidth, newHeight);
        }

        public static Dimensions GetTransformDimensionsOnHeight(Dimensions actualDimensions, Dimensions maxDimensions)
        {
            int newHeight = maxDimensions.Height > 0 && actualDimensions.Height > maxDimensions.Height
              ? maxDimensions.Height
              : actualDimensions.Height;
            float heightFactor = actualDimensions.Height / (float)newHeight;

            int newWidth = Convert.ToInt32(actualDimensions.Width / heightFactor);

            return new Dimensions(newWidth, newHeight);
        }
    }
}