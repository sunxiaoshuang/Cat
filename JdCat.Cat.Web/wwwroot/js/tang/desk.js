(function () {
    var typetemplate = `
        <div class="row form-horizontal">
            <div class="form-group">
                <label class="control-label col-xs-3">
                    <span class="require">区域名称：</span>
                </label>
                <div class="col-xs-8">
                    <input class="form-control" id="typeName" />
                </div>
            </div>
        </div>
    `;
    var editTypeTemplate = `
        <div class="row form-horizontal" id="edit-type">
            <div class="form-group" v-for="item in items">
                <label class="control-label col-xs-3">
                    区域名称：
                </label>
                <div class="col-xs-4">
                    <input class="form-control" v-model.trim="item.name" />
                </div>
                <label class="control-label col-xs-2">
                    排序码：
                </label>
                <div class="col-xs-2">
                    <input class="form-control" type="number" v-model="item.sort" />
                </div>
            </div>
        </div>
    `;
    var editDeskTemplate = `
        <div class="row form-horizontal" id="edit-desk">
            <div class="form-group">
                <label class="control-label col-xs-3">
                    <span class="require">餐台名称：</span>
                </label>
                <div class="col-xs-8">
                    <input class="form-control" v-model.trim="entity.name" />
                </div>
            </div>
            <div class="form-group">
                <label class="control-label col-xs-3">
                    <span class="require">用餐人数：</span>
                </label>
                <div class="col-xs-8">
                    <input class="form-control" type="number" v-model.trim="entity.quantity" />
                </div>
            </div>
            <div class="form-group">
                <label class="control-label col-xs-3">
                    <span class="require">所属区域：</span>
                </label>
                <div class="col-xs-8">
                    <select class="form-control" v-model="entity.deskTypeId">
                        <option :value="option.id" v-for="option in options">{{option.name}}</option>
                    </select>
                </div>
            </div>
        </div>
    `;
    var app = new Vue({
        el: "#app",
        data: {
            types: null,
            desks: null,
            count: 0,
            allType: true,
            selectedType: null
        },
        methods: {
            allClick: function () {
                if (this.allType) return;
                this.allType = true;
                this.allDesk();
                this.selectedType = null;
            },
            typeClick: function (type) {
                this.allType = false;
                this.selectedType = type;
                this.types.forEach(function (obj) { obj.selected = false; });
                type.selected = true;
                this.desks = type.desks;
            },
            addType: function () {
                var self = this;
                $.view({
                    title: "新增类别",
                    footDisplay: "block",
                    template: typetemplate,
                    load: function () {
                        setTimeout(function () { $("#typeName").focus(); }, 200);
                    },
                    submit: function () {
                        var name = $.trim($("#typeName").val());
                        if (!name) {
                            $.alert("请输入类别名称");
                            return;
                        }
                        var sort = (self.types.length === 0 ? 0 : self.types[self.types.length - 1].sort) + 1;
                        axios.post(`/tang/addDeskType`, { name, sort })
                            .then(function (res) {
                                if (!res.data.success) {
                                    $.alert(res.data.msg);
                                    return;
                                }
                                $.alert(res.data.msg, "success");
                                res.data.data.selected = false;
                                res.data.data.count = 0;
                                self.types.push(res.data.data);
                            })
                            .catch(function (err) { $.alert(err); });
                        return true;
                    }
                });
            },
            removeType: function () {
                var self = this, type = this.types.first(function (obj) { return obj.selected; });
                if (!type) {
                    $.alert("请选择要删除的区域", "warning");
                    return;
                }
                if (type.count > 0) {
                    $.alert("区域下存在餐台，不允许删除");
                    return;
                }
                $.primary(`确定删除区域[${type.name}]吗？`, function () {
                    axios.delete(`/tang/deleteDeskType/${type.id}`)
                        .then(function (res) {
                            if (!res.data.success) {
                                $.alert(res.data.msg);
                                return;
                            }
                            $.alert(res.data.msg, "success");
                            self.types.remove(type);
                        })
                        .catch(function (err) { $.alert(err); });
                    return true;
                });
            },
            modifyType: function () {
                if (!app.types || app.types.length === 0) {
                    $.alert("不存在任何区域", "warning");
                    return;
                }
                var vueObj, items = app.types.map(function (obj) { return { id: obj.id, name: obj.name, sort: obj.sort }; });
                $.view({
                    title: "修改区域",
                    footDisplay: "block",
                    template: editTypeTemplate,
                    load: function () {
                        vueObj = new Vue({
                            el: "#edit-type",
                            data: {
                                items: items
                            }
                        });
                        destroyVue.call(this, vueObj);
                    },
                    submit: function () {
                        var result = vueObj.items.some(function (obj) { return !obj.name; });
                        if (result) {
                            $.alert("区域名称不能为空");
                            return;
                        }
                        result = vueObj.items.some(function (obj) { return !obj.sort; });
                        if (result) {
                            $.alert("排序码必须大于零");
                            return;
                        }
                        axios.put("/tang/updateDeskTypes", vueObj.items)
                            .then(function (res) {
                                if (!res.data.success) {
                                    $.alert(res.data.msg);
                                    return;
                                }
                                $.alert(res.data.msg, "success");
                                vueObj.items.forEach(function (obj) {
                                    var item = app.types.first(function (type) { return type.id === obj.id; });
                                    item.name = obj.name;
                                    item.sort = +obj.sort;
                                });
                                app.types.sort(function (a, b) { return a.sort - b.sort; });
                            })
                            .catch(function (err) { $.alert(err); });
                        return true;
                    }
                });
            },
            addDesk: function () {
                if (!app.types || app.types.length === 0) {
                    $.alert("请先创建餐台区域后新增餐台");
                    return;
                }
                var options = app.types.map(function (obj) { return { id: obj.id, name: obj.name }; }),
                    entity = { name: "", quantity: "", deskTypeId: app.selectedType ? app.selectedType.id : 0 }, vueObj;

                $.view({
                    title: "新增餐台",
                    footDisplay: "block",
                    template: editDeskTemplate,
                    load: function () {
                        vueObj = new Vue({
                            el: "#edit-desk",
                            data: {
                                options,
                                entity
                            }
                        });
                        destroyVue.call(this, vueObj);
                    },
                    submit: function () {
                        if (!vueObj.entity.name || vueObj.entity.quantity <= 0 || !vueObj.entity.deskTypeId) {
                            $.alert("请正确输入餐台信息");
                            return;
                        }
                        axios.post("/tang/addDesk", vueObj.entity)
                            .then(function (res) {
                                $.alert(res.data.msg, "success");
                                var type = app.types.first(function (obj) { return obj.id === res.data.data.deskTypeId; });
                                type.desks = type.desks || [];
                                type.desks.push(res.data.data);
                                type.count = type.desks.length;
                                if (!app.selectedType) {
                                    app.allDesk();
                                }
                            })
                            .catch(function (err) { $.alert(err); });
                        return true;
                    }
                });
            },
            editDesk: function (desk) {
                var entity = { name: desk.name, id: desk.id, deskTypeId: desk.deskTypeId, quantity: desk.quantity }, vueObj,
                    options = app.types.map(function (obj) { return { id: obj.id, name: obj.name }; }),
                    oldType = app.types.first(function (obj) { return obj.id === desk.deskTypeId; });
                $.view({
                    title: "编辑餐台",
                    footDisplay: "block",
                    template: editDeskTemplate,
                    load: function () {
                        vueObj = new Vue({
                            el: "#edit-desk",
                            data: {
                                entity,
                                options
                            }
                        });
                    },
                    submit: function () {
                        axios.put(`/tang/updateDesk`, vueObj.entity)
                            .then(function (res) {
                                $.alert(res.data.msg, "success");
                                desk.name = vueObj.entity.name;
                                desk.quantity = vueObj.entity.quantity;
                                desk.deskTypeId = vueObj.entity.deskTypeId;
                                if (desk.deskTypeId === oldType.id) return;
                                var type = app.types.first(function (obj) { return obj.id === desk.deskTypeId; });
                                oldType.desks.remove(desk);
                                oldType.count--;
                                type.desks.push(desk);
                                type.count++;
                            })
                            .catch(function (err) { $.alert(err); });
                        return true;
                    }
                });
            },
            removeDesk: function (desk) {
                $.primary(`确定删除餐台[${desk.name}]吗？`, function () {
                    axios.delete(`/tang/deleteDesk/${desk.id}`)
                        .then(function (res) {
                            if (!res.data.success) {
                                $.alert(res.data.msg);
                                return;
                            }
                            $.alert(res.data.msg, "success");
                            var type = app.types.first(function (obj) { return obj.id === desk.deskTypeId; });
                            type.desks.remove(desk);
                            type.count--;
                            app.desks.remove(desk);
                            app.count--;
                        })
                        .catch(function (err) { $.alert(err); });
                    return true;
                });
            },





            allDesk: function () {
                var desks = [];
                this.types.forEach(function (obj) {
                    obj.selected = false;
                    if (!obj.desks) return;
                    desks = desks.concat(obj.desks);
                });
                this.desks = desks;
                this.count = desks.length;
            }
        },
        created: function () {
            var self = this;
            axios.get("/tang/getDesks")
                .then(function (res) {
                    if (!res.data) return;
                    res.data.forEach(function (obj) {
                        obj.count = obj.desks ? obj.desks.length : 0;
                        obj.selected = false;
                    });
                    self.types = res.data;
                    self.allDesk();
                });
        }
    });


    // 销毁vue对象
    function destroyVue(obj) {
        this.on("hidden.bs.modal", function () {
            if (!obj) return;
            obj.$destroy();
            obj = null;
        });
    }
})();

