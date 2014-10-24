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
            crop: function(name) {
                $("#" + this.getFileElementId(name)).change(function(e) {
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


                        reader.onloadend = function() {
                            img.src = reader.result;


                            var $dialog = $('<div><div class="jc-dialog"><img src="' + reader.result + '" /></div></div>');

                            $dialog.find('img').Jcrop({
                                onSelect: updateCoords,
                                onChange: updateCoords,
                                bgColor: 'black',
                                bgOpacity: .4,
                                setSelect: [100, 100, 50, 50],
                                aspectRatio: (settings.keepRatioOnly == 1 ? settings.width / settings.height : 0)
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

                                        },
                                        "Close": function() {
                                            resetPreviewImage(name);
                                            $(this).dialog('close');
                                        }
                                    }
                                });
                            });

                            function resetPreviewImage(imageFieldName) {
                                var previewImg = $('#' + $.RimangoImageField.getPreLoadImageId(imageFieldName));
                                previewImg.removeAttr("style");
                            }

                            function updateCoords(coords) {
                                var rx = 100 / coords.w;
                                var ry = 100 / coords.h;

                                var previewImg = $('#' + $.RimangoImageField.getPreLoadImageId(name));

                                var previewHeight = previewImg[0].naturalHeight;
                                var previewWidth = previewImg[0].naturalWidth;

                                previewImg.css({
                                    width: Math.round(rx * previewWidth) + 'px',
                                    height: Math.round(ry * previewHeight) + 'px',
                                    marginLeft: '-' + Math.round(rx * coords.x) + 'px',
                                    marginTop: '-' + Math.round(ry * coords.y) + 'px'
                                });

                                $("#" + $.RimangoImageField.getHiddenFieldId(name, "Coordinates_x")).val(coords.x);
                                $("#" + $.RimangoImageField.getHiddenFieldId(name, "Coordinates_y")).val(coords.y);
                                $("#" + $.RimangoImageField.getHiddenFieldId(name, "CropedWidth")).val(coords.w);
                                $("#" + $.RimangoImageField.getHiddenFieldId(name, "CropedHeight")).val(coords.h);

                            };
                        }
                        reader.readAsDataURL(file);

                        previewImgDiv.remove("img");
                        previewImgDiv.append(img);

                    }


                });
            }
        };
    })();
});







