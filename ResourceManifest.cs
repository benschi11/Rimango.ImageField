using Orchard.UI.Resources;

namespace Rimango.ImageField
{
    public class ResourceManifest : IResourceManifestProvider
    {
        public void BuildManifests(ResourceManifestBuilder builder) {
            var manifest = builder.Add();

            manifest.DefineStyle("Rimango.ImageField.Admin").SetUrl("Rimango.ImageField.Admin.css");
            manifest.DefineStyle("Rimango.ImageField.jCrop").SetUrl("jCrop/jquery.Jcrop.min.css", "jCrop/jquery.Jcrop.css");


            manifest.DefineScript("Rimango.ImageField.Base").SetUrl("ImageField.Base.js").SetDependencies("jQuery", "Rimango.ImageField.jCrop");
            manifest.DefineScript("Rimango.ImageField.jCrop").SetUrl("jCrop/jquery.Jcrop.min.js", "jCrop/jquery.Jcrop.js").SetDependencies("jQuery");
            //manifest.DefineScript("Rimango.ImageField.Crop").SetUrl("ImageField.Crop.js").SetDependencies("jQuery");
        }
    }
}