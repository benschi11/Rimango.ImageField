using Orchard.UI.Resources;

namespace Rimango.ImageField
{
    public class ResourceManifest : IResourceManifestProvider
    {
        public void BuildManifests(ResourceManifestBuilder builder) {
            var manifest = builder.Add();

            manifest.DefineStyle("Rimango.ImageField.Admin").SetUrl("Rimango.ImageField.Admin.css");


            manifest.DefineScript("Rimango.ImageField.Base").SetUrl("ImageField.Base.js").SetDependencies("jQuery");
            manifest.DefineScript("Rimango.ImageField.Crop").SetUrl("ImageField.Crop.js").SetDependencies("jQuery");
        }
    }
}