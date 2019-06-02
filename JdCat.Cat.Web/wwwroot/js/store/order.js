(function () {
    var now = jdCat.utilMethods.now();
    var app = new Vue({
        el: "#app",
        data: {
            startDate: now,
            endDate: now,
            code: undefined,
            paging: {
                pageIndex: 1,
                pageSize: 20,
                pageCount: 1
            },
            items: null
        },
        methods: {
            loadData: function () {
                var self = this;
                axios.get(`/Store/GetOrders?pageIndex=${this.paging.pageIndex}&pageSize=${this.paging.pageSize}&startDate=${this.startDate}&endDate=${this.endDate}&code=${this.code || ""}`)
                    .then(function (res) {
                        self.paging.pageCount = Math.ceil(res.data.count / self.paging.pageSize);
                        self.items = res.data.rows;
                    })
                    .catch(function (err) {
                        $.alert(err);
                    });
            },
            next: function () {
                if (this.paging.pageIndex >= this.paging.pageCount) {
                    this.paging.pageIndex = this.paging.pageCount;
                    return;
                }
                this.paging.pageIndex++;
                this.loadData();
            },
            pre: function () {
                if (this.paging.pageIndex <= 1) {
                    this.paging.pageIndex = 1;
                    return;
                }
                this.paging.pageIndex--;
                this.loadData();
            },
            page: function (index) {
                if (this.paging.pageIndex === index) return;
                this.paging.pageIndex = index;
                this.loadData();
            },
            search: function () {
                this.loadData();
            },
            orderStatus: (function () {
                statusName = { 1: "正在点单", 2: "正在用餐", 4: "已结算", 8: "反结算", 16: "已取消", 32: "已退款" };
                return function (status) {
                    return statusName[status];
                };
            })(),
            catInfo: function (order) {
                $.loading();
                axios.get(`/Store/OrderDetail/${order.id}`)
                    .then(function (res) {
                        $.loaded();
                        $.view({
                            title: "订单详情",
                            template: res.data,
                            dialogWidth: 700
                        });
                    })
                    .catch(function (err) { $.loaded(); $.alert(err); });
            },
            reversePay: function (order) {
                reserve(order);
            }
        },
        created: function () {
            this.loadData();
        }
    });


    // 反结算
    var templateObj = {
        payment: `
        <div class="form-horizontal" id="paymentView">
            <div class="form-group" style="margin-bottom: 5px;">
                <label class="col-xs-3">
                    订单实际金额：
                </label>
                <div class="col-xs-4">
                    <span style='color: red;'>{{'￥ ' + amount}}</span>
                </div>
                <div class="col-xs-5">
                    <a @click.prevent="add()" class="text-primary pull-right"><i class="fa fa-plus"> 新增</i></a>
                </div>
            </div>
            <hr style="margin: 0 0 5px 0;" />
            <div class="form-group" v-for="payment in payments">
                <div class="col-xs-6">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <i class="fa fa-paypal"></i>
                        </span>
                        <select class="form-control" v-model="payment.name">
                            <option v-for="item in paymentTypes" :value="item.name">{{item.name}}</option>
                        </select>
                    </div>
                </div>
                <div class="col-xs-4">
                    <div class="input-group">
                        <span class="input-group-addon"><i class="fa fa-cny"></i></span>
                        <input type="number" class="form-control" v-model="payment.amount" @blur="calcReceiveAmount()" placeholder="支付金额" />
                    </div>
                </div>
                <div class="col-xs-2">
                    <a @click="remove(payment)" class="text-primary"><i class="fa fa-minus-circle" style="font-size: 20px;margin-top: 6px;"></i></a>
                </div>
            </div>
            <div class="form-group" style="margin-bottom: 5px;">
                <label class="col-xs-offset-7 col-xs-4">
                    收款总金额：
                    <span style='color: red;'>{{'￥ ' + receiveAmount}}</span>
                </label>
            </div>
        </div>
        `
    };
    var reserve = function (obj) {
        var template, vueObj, paymentTypes;

        axios.get('/Store/ReservePay')
            .then(function (res) {
                template = res.data;
                reserve = pay;
                pay(obj);
            });
        function pay(item) {
            $.loading();
            axios.get(`/Store/GetOrder/${item.id}`)
                .then(function (res) {
                    $.loaded();
                    loadOrder(res.data);
                })
                .catch(function (err) { $.loaded(); $.alert(err); });
        }
        function loadOrder(order) {
            $.view({
                title: "反结账",
                template,
                load: function () {
                    vueObj = new Vue({
                        el: "#reservePay",
                        data: {
                            entity: order
                        },
                        methods: {
                            editPayment: function () {
                                var vuePayment;

                                $.view({
                                    title: "修改订单支付方式",
                                    footDisplay: "block",
                                    template: templateObj.payment,
                                    load: function () {
                                        vuePayment = new Vue({
                                            el: "#paymentView",
                                            data: {
                                                amount: vueObj.entity.actualAmount,
                                                payments: JSON.parse(JSON.stringify(vueObj.entity.tangOrderPayments)),
                                                paymentTypes: JSON.parse(JSON.stringify(paymentTypes) || null),
                                                receiveAmount: 0
                                            },
                                            methods: {
                                                add: function () {
                                                    if (this.payments.length >= 5) {
                                                        $.alert("最多只能设置5种支付方式", "warning");
                                                        return;
                                                    }
                                                    var obj = { name: "", amount: +(this.amount - this.receiveAmount).toFixed(2), tangOrderId: order.id, orderObjectId: order.objectId };
                                                    this.payments.push(obj);
                                                    this.calcReceiveAmount();
                                                },
                                                remove: function (payment) {
                                                    if (this.payments.length === 1) return;
                                                    this.payments.remove(payment);
                                                    this.calcReceiveAmount();
                                                },
                                                calcReceiveAmount: function () {
                                                    this.receiveAmount = +this.payments.sum(a => +a.amount).toFixed(2);
                                                }
                                            },
                                            created: function () {
                                                this.calcReceiveAmount();
                                                if (paymentTypes) return;
                                                var self = this;
                                                axios.get("/Store/GetPayments").then(function (res) {
                                                    paymentTypes = res.data;
                                                    self.paymentTypes = JSON.parse(JSON.stringify(paymentTypes));
                                                });
                                            }
                                        });
                                        this.css("z-index", 1061);
                                        this.next().css("z-index", 1060);

                                        destroyVue.call(this, vuePayment);
                                    },
                                    submit: function () {
                                        vuePayment.calcReceiveAmount();
                                        if (vuePayment.amount !== vuePayment.receiveAmount) {
                                            $.alert("收款总金额必须等于订单实际金额");
                                            return false;
                                        }
                                        if (vuePayment.payments.first(a => !a.name)) {
                                            $.alert("请选择收款方式");
                                            return false;
                                        }
                                        if (vuePayment.payments.first(a => a.amount <= 0)) {
                                            $.alert("收款金额必须大于零");
                                            return false;
                                        }
                                        var groups = vuePayment.payments.group(a => a.name);
                                        if (groups.first(a => a.list.length > 1)) {
                                            $.alert("不能存在两种相同的支付方式");
                                            return false;
                                        }
                                        vuePayment.payments.forEach(payment => {
                                            if (payment.id > 0) return;
                                            var type = paymentTypes.first(a => a.name === payment.name);
                                            payment.paymentTypeId = type.id;
                                        });
                                        $.loading();
                                        axios.post("/Store/UpdatePayments", vuePayment.payments)
                                            .then(function (res) {

                                            })
                                            .catch(function (err) { $.loaded(); $.alert(err); });
                                        return true;
                                    }
                                });
                            }
                        }
                    });

                    destroyVue.call(this, vueObj);
                },
                dialogWidth: 800
            });
        }

    };
    // 重新计算订单的各种金额
    function calcOrder() {

    }

    Vue.filter("paymentFilter", function (items) {
        if (!items || items.length === 0) return '';
        if (items.length === 1) return items[0].name;
        let result = '';
        items.forEach(function (item) { result += `${item.name}:${item.amount},`; });
        result = result.slice(0, result.length - 1);
        return result;
    });
    var sourceData = { 1: "收银台", 2: "扫码点餐" };
    Vue.filter("sourceFilter", function (orderSource) {
        return sourceData[orderSource];
    });
    var modeData = { 0: "未定义", 1: "快餐", 2: "中餐" };
    Vue.filter("modeFilter", function (mode) {
        return modeData[mode];
    });
    var statusData = { 1: "正在点单", 2: "用餐中", 4: "已结算", 8: "反结算", 16: "已取消", 32: "已退款" };
    Vue.filter("statusFilter", function (status) {
        return statusData[status];
    });


    $("#txtStartDate").datetimepicker(jdCat.utilData.dateOptions).on("changeDate", function (e) {
        app.startDate = this.value;
    });
    $("#txtEndDate").datetimepicker(jdCat.utilData.dateOptions).on("changeDate", function (e) {
        app.endDate = this.value;
    });

    // 释放vue对象
    function destroyVue(obj) {
        this.on("hidden.bs.modal", function () {
            if (!obj) return;
            obj.$destroy();
            obj = null;
        });
    }
})();