
(function () {

    var vm = new Vue({
        el: "#app",
        data: {
            entity: pageData.entity || {
                name: "",
                startDate: "",
                endDate: "",
                isForeverValid: false,
                minPrice: 0,
                reduceMoney: 0
            },
            showError: false,
            btnDisplay: false
        },
        methods: {
            create: function () {
                var entity = this.entity, self = this;
                if (!entity.name || !entity.minPrice || !entity.reduceMoney) {
                    $.alert("请将信息输入完整");
                    self.showError = true;
                    return
                }
                if (!entity.isForeverValid) {
                    if (!entity.startDate || !entity.endDate) {
                        $.alert("请输入活动日期或选择永久有效");
                        self.showError = true;
                        return;
                    }
                }
                if (entity.endDate < entity.startDate) {
                    $.alert("截止日期不能小于起始日期");
                    self.showError = true;
                    return;
                }
                self.showError = false;
                self.btnDisplay = true;
                var url = !entity.id ? "/Sale/CreateFullReduce" : "/Sale/UpdateFullReduce";
                $.loading();
                axios.post(url, entity)
                    .then(function (res) {
                        $.loaded();
                        if (!res.data.success) {
                            $.alert(res.data.msg);
                            self.btnDisplay = false;
                            return;
                        }
                        $.alert(res.data.msg, "success");
                        setTimeout(function () {
                            window.location = "/Sale/FullReduceList";
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
        filters: {
            date: function (time) {
                if (!time) return "";
                return time.substring(0, 10);
            }
        },
        created: function () {
            console.log(this);
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
