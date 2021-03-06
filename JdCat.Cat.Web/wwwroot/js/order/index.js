﻿; (function ($) {
    var typeName = ["外卖", "堂食"], paymentName = ["", "线上支付"],
        statusName = {
            "1": "已付款", "2": "已拒单", "4": "待配送", "8": "待配送", "16": "配送中", "32": "配送异常", "64": "已送达", "128": "用户已确认收货", "256": "未付款", "512": "已评价", "1024": "已关闭", "2048": "配送已取消", "4096": "订单已取消"
        },
        providerName = { "0": "未知", "1": "自己配送", "2": "达达配送", "3": "美团配送", "4": "蜂鸟配送", "5": "点我达配送", "6": "一城飞客配送", "7": "顺丰同城" },
        delivered = 8 + 16 + 64 + 128 + 512;// 已发货

    var vueReason = null, $tipModal, cancelOrderVue;
    var app = new Vue({
        el: "#app",
        data: {
            buttons: [
                { name: "全部订单", type: 0, selected: true },
                { name: "待付款", type: 256, selected: false },
                { name: "待接单", type: 1, selected: false },
                { name: "已接单", type: 2052, selected: false },
                //{ name: "配送中", type: 16, selected: false },
                { name: "待确认", type: 64, selected: false },
                { name: "已关闭", type: 1024, selected: false },
                { name: "已完成", type: 128, selected: false }
            ],
            orderList: [],
            deviceList: pageObj.deviceList,
            pageObj: {
                pageIndex: 1,
                pageSize: 20,
                pageCount: 0,
                recordCount: 0
            },
            curType: null,              // 当前选择的订单状态
            sendTypes: [
                { name: "达达配送", selected: false, type: 0, logisticsType: 2, doing: false },
                { name: "美团配送", selected: false, type: 0, logisticsType: 3, doing: true },
                //{ name: "蜂鸟配送", selected: false, type: 0, logisticsType: 4, doing: true },
                { name: "点我达配送", selected: false, type: 0, logisticsType: 5, doing: false },
                { name: "一城飞客配送", selected: false, type: 0, logisticsType: 6, doing: false },
                { name: "自己配送", selected: false, type: 1, logisticsType: 1, doing: false },
                { name: "顺丰同城", selected: false, type: 1, logisticsType: 7, doing: false }
            ],
            curOrder: null,             // 当前选择配送的订单
            printerCode: localStorage.getItem("defaultPrinter"),          // 当前选择的打印机编码
            curSendType: null,
            search_code: pageObj.code,
            search_phone: ""
        },
        methods: {
            initPageObj: function () {
                this.pageObj.pageIndex = 1;
                this.pageObj.pageSize = 20;
                this.pageObj.pageCount = 0;
                this.pageObj.recordCount = 0;
            },
            loadData: function () {
                var self = this;
                var time = $("#txtStartDate").val();
                axios.post(`/order/getorders?status=${this.curType.type}&code=${this.search_code}&phone=${this.search_phone}&startDate=${time}&endDate=${time}`, this.pageObj)
                    .then(function (res) {
                        self.orderList = handerData(res.data.data.list);
                        self.pageObj.recordCount = res.data.data.rows;
                        self.pageObj.pageCount = Math.ceil(self.pageObj.recordCount / self.pageObj.pageSize);
                    })
                    .catch(function (msg) {
                        $.loaded();
                        $.alert(msg);
                    });
            },
            changeType: function (item) {
                this.buttons.forEach(obj => { obj.selected = false; });
                item.selected = true;
                this.curType = item;
                this.initPageObj();
                this.loadData();
            },
            expend: function (order) {
                order.expend = !order.expend;
                if (order.loaded) return;
                axios.get("/Order/GetOrderDetail/" + order.id)
                    .then(function (res) {
                        order.loaded = true;
                        for (var key in order) {
                            if (!order[key]) {
                                order[key] = res.data[key];
                            }
                        }
                    })
                    .catch(function (err) { $.alert(err); });
            },
            prevPage: function () {
                if (this.pageObj.pageIndex === 1) return;
                this.pageObj.pageIndex--;
                this.loadData();
            },
            changePage: function (num) {
                if (this.pageObj.pageIndex === num) return;
                this.pageObj.pageIndex = num;
                this.loadData();
            },
            nextPage: function () {
                if (this.pageObj.pageIndex === this.pageObj.pageCount) return;
                this.pageObj.pageIndex = this.pageObj.pageIndex + 1;
                this.loadData();
            },
            receive: function (order) {
                $("#modal-sendType").modal({ backdrop: "static" });
                this.curOrder = order;
            },
            reject: function (order) {
                $.view({
                    title: "原因",
                    footDisplay: true,
                    template: `
                        <div class='row'>
                            <div class='col-xs-12'>请输入拒绝原因</div>
                            <div class='col-xs-12'>
                                <textarea class='form-control'></textarea>
                            </div>
                            <div class='col-xs-12'>
                                <h5 class='text-danger color-red'>提示：拒绝或取消订单后，订单金额将原路返回消费者</h5>
                            </div>
                        </div>`,
                    submit: function (e, $modal) {
                        var reason = $modal.find("textarea").val();
                        if (!reason) {
                            $.alert("请输入拒绝原因");
                            return false;
                        }
                        $.loading();
                        axios.get("/order/reject/" + order.id + "?msg=" + reason)
                            .then(function (res) {
                                $.loaded();
                                if (res.data.success) {
                                    $.alert(res.data.msg, "success");
                                    order.status = res.data.data.status;
                                    order.refundStatus = res.data.data.refundStatus;

                                } else {
                                    $.alert(res.data.msg);
                                }
                            })
                            .catch(function (err) {
                                $.loaded();
                                $.alert(err);
                            });
                        return true;
                    }
                });
            },
            cancel: function (order) {                                  // 取消配送
                $.view({
                    title: "取消原因",
                    saveText: "确定",
                    footDisplay: true,
                    template: `
                        <div class='row' id='vueReason'>
                            <label class='label-control col-xs-12 nowarp'>选择原因：</label>
                            <div class='col-xs-12'>
                                <select class='form-control' v-model='flagId'>
                                    <option v-for='reason in reasonList' :value='reason.flagId'>{{reason.reason}}</option>
                                </select>
                            </div>
                            <hr class='clearfix' />
                            <label class='label-control col-xs-2 nowarp'>备注：</label>
                            <div class='col-xs-12'>
                                <textarea class='form-control' placeholder='输入其他原因' v-model='reason'></textarea>
                            </div>
                            <div class='col-xs-12' v-if='tipShow'>
                                <h5 class='text-danger'>提示：配送取消后，将会产生一定的违约金</h5>
                            </div>
                        </div>`,
                    load: function () {
                        vueReason = new Vue({
                            el: "#vueReason",
                            data: {
                                reasonList: pageObj.reasonList,
                                flagId: 1,
                                reason: null,
                                tipShow: order.deliveryMode == 0
                            }
                        });
                        this.on("hidden.bs.modal", function () {
                            if (!vueReason) return;
                            vueReason.$destroy();
                            vueReason = null;
                        });
                    },
                    submit: function () {
                        axios.get(`/order/cancel/${order.id}?flagId=${vueReason.flagId}&reason=${vueReason.reason}`)
                            .then(function (res) {
                                if (res.data.success) {
                                    $.alert(res.data.msg, "success");
                                    order.status = res.data.data;
                                } else {
                                    $.alert(res.data.msg);
                                }
                            })
                            .catch(function (err) {
                                $.alert(err);
                            });
                        return true;
                    }
                });
            },
            achieve: function (order) {                                 // 商家自己配送的订单，自己操作配送完成
                $.primary("确保订单已经送达？", function () {
                    axios.get("/order/achieve/" + order.id)
                        .then(function (res) {
                            if (res.data.success) {
                                $.alert(res.data.msg, "success");
                                order.status = res.data.data;
                            } else {
                                $.alert(res.data.msg);
                            }
                        })
                        .catch(function (err) {
                            $.alert(err);
                        });
                    return true;
                });
            },
            selectSendType: function (item) {                           // 选择配送类别
                if (item.doing) {
                    $.alert("工程师正在努力对接中...", "warning");
                    return;
                }
                this.sendTypes.forEach(a => a.selected = false);
                item.selected = true;
                this.curSendType = item;
            },
            submitSend: function () {                                   // 提交配送
                var self = this;
                if (!this.curSendType) {
                    $.alert("请选择配送方式");
                    return;
                }
                if (!this.curOrder) {
                    $.alert("操作错误，请重新选择要配送的订单");
                    return;
                }
                $.loading();
                $("#modal-sendType").modal("hide");
                axios.get(`/order/send/${this.curOrder.id}?type=${this.curSendType.type}&logisticsType=${this.curSendType.logisticsType}`)
                    .then(function (res) {
                        $.loaded();
                        if (res.data.success) {
                            $.alert("操作成功", "success");
                            self.curOrder.status = res.data.data.status;
                            self.curOrder.deliveryMode = res.data.data.mode;
                            self.curOrder.distributionFlow = res.data.data.flow;
                        } else {
                            $.alert(res.data.msg);
                        }
                    })
                    .catch(function (err) {
                        $.loaded();
                        $.alert(err);
                    });
            },
            changePrinter: function () {
                localStorage.setItem("defaultPrinter", this.printerCode);       // 每次选择打印机后，将当前选择的打印机编码存储在本地
            },
            print: function (order) {
                var self = this;
                var printer = this.deviceList.filter(function (obj) { return obj.code == self.printerCode; })[0];
                if (!printer) {
                    $.alert("请选择小票打印设备！");
                    return;
                }
                axios.get(`/order/print/${order.id}?device_id=${printer.id}`)
                    .then(function (res) {
                        if (res.data.success) {
                            $.alert(res.data.msg, "success");
                        } else {
                            $.alert(res.data.msg);
                        }
                    })
                    .catch(function (err) {
                        $.alert(err);
                    });
            },
            addTip: function (order) {
                $tipModal = $.view({
                    template: `
                        <div class="row form-horizontal">
                            <div class="form-group">
                                <label class="control-label col-xs-3">
                                    <span class="require">小费总金额：</span>
                                </label>
                                <div class="col-xs-8">
                                    <input class="form-control add-tip" type="number" value="1" />
                                </div>
                            </div>
                        </div>
                    `,
                    title: "添加小费",
                    footDisplay: true,
                    submit: function () {
                        var tip = +$tipModal.find(".add-tip").val();
                        if (tip > 50) {
                            $.alert("小费金额不能大于50");
                            return false;
                        }
                        axios.get(`/Order/AddTip/${order.id}?tip=${tip}&code=${order.orderCode}&distributionFlow=${order.distributionFlow}`)
                            .then(function (res) {
                                if (res.data.success) {
                                    $.alert("添加小费成功", "success");
                                    return;
                                }
                                $.alert(res.data.message);
                            })
                            .catch(function (err) {
                                $.alert(err);
                            });
                        return true;
                    }
                });
            },
            cancelOrder: function (order) {
                $.view({
                    title: "填写取消原因",
                    saveText: "确定",
                    footDisplay: true,
                    template: `<div class='form-horizontal'id='cancelReason'>
                                <div class='row'>
                                    <label class='label-control col-xs-2 nowarp'>取消原因：</label>
                                    <div class='col-xs-10'>
                                        <select class='form-control' v-model='selectedValue'>
                                            <option v-for='item in reasons' :value='item.reason'>{{item.reason}}</option>
                                        </select>
                                    </div>
                                </div>
                                <div class='row margin-top-10'>
                                    <label class='label-control col-xs-2 nowarp'>其他原因：</label>
                                    <div class='col-xs-10'>
                                        <textarea class='form-control' v-model='reason'></textarea>
                                    </div>
                                </div>
                                <div class='row margin-top-10'>
                                    <div class='col-xs-12'>
                                        <h5 class='text-danger color-red'>提示：拒绝或取消订单后，订单金额将原路返回消费者</h5>
                                    </div>
                                </div>
                            </div>`,
                    load: function () {
                        var reasons = JSON.parse(JSON.stringify(pageObj.reasonList));
                        cancelOrderVue = new Vue({
                            el: "#cancelReason",
                            data: {
                                reason: order.refundReason,
                                reasons: reasons,
                                selectedValue: ""
                            }
                        });
                        this.on("hidden.bs.modal", function () {
                            if (!cancelOrderVue) return;
                            cancelOrderVue.$destroy();
                            cancelOrderVue = null;
                        });
                    },
                    submit: function () {
                        var reason = cancelOrderVue.reason || cancelOrderVue.selectedValue;
                        if (!reason) {
                            $.alert("请选择或输入取消原因");
                            return false;
                        }
                        $.loading();
                        axios.get(`/order/cancelOrder/${order.id}?reason=${reason}`)
                            .then(function (res) {
                                $.loaded();
                                if (res.data.success) {
                                    $.alert(res.data.msg, "success");
                                    order.status = res.data.data.status;
                                    order.refundStatus = res.data.data.refundStatus;
                                } else {
                                    $.alert(res.data.msg);
                                }
                            })
                            .catch(function (err) {
                                $.loaded();
                                $.alert(err);
                            });
                        return true;
                    }
                });
            },
            search: function () {
                this.initPageObj();
                this.loadData();
            },
            checklogistics: function (order) {
                axios.get(`/Dianwoda/OrderDetail?orderCode=${order.orderCode}_${order.distributionFlow}`)
                    .then(function (res) {
                        $.view({
                            template: res.data,
                            title: "配送信息"
                        });
                    })
                    .catch(function (err) {
                        $.alert(err);
                    });
            },
            category: function (type) {
                return typeName[type];
            },
            paymentType: function (type) {
                return paymentName[type];
            },
            status: function (type) {
                return statusName[type];
            },
            deliveryType: function (order) {
                if (+order.status & delivered === 0) return "";
                return providerName[order.logisticsType];
            },
            gender: function (order) {
                if (order.receiverName.length > 1) return "";
                else if (order.gender == 1) return "先生";
                else if (order.gender == 2) return "女士";
                return "";
            }
        },
        created: function () {
            var self = this;
            this.curType = this.buttons[0];
            this.loadData();
        }
    });

    function handerData(list) {
        if (list.length === 1) {
            list[0].expend = true;
        }
        else {
            list.map(function (obj) {
                obj.expend = false;
            });
        }
        return list;
    }

    var dateOptions = {
        format: 'yyyy-mm-dd',
        autoclose: true,
        maxView: 1,
        minView: 2,
        todayBtn: true,
        todayHighlight: true,
        language: "zh-CN"
    };
    $("#txtStartDate").datetimepicker(dateOptions);

    $("#modal-sendType").on("hidden.bs.modal", () => {
        app.sendTypes.forEach(a => a.selected = false);
        app.curSendType = null;
    });

})(jQuery);