
(function () {
    var statusName = {
        "1": "未使用", "2": "已使用", "3": "已过期", "4": "上架", "5": "下架", "6": "已领完", "7": "未开始"
    };
    var vm = new Vue({
        el: "#app",
        data: {
            list: pageData.list,
            statusList: [
                { name: "--优惠券状态--", value: "0" },
                { name: "上架", value: "4" },
                { name: "下架", value: "5" },
                { name: "未开始", value: "7" },
                { name: "已过期", value: "3" },
                { name: "已领完", value: "6" }
            ],
            status: "0"
        },
        methods: {
            validFilter: function (coupon) {
                if (coupon.validDay > 0) return "领取后" + coupon.validDay + "天";
                return coupon.startDate.substring(0, 10) + " 至 " + coupon.endDate.substring(0, 10);
            },
            remove: function (coupon) {
                var self = this;
                $.primary("确定删除吗？", function () {
                    axios.delete("/Sale/DeleteCoupon/" + coupon.id)
                        .then(function (res) {
                            if (!res.data.success) {
                                $.alert(res.data.msg);
                                return;
                            }
                            $.alert(res.data.msg, "success");
                            self.list.remove(coupon);
                            var item = pageData.list.filter(a => a.id == coupon.id)[0];
                            pageData.list.remove(item);
                        })
                        .catch(function (err) {
                            $.alert(err);
                        });
                    return true;
                });
            },
            down: function (coupon) {
                var self = this;
                $.primary("下架后将无法复原，确定吗？", function () {
                    axios.get("/Sale/DownCoupon/" + coupon.id)
                        .then(function (res) {
                            if (!res.data.success) {
                                $.alert(res.data.msg);
                                return;
                            }
                            $.alert(res.data.msg, "success");
                            coupon.isActive = false;
                            coupon.status = "5";
                        })
                        .catch(function (err) {
                            $.alert(err);
                        });
                    return true;
                });
            },
            statusFilter: function (status) {
                return statusName[status];
            }
        },
        watch: {
            status: function () {
                var self = this;
                if (this.status == 0) {
                    this.list = pageData.list;
                    return;
                }
                this.list = pageData.list.filter(a => a.status == self.status);
            }
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
        vm.entity.startDate = this.value;
    });
    $("#txtEndDate").datetimepicker(dateOptions).on("changeDate", function (e) {
        vm.entity.endDate = this.value;
    });

})();
