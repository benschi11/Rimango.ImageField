using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Builders;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.ContentManagement.ViewModels;

namespace Rimango.ImageField.Settings
{
    public class ImageFieldEditorEvents : ContentDefinitionEditorEventsBase
    {

        public override IEnumerable<TemplateViewModel> PartFieldEditor(ContentPartFieldDefinition definition)
        {
            if (definition.FieldDefinition.Name == "ImageField")
            {
                var model = definition.Settings.GetModel<ImageFieldSettings>();
                yield return DefinitionTemplate(model);
            }
        }

        public override IEnumerable<TemplateViewModel> PartFieldEditorUpdate(ContentPartFieldDefinitionBuilder builder, IUpdateModel updateModel)
        {
            var model = new ImageFieldSettings();
            if (updateModel.TryUpdateModel(model, "ImageFieldSettings", null, null))
            {
                builder.WithSetting("ImageFieldSettings.Hint", model.Hint);
                builder.WithSetting("ImageFieldSettings.MaxHeight", model.MaxHeight.ToString());
                builder.WithSetting("ImageFieldSettings.MaxWidth", model.MaxWidth.ToString());
                builder.WithSetting("ImageFieldSettings.Required", model.Required.ToString());
                builder.WithSetting("ImageFieldSettings.MediaFolder", model.MediaFolder);
                builder.WithSetting("ImageFieldSettings.FileName", model.FileName);
                builder.WithSetting("ImageFieldSettings.AlternateText", model.AlternateText.ToString());
                builder.WithSetting("ImageFieldSettings.ResizeAction", model.ResizeAction.ToString());
                builder.WithSetting("ImageFieldSettings.UserCropOption", model.UserCropOption.ToString());
            }

            yield return DefinitionTemplate(model);
        }
    }
}