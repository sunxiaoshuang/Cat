;
(function () {
    var statusName = {
        "1": "已付款", "2": "已拒单", "4": "待配送", "8": "待配送", "16": "配送中", "32": "配送异常", "64": "已送达", "128": "用户已确认收货", "256": "未付款", "512": "已评价", "1024": "已关闭", "2048": "配送已取消", "4096": "订单已取消"
    };
    var vm = new Vue({
        el: "#app",
        data: {
            orderList: [],
            startDate: new Date().format("yyyy-MM-dd"),
            endDate: new Date().format("yyyy-MM-dd"),
            sources: [{ name: "全部", val: "99" }, { name: "美团", val: '0' }, { name: "饿了么", val: '1' }],
            dayNum: '',
            selectedSource: "99",
            paging: {
                pageSize: 20,
                pageIndex: 1,
                pageCount: 1,
                recordCount: 0
            }
        },
        methods: {
            getData: function () {
                var self = this;
                if (!this.startDate || !this.endDate) {
                    $.alert("请选择订单起止时间");
                    return;
                }
                if (this.endDate < this.startDate) {
                    $.alert("订单开始时间不能大于截止时间");
                    return;
                }
                var timespan = new Date(this.endDate) - new Date(this.startDate);
                var days = timespan / (1000 * 60 * 60 * 24);
                if (days > 31) {
                    $.alert("订单起止时间间隔必须30天以内");
                    return;
                }
                var num = this.dayNum || 0;
                if (num.toString().indexOf(".") > -1) num = 0;
                $.loading();
                var query = `pageSize=${pageSize = this.paging.pageSize}&pageIndex=${this.paging.pageIndex}&source=${this.selectedSource}&start=${this.startDate}&end=${this.endDate}&dayNum=${num}`;
                axios.get(`/ThirdOrder/GetOrders?${query}`)
                    .then(function (res) {
                        $.loaded();
                        self.orderList = res.data.list;
                        self.paging.pageCount = res.data.pages;
                        self.paging.recordCount = res.data.rows;
                    })
                    .catch(function (err) {
                        $.loaded();
                        $.alert(err);
                    });
            },
            search: function () {
                this.initPaging();
                this.getData();
            },
            initPaging: function () {
                this.paging.pageIndex = 1;
                this.paging.pageSize = 20;
                this.paging.pageCount = 1;
                this.paging.recordCount = 0;
            },
            nextPage: function () {
                if (this.paging.pageIndex >= this.paging.pageCount) return;
                this.paging.pageIndex++;
                this.getData();
            },
            changePage: function (num) {
                if (this.paging.pageIndex === num) return;
                this.paging.pageIndex = num;
                this.getData();
            },
            prevPage: function () {
                if (this.paging.pageIndex <= 1) return;
                this.paging.pageIndex--;
                this.getData();
            },
            status: function (type) {
                return statusName[type];
            },
            cat: function (order) {
                $.view({
                    title: "订单详情",
                    url: "/ThirdOrder/Detail/" + order.id,
                    dialogWidth: 800
                });
            },
            print: function (order) {
                axios.get("/ThirdOrder/Print/" + order.id)
                    .then(function () {
                        $.alert("正在补打订单，请稍等...", "success");
                    });
            }
        },
        created: function () {
            this.initPaging();
            this.getData();
        }
    });

    $("#txtStartDate").datetimepicker(jdCat.utilData.dateOptions).on("changeDate", function (e) {
        vm.startDate = this.value;
    });
    $("#txtEndDate").datetimepicker(jdCat.utilData.dateOptions).on("changeDate", function (e) {
        vm.endDate = this.value;
    });

})();

