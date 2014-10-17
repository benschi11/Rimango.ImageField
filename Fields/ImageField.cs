using Orchard.ContentManagement;
using Orchard.ContentManagement.FieldStorage;

namespace Rimango.ImageField.Fields {
    public class ImageField : ContentField {
        public string FileName {
            get { return Storage.Get<string>(); }
            set { Storage.Set(value); }
        }

        public string AlternateText {
            get { return Storage.Get<string>("AlternateText"); }
            set { Storage.Set("AlternateText", value); }
        }

        public int Width {
            get { return Storage.Get<int>("Width"); }
            set { Storage.Set("Width", value); }
        }

        public int Height {
            get { return Storage.Get<int>("Height"); }
            set { Storage.Set("Height", value); }
        }

    }
}
