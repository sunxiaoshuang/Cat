
(function ($) {
    "use strict";
    var comment = [
        { name: "很差", value: 1, check: true },
        { name: "一般", value: 2, check: true },
        { name: "满意", value: 4, check: true },
        { name: "非常满意", value: 8, check: true },
        { name: "无可挑剔", value: 16, check: true }
    ], end = new Date(), start = new Date(end - 7 * 24 * 60 * 60 * 1000);
    if (pageObj.stores) {
        pageObj.stores.unshift({ item1: 0, item2: "全部" });
    }
    var vm = new Vue({
        el: "#app",
        data: {
            isChain: pageObj.isChain || false,
            stores: pageObj.stores,
            paging: {
                pageSize: 10,
                pageIndex: 1,
                pageCount: 0
            },
            businessId: 0,
            delivery: JSON.parse(JSON.stringify(comment)),
            order: JSON.parse(JSON.stringify(comment)),
            startDate: start.format("yyyy-MM-dd"),
            endDate: end.format("yyyy-MM-dd"),
            comments: null,
            stars: JSON.parse(JSON.stringify(comment))
        },
        methods: {
            loadData: function () {
                let deliveryLevel = 0, orderLevel = 0, self = this;
                if (this.delivery.some(a => !a.check)) {
                    this.delivery.forEach(a => {
                        if (!a.check) return;
                        deliveryLevel += a.value;
                    });
                }
                if (this.order.some(a => !a.check)) {
                    this.order.forEach(a => {
                        if (!a.check) return;
                        orderLevel += a.value;
                    });
                }
                let data = {
                    pageIndex: this.paging.pageIndex,
                    pageSize: this.paging.pageSize,
                    deliveryLevel,
                    orderLevel,
                    startDate: this.startDate,
                    endDate: this.endDate,
                    businessId: this.businessId
                };
                axios.get("/comment/getComments?" + $.param(data))
                    .then(function (res) {
                        self.paging.pageCount = Math.ceil(res.data.count / self.paging.pageSize);
                        self.comments = res.data.rows;
                    })
                    .catch(function (err) {
                        $.alert(err);
                    });
            },
            next: function () {
                if (this.paging.pageIndex >= this.paging.pageCount) return;
                this.paging.pageIndex++;
                this.loadData();
            },
            page: function (num) {
                if (this.paging.pageIndex === num) return;
                this.paging.pageIndex = num;
                this.loadData();
            },
            prev: function () {
                if (this.paging.pageIndex <= 1) return;
                this.paging.pageIndex--;
                this.loadData();
            },
            search: function () {
                this.paging.pageIndex = 1;
                this.loadData();
            },
            commentText: function (score) {
                var list = comment.filter(a => a.value === score);
                if (list.length === 0) return "";
                return list[0].name;
            },
            change: function (comment) {
                $.loading();
                console.log(comment);
                var visible = !comment.isShow;
                axios.get(`/comment/changeVisible/${comment.id}?visible=${+visible}`)
                    .then(function (res) {
                        $.loaded();
                        comment.isShow = visible;
                    });
            },
            catOrder: function (comment) {
                axios.get(`/Order/OrderDetailView/${comment.orderId}`)
                    .then(function (res) {
                        $.view({
                            template: res.data,
                            title: "订单信息",
                            dialogWidth: 800
                        });
                    })
                    .catch(function (err) {
                        $.alert(err);
                    });
            }
        },
        created: function () {
            this.loadData();
        }
    });

    var dateOptions = {
        format: 'yyyy-mm-dd',
        autoclose: true,
        maxView: 1,
        minView: 2,
        todayBtn: true,
        todayHighlight: true,
        language: "zh-CN"
    };
    $("#txtStartDate").datetimepicker(dateOptions).on("changeDate", function (e) {
        vm.startDate = this.value;
    });
    $("#txtEndDate").datetimepicker(dateOptions).on("changeDate", function (e) {
        vm.endDate = this.value;
    });

}(jQuery));
