
(function ($) {

    // 修改实体对象过滤
    var imgsrc = null,
        selectTemplate, // 选择套餐的模版
        productTypes;   // 套餐可选择的产品类型列表
    if (pageData.entity) {
        pageData.entity.attributes.forEach(function (obj) {
            obj.container = {
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
            };
        });
        if (!!pageData.entity && pageData.entity.images.length > 0) {
            imgsrc = appConfig.apiUrl + "/File/Product/" + pageData.entity.businessId + "/400x300/" + pageData.entity.images[0].name + "." + pageData.entity.images[0].extensionName;
        }
    }
    var appData = new Vue({
        el: "#app",
        data: {
            typeList: pageData.types,   // 待选择的分类
            attrList: pageData.attrs,   // 待选择的属性
            productTypes: null,
            scopeList: [{ name: "外卖", value: 1, checked: false }, { name: "堂食", value: 2, checked: false }],
            entity: pageData.entity || {// 商品实体
                id: 0,
                productTypeId: null,
                name: "",
                description: "",
                unitName: "份",
                minBuyQuantity: 1,
                formats: [
                    { id: 0, code: null, name: "", price: 0, stock: -1, packingPrice: 0, packingQuantity: 1 }
                ],
                attributes: [],
                scope: 3,
                tag1: [],
                feature: 0
            },
            imgsrc: imgsrc,
            showError: false,           // 是否表单错误
            isDisabled: false
        },
        created: function () {
            var self = this;
            this.scopeList.forEach(function (obj) {
                if (self.entity.scope & obj.value) obj.checked = true;
            });
        },
        computed: {
            feature: function () {
                if (this.entity.feature == 0) return "单品";
                else if (this.entity.feature == 1) return "招牌";
                else if (this.entity.feature == 2) return "套餐";
                return "";
            }
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
                var entity = $.extend({}, this._data.entity), self = this;

                if (!entity.productTypeId || !entity.name || !entity.unitName || !entity.minBuyQuantity) {
                    $.alert("请将表单填写完整！", "warning");
                    this._data.showError = true;
                    return;
                }
                if (entity.feature == 2 && !entity.productIdSet) {
                    $.alert("请选择套餐商品！", "error");
                    return;
                }
                // 验证是否填写规格名称
                var noNameFormat = 0;
                if (entity.formats.length > 1) {
                    noNameFormat = entity.formats.filter(function (obj) {
                        return !$.trim(obj.name);
                    }).length;
                }
                if (noNameFormat > 0) {
                    $.alert("请输入规格名称");
                    return;
                }
                // 验证是否填写属性名称
                if (entity.attributes.length > 0) {
                    for (var i = 0, len = entity.attributes.length; i < len; i++) {
                        if (!$.trim(entity.attributes[i].name)) {
                            $.alert("请填写属性名称");
                            return;
                        }
                        //                        delete entity.attributes[i].container;
                    }
                }

                // 保存三种格式的图片：400*300，200*150，100*75
                if (this._data.imgsrc && this._data.imgsrc.indexOf(appConfig.apiUrl) === -1) {
                    var img = document.getElementById("img");
                    entity.img400 = this._data.imgsrc;
                    entity.img200 = compress(img, 200, 150, 400, 300);
                    entity.img100 = compress(img, 100, 75, 400, 300);
                }
                if (entity.id === 0) {
                    save.call(self, entity, flag);
                } else {
                    axios.get("/Product/IsCanUpdate/" + entity.id)
                        .then(function (res) {
                            if (!res.data.success) {
                                $.alert("折扣中的商品不允许修改！");
                                return;
                            }
                            save.call(self, entity, flag);
                        });
                }
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
                this._data.entity.formats.push({ id: 0, code: null, name: "", price: 0, stock: -1, packingPrice: 0, packingQuantity: 1 });
            },
            removeFormat: function (item) {
                this._data.entity.formats.remove(item);
            },
            addAttr: function () {
                this._data.entity.attributes.push({
                    id: 0,
                    name: "",
                    item1: "",
                    item2: "",
                    item3: "",
                    item4: "",
                    item5: "",
                    item6: "",
                    item7: "",
                    item8: "",
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
                this._data.entity.attributes.remove(item);
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
                item.container.detailLeft = e.target.offsetLeft - 40;
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
                item.name = attr.name;
            },
            selectDetail: function (item, name, index) {
                item["item" + index] = name;
            },
            selectProduct: function () {
                var self = this;
                axios.get("/Product/SelectTypes")
                    .then(function (res) {
                        productTypes = res.data;
                        axios.get("/Product/SelectProduct").then(function (res) {
                            selectTemplate = res.data;
                            self.selectProduct = function () {
                                $.view({
                                    name: "selectproduct",
                                    title: "选择套餐商品",
                                    footDisplay: "block",
                                    template: selectTemplate,
                                    load: function () {
                                        initSetMeal.call(this);
                                    },
                                    submit: function () {
                                        if (setMealVm.productList.length === 0) {
                                            appData.entity.productIdSet = null;
                                            return true;
                                        }
                                        var ids = "";
                                        appData.entity.tag1 = appData.entity.tag1.slice(0, 0);
                                        setMealVm.productList.forEach(function (obj) {
                                            ids += obj.id + ",";
                                            appData.entity.tag1.push({ key: obj.id, value: obj.name });
                                        });
                                        ids = ids.slice(0, ids.length - 1);
                                        appData.entity.productIdSet = ids;
                                        return true;
                                    }
                                });
                            };
                            self.selectProduct();
                        });
                    })
                    .catch(function (err) {
                        $.alert(err);
                    });
            }
        }
    });

    function save(entity, flag) {
        var self = this;
        var scope = 0;
        this.scopeList.forEach(function (obj) {
            if (!obj.checked) return;
            scope += obj.value;
        });
        entity.scope = scope;
        $.loading();
        this.isDisabled = true;                     // 按钮是否禁用
        var url = entity.id === 0 ? "/Product/Save" : "/Product/Update";
        axios.post(url, entity)
            .then(function (response) {
                $.loaded();
                if (!response.data.success) {
                    $.alert(response.data.msg);
                    return;
                }
                $.alert("保存成功", "success");
                setTimeout(() => {
                    if (flag) {
                        window.location.reload();
                        return;
                    }
                    window.location.href = "/Product";
                }, 2000);
            })
            .catch(function (msg) {
                self.isDisabled = false;
                $.loaded();
                $.alert(msg);
            });
    }

    var setMealVm;
    function initSetMeal() {
        var typeList = JSON.parse(JSON.stringify(productTypes));
        typeList.forEach(function (obj) { obj.checked = false; });
        setMealVm = new Vue({
            el: `#setMeal`,
            data: {
                typeList: typeList,
                productList: []
            },
            methods: {
                open: function (item) {
                    var state = item.checked;
                    this.typeList.forEach(function (type) { type.checked = false; });
                    item.checked = !state;
                },
                choice: function (product) {
                    var item = this.productList.filter(a => a.id === product.id);
                    if (item.length > 0) return;
                    this.productList.push(product);
                },
                remove: function (index) {
                    this.productList.splice(index, 1);
                }
            }
        });
        // 隐藏时销毁
        this.on("hidden.bs.modal", function () {
            if (!setMealVm) return;
            setMealVm.$destroy();
            setMealVm = null;
        });
    }

    $(document.body).on("click", "#typeView .btn-save", function () {
        var $modal = $("#typeView"), $input = $modal.find("input"), val = $.trim($input.val());
        if (!val) {
            $.alert("请填写分类名称");
            $input.focus().closest(".form-group").addClass("has-error");
            return;
        }
        var sort = appData._data.typeList.length === 0 ? 1 : (appData._data.typeList[appData._data.typeList.length - 1].sort + 1);
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

    $("#btn-closeArea").click(function () {
        $('.cover-wrap').fadeOut();
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

