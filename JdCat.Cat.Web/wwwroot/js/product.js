; (function ($) {
    var templateObj = {
        listItemType: '<div class="list-type-item form-horizontal clearfix new" data-id="{id}">\
                <label class="pull-left control-label">\
                    <span>分类名称：</span>\
                </label>\
                <div class="pull-left width-35 margin-right-20 require relative">\
                    <input type="text" class="form-control" name="name" value="{name}" placeholder="请输入分类名称" />\
                </div>\
                <div class="pull-left width-20 margin-right-20 require relative">\
                    <input type="number" value="{sort}" name="sort" class="form-control" placeholder="排序码" />\
                </div>\
                <label class="pull-left control-label">\
                    <i class="fa pull-left opr-slide opr-icon absolute fa-remove type-remove" title="删除"></i>\
                </label>\
            </div>'
    };

    $("#btnType").click(function () {
        $.view({
            name: "type", title: "添加类别", url: "/product/addtype", footDisplay: "block"
        });
    });

    $(document)
        .on("click", "#btnAddType", function () {
            var name = $("#txtTypeName").val(), $list = $(".list-type"), maxSort = 0;
            var $numbers = $list.find(":not(div.remove) input[type='number']");
            $numbers.each(function () {
                var sort = $(this).val();
                sort = sort || 0;
                sort = parseInt(sort);
                if (maxSort < sort) maxSort = sort;
            });
            var html = templateObj.listItemType.format({ id: 0, name: name, sort: maxSort + 1 });
            $list.append(html);
            $("#txtTypeName").val("");
            return false;
        })
        .on("click", "#type .btn-save", function () {
            var $list = $(".list-type");
            var $new = $list.find(".list-type-item.new"),
                $edit = $list.find(".list-type-item.edit"),
                $remove = $list.find(".list-type-item.remove");
            var data = { add: [], edit: [], remove: [] };
            $new.each(function () {
                var self = $(this);
                var name = self.find("input[name='name']").val(), sort = self.find("input[name='sort']").val();
                data.add.push({ name: name, sort: sort, id: 0 });
            });
            $edit.each(function () {
                var self = $(this);
                var name = self.find("input[name='name']").val(), sort = self.find("input[name='sort']").val(), id = self.data("id");
                data.edit.push({ name: name, sort: sort, id: id });
            });
            $remove.each(function () {
                var id = $(this).data("id");
                data.remove.push(id);
            });
            $.loading();
            $.post("/product/updatetypes", data, function (msg) {
                $.loaded();
                if (!msg.success) {
                    $.error(msg.msg);
                    return;
                }
                var types = msg.data;
                $("#type").modal("hide");
                types.forEach(function (obj) { obj.selected = false; });
                var selected = category.list.first(function (obj) { return obj.selected; });
                if (selected) {
                    types.first(function (obj) { return obj.id === selected.id; }).selected = true;
                }
                category.list = types;
            });
            return false;
        })
        .on("change", "#type input", function () {
            var self = $(this), $parent = self.closest(".list-type-item");
            if ($parent.length === 0) return;
            if ($parent.hasClass("new")) return;
            $parent.addClass("edit");
        })
        .on("click", "#type .fa-remove", function () {
            var $parent = $(this).closest(".list-type-item");
            if ($parent.hasClass("new")) {
                $parent.remove();
                return;
            }
            $.post("/product/existproduct/" + $parent.data("id"), null, function (data) {
                if (data.data) {
                    $.danger("类别下存在商品，不允许删除");
                    return;
                }
                $parent.removeClass("edit").addClass("remove");
                $parent.hide();
            });
        });

    // 方法
    var pageHandler = {
        count: function (types) {
            if (types.length === 0) {
                return 0;
            }
            var all = 0;
            types.forEach(function (obj) { all += obj.count; });
            return all;
        }
    }

    var list = pageData.types.map(function (obj) {
        obj.selected = false;
        return obj;
    });
    // 数据
    var category = new Vue({
        el: "#category",
        data: {
            list: pageData.types,
            productCount: pageHandler.count(pageData.types),
            allSelected: true
        },
        methods: {
            typeClick: function (item) {
                this.list.forEach(function (obj) { obj.selected = false; });
                item.selected = true;
                this.allSelected = false;
            },
            allClick: function () {
                this.list.forEach(function (obj) { obj.selected = false; });
                this.allSelected = true;
            }
        }
    });
    // 过滤器
    Vue.filter("stock", function (num) {
        return num < 0 ? "无限" : num;
    });
    Vue.filter("attribute", function (attrs) {
        if (!attrs || attrs.length === 0) return "";
        let name = "";
        attrs.forEach(function (obj) { name += obj.name + "/"; });
        return name.substr(0, name.length - 1);
    });
    // 商品列表
    var productList = new Vue({
        el: "#productList",
        data: {
            list: []
        },
        methods: {
            getImage: function (item) {
                if (item.images.length === 0) return "";
                return "http://localhost:5002/File/Product/" + item.businessId + "/200x150/" + item.images[0].name + "." + item.images[0].extensionName;
            },
            delProduct: function (product) {
                var self = this;
                $.confirm("提示", "确定删除商品【" + product.name + "】？", function () {
                    axios.post("/Product/DelProduct/" + product.id).then(function (response) {
                        if (!response.data.success) {
                            $.alert(response.data.msg)
                            return;
                        }
                        $.alert("删除成功", "success");
                        self.list.remove(product);
                    }).catch(function (error) {
                        $.alert(error);
                    });
                    return true;
                });
            },
            toggleStatus: function (item) {
                var url = item.status === 1 ? "Down" : "Up"
                axios.post("/Product/" + url + "/" + item.id)
                    .then(function (response) {
                        item.status = item.status === 1 ? 2 : 1;
                        $.alert(response.data, "success");
                    })
                    .catch(function (error) {
                        $.alert(data);
                    });
            }
        }
    });
    $.loading();
    axios.post("/Product/GetProducts")
        .then(function (response) {
            $.loaded();
            productList.list = response.data;
        })
        .catch(function (error) {
            $.loaded();
            $.alert(error);
        });


})(jQuery);

