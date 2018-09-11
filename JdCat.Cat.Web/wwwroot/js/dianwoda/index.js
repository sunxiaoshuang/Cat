
(function () {
    var mainVue, createVue, cityList;
    
    axios.get("/data/dianwoda.json").then(function (data) { cityList = data.data; });

    mainVue = new Vue({
        el: "#app",
        data: {
            isCreated: !!pageObj.dwd.shop_title,
            shop: pageObj.dwd,
            balance: pageObj.balance
        },
        methods: {
            create: function () {
                if (this.balance > 0) {
                    $.alert("账户余额大于0，不可以重新创建。");
                    return;
                }
                $.view({
                    name: "add",
                    title: "创建商户",
                    footDisplay: "block",
                    url: "/Dianwoda/Create",
                    saveText: "创建商户",
                    load: function () {
                        createVue = new Vue({
                            el: "#createPage",
                            data: {
                                cityList: cityList,
                                obj: createPageObj
                            },
                            methods: {
                                change: function (e) {
                                    if (!e.target.selectedOptions[0]) return;
                                    this.obj.city_name = e.target.selectedOptions[0].text;
                                }
                            }
                        });
                    },
                    submit: function () {
                        var data = createVue.obj;
                        if (!data.city_code || !data.shop_title || !data.mobile || !data.lng || !data.lat || !data.addr) {
                            $.alert("请将商户信息输入完整");
                            return false;
                        }
                        data.lng = data.lng.toString().replace(".", "");
                        data.lat = data.lat.toString().replace(".", "");
                        axios.post("/Dianwoda/Create", data)
                            .then(function (res) {
                                if (!res.data.success) {
                                    $.alert(res.data.msg);
                                    return;
                                }
                                mainVue.isCreated = true;
                                mainVue.shop = res.data.data;
                            })
                            .catch(function (err) {
                                $.alert(err);
                            });
                        return true;
                    }
                });
            }
        }
    });

})();

