; (function ($) {
    var typeName = ["外卖", "堂食"], paymentName = ["", "线上支付"], deliveryName = ["第三方平台", "自己配送", "自提"],
        statusName = {
            "1": "已付款", "2": "已拒单", "4": "待配送", "8": "待配送", "16": "配送中", "32": "配送异常", "64": "已送达", "128": "用户已确认收货", "256": "未付款", "512": "已评价", "1024": "已关闭", "2048": "配送已取消", "4096": "订单已取消"
        };

    new Vue({

        el: "#app",
        data: {
            pageObj: {
                pageIndex: 1,
                pageSize: 20,
                pageCount: 0,
                recordCount: 0
            },
            orderList: []
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
                var startDate = $("#txtStartDate").val();
                var endDate = $("#txtEndDate").val();
                axios.post(`/order/getorder?status=0&startDate=${startDate}&endDate=${endDate}`, this.pageObj)
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
            search: function () {
                this.initPageObj();
                this.loadData();
            },
            expend: function (order) {
                order.expend = !order.expend;
            },
            changePage: function (num) {
                if (this.pageObj.pageIndex === num) return;
                this.pageObj.pageIndex = num;
                this.loadData();
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
            deliveryType: function (type) {
                return deliveryName[type];
            }
        },
        created: function () {
            var self = this;
            this.loadData();
        }
    });

    function handerData(list) {
        list.map(function (obj) {
            obj.expend = false;
        });
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
    $("#txtEndDate").datetimepicker(dateOptions);


})(jQuery);