﻿using System;
﻿using System.Drawing.Drawing2D;
﻿using System.Drawing.Imaging;
﻿using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
﻿using JetBrains.Annotations;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Localization;
﻿using Orchard.MediaLibrary.Services;
﻿using Orchard.UI.Notify;
using System.Drawing;
﻿using Orchard.Utility.Extensions;
﻿using Rimango.ImageField.Helper;
﻿using Rimango.ImageField.Settings;
﻿using Rimango.ImageField.ViewModels;


namespace Rimango.ImageField.Driver
{
    using Rimango.ImageField.Services;

    [UsedImplicitly]
    public class ImageFieldDriver : ContentFieldDriver<Fields.ImageField>
    {
        private const string TemplateName = "Fields/Rimango.Image";
        private const string TokenContentType = "{content-type}";
        private const string TokenFieldName = "{field-name}";
        private const string TokenContentItemId = "{content-item-id}";

        private readonly IMediaLibraryService _mediaLibraryService;

        private readonly IImageService _imageService;

        public IOrchardServices Services { get; set; }

        public ImageFieldDriver(
            IOrchardServices services,
            IMediaLibraryService mediaLibraryService,
            IImageService imageservice)
        {
            Services = services;
            _mediaLibraryService = mediaLibraryService;
            _imageService = imageservice;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        private static string GetPrefix(ContentField field, ContentPart part)
        {
            return part.PartDefinition.Name + "." + field.Name;
        }

        private static string GetDifferentiator(Fields.ImageField field, ContentPart part)
        {
            return field.Name;
        }

        protected override DriverResult Display(ContentPart part, Fields.ImageField field, string displayType, dynamic shapeHelper)
        {
            var settings = field.PartFieldDefinition.Settings.GetModel<ImageFieldSettings>();
            return ContentShape("Fields_Rimango_Image", GetDifferentiator(field, part),
                () =>
                    shapeHelper.Fields_Rimango_Image( // this is the actual Shape which will be resolved (Fields/Rimango.Image.cshtml)
                        ContentPart: part, // it will allow to access the content item
                        ContentField: field,
                        Settings: settings
                        )
                );
        }

        protected override DriverResult Editor(ContentPart part, Fields.ImageField field, dynamic shapeHelper)
        {
            var settings = field.PartFieldDefinition.Settings.GetModel<ImageFieldSettings>();

            AssignDefaultMediaFolder(settings);

            var viewModel = new ImageFieldViewModel
            {
                Settings = settings,
                Field = field,
                AlternateText = field.AlternateText
            };

            return ContentShape("Fields_Rimango_Image_Edit", GetDifferentiator(field, part),
                () => shapeHelper.EditorTemplate(TemplateName: TemplateName, Model: viewModel, Prefix: GetPrefix(field, part)));
        }

        protected override DriverResult Editor(ContentPart part, Fields.ImageField field, IUpdateModel updater, dynamic shapeHelper)
        {
            var settings = field.PartFieldDefinition.Settings.GetModel<ImageFieldSettings>();
            var viewModel = new ImageFieldViewModel
            {
                Settings = settings,
                Field = field,
                Removed = false,
                //Coordinates = new Coordinates()
            };

            if (updater.TryUpdateModel(viewModel, GetPrefix(field, part), null, null))
            {
                var postedFile = ((Controller)updater).Request.Files["ImageField-" + field.Name];

                AssignDefaultMediaFolder(settings);

                var mediaFolder = FormatWithTokens(settings.MediaFolder, part.ContentItem.ContentType, field.Name, part.ContentItem.Id);

                if (postedFile != null && postedFile.ContentLength != 0)
                {

                    // read the file in memory in case we need to apply some transformation
                    var postedFileLength = postedFile.ContentLength;
                    var postedFileStream = postedFile.InputStream;
                    var postedFileData = new byte[postedFileLength];
                    var postedFileName = Path.GetFileName(postedFile.FileName);
                    postedFileStream.Read(postedFileData, 0, postedFileLength);

                    string uploadedFileName = String.Empty;

                    Image image;
                    using (var stream = new MemoryStream(postedFileData))
                    {
                        image = Image.FromStream(stream);
                    }

                    var imageDimensions = new Dimensions(image.Width, image.Height);
                    var maxDimensions = new Dimensions(settings.MaxWidth, settings.MaxHeight);

                    var newDimensions = TransformationHelper.GetTransformDimensionsOnMax(imageDimensions, maxDimensions);

                    // create a unique file name
                    var uniqueFileName = _mediaLibraryService.GetUniqueFilename(mediaFolder, postedFileName);

                    // resize the image
                    Image target = null;
                    switch (settings.ResizeAction)
                    {
                        case ResizeActions.Validate:
                            if (!_imageService.ValidateImage(image, maxDimensions, ValidationType.Max))
                            {
                                updater.AddModelError("File",
                                    T("The file is bigger than the allowed size: {0}x{1}",
                                        image.Width, image.Height));
                            }
                            else
                                target = new Bitmap(image);
                            break;
                        case ResizeActions.Resize:
                            target = _imageService.Resize(image, maxDimensions, ResizeType.KeepRatioOnMax);
                            Services.Notifier.Information(T("The image {0} has been resized to {1}x{2}",
                                field.Name.CamelFriendly(), newDimensions.Width, newDimensions.Height));
                            break;
                        case ResizeActions.Scale:
                            target = new Bitmap(image);
                            break;
                        case ResizeActions.Crop:
                            target = _imageService.Crop(image, new Point(0, 0), maxDimensions);
                            Services.Notifier.Information(T("The image {0} has been cropped to {1}x{2}",
                                field.Name.CamelFriendly(), maxDimensions.Width, maxDimensions.Height));
                            break;
                        case ResizeActions.UserCrop:
                            //target = CropImage(field, viewModel.CropedWidth, viewModel.CropedHeight, image, new Point(viewModel.Coordinates.x, viewModel.Coordinates.y));
                            target = _imageService.Crop(
                                image,
                                new Point(viewModel.Coordinates.x, viewModel.Coordinates.y),
                                viewModel.CropedWidth,
                                viewModel.CropedHeight);
                            Services.Notifier.Information(T("The image {0} has been cropped to {1}x{2}",
                                field.Name.CamelFriendly(), viewModel.CropedWidth, viewModel.CropedHeight));
                            break;
                    }


                    if (target != null)
                    {
                        using (var imageStream = new MemoryStream())
                        {
                            // keeps the original format
                            ImageFormat imageFormat;
                            switch (Path.GetExtension(postedFileName).ToLower())
                            {
                                case ".bmp":
                                    imageFormat = ImageFormat.Bmp;
                                    break;
                                case ".gif":
                                    imageFormat = ImageFormat.Gif;
                                    break;
                                case ".jpg":
                                    imageFormat = ImageFormat.Jpeg;
                                    break;
                                default:
                                    imageFormat = ImageFormat.Png;
                                    break;
                            }

                            target.Save(imageStream, imageFormat);
                            //uploadedFileName = _mediaService.UploadMediaFile(mediaFolder, uniqueFileName, imageStream.ToArray(), false);
                            uploadedFileName = _mediaLibraryService.UploadMediaFile(mediaFolder, uniqueFileName, imageStream.ToArray());
                        }

                        // assigning actual size to be rendered in html
                        field.Width = newDimensions.Width;
                        field.Height = newDimensions.Height;

                        // don't convert the url to ~/ if it's a fully qualified
                        // as it might be stored on a remote storage (cdn, blob, ...)
                        field.FileName = VirtualPathUtility.IsAbsolute(uploadedFileName)
                            ? VirtualPathUtility.ToAppRelative(uploadedFileName)
                            : uploadedFileName;
                    }
                    else
                    {
                        field.Width = 0;
                        field.Height = 0;
                        field.FileName = String.Empty;
                    }
                }
                else
                {
                    if (settings.Required && string.IsNullOrWhiteSpace(field.FileName))
                    {
                        updater.AddModelError("File", T("You must provide an image file for {0}.", field.Name.CamelFriendly()));
                    }

                    if (!settings.Required && viewModel.Removed)
                    {
                        field.FileName = null;
                        field.Height = 0;
                        field.Width = 0;

                        Services.Notifier.Information(T("{0} was removed.", field.Name.CamelFriendly()));
                    }
                }

                // define alernate text
                if (settings.AlternateText)
                {
                    field.AlternateText = String.IsNullOrWhiteSpace(viewModel.AlternateText)
                        ? postedFile == null || String.IsNullOrEmpty(postedFile.FileName) ? String.Empty : VirtualPathUtility.GetFileName(postedFile.FileName)
                        : viewModel.AlternateText;
                }
            }

            return Editor(part, field, shapeHelper);
        }

        private static string FormatWithTokens(string value, string contentType, string fieldName, int contentItemId)
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                return String.Empty;
            }

