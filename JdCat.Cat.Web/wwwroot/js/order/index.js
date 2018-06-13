; (function ($) {
    var typeName = ["外卖", "堂食"], paymentName = ["", "线上支付"], deliveryName = ["第三方平台", "自己配送", "自提"],
        statusName = {
            "1": "已付款", "2": "已拒单", "4": "待配送", "8": "待配送", "16": "配送中", "32": "配送异常", "64": "已送达", "128": "用户已确认收货", "256": "未付款", "512": "已评价", "1024": "已关闭"
        };
    new Vue({
        el: "#app",
        data: {
            buttons: [
                { name: "全部订单", type: 0, selected: true },
                { name: "待付款", type: 256, selected: false },
                { name: "待接单", type: 1, selected: false },
                { name: "已接单", type: 4, selected: false },
                { name: "待配送", type: 8, selected: false },
                { name: "配送中", type: 16, selected: false },
                { name: "待确认", type: 64, selected: false },
                { name: "已关闭", type: 1024, selected: false },
                { name: "已完成", type: 128, selected: false },
            ],
            orderList: [
                { id: 0, orderCode: "85412654545812", createTime: "2018-12-31 11:34:27", category: 0, paymentType: 1, status: 1, price: 552.65, receiverAddress: "湖北省武汉市武汉经济技术开发区神龙大道69号", receiverName: "张三", phone: "17355698852", expend: false },
                { id: 1, orderCode: "80012015204551", createTime: "2018-05-22 06:11:00", category: 0, paymentType: 1, status: 2, price: 25633.65, expend: false },
                { id: 2, orderCode: "56333221478992", createTime: "2012-02-09 23:47:27", category: 0, paymentType: 1, status: 4, price: 7855200.65, expend: false },
            ],
            pageObj: {
                pageIndex: 1,
                pageSize: 20,
                pageCount: 0,
                recordCount: 0
            },
            curType: null
        },
        methods: {
            initPageObj: function(){
                this.pageObj.pageIndex = 1;
                this.pageObj.pageSize = 20;
                this.pageObj.pageCount = 0;
                this.pageObj.recordCount = 0;
            },
            loadData: function () {
                var self = this;
                axios.post(`/order/getorder?status=${this.curType.type}`, this.pageObj)
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
            changeType: function(item) {
                this.buttons.forEach(obj => { obj.selected = false; });
                item.selected = true;
                this.curType = item;
                this.initPageObj();
                this.loadData();
            },
            expend: function(order) {
                order.expend = !order.expend;
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
                axios.post("/order/receive/" + order.id)
                    .then(function (res) {
                        if (res.data.success) {
                            $.alert(res.data.msg, "success");
                            order.status = 4;
                        } else {
                            $.alert(res.data.msg);
                        }
                    })
                    .catch(function (err) {
                        $.alert(err);
                    });
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
                        </div>`,
                    submit: function (e, $modal) {
                        var reason = $modal.find("textarea").val();
                        if (!reason) {
                            $.alert("请输入拒绝原因");
                            return false;
                        }
                        axios.post("/order/reject/" + order.id)
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
                        return true;
                    }
                });
            },
            thirdSend: function (order) {

            },
            selfSend: function (order) {

            }
        },
        filters: {
            category: function(type) {
                return typeName[type];
            },
            paymentType: function(type) {
                return paymentName[type];
            },
            status: function(type) {
                return statusName[type];
            },
            deliveryType: function(type) {
                return deliveryName[type];
            }
        },
        created: function () {
            var self = this;
            this.curType = this.buttons[0];
            this.loadData();
        }
    });

    function handerData(list) {
        list.map(function (obj) {
            obj.expend = false;
        });
        return list;
    }
})(jQuery);