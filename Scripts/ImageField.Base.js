jQuery(function ($) {

    $.RimangoImageField = (function () {
        var fieldSettings = {};
        return {
            addField: function (name, settings) {
                fieldSettings[name] = settings;
            },
            getSettings: function (name) {
                return fieldSettings[name];
            },
            getFileElementId: function (name) {
                return "ImageField-" + name;
            },
            getPreLoadImageDivId: function (name) {
                return "PreLoadImageDiv-" + name;
            },
            getPreLoadImageId: function (name) {
                return "PreLoadImage-" + name;
            },
            getCropLinkId: function(name) {
                return "CropLink-" + name;
            },
            getHiddenFieldId: function (name, coordname) {
                return this.getSettings(name).contentTypeName + "_" + coordname;
            },
            getSpinnerId: function(name) {
                return "Spinner-" + name;
            },
            resetPreviewImage: function (name) {
                var previewImgDivId = $.RimangoImageField.getPreLoadImageDivId(name);
                var previewImgDiv = $('#' + previewImgDivId);
                previewImgDiv.empty();
            },
            addReCropLink: function (name) {
                var previewImgDivId = $.RimangoImageField.getPreLoadImageDivId(name);
                var previewImgDiv = $('#' + previewImgDivId);
                if (previewImgDiv.parent().find(".reCrop").length == 0) {
                    var link = $("<a href='#' class='reCrop' id='"+$.RimangoImageField.getCropLinkId(name)+"'>New Crop</a>");
                    previewImgDiv.after(link);
                    $.RimangoImageField.addCropEventHandler(name, link);
                }
            },
            toggleSpinner: function(name) {
                var spinnerId = $.RimangoImageField.getSpinnerId(name);
                var spinner = $("#" + spinnerId);
                if (spinner.hasClass("spinner")) {
                    spinner.removeClass("spinner");
                } else {
                    spinner.addClass("spinner");
                }
            },
            resetFileElement: function (name) {
                var fileId = $.RimangoImageField.getFileElementId(name);
                var fileElement = $("#" + fileId);
                fileElement.wrap('<form>').closest('form').get(0).reset();
                fileElement.unwrap();
            },
            calculatePreviewDimension: function (actualDimensions, maxDimensions) {
                //console.log("old (w/h) (" + actualDimensions.Width + "/" + actualDimensions.Height + ")");
                var newWidth = maxDimensions.Width > 0 && actualDimensions.Width > maxDimensions.Width
                ? maxDimensions.Width
                : actualDimensions.Width;
                var widthFactor = actualDimensions.Width / newWidth;
                //console.log("Widthfactor:" + widthFactor);
                var newHeight = maxDimensions.Height > 0 && actualDimensions.Height > maxDimensions.Height
                    ? maxDimensions.Height
                    : actualDimensions.Height;
                var heightFactor = actualDimensions.Height / newHeight;
                //console.log("HeightFactor:" + heightFactor);

                if (widthFactor !== heightFactor) {
                    if (widthFactor > heightFactor) {
                        newHeight = Math.round(actualDimensions.Height / widthFactor);
                    }
                    else {
                        newWidth = Math.round(actualDimensions.Width / heightFactor);
                    }
                }

                return { Width: newWidth, Height: newHeight };
            },
            addCropEventHandler: function (name, object) {

                // Default - Add to FileElement
                if (object == null) {
                    $("#" + this.getFileElementId(name)).change(function(e) {
                        return $.RimangoImageField.crop(name);
                    });
                    // Use the Click Event of the object
                } else {
                    $(object).click(function (e) {
                        return $.RimangoImageField.crop(name);
                    });
                }
            },
            crop: function (name) {

                var settings = $.RimangoImageField.getSettings(name);
                var fileId = $.RimangoImageField.getFileElementId(name);
                var file = $("#" + fileId)[0].files[0];

                if (file === undefined) {
                    alert("No File is selected!");
                    return;
                }

                $.RimangoImageField.toggleSpinner(name);

                var imageType = /image.*/;

                if (file && file.type.match(imageType)) {

                    var jcrop_api;

                    var reader = new FileReader();

                    var previewImgDivId = $.RimangoImageField.getPreLoadImageDivId(name);
                    var imageId = $.RimangoImageField.getPreLoadImageId(name);
                    var img = document.createElement("img");
                    img.id = imageId;
                    var previewImgDiv = $('#' + previewImgDivId);
                    $.RimangoImageField.resetPreviewImage(name);


                    reader.onloadend = function () {
                        img.src = reader.result;
                    };

                    img.onload = function () {
                        var origDimensions = { Width: img.width, Height: img.height };
                        var dialogPreviewDimension = $.RimangoImageField.calculatePreviewDimension({ Width: img.width, Height: img.height }, { Width: 800, Height: 800 });

                        img.height = dialogPreviewDimension.Height;
                        img.width = dialogPreviewDimension.Width;

                        previewImgDiv.remove("img");
                        previewImgDiv.append(img);

                        var $dialog = $('<div><div class="jc-dialog"><img width="' + dialogPreviewDimension.Width + '" height="' + dialogPreviewDimension.Height + '" src="' + reader.result + '" /></div></div>');


                        $dialog.find('img').Jcrop({
                            onSelect: updateCoords,
                            onChange: updateCoords,
                            bgColor: 'black',
                            bgOpacity: 0.4,
                            setSelect: [0, 0, settings.width, settings.height],
                            allowResize: (settings.userCropOption === "Fixed" ? 0 : 1),
                            allowSelect: 0,
                            aspectRatio: ((settings.userCropOption === "OnlyKeepRatio" || settings.userCropOption === "Fixed") ? settings.width / settings.height : 0)
                        }, function () {
                            jcrop_api = this;
                            $dialog.dialog({
                                modal: true,
                                title: 'Crop your Image',
                                close: function (event) {
                                    if (event.originalEvent) {
                                        $.RimangoImageField.resetPreviewImage(name);
                                        $.RimangoImageField.resetFileElement(name);
                                    }
                                    $dialog.remove();
                                },
                                open: function () {
                                    $.RimangoImageField.toggleSpinner(name);
                                },
                                width: jcrop_api.getBounds()[0] + 34,
                                resizable: false,
                                buttons: {
                                    "Save": function () {
                                        $.RimangoImageField.addReCropLink(name);
                                        $(this).dialog('close');
                                    }
                                }
                            });
                        });



                        function updateCoords(coords) {
                            var previewDivDim = $.RimangoImageField.calculatePreviewDimension({ Width: coords.w, Height: coords.h }, { Width: 200, Height: 200 });

                            var previewDiv = $("#" + $.RimangoImageField.getPreLoadImageDivId(name));
                            console.log(previewDivDim.Width + ":" + previewDivDim.Height);
                            previewDiv.css("width", previewDivDim.Width);
                            previewDiv.css("height", previewDivDim.Height);

                            var rx = previewDivDim.Width / coords.w;
                            var ry = previewDivDim.Height / coords.h;


                            var previewImg = $('#' + $.RimangoImageField.getPreLoadImageId(name));


                            var previewHeight = dialogPreviewDimension.Height;
                            var previewWidth = dialogPreviewDimension.Width;

                            previewImg.css({
                                width: Math.round(rx * previewWidth) + 'px',
                                height: Math.round(ry * previewHeight) + 'px',
                                marginLeft: '-' + Math.round(rx * coords.x) + 'px',
                                marginTop: '-' + Math.round(ry * coords.y) + 'px'
                            });

                            console.log("(" + coords.w + "/" + coords.h + ")");


                            var widthFactor = (origDimensions.Width / dialogPreviewDimension.Width);
                            var heightFactor = (origDimensions.Width / dialogPreviewDimension.Width);

                            $("#" + $.RimangoImageField.getHiddenFieldId(name, "Coordinates_x")).val(Math.floor(coords.x * widthFactor));
                            $("#" + $.RimangoImageField.getHiddenFieldId(name, "Coordinates_y")).val(Math.floor(coords.y * heightFactor));
                            $("#" + $.RimangoImageField.getHiddenFieldId(name, "CropedWidth")).val(Math.floor(coords.w * widthFactor));
                            $("#" + $.RimangoImageField.getHiddenFieldId(name, "CropedHeight")).val(Math.floor(coords.h * heightFactor));

                        }
                    }
                    reader.readAsDataURL(file);

                    //previewImgDiv.remove("img");
                    //previewImgDiv.append(img);

                } else {
                    $('#' + $.RimangoImageField.getPreLoadImageId(name)).remove();
                }
            }
        };
    })();
});







