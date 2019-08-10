(function () {
    var template = `
        <div class="row form-horizontal" id="add-menu">
            <div class="form-group">
                <label class="control-label col-xs-3">
                    菜单类型：
                </label>
                <div class="col-xs-8">
                    <select class="form-control" v-model="menu.type">
                        <option v-for="item in types" :value="item.value">{{item.name}}</option>
                    </select>
                </div>
            </div>
            <div class="form-group">
                <label class="control-label col-xs-3">
                    <span class="require">菜单标题：</span>
                </label>
                <div class="col-xs-8">
                    <input type="text" class="form-control" v-model="menu.name" />
                </div>
            </div>
            <div class="form-group">
                <label class="control-label col-xs-3">
                    KEY值：
                </label>
                <div class="col-xs-8">
                    <input type="text" class="form-control" v-model="menu.key" />
                </div>
            </div>
            <div class="form-group">
                <label class="control-label col-xs-3">
                    URL：
                </label>
                <div class="col-xs-8">
                    <input type="text" class="form-control" v-model="menu.url" />
                </div>
            </div>
            <div class="form-group">
                <label class="control-label col-xs-3">
                    小程序AppID：
                </label>
                <div class="col-xs-8">
                    <input type="text" class="form-control" v-model="menu.appid" />
                </div>
            </div>
            <div class="form-group">
                <label class="control-label col-xs-3">
                    小程序路径：
                </label>
                <div class="col-xs-8">
                    <input type="text" class="form-control" v-model="menu.pagepath" />
                </div>
            </div>
        </div>
    `, types = [{ name: "", value: 0 }, { name: "点击类型", value: 1 }, { name: "网页类型", value: 2 }, { name: "小程序类型", value: 3 }],
        typeMaps = { "click": 1, "view": 2, "miniprogram": 3 };

    var app = new Vue({
        el: "#app",
        data: {
            tree: [],
            selectItem: {},
            menu: {},
            types: JSON.parse(JSON.stringify(types))
        },
        methods: {
            open: function (menu) {
                var result = !menu.checked;
                this.tree.forEach(function (obj) { obj.checked = false; });
                menu.checked = result;
                if (result) {
                    this.menu = menu;
                } else {
                    this.menu = {};
                }
            },
            choice: function (menu, parent) {
                parent.sub_button.forEach(a => a.checked = false);
                parent.checked = false;
                menu.checked = true;
                this.menu = menu;
            },
            save: function () {
                axios.post("/weixin/createAppMenu", this.tree)
                    .then(function () {
                        $.alert("保存成功", "success");
                    })
                    .catch(function (err) { $.alert(err); });
            },
            remove: function () {
                $.confirm("提示", "公众号菜单将被清空，确定删除吗？", function () {
                    axios.post("/weixin/createAppMenu", {})
                        .then(function () {
                            $.alert("删除成功", "success");
                            app.tree = [];
                        })
                        .catch(function (err) { $.alert(err); });
                    return true;
                });
            },
            add: function () {
                var vueObj;
                $.view({
                    title: "添加菜单",
                    template,
                    footDisplay: "block",
                    load: function () {
                        vueObj = new Vue({
                            el: "#add-menu",
                            data: {
                                menu: { type: "", name: "", key: "", url: "", appid: "", pagepath: "", checked: false, sub_button: [] },
                                types: JSON.parse(JSON.stringify(types))
                            }
                        });
                        destroyVue.call(this, vueObj);
                    },
                    submit: function () {
                        var menu = JSON.parse(JSON.stringify(vueObj.menu));
                        var parent = app.tree.first(function (obj) { return obj.checked; });
                        if (parent) {
                            parent.sub_button.push(menu);
                        } else {
                            app.tree.push(menu);
                        }
                        return true;
                    }
                });
            }
        },
        created: function () {
            var self = this;
            axios.get("/weixin/getAppMenu")
                .then(function (res) {
                    var result = JSON.parse(res.data);
                    if (!result.menu) return;
                    self.tree = result.menu.button.map(function (obj) {
                        obj.checked = false;
                        obj.type = typeMaps[obj.type];
                        return obj;
                    });
                })
                .catch(function (err) {
                    $.alert(err.response.data.message || err);
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
