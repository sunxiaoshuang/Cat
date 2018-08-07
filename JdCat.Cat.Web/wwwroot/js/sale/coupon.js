
(function () {


    if (pageData.entity && !!pageData.entity.startDate) {
        pageData.entity.startDate = pageData.entity.startDate.substring(0, 10);
        pageData.entity.endDate = pageData.entity.endDate.substring(0, 10);
    }

    var vm = new Vue({
        el: "#app",
        data: {
            timeType: !pageData.entity ? 0 : (pageData.entity.validDay > 0 ? 0 : 1),
            quantityType: !pageData.entity ? 0 : (pageData.entity.quantity < 0 ? 0 : 1),
            minConsumeType: !pageData.entity ? 0 : (pageData.entity.minConsume < 0 ? 0 : 1),
            showError: false,
            entity: pageData.entity || {
                name: "",
                value: 1,
                quantity: -1,
                minConsume: -1,
                startDate: null,
                endDate: null,
                validDay: 30
            },
            btnDisplay: false,
            isCheck: !!pageData.entity
        },
        methods: {
            create: function () {
                var entity = this.entity, self = this;
                if (!entity.name) {
                    $.alert("请输入优惠券名称");
                    self.showError = true;
                    return;
                }
                if (this.timeType == 0 && entity.validDay <= 0) {
                    $.alert("请正确输入有效天数");
                    self.showError = true;
                    return;
                }
                if (this.timeType == 1) {
                    if (!entity.startDate || !entity.endDate) {
                        $.alert("请选择优惠券有效期时间");
                        self.showError = true;
                        return;
                    }
                    if (entity.startDate > entity.endDate) {
                        $.alert("请正确选择优惠券有效期时间");
                        self.showError = true;
                        return;
                    }
                }
                if (entity.value <= 0) {
                    $.alert("券面金额必须大于零");
                    self.showError = true;
                    return;
                }
                if (this.quantityType == 1 && entity.quantity <= 0) {
                    $.alert("发放数量必须大于零");
                    self.showError = true;
                    return;
                }
                if (this.minConsumeType == 1 && entity.minConsume <= 0) {
                    $.alert("最低消费必须大于零");
                    self.showError = true;
                    return;
                }

                self.showError = false;
                self.btnDisplay = true;
                $.loading();
                axios.post("/Sale/CreateCoupon", entity)
                    .then(function (res) {
                        $.loaded();
                        if (!res.data.success) {
                            $.alert(res.data.msg);
                            self.btnDisplay = false;
                            return;
                        }
                        $.alert(res.data.msg, "success");
                        setTimeout(function () {
                            window.location = "/Sale/CouponList";
                        }, 2000);
                    })
                    .catch(function (err) {
                        $.loaded();
                        $.alert(err);
                        self.btnDisplay = false;
                    });
            },
            ret: function () {
                history.go(-1);
            }
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
