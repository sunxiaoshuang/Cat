;
(function ($) {
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
            return false;
        })
        .on("click", "#type .btn-save", function () {
            var $list = $(".list-type");
            var $new = $list.find(".list-type-item.new"),
                $edit = $list.find(".list-type-item.edit"),
                $remove = $list.find(".list-type-item.remove");
            var data = {add: [], edit: [], remove:[]};
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
            $.post("/product/updatetypes", data, function () {
                $.loaded();
                $("#type").modal("hide");
            });
            return false;
        })
        .on("change", "input", function () {
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
            $parent.removeClass("edit").addClass("remove");
            $parent.hide();
        });


})(jQuery);

