
(function () {

    Vue.prototype.currency = function (input) {
        return "￥ " + +input.toFixed(2);
    };

    var vm = new Vue({
        el: "#app",
        data: {
            list: [],
            start: undefined,
            end: undefined,
            quantity: 0,
            goodAmount: 0,
            actualGoodAmount: 0,
            mealFee: 0,
            goodDiscountAmount: 0,
            orderDiscountAmount: 0,
            preferentialAmount: 0,
            amount: 0,
            actualAmount: 0
        },
        methods: {
            search: function () {
                var self = this, start = +new Date(this.start), end = +new Date(this.end), timespan = 1000 * 60 * 60 * 24 * 31;
                if (start > end) {
                    $.alert("开始时间必须大于结束时间");
                    return;
                }
                if (start + timespan < end) {
                    $.alert("起止时间跨度最大为31天");
                    return;
                }
                this.clearTotal();
                axios.get(`/Report/GetTangData?start=${this.start}&end=${this.end}`)
                    .then(function (res) {
                        self.list = res.data;
                        self.list.forEach(item => {
                            // 合计
                            self.quantity += item.quantity;
                            self.goodAmount += item.goodAmount;
                            self.actualGoodAmount += item.actualGoodAmount;
                            self.mealFee += item.mealFee;
                            self.goodDiscountAmount += item.goodDiscountAmount;
                            self.orderDiscountAmount += item.orderDiscountAmount;
                            self.preferentialAmount += item.preferentialAmount;
                            self.amount += item.amount;
                            self.actualAmount += item.actualAmount;
                        });
                    })
                    .catch(function (msg) {
                        $.alert(msg);
                    });
            },
            clearTotal: function () {
                this.quantity = this.goodAmount = this.actualGoodAmount = this.mealFee = this.goodDiscountAmount = this.preferentialAmount = this.activityAmount = this.amount = this.actualAmount = 0;
            },
            report: function () {
                window.open(`/Report/ExportTangData?start=${this.start}&end=${this.end}`);
            }
        },
        created: function () {
            this.start = $("#txtStartDate").val();
            this.end = $("#txtEndDate").val();
            this.search();
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
        vm.start = this.value;
    });
    $("#txtEndDate").datetimepicker(dateOptions).on("changeDate", function (e) {
        vm.end = this.value;
    });

})();