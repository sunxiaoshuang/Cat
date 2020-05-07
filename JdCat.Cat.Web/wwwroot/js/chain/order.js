;
(function () {
    pageObj.stores.unshift({ item1: 0, item2: "全部" });
    var statusName = {
        "1": "已付款", "2": "已拒单", "4": "待配送", "8": "待配送", "16": "配送中", "32": "配送异常", "64": "已送达", "128": "用户已确认收货", "256": "未付款", "512": "已评价", "1024": "已关闭", "2048": "配送已取消", "4096": "订单已取消"
    };
    var vm = new Vue({
        el: "#app",
        data: {
            orderList: [],
            stores: pageObj.stores.map(function (a) { return { id: a.item1, name: a.item2 };}),
            selectedItem: 0,
            startDate: pageObj.date,
            endDate: pageObj.date,
            buttons: [
                { name: "全部订单", type: 0, selected: true },
                { name: "待接单", type: 1, selected: false },
                { name: "已接单", type: 2048 + 4, selected: false },
                { name: "已关闭", type: 1024, selected: false },
                { name: "已完成", type: 128 + 64, selected: false }
            ],
            paging: {
                pageSize: 20,
                pageIndex: 1,
                pageCount: 1,
                recordCount: 0
            },
            deliveryMode: [
                { name: '全部', value: 99 },
                { name: '外送', value: 0 },
                { name: '商家自送', value: 1 },
                { name: '自提', value: 2 }
            ],
            delivery: 99
        },
        methods: {
            getData: function () {
                var self = this, type = this.buttons.filter(a => a.selected)[0];
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
                var query = `pageSize=${pageSize = this.paging.pageSize}&pageIndex=${this.paging.pageIndex}&status=${type.type}&startDate=${this.startDate}&endDate=${this.endDate}&businessId=${this.selectedItem}&delivery=${this.delivery}`;
                axios.get(`/Chain/GetOrders?${query}`)
                    .then(function (res) {
                        self.orderList = res.data.list;
                        self.paging.pageCount = res.data.pages;
                        self.paging.recordCount = res.data.rows;
                    })
                    .catch(function (err) { $.alert(err); });
            },
            search: function () {
                this.initPaging();
                this.getData();
            },
            changeType: function (item) {
                this.buttons.forEach(obj => { obj.selected = false; });
                item.selected = true;
                this.curType = item;
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

