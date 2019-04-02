(function () {
    var now = jdCat.utilMethods.now();
    var app = new Vue({
        el: "#app",
        data: {
            startDate: now,
            endDate: now,
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
                axios.get(`/Store/GetOrders?pageIndex=${this.paging.pageIndex}&pageSize=${this.paging.pageSize}&startDate=${this.startDate}&endDate=${this.endDate}`)
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
            orderStatus: (function () {
                statusName = { 1: "正在点单", 2: "正在用餐", 4: "已结算", 8: "反结算", 16: "已取消", 32: "已退款" };
                return function (status) {
                    return statusName[status];
                };
            })()
        },
        created: function () {
            this.loadData();
        }
    });

    $("#txtStartDate").datetimepicker(jdCat.utilData.dateOptions).on("changeDate", function (e) {
        app.startDate = this.value;
    });
    $("#txtEndDate").datetimepicker(jdCat.utilData.dateOptions).on("changeDate", function (e) {
        app.endDate = this.value;
    });
})();