(function () {
    var template = `
        <div class="row form-horizontal">
            <div class="form-group">
                <label class="control-label col-xs-3">
                    <span class="require">档口名称：</span>
                </label>
                <div class="col-xs-8">
                    <input class="form-control" autocomplete="off" id="name" />
                </div>
            </div>
        </div>
    `;
    var app = new Vue({
        el: "#app",
        data: {
            items: []
        },
        methods: {
            add: function () {
                edit();
            },
            modify: function (staff) {
                edit(staff);
            },
            remove: function (item) {
                $.primary(`确定删除档口[${item.name}]吗？`, function () {
                    axios.delete(`/tang/deleteBooth/${item.id}`)
                        .then(function (res) {
                            if (!res.data.success) {
                                $.alert("删除失败，请刷新后重试！");
                                return;
                            }
                            app.items.remove(item);
                            $.alert("删除成功", "success");
                        })
                        .catch(function (err) { $.alert(err); });
                    return true;
                });
            },
            bindProduct: function (cook) {
                bindProducts(cook);
            }
        },
        created: function () {
            var self = this;
            axios.get("/tang/getBooths")
                .then(function (res) {
                    res.data.forEach(a => a.productIds = null);
                    self.items = res.data;
                });
        }
    });

    var edit = function (booth) {
        var isNew = !booth, name = isNew ? "" : booth.name;
        $.view({
            title: isNew ? "新增档口" : "修改档口",
            template,
            footDisplay: "block",
            load: function () { $("#name").val(name); },
            submit: function () {
                var boothName = $.trim($("#name").val());
                if (!boothName) {
                    $.alert("请输入档口名称");
                    return;
                }
                var url = isNew ? "/tang/addBooth" : "/tang/updateBooth";
                axios.post(url, { id: isNew ? 0 : booth.id, name: boothName })
                    .then(function (res) {
                        if (isNew) {
                            res.data.data.productIds = [];
                            app.items.push(res.data.data);
                            $.alert("新增成功", "success");
                        } else {
                            booth.name = boothName;
                            $.alert("修改成功", "success");
                        }
                    })
                    .catch(function (err) { $.alert(err); });
                return true;
            }
        });
    };

    var bindProducts = function (obj) {
        var template, types;
        $.loading();
        axios.all([axios.get(`/product/getProductTree?isSetMeal=1`), axios.get(`/tang/checkProducts`)])
            .then(function (res) {
                template = res[1].data;
                types = res[0].data.map(function (type) { type.checked = false; return type; });
                $.loaded();

                var openDialog = function (productIds) {
                    var vueObj,
                        typeList = JSON.parse(JSON.stringify(types)),   // 商品类别
                        productList = [],                               // 已经勾选的商品
                        allProducts = [],                               // 所有商品
                        self = this;                                    // 档口对象
                    typeList.forEach(function (type) { allProducts = allProducts.concat(type.list); });
                    allProducts.forEach(function (product) {
                        product.checked = false;
                        product.selected = false;
                        product.title = "";
                        app.items.forEach(item => {
                            if (item.productIds.indexOf(product.id) > -1) {
                                product.title += item.name + ',';
                            }
                        });
                        if (!product.title) return;
                        product.title = `已绑定${product.title.slice(0, product.title.length - 1)}`;
                    });

                    if (productIds && productIds.length > 0) {
                        productIds.forEach(function (id) {
                            var product = allProducts.first(function (item) { return item.id === id; });
                            if (!product) return;
                            product.checked = true;
                            productList.push(product);
                        });
                    }
                    $.view({
                        title: `档口[${self.name}]绑定菜单`,
                        footDisplay: "block",
                        template,
                        dialogWidth: 800,
                        keyboard: false,
                        load: function () {
                            vueObj = new Vue({
                                el: "#bindProduct",
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
                            // 隐藏时销毁
                            destroyVue.call(this, vueObj);
                        },
                        submit: function () {
                            var relative = vueObj.productList.map(function (obj) { return { staffId: self.id, productId: obj.id }; });
                            axios.post(`/tang/bindProductForBooth/${self.id}`, relative)
                                .then(function (res) {
                                    self.productIds = res.data.data.map(function (obj) { return obj.productId; });
                                    $.alert("绑定成功", "success");
                                })
                                .catch(function (err) { $.alert(err); });
                            return true;
                        }


                    });
                };

                bindProducts = function (booth) {
                    if (!booth.productIds) {
                        //axios.get(`/tang/getProductIdsByBooth/${booth.id}`)
                        //    .then(function (res) {
                        //        booth.productIds = res.data;
                        //        openDialog.call(booth, res.data);
                        //    })
                        //    .catch(function (err) { $.alert(err); });

                        axios.get(`/tang/getProductIdsWithBusinessBooth`)
                            .then(function (res) {
                                var booths = res.data;
                                app.items.forEach(item => {
                                    var first = booths.first(a => a.id === item.id);
                                    if (!first) return;
                                    item.productIds = first.ids;
                                });
                                openDialog.call(booth, booth.productIds);
                            })
                            .catch(function (err) { $.alert(err); });
                    } else {
                        openDialog.call(booth, booth.productIds);
                    }

                };
                bindProducts(obj);
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

    $(document).on("mouseenter", "a[title!=''], div[title!='']", function () {
        if (!this.title) return;
        $(this).tooltip();
    });

})();

