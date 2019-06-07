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

    
    var templateObj = {
        // 支付方式更新视图模版
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
                        <input type="number" class="form-control" v-model.number="payment.amount" @blur="calcReceiveAmount()" placeholder="支付金额" />
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
        `,
        // 新增商品视图模版
        product: `
        <div class="form-horizontal" id="productView">
            <p class="text-primary">注：添加的商品金额将累加到订单的总金额中，添加完成后，请修改对应的支付方式，否则报表统计将会出错！</p>
            <div class="form-group" style="margin-bottom: 5px;">
                <div class="col-xs-12" style="relative">
                    <div class="input-group">
                        <span class="input-group-addon">
                            商品搜索
                        </span>
                        <input text="number" class="form-control" v-model="search.key" @keyup.esc="clear()" @keyup.enter="enter()" @keyup.up="up()" @keyup.down="down()" @blur="blur()" @focus="focus()" />
                        <span class="input-group-addon">
                            <i class="fa fa-search"></i>
                        </span>
                    </div>
                    <div class="search-container" style="width: 90%;" v-bind:style="{'min-height': search.boxHeight}">
                        <div class="search-box">
                            <div class="search-item" v-for="item in search.result" v-bind:class="{'selected': item.selected}" @click="clickItem(item)">
                                {{item.name}}
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <hr />
            <div class="form-group">
                <label class="col-xs-3 control-label">
                    商品名称：
                </label>
                <label class="col-xs-7 control-label" style="text-align: left;">
                    {{entity.name}}
                </label>
            </div>
            <div class="form-group" v-show="formats.length > 1">
                <label class="col-xs-3 control-label">
                    商品规格：
                </label>
                <div class="col-xs-7">
                    <select class="form-control" v-model="entity.formatId">
                        <option v-for="format in formats" :value="format.id">{{format.name}}</option>
                    </select>
                </div>
            </div>
            <div class="form-group">
                <label class="col-xs-3 control-label">
                    商品单价：
                </label>
                <div class="col-xs-7">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <i class="fa fa-cny"></i>
                        </span>
                        <input text="number" class="form-control" v-model.number="entity.price" />
                    </div>
                </div>
            </div>
            <div class="form-group">
                <label class="col-xs-3 control-label">
                    商品数量：
                </label>
                <div class="col-xs-7">
                    <input text="number" class="form-control" v-model.number="entity.quantity" />
                </div>
            </div>
            <div class="form-group">
                <label class="col-xs-3 control-label">
                    商品总价：
                </label>
                <label class="col-xs-7 control-label" style="text-align: left;">
                    <i class="fa fa-cny" style="color: red;"> {{entity.amount}}</i>
                </label>
            </div>
            
        </div>
        `
    };
    // 反结账
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
                            // 更改支付方式
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
                                                    this.receiveAmount = +this.payments.sum(a => a.amount).toFixed(2);
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
                                            payment.paymentTypeObjectId = type.id;
                                        });
                                        $.loading();
                                        axios.post(`/Store/UpdatePayments/${order.id}`, vuePayment.payments)
                                            .then(function (res) {
                                                $.loaded();
                                                $.alert("修改成功", "success");
                                                order.tangOrderPayments = res.data;
                                            })
                                            .catch(function (err) { $.loaded(); $.alert(err); });
                                        return true;
                                    }
                                });
                            },
                            // 添加商品
                            add: function () {
                                increase(order);
                            },
                            // 退菜
                            ret: function (product) {
                                if (order.orderDiscount !== 10) {
                                    $.alert("整单折扣的订单不可以退货！", "warning");
                                    return;
                                }
                                $.primary("退货后订单金额将会减去退掉的商品金额，需要您手动修改支付方式，否则将导致报表错误，确定退货吗？", function () {
                                    $.loading();
                                    axios.post("/Store/RetTangProduct", product)
                                        .then(function (res) {
                                            $.loaded();
                                            $.alert("退货成功", "success");
                                            order.actualAmount = res.data.actualAmount;
                                            order.amount = res.data.amount;
                                            order.originalAmount = res.data.originalAmount;
                                            product.productStatus = 16;
                                            var item = app.items.first(a => a.id === order.id);
                                            if (item) item.actualAmount = order.actualAmount;
                                        })
                                        .catch(function (err) { $.loaded(); $.alert(err); });
                                    return true;
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

    var increase = function (obj) {
        var products, vueObj;
        axios.get("/Store/GetSimpleStoreProducts")
            .then(function (res) {
                products = res.data || [];
                products.forEach(a => a.selected = false);
                increase = increaseProduct;
                increase(obj);
            });

        function increaseProduct(order) {
            $.view({
                title: "添加订单商品",
                template: templateObj.product,
                footDisplay: "block",
                load: function () {
                    vueObj = new Vue({
                        el: "#productView",
                        data: {
                            products: [],
                            entity: {
                                name: "",
                                quantity: 1,
                                price: 0,
                                amount: 0,
                                description: "",
                                productIdSet: "",
                                productId: 0,
                                orderId: 0,
                                formatId: 0
                            },
                            formats: [],
                            search: {
                                key: "",
                                result: [],
                                boxHeight: 0
                            }
                        },
                        watch: {
                            "search.key": function () {
                                this.seek(this.search.key);
                            },
                            "entity.quantity": function () {
                                this.calc();
                            },
                            "entity.price": function () {
                                this.calc();
                            },
                            "entity.formatId": function () {
                                var format = this.formats.first(a => a.id === this.entity.formatId);
                                this.entity.description = format.name;
                                this.entity.price = format.price;
                            }
                        },
                        methods: {
                            focus: function () {
                                this.seek(this.search.key);
                            },
                            blur: function () {
                                var self = this;
                                setTimeout(function () { self.search.boxHeight = 0; }, 200);
                            },
                            seek: function (key) {
                                if (!key) {
                                    this.search.boxHeight = "0";
                                    this.search.result = [];
                                    return;
                                }
                                this.search.boxHeight = "120px";
                                products.forEach(a => a.selected = false);
                                this.search.result = products
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
                                $.extend(this.entity, { name: product.name, price: product.format[0].price, productIdSet: product.productIdSet, productId: product.id, orderId: order.id, formatId: product.format[0].id, orderObjectId: order.orderObjectId });
                                this.formats = product.format;
                                this.clear();
                            },
                            calc: function () {
                                this.entity.amount = +(this.entity.price * this.entity.quantity).toFixed(2);
                            }
                        }
                    });
                    this.css("z-index", 1061);
                    this.next().css("z-index", 1060);
                    destroyVue.call(this, vueObj);
                },
                submit: function () {
                    if (!vueObj.entity.name) {
                        $.alert("请选择需要添加的商品");
                        return false;
                    }
                    if (vueObj.entity.price <= 0) {
                        $.alert("商品金额必须大于零");
                        return false;
                    }
                    if (vueObj.entity.quantity <= 0) {
                        $.alert("商品数量必须大于零");
                        return false;
                    }
                    $.loading();
                    axios.post("/Store/IncreaseOrderProduct", $.extend({}, vueObj.entity, { discount: 10, originalPrice: vueObj.entity.price}))
                        .then(function (res) {
                            $.loaded();
                            $.alert("添加成功", "success");
                            order.actualAmount = res.data.order.actualAmount;
                            order.amount = res.data.order.amount;
                            order.originalAmount = res.data.order.originalAmount;
                            order.tangOrderProducts.push(res.data.product);
                            var item = app.items.first(a => a.id === order.id);
                            if (item) item.actualAmount = order.actualAmount;
                        })
                        .catch(function (err) { $.loaded(); $.alert(err); });
                    return true;
                }
            });
        }
    };

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
        });
    }
})();