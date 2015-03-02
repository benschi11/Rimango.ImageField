using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Localization;
using Orchard.Tokens;

namespace Rimango.ImageField.Tokens
{
    public class ImageFieldTokens : ITokenProvider
    {
        public ImageFieldTokens() {
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public void Describe(DescribeContext context) {
            context.For("ImageField", T("ImageFields"), T("ImageFields"))
                .Token("FileName", T("Filename"), T("Returns the path of the image."),"Url");

        }

        public void Evaluate(EvaluateContext context) {
            context.For<Fields.ImageField>("ImageField")
                .Token("FileName", field => field.FileName)
                .Chain("FileName", "Url", field => field.FileName);
        }
    }
}