            return value
                .Replace(TokenContentType, contentType)
                .Replace(TokenFieldName, fieldName)
                .Replace(TokenContentItemId, Convert.ToString(contentItemId));

        }

        private static void AssignDefaultMediaFolder(ImageFieldSettings settings)
        {
            if (String.IsNullOrWhiteSpace(settings.MediaFolder))
            {
                settings.MediaFolder = TokenContentType + "/" + TokenFieldName;
            }
        }

        protected override void Exporting(ContentPart part, Fields.ImageField field, ExportContentContext context)
        {
            context.Element(field.FieldDefinition.Name + "." + field.Name).SetAttributeValue("AlternateText", field.AlternateText);
            context.Element(field.FieldDefinition.Name + "." + field.Name).SetAttributeValue("FileName", field.FileName);
            context.Element(field.FieldDefinition.Name + "." + field.Name).SetAttributeValue("Height", field.Height);
            context.Element(field.FieldDefinition.Name + "." + field.Name).SetAttributeValue("Width", field.Width);
        }

        protected override void Importing(ContentPart part, Fields.ImageField field, ImportContentContext context)
        {
            field.AlternateText = context.Attribute(field.FieldDefinition.Name + "." + field.Name, "AlternateText");
            field.FileName = context.Attribute(field.FieldDefinition.Name + "." + field.Name, "FileName");
            field.Height = Int32.Parse(context.Attribute(field.FieldDefinition.Name + "." + field.Name, "Height"));
            field.Width = Int32.Parse(context.Attribute(field.FieldDefinition.Name + "." + field.Name, "Width"));
        }
    }
}