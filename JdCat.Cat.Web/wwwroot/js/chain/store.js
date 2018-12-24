
(function () {
    var appVue, createVue, bindVue, cityJson;
    appVue = new Vue({
        el: "#app",
        data: {
            paging: {
                pageSize: 20,
                pageIndex: 1
            },
            stores: []
        },
        methods: {
            loadData: function () {
                var self = this;
                axios.get(`/chain/getstores?pageIndex=${self.paging.pageIndex}&pageSize=${self.paging.pageSize}`)
                    .then(function (res) {
                        var index = (self.paging.pageIndex - 1) * self.paging.pageSize;
                        self.stores = res.data;
                        self.stores.forEach((store, num) => { store.index = index + num + 1; });
                    })
                    .catch(function (err) { $.alert(err); });
            },
            create: function () {
                $.view({
                    title: "新增门店",
                    footDisplay: true,
                    url: "/chain/create",
                    load: function () {
                        createVue = new Vue({
                            el: "#createVue",
                            data: {
                                entity: { city: null },
                                password: null,
                                showError: false,
                                province: [],
                                city: null,
                                area: null
                            },
                            created: function () {
                                for (var key in cityJson) {
                                    this.province.push(key);
                                }
                            },
                            watch: {
                                "entity.province": function (val) {
                                    var citys = cityJson[val];
                                    this.city = [];
                                    for (var key in citys) {
                                        this.city.push(key);
                                    }
                                    this.area = [];
                                    this.entity.city = null;
                                    this.entity.area = null;
                                },
                                "entity.city": function (val) {
                                    if (!val) return;
                                    var citys = cityJson[this.entity.province];
                                    var areas = citys[val];
                                    this.area = JSON.parse(JSON.stringify(areas));
                                    this.entity.area = null;
                                }
                            }
                        });

                        this.on("hidden.bs.modal", function () {
                            if (!createVue) return;
                            createVue.$destroy();
                            createVue = null;
                        });
                    },
                    submit: function () {
                        createVue.showError = true;
                        var entity = createVue.entity;

                        if (!entity.name || !entity.code || !entity.password || !entity.contact || !entity.mobile || !entity.province || !entity.city || !entity.area || !createVue.password) {
                            $.alert("请将表单输入完整！");
                            return false;
                        }
                        if (entity.password !== createVue.password) {
                            $.alert("两次输入的密码不一致！");
                            return false;
                        }
                        axios.post("/chain/create", entity)
                            .then(function (res) {
                                if (!res.data.success) {
                                    $.alert(res.data.msg);
                                    return;
                                }
                                $.alert(res.data.msg, "success");
                                var index = 1;
                                if (appVue.stores.length > 0) {
                                    index = appVue.stores[appVue.stores.length - 1].index + 1;
                                }
                                res.data.data.index = index;
                                appVue.stores.push(res.data.data);
                            })
                            .catch(function (err) { $.alert(err); });
                        return true;
                    }
                });
            },
            bind: function () {
                $.view({
                    title: "绑定门店",
                    footDisplay: true,
                    url: "/chain/bind",
                    load: function () {
                        bindVue = new Vue({
                            el: "#bindVue",
                            data: {
                                storeId: null,
                                code: null,
                                password: null,
                                showError: false
                            }
                        });

                        this.on("hidden.bs.modal", function () {
                            if (!bindVue) return;
                            bindVue.$destroy();
                            bindVue = null;
                        });
                    },
                    submit: function () {
                        bindVue.showError = true;
                        if (!bindVue.storeId || !bindVue.code || !bindVue.password) {
                            $.alert("请将信息填写完整");
                            return false;
                        }
                        axios.post(`/chain/bind?storeId=${bindVue.storeId}&code=${bindVue.code}&password=${bindVue.password}`)
                            .then(function (res) {
                                if (!res.data.success) {
                                    $.alert(res.data.msg);
                                    return;
                                }
                                $.alert(res.data.msg, "success");
                                var index = 1;
                                if (appVue.stores.length > 0) {
                                    index = appVue.stores[appVue.stores.length - 1].index + 1;
                                }
                                res.data.data.index = index;
                                appVue.stores.push(res.data.data);
                            })
                            .catch(function (err) { $.alert(err); });
                        return true;
                    }
                });
            },
            unbind: function (store) {
                var self = this;
                $.primary(`确定解绑门店【${store.name}】吗？`, function () {
                    axios.get(`/chain/unbind/${store.id}`)
                        .then(function (res) {
                            if (!res.data.success) {
                                $.alert(res.data.msg);
                                return;
                            }
                            $.alert(res.data.msg, "success");
                            self.stores.remove(store);
                        })
                        .catch(function (err) { $.alert(err); });
                    return true;
                });
            }
        },
        created: function () {
            this.loadData();

            axios.get("/data/city.json")
                .then(function (res) { cityJson = res.data; })
                .catch(function (err) { $.alert(err); });
        }
    });

})();