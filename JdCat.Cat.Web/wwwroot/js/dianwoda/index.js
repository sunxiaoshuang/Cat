
(function () {
    var infoVue, createVue, detailVue, chargeVue, cityList, rechargeVue;

    axios.get("/data/dianwoda.json").then(function (data) { cityList = data.data; });

    $("#btnCreate").click(function () {
        createStore();
    });
    if (!pageObj.exist) return;

    $('a[data-toggle="tab"]').on('show.bs.tab', function (e) {
        var activeTab = $(e.target).text();
        if (activeTab == "商户信息") {

        } else if (activeTab == "交易明细") {
            if (!detailVue) {
                initDetail();
            }
        } else if (activeTab == "账单") {
            if (!chargeVue) {
                initCharge();
            }
        }
    });
    initInfo();

    // 初始化商户信息选项卡
    function initInfo() {
        axios.get("/Dianwoda/Info")
            .then(function (res) {
                $("#info").html(res.data);
                infoVue = new Vue({
                    el: "#infoPage",
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
                            createStore();
                        },
                        recharge: function () {
                            $.view({
                                name: "recharge",
                                title: "商户充值",
                                footDisplay: "block",
                                template: `
                                    <div class="row form-horizontal" id="recharge-container">
                                        <div class="form-group">
                                            <label class="control-label col-xs-3">
                                                <span class="require">充值金额：</span>
                                            </label>
                                            <div class="col-xs-8">
                                                <input class="form-control" v-model="amount" type="number" />
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label class="control-label col-xs-3">
                                                <span class="require">充值方式：</span>
                                            </label>
                                            <div class="col-xs-9">
                                                <div class="kui-checkbox checkbox pull-left m-left-md m-right-md margin-right-20">
                                                    <label class="label-checkbox">
                                                        <input type="radio" name="serviceProvider" value="0" v-model="mode">
                                                        <span class="custom-checkbox"></span>
                                                        支付宝
                                                    </label>
                                                </div>
                                                <div class="kui-checkbox checkbox pull-left m-left-md m-right-md margin-right-20">
                                                    <label class="label-checkbox">
                                                        <input type="radio" name="serviceProvider" value="1" v-model="mode">
                                                        <span class="custom-checkbox"></span>
                                                        微信
                                                    </label>
                                                </div>
                                            </div>
                                        </div>
                                    </div>`,
                                load: function () {
                                    rechargeVue = new Vue({
                                        el: "#recharge-container",
                                        data: {
                                            amount: 0,
                                            mode: 0
                                        }
                                    });
                                    // 隐藏时销毁
                                    destroyVue.call(this, rechargeVue);
                                },
                                submit: function () {
                                    if (rechargeVue.amount == 0) {
                                        $.alert("请输入充值金额");
                                        return;
                                    }
                                    var url = rechargeVue.mode == 0 ? "Alipay" : "Wechat";
                                    //window.open(`/Dianwoda/${url}?amount=${rechargeVue.amount}`);
                                    window.open(`/Dianwoda/RechargePage?type=${url}&amount=${rechargeVue.amount}`);
                                    $.primary("充值完成后，点击确定刷新本页面", function () {
                                        location.reload();
                                    });
                                    return true;
                                }
                            });
                        }
                    }
                });
            });
    }
    // 初始化交易明细选项卡
    function initDetail() {
        axios.get("/Dianwoda/Detail")
            .then(function (res) {
                var $detail = $("#detail");
                $detail.html(res.data);
                detailVue = new Vue({
                    el: "#detailPage",
                    data: {
                        startDate: $detail.find("#txtStartDate").val(),
                        endDate: $detail.find("#txtEndDate").val(),
                        pageCount: 0,
                        pageIndex: 1,
                        list: null
                    },
                    methods: {
                        loadData: function () {
                            var self = this;
                            axios.get(`/Dianwoda/DetailList?pageIndex=${this.pageIndex}&startDate=${this.startDate}&endDate=${this.endDate}`)
                                .then(function (res) {
                                    self.list = res.data.result.detail;
                                    self.pageCount = res.data.result.page_count;
                                    console.log(self)
                                })
                                .catch(function (err) {
                                    $.alert(err);
                                });
                        },
                        search: function () {
                            this.loadData();
                        },
                        prev: function () {
                            this.pageIndex--;
                            this.loadData();
                        },
                        page: function (num) {
                            this.pageIndex = num;
                            this.loadData();
                        },
                        next: function () {
                            this.pageIndex++;
                            this.loadData();
                        }
                    },
                    created: function () {
                        this.loadData();
                    }
                });
                $detail.find("#txtStartDate").datetimepicker($.dateOptions).on("changeDate", function (e) {
                    detailVue.startDate = this.value;
                });
                $detail.find("#txtEndDate").datetimepicker($.dateOptions).on("changeDate", function (e) {
                    detailVue.endDate = this.value;
                });
            });
    }
    // 初始化账单选项卡
    function initCharge() {

    }
    // 创建新商户
    function createStore() {
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
                // 隐藏时销毁
                destroyVue.call(this, createVue);
            },
            submit: function () {
                var data = createVue.obj;
                if (!data.city_code || !data.shop_title || !data.mobile || !data.lng || !data.lat || !data.addr) {
                    $.alert("请将商户信息输入完整");
                    return false;
                }
                data.lng = data.lng;
                data.lat = data.lat;
                axios.post("/Dianwoda/Create", data)
                    .then(function (res) {
                        if (!res.data.success) {
                            $.alert(res.data.msg);
                            return;
                        }
                        location.reload();
                    })
                    .catch(function (err) {
                        $.alert(err);
                    });
                return true;
            }
        });
    }
    // 销毁vue对象
    function destroyVue(obj) {
        this.on("hidden.bs.modal", function () {
            if (!obj) return;
            obj.$destroy();
            obj = null;
        });
    }

})();

