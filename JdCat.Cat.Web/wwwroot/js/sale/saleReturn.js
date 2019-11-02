
(function () {
    var statusName = {
        "1": "未使用", "2": "已使用", "3": "已过期", "4": "上架", "5": "下架", "6": "已领完", "7": "未开始"
    };

    var template;

    var vm = new Vue({
        el: "#app",
        data: {
            all: [],
            statusList: [
                { name: "--优惠券状态--", value: "0" },
                { name: "上架", value: "4" },
                //{ name: "下架", value: "5" },
                { name: "未开始", value: "7" },
                { name: "已过期", value: "3" },
                { name: "已领完", value: "6" }
            ],
            status: "0"
        },
        created: function () {
            var self = this;
            axios.get("/sale/getReturnList")
                .then(function ({ data }) {
                    self.all = data;
                });
        },
        methods: {
            validFilter: function (coupon) {
                if (coupon.validDay > 0) return "领取后" + coupon.validDay + "天";
                return coupon.startDate.substring(0, 10) + " 至 " + coupon.endDate.substring(0, 10);
            },
            create: function () {
                if (!template) {
                    this.loadTemplate(this.create);
                    return;
                }
                var obj;
                $.view({
                    title: '创建优惠券',
                    template,
                    dialogWidth: 700,
                    footDisplay: "block",
                    load: function () {
                        obj = new Vue({
                            el: "#return",
                            data: {
                                entity: {
                                    costAmount: 0,
                                    startDate: '',
                                    endDate: '',
                                    value: 1,
                                    validDay: 30,
                                    quantity: -1,
                                    minConsume: -1
                                },
                                timeType: 0,
                                isCreate: true,
                                quantityType: 0,
                                minConsumeType: 0
                            },
                            watch: {
                                quantityType: function () {
                                    if (this.quantityType == 0) {
                                        this.entity.quantity = -1;
                                        return;
                                    }
                                    this.entity.quantity = 1000;
                                },
                                minConsumeType: function () {
                                    if (this.minConsumeType == 0) {
                                        this.entity.minConsume = -1;
                                        return;
                                    }
                                    this.entity.minConsume = 1;
                                },
                                timeType: function () {
                                    if (this.timeType == 0) {
                                        this.entity.startDate = null;
                                        this.entity.endDate = null;
                                        this.entity.validDay = 30;
                                    } else {
                                        this.entity.validDay = -1;
                                    }
                                }
                            }
                        });
                        
                        this.find("#txtStartDate").datetimepicker(dateOptions).on("changeDate", function (e) {
                            obj.entity.startDate = this.value;
                        });
                        this.find("#txtEndDate").datetimepicker(dateOptions).on("changeDate", function (e) {
                            obj.entity.endDate = this.value;
                        });
                        // 隐藏时销毁
                        this.on("hidden.bs.modal", function () {
                            if (!obj) return;
                            obj.$destroy();
                            obj = null;
                        });
                    },
                    submit: function () {
                        var entity = obj.entity;
                        if (obj.timeType == 0 && entity.validDay <= 0) {
                            $.alert("请正确输入有效天数");
                            return;
                        }
                        if (obj.timeType == 1) {
                            if (!entity.startDate || !entity.endDate) {
                                $.alert("请选择优惠券有效期时间");
                                return;
                            }
                            if (entity.startDate > entity.endDate) {
                                $.alert("请正确选择优惠券有效期时间");
                                return;
                            }
                        }
                        if (entity.value <= 0) {
                            $.alert("券面金额必须大于零");
                            return;
                        }
                        if (obj.quantityType == 1 && entity.quantity <= 0) {
                            $.alert("发放数量必须大于零");
                            return;
                        }
                        if (obj.minConsumeType == 1 && entity.minConsume <= 0) {
                            $.alert("最低消费必须大于零");
                            return;
                        }

                        $.loading();
                        axios.post("/sale/saveReturnCoupon", entity)
                            .then(function ({ data }) {
                                $.alert("创建成功", "success");
                                vm.all.push(data.data);
                                $.loaded();
                            })
                            .catch(function (err) {
                                $.alert(err.message);
                                $.loaded();
                            });
                        return true;
                    }
                });
            },
            loadTemplate: function (callback) {
                var self = this;
                axios.get("/sale/returnCoupon")
                    .then(function ({ data }) {
                        template = data;
                        if (callback) {
                            callback.call(self);
                        }
                    });
            },
            remove: function (coupon) {
                var self = this;
                $.primary("确定删除吗？", function () {
                    axios.delete("/sale/deleteReturnCoupon/" + coupon.id)
                        .then(function (res) {
                            $.alert("删除成功", "success");
                            self.all.remove(coupon);
                        })
                        .catch(function (err) {
                            $.alert(err);
                        });
                    return true;
                });
            },
            cat: function (coupon) {
                console.log(coupon)
            },
            statusFilter: function (status) {
                return statusName[status];
            }
        },
        computed: {
            list: function () {
                if (this.status == 0) return this.all;
                return this.all.filter(a => a.status == this.status);
            }
        }
        //watch: {
        //    status: function () {
        //        var self = this;
        //        if (this.status === 0) {
        //            this.list = this.list;
        //            return;
        //        }
        //        this.list = this.list.filter(a => a.status === self.status);
        //    }
        //}
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
