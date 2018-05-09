
(function ($) {


    var appData = new Vue({
        el: "#app",
        data: {
            typeList: pageData.types,
            attrList: pageData.attrs,
            entity: {
                ProductTypeId: null,
                Name: "",
                Description: "",
                UnitName: "份",
                MinBuyQuantity: 1,
                Formats: [
                    { ID: 0, Code: "", Name: "", Price: 0, Stock: -1, PackingPrice: 0, PackingQuantity: 1 }
                ],
                Attributes: []
            },
            imgsrc: null,
            showError: false
        },
        methods: {
            addType: function () {
                $.view({
                    name: "typeView",
                    title: "添加分类",
                    footDisplay: "block",
                    template: '<div class="panel-body form-horizontal">\
                            <div class="form-group">\
                                <label class= "control-label col-xs-2">\
                                    <span class="require">分类名称</span>\
                                </label>\
                            <div class="col-xs-9">\
                                <input type="text" autofocus placeholder="请填写分类名称" class="form-control" />\
                            </div>\
                        </div>'
                });
            },
            save: function (flag) {
                var entity = $.extend({}, this._data.entity);
                
                if (!entity.ProductTypeId || !entity.Name || !entity.UnitName || !entity.MinBuyQuantity) {
                    $.alert("请将表单填写完整！", "warning");
                    this._data.showError = true;
                    return;
                }
                // 验证是否填写规格名称
                var noNameFormat = 0;
                if (entity.Formats.length > 1) {
                    noNameFormat = entity.Formats.filter(function (obj) {
                        return !$.trim(obj.Name);
                    }).length;
                }
                if (noNameFormat > 0) {
                    $.alert("请输入规格名称");
                    return;
                }
                // 验证是否填写属性名称
                if (entity.Attributes.length > 0) {
                    for (var i = 0, len = entity.Attributes.length; i < len; i++) {
                        if (!$.trim(entity.Attributes[i].Name)) {
                            $.alert("请填写属性名称");
                            return;
                        }
                        delete entity.Attributes[i].container;
                    }
                }

                // 保存三种格式的图片：400*300，200*150，100*75
                if (this._data.imgsrc) {
                    var img = document.getElementById("img");
                    entity.img400 = this._data.imgsrc;
                    entity.img200 = compress(img, 200, 150, 400, 300);
                    entity.img100 = compress(img, 100, 75, 400, 300);
                }
                $.loading();
                axios.post("/Product/Save", entity)
                    .then(function (response) {
                        $.loaded();
                        if (!response.data.success) {
                            $.alert(response.data.msg);
                            return;
                        }
                        $.alert("保存成功");
                        setTimeout(() => {
                            if (flag) {
                                window.location.reload();
                                return;
                            }
                            window.location.href = "/Product";
                        }, 2000);
                    });
            },
            cancel: function () {
                window.location.href = "/Product";
            },
            selectFile: function () {
                $("#file").click();
            },
            removeImg: function () {
                this._data.imgsrc = null;
            },
            addFormat: function () {
                this._data.entity.Formats.push({ ID: 0, Code: "", Name: "", Price: 0, Stock: -1, PackingPrice: 0, PackingQuantity: 1 });
            },
            removeFormat: function (item) {
                this._data.entity.Formats.remove(item);
            },
            addAttr: function () {
                this._data.entity.Attributes.push({
                    ID: 0,
                    Name: "",
                    Item1: "",
                    Item2: "",
                    Item3: "",
                    Item4: "",
                    Item5: "",
                    Item6: "",
                    Item7: "",
                    Item8: "",
                    container: {
                        list: [],
                        attrLeft: 0,
                        attrTop: 0,
                        attrDisplay: false,
                        attrOpacity: 0,
                        detailLeft: 0,
                        detailTop: 0,
                        detailDisplay: false,
                        detailOpacity: 0,
                        detailIndex: 1
                    }
                });
            },
            removeAttr: function (item) {
                this._data.entity.Attributes.remove(item);
            },
            attrFocus: function (e, item) {
                item.container.attrOpacity = 1;
                item.container.attrLeft = e.target.offsetLeft;
                item.container.attrTop = e.target.offsetTop + 60;
                item.container.attrDisplay = true;
            },
            attrBlur: function (e, item) {
                item.container.attrOpacity = 0;
                setTimeout(function () {
                    item.container.attrOpacity = 0;
                    item.container.attrDisplay = false;
                }, 200);
            },
            detailFocus: function (e, item, index) {
                item.container.detailOpacity = 1;
                item.container.detailLeft = e.target.offsetLeft;
                item.container.detailTop = e.target.offsetTop + 40;
                item.container.detailDisplay = true;
                item.container.detailIndex = index;
            },
            detailBlur: function (e, item) {
                item.container.detailOpacity = 0;
                setTimeout(function () {
                    item.container.detailOpacity = 0;
                    item.container.detailDisplay = false;
                }, 200);
            },
            selectAttr: function (item, attr) {
                item.container.list = attr.childs;
                item.Name = attr.name;
            },
            selectDetail: function (item, name, index) {
                item["Item" + index] = name;
            }

        }
    });

    $(document.body).on("click", "#typeView .btn-save", function () {
        var $modal = $("#typeView"), $input = $modal.find("input"), val = $.trim($input.val());
        if (!val) {
            $.alert("请填写分类名称");
            $input.focus().closest(".form-group").addClass("has-error");
            return;
        }
        var sort = appData._data.typeList[appData._data.typeList.length - 1].sort + 1;
        var param = { name: val, sort: sort, id: 0 };
        axios.post("/Product/AddType", param)
            .then(function (response) {
                appData._data.typeList.push(response.data);
                $modal.modal("hide");
            });
    });

    // 裁剪图片
    var clipArea = new bjj.PhotoClip("#clipArea", {
        size: [400, 300], // 截取框的宽和高组成的数组。默认值为[260,260]
        outputSize: [400, 300], // 输出图像的宽和高组成的数组。默认值为[0,0]，表示输出图像原始大小
        //outputType: "jpg", // 指定输出图片的类型，可选 "jpg" 和 "png" 两种种类型，默认为 "jpg"
        file: "#file", // 上传图片的<input type="file">控件的选择器或者DOM对象
        // view: "#view", // 显示截取后图像的容器的选择器或者DOM对象
        ok: "#clipBtn", // 确认截图按钮的选择器或者DOM对象
        loadStart: function () {
            // 开始加载的回调函数。this指向 fileReader 对象，并将正在加载的 file 对象作为参数传入
            $('.cover-wrap').fadeIn();
        },
        loadComplete: function () {
            // 加载完成的回调函数。this指向图片对象，并将图片地址作为参数传入
            //console.log(this);
        },
        //loadError: function(event) {}, // 加载失败的回调函数。this指向 fileReader 对象，并将错误事件的 event 对象作为参数传入
        clipFinish: function (dataURL) {
            // 裁剪完成的回调函数。this指向图片对象，会将裁剪出的图像数据DataURL作为参数传入
            $('.cover-wrap').fadeOut();
            appData.imgsrc = dataURL;
        }
    });

    // 压缩图片
    function compress(img, theW, theH, realW, realH) {
        var canvas = document.createElement("canvas");
        canvas.width = theW;
        canvas.height = theH;
        var context = canvas.getContext("2d");
        context.drawImage(img, 0, 0, realW, realH, 0, 0, theW, theH);
        return canvas.toDataURL("image/jpeg");
    }


})(jQuery);

