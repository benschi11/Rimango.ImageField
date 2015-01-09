jQuery(function($) {

    $.RimangoImageField = (function() {
        var fieldSettings = {};
        return {
            addField: function(name, settings) {
                fieldSettings[name] = settings;
            },
            getSettings: function(name) {
                return fieldSettings[name];
            },
            getFileElementId: function(name) {
                return "ImageField-" + name;
            },
            getPreLoadImageDivId: function(name) {
                return "PreLoadImageDiv-" + name;
            },
            getPreLoadImageId: function(name) {
                return "PreLoadImage-" + name;
            },
            getHiddenFieldId: function(name, coordname) {
                return this.getSettings(name).contentTypeName + "_" + coordname;
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

                if (widthFactor !== heightFactor)
                {
                    if (widthFactor > heightFactor)
                    {
                        newHeight = Math.round(actualDimensions.Height / widthFactor);
                    }
                    else
                    {
                        newWidth = Math.round(actualDimensions.Width / heightFactor);
                    }
                }

                return { Width: newWidth, Height: newHeight };
            },
            crop: function(name) {
                $("#" + this.getFileElementId(name)).change(function (e) {
                    var settings = $.RimangoImageField.getSettings(name);
                    var file = e.originalEvent.srcElement.files[0];

                    var imageType = /image.*/;

                    if (file && file.type.match(imageType)) {

                        var jcrop_api;

                        var reader = new FileReader();

                        var previewImgDivId = $.RimangoImageField.getPreLoadImageDivId(name);
                        var imageId = $.RimangoImageField.getPreLoadImageId(name);
                        var img = document.createElement("img");
                        img.id = imageId;
                        var previewImgDiv = $('#' + previewImgDivId);
                        previewImgDiv.empty();


                        reader.onloadend = function() {
                            img.src = reader.result;

                            var $dialog = $('<div><div class="jc-dialog"><img src="' + reader.result + '" /></div></div>');

                            $dialog.find('img').Jcrop({
                                onSelect: updateCoords,
                                onChange: updateCoords,
                                bgColor: 'black',
                                bgOpacity: 0.4,
                                setSelect: [0, 0, settings.width, settings.height],
                                allowResize: (settings.userCropOption === "Fixed" ? 0 : 1),
                                allowSelect: 0,
                                aspectRatio: ((settings.userCropOption === "OnlyKeepRatio" || settings.userCropOption === "Fixed") ? settings.width / settings.height : 0)
                            }, function() {
                                jcrop_api = this;
                                $dialog.dialog({
                                    modal: true,
                                    title: 'Crop your Image',
                                    close: function() { $dialog.remove(); },
                                    width: jcrop_api.getBounds()[0] + 34,
                                    resizable: false,
                                    buttons: {
                                        "Save": function() {
                                            $(this).dialog('close');
                                        },
                                        //"Close": function() {
                                        //    resetPreviewImage(name);
                                        //    $(this).dialog('close');
                                        //}
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

                                var previewHeight = previewImg[0].naturalHeight;
                                var previewWidth = previewImg[0].naturalWidth;

                                previewImg.css({
                                    width: Math.round(rx * previewWidth) + 'px',
                                    height: Math.round(ry * previewHeight) + 'px',
                                    marginLeft: '-' + Math.round(rx * coords.x) + 'px',
                                    marginTop: '-' + Math.round(ry * coords.y) + 'px'
                                });

                                console.log("(" + coords.w + "/" + coords.h + ")");

                                $("#" + $.RimangoImageField.getHiddenFieldId(name, "Coordinates_x")).val(coords.x);
                                $("#" + $.RimangoImageField.getHiddenFieldId(name, "Coordinates_y")).val(coords.y);
                                $("#" + $.RimangoImageField.getHiddenFieldId(name, "CropedWidth")).val(Math.round(coords.w));
                                $("#" + $.RimangoImageField.getHiddenFieldId(name, "CropedHeight")).val(Math.round(coords.h));

                            }
                        };
                        reader.readAsDataURL(file);

                        previewImgDiv.remove("img");
                        previewImgDiv.append(img);

                    } else {
                        $('#' + $.RimangoImageField.getPreLoadImageId(name)).remove();
                    }


                });
            }
        };
    })();
});







