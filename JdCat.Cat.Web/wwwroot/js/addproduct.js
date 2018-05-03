﻿
(function ($) {


    var appData = new Vue({
        el: "#app",
        data: {
            typeList: pageData.types,
            entity: {
                ProductTypeId: null,
                Name: "",
                Description: "",
                UnitName: '份',
                MinBuyQuantity: 1
            }
        },
        methods: {
            addType: function () {
                $.view({
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
            saveReturn: function () {
                axios.post("/product/Save", this._data.entity)
                    .then(function (response) {
                        console.log(response);
                    });
            },
            saveContinu: function () {

            },
            cancel: function () {

            }
        }
    });

})(jQuery);