
(function () {

    var appVue = new Vue({
        el: "#app",
        data: {
            storeTree: [],
            typeTree: []
        },
        computed: {
            checkedStores: function () {
                var list = [];
                this.storeTree.forEach(function (province) {
                    province.list.forEach(function (city) {
                        city.list.forEach(function (area) {
                            area.list.forEach(function (store) {
                                if (!store.checked) return;
                                list.push(store);
                            });
                        });
                    });
                });
                return list;
            },
            checkedProducts: function () {
                var list = [];
                this.typeTree.forEach(function (type) {
                    type.list.forEach(function (product) {
                        if (!product.checked) return;
                        list.push(product);
                    });
                });
                return list;
            }
        },
        methods: {
            expand: function (item) {
                item.expanded = !item.expanded;
            },
            checkChange: (function () {
                var checkChild = function (obj) {
                    if (!obj.list) return;
                    obj.list.forEach(function (child) {
                        child.checked = obj.checked;
                        checkChild(child);
                    });
                };
                var checkParent = function (obj) {
                    if (!obj.parent) return;
                    obj.parent.checked = false;
                    checkParent(obj.parent);
                };
                return function (item) {
                    checkChild(item);
                    if (!item.checked) {
                        checkParent(item);
                    }
                };
            })(),
            submit: function () {
                var storeName = "", productName = "", tip = "确定复制吗？";
                if (appVue.checkedStores.length === 0) {
                    $.alert("请勾选要复制商品的门店！");
                    return;
                }
                if (appVue.checkedProducts.length === 0) {
                    $.alert("请勾选要复制的商品！");
                    return;
                }
                if (appVue.checkedStores.length < 10) {
                    appVue.checkedStores.forEach(function (store) {
                        storeName += store.name + "、";
                    });
                    storeName = storeName.substring(0, storeName.length - 1);
                }
                if (appVue.checkedProducts.length < 10) {
                    appVue.checkedProducts.forEach(function (product) {
                        productName += product.name + "、";
                    });
                    productName = productName.substring(0, productName.length - 1);
                }
                //if (!storeName && !productName) {
                //    tip = "复制商品操作不可逆，且复制的商品属于无分类，确定向选择的商户复制商品吗？";
                //} else if (!storeName) {
                //    tip = "复制商品操作不可逆，且复制的商品属于无分类，确定向选择的商户复制商品【" + productName + "】吗？";
                //} else if (!productName) {
                //    tip = "复制商品操作不可逆，且复制的商品属于无分类，确定向商户【" + storeName + "】复制已选择的商品吗？";
                //} else {
                //    tip = "复制商品操作不可逆，且复制的商品属于无分类，确定向商户【" + storeName + "】复制商品【" + productName + "】吗？";
                //}
                tip = "复制商品操作不可逆，且复制的商品属于无分类状态，需要用户登陆商户调整商品分类，确定向选择的商户复制商品吗？";
                $.primary(tip, function () {
                    $.loading();
                    var storeIds = appVue.checkedStores.map(function (store) { return store.id; });
                    var productIds = appVue.checkedProducts.map(function (product) { return product.id; });
                    axios.post("/Product/Copy", { storeIds, productIds })
                        .then(function (res) {
                            $.loaded();
                            if (!res.data.success) {
                                $.alert(res.data.msg);
                                return;
                            }
                            $.alert(`操作成功`, "success");
                            appVue.typeTree.forEach(function (type) {
                                type.list.forEach(function (product) {
                                    product.checked = false;
                                });
                            });
                        })
                        .catch(function (err) {
                            $.loaded();
                            $.alert(err);
                        });
                    return true;
                });
            },
            cancel: function () {
                history.back();
            }
        },
        created: function () {
            var self = this;
            axios.get("/Product/GetStoreTree")
                .then(function (res) {
                    res.data.forEach(function (province) {
                        province.checked = false;
                        province.expanded = false;
                        province.list.forEach(function (city) {
                            city.checked = false;
                            city.expanded = false;
                            city.parent = province;
                            city.list.forEach(function (area) {
                                area.checked = false;
                                area.expanded = false;
                                area.parent = city;
                                area.list.forEach(function (store) {
                                    store.checked = false;
                                    store.expanded = false;
                                    store.parent = area;
                                });
                            });
                        });
                    });
                    self.storeTree = res.data;
                })
                .catch(function (err) { $.alert(err); });

            axios.get("/Product/GetProductTree")
                .then(function (res) {
                    res.data.forEach(function (type) {
                        type.checked = false;
                        type.expanded = false;
                        type.list.forEach(function (product) {
                            product.checked = false;
                            product.expanded = false;
                            product.parent = type;
                        });
                    });
                    self.typeTree = res.data;
                })
                .catch(function (err) { $.alert(err); });

        }
    });

})();

