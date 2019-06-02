(function () {
    var template = `
        <div class="row form-horizontal" id="edit-printer">
            <div class="form-group">
                <label class="control-label col-xs-3">
                    打印机名称：
                </label>
                <div class="col-xs-8">
                    <input class="form-control" v-model.trim="entity.name" />
                </div>
            </div>
            <div class="form-group">
                <label class="control-label col-xs-3">
                    IP地址：
                </label>
                <div class="col-xs-8">
                    <input class="form-control" v-model.trim="entity.ip" />
                </div>
            </div>
            <div class="form-group">
                <label class="control-label col-xs-3">
                    端口号：
                </label>
                <div class="col-xs-8">
                    <input class="form-control" type="number" v-model.trim="entity.port" />
                </div>
            </div>
            <div class="form-group">
                <label class="control-label col-xs-3">
                    打印机类型：
                </label>
                <div class="col-xs-8">
                    <select class="form-control" v-model="entity.type">
                        <option v-for="item in types" :value="item.value">{{item.name}}</option>
                    </select>
                </div>
            </div>
            <div class="form-group">
                <label class="control-label col-xs-3">
                    打印数量：
                </label>
                <div class="col-xs-8">
                    <input class="form-control" type="number" v-model.trim="entity.quantity" />
                </div>
            </div>
            <div class="form-group">
                <label class="control-label col-xs-3">
                    打印模式：
                </label>
                <div class="col-xs-8">
                    <select class="form-control" v-model="entity.mode">
                        <option v-for="item in modes" :value="item.value">{{item.name}}</option>
                    </select>
                </div>
            </div>
            <div class="form-group">
                <label class="control-label col-xs-3">
                    打印机规格：
                </label>
                <div class="col-xs-8">
                    <select class="form-control" v-model="entity.format">
                        <option v-for="item in formats" :value="item.value">{{item.name}}</option>
                    </select>
                </div>
            </div>
            <div class="form-group">
                <label class="control-label col-xs-3">
                    打印范围：
                </label>
                <div class="col-xs-8">
                    <div class="kui-checkbox checkbox pull-left m-left-md m-right-md margin-right-20" v-for="scope in scopeList">
                        <label class="label-checkbox">
                            <input type="checkbox" name="scope" v-model="scope.checked">
                            <span class="custom-checkbox"></span>
                            <span v-text="scope.name"></span>
                        </label>
                    </div>
                </div>
            </div>
            <div class="form-group">
                <label class="control-label col-xs-3">
                    相关收银台：
                </label>
                <div class="col-xs-8">
                    <input class="form-control" v-model.trim="entity.cashierName" />
                </div>
            </div>
            <div class="form-group">
                <label class="control-label col-xs-3">
                    状态：
                </label>
                <div class="col-xs-8">
                    <select class="form-control" v-model="entity.state">
                        <option v-for="item in states" :value="item.value">{{item.name}}</option>
                    </select>
                </div>
            </div>
        </div>
    `;
    var app = new Vue({
        el: "#app",
        data: {
            items: null
        },
        methods: {
            add: function () {
                editPrinter();
            },
            modify: function (printer) {
                editPrinter(printer);
            },
            remove: function (printer) {
                var self = this;
                $.primary(`确定删除打印机[${printer.name}]吗？`, function () {
                    axios.delete(`/tang/deletePrinter/${printer.id}`).
                        then(function (res) {
                            if (!res.data.success) {
                                $.alert(res.data.msg);
                                return;
                            }
                            $.alert(res.data.msg, "success");
                            self.items.remove(printer);
                        })
                        .catch(function (err) { $.alert(err); });
                    return true;
                });
            },
            select: function (printer) {
                selectProduct(printer);
            },


            modeFormat: function (mode) {
                if (mode === 0) return "一菜一打";
                else if (mode === 1) return "一份一打";
                else if (mode === 2) return "一单一打";
            },
            scopeFormat: function (scope) {
                var result = "";
                if (scope & 1) result += "外卖;";
                if (scope & 2) result += "堂食;";
                return result;
            }
        },
        created: function () {
            var self = this;
            axios.get(`/tang/getPrinters`)
                .then(function (res) {
                    self.items = res.data;
                });
        }
    });

    function editPrinter(printer) {
        var isNew = !printer,
            entity = !isNew ? JSON.parse(JSON.stringify(printer)) : { id: 0, name: "", ip: "", port: 9100, type: 2, quantity: 1, mode: 0, format: 80, scope: 0, state: 1 },
            types = [{ name: "前台", value: 1 }, { name: "后厨", value: 2 }],
            modes = [{ name: "一菜一打", value: 0 }, { name: "一份一打", value: 1 }, { name: "一单一打", value: 2 }],
            formats = [{ name: "58mm", value: 58 }, { name: "80mm", value: 80 }],
            states = [{ name: "正常", value: 1 }, { name: "停用", value: 2 }],
            scopeList = [{ name: "外卖", value: 1, checked: false }, { name: "堂食", value: 2, checked: false }],
            vueObj;
        $.view({
            title: isNew ? "新增打印机" : "编辑打印机",
            footDisplay: "block",
            template,
            load: function () {
                vueObj = new Vue({
                    el: "#edit-printer",
                    data: {
                        entity, types, modes, formats, states, scopeList
                    },
                    created: function () {
                        var self = this;
                        this.scopeList.forEach(function (obj) {
                            if (self.entity.scope & obj.value) obj.checked = true;
                        });
                    }
                });
                destroyVue.call(this, vueObj);
            },
            submit: function () {
                var entity = vueObj.entity;
                if (!entity.name) {
                    $.alert("请输入打印机名称");
                    return;
                }
                if (!entity.ip) {
                    $.alert("请输入IP地址");
                    return;
                }
                if (!jdCat.utilMethods.ipValid(entity.ip)) {
                    $.alert("IP地址输入错误");
                    return;
                }
                if (!entity.port) {
                    $.alert("请输入端口号");
                    return;
                }
                if (!entity.quantity) {
                    $.alert("请输入打印数量");
                    return;
                }
                var scope = 0;
                vueObj.scopeList.forEach(function (obj) {
                    if (!obj.checked) return;
                    scope += obj.value;
                });
                entity.scope = scope;
                var url = isNew ? "/tang/addPrinter" : "/tang/updatePrinter";
                axios.post(url, entity)
                    .then(function (res) {
                        $.alert(isNew ? "新增成功" : "修改成功", "success");
                        if (isNew) {
                            app.items.push(res.data);
                            return;
                        }
                        printer.name = entity.name;
                        printer.ip = entity.ip;
                        printer.port = entity.port;
                        printer.type = entity.type;
                        printer.mode = entity.mode;
                        printer.format = entity.format;
                        printer.quantity = entity.quantity;
                        printer.scope = entity.scope;
                        printer.state = entity.state;
                        printer.cashierName = entity.cashierName;
                    })
                    .catch(function (err) { $.alert(err); });
                return true;
            }
        });
    }

    var selectProduct = function (obj) {
        var template, types;
        $.loading();
        axios.all([axios.get(`/product/getProductTree`), axios.get(`/tang/selectProduct`)])
            .then(function (res) {
                template = res[1].data;
                types = res[0].data.map(function (type) { type.checked = false; return type; });
                $.loaded();
                selectProduct = function (printer) {
                    var vueObj, typeList = JSON.parse(JSON.stringify(types)), productList = [], allProducts = [];
                    typeList.forEach(function (type) { allProducts = allProducts.concat(type.list); });
                    allProducts.forEach(function (product) {
                        product.checked = false;
                        product.selected = false;
                    });
                    if (printer.foodIds) {
                        JSON.parse(printer.foodIds).forEach(function (id) {
                            var product = allProducts.first(function (item) { return item.id === id; });
                            if (!product) return;
                            product.checked = true;
                            productList.push(product);
                        });
                    }
                    $.view({
                        title: `打印机[${printer.name}]绑定菜单`,
                        footDisplay: "block",
                        template,
                        dialogWidth: 800,
                        keyboard: false,
                        load: function () {
                            vueObj = new Vue({
                                el: "#bindPrinter",
                                data: {
                                    typeList, products: allProducts,
                                    productList,
                                    search: {
                                        key: "",
                                        result: [],
                                        boxHeight: 0
                                    }
                                },
                                watch: {
                                    'search.key': function (key) {
                                        this.seek(key);
                                    }
                                },
                                methods: {
                                    focus: function () {
                                        this.seek(this.search.key);
                                    },
                                    blur: function () {
                                        this.search.boxHeight = 0;
                                    },
                                    seek: function (key) {
                                        if (!key) {
                                            this.search.result = [];
                                            this.search.boxHeight = "0";
                                            return;
                                        }
                                        this.search.boxHeight = "120px";
                                        this.products.forEach(a => a.selected = false);
                                        this.search.result = this.products
                                            .filter(a => a.name.indexOf(key) > -1 || a.code.indexOf(key) > -1 || a.pinyin.indexOf(key) > -1 || a.firstLetter.indexOf(key) > -1)
                                            .slice(0, 8);
                                    },
                                    down: function () {
                                        var self = this;
                                        this.move(function (index) {
                                            if (index === self.search.result.length - 1) {
                                                index = 0;
                                            } else {
                                                index++;
                                            }
                                            return index;
                                        });
                                    },
                                    up: function () {
                                        var self = this;
                                        this.move(function (index) {
                                            if (index === 0) {
                                                index = self.search.result.length - 1;
                                            } else {
                                                if (index === -1) {
                                                    index = 0;
                                                } else {
                                                    index--;
                                                }
                                            }
                                            return index;
                                        });
                                    },
                                    enter: function () {
                                        var products = this.search.result.filter(a => a.selected);
                                        if (products.length === 0) return;
                                        this.clickItem(products[0]);
                                    },
                                    clear: function () {
                                        this.search.key = "";
                                    },
                                    move: function (callback) {
                                        if (this.search.result.length === 0) return;
                                        var selected, index;
                                        var selecteds = this.search.result.filter(a => a.selected);
                                        if (selecteds.length > 0) selected = selecteds[0];
                                        index = this.search.result.indexOf(selected);
                                        index = callback(index);
                                        selecteds.forEach(a => a.selected = false);
                                        this.search.result[index].selected = true;
                                    },
                                    clickItem: function (product) {
                                        if (product.checked) return;
                                        product.checked = true;
                                        this.productList.push(product);
                                    },
                                    open: function (type) {
                                        type.checked = !type.checked;
                                    },
                                    choice: function (product) {
                                        if (product.checked) return;
                                        product.checked = true;
                                        this.productList.push(product);
                                    },
                                    remove: function (product) {
                                        product.checked = false;
                                        this.productList.remove(product);
                                    }
                                }
                            });
                            
                            destroyVue.call(this, vueObj);
                        },
                        submit: function () {
                            var foodIds = JSON.stringify(vueObj.productList.map(function (obj) { return obj.id; }));
                            axios.put(`/tang/updatePrinterProducts`, { id: printer.id, foodIds })
                                .then(function (res) {
                                    $.alert(res.data.msg, "success");
                                    printer.foodIds = foodIds;
                                })
                                .catch(function (err) { $.alert(err); });
                            return true;
                        }
                    });
                };
                selectProduct(obj);
            })
            .catch(function (err) { $.alert(err); });
    };

    // 销毁vue对象
    function destroyVue(obj) {
        this.on("hidden.bs.modal", function () {
            if (!obj) return;
            obj.$destroy();
            obj = null;
        });
    }
})();

