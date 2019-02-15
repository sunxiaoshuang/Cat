
(function () {
    pageObj.stores.unshift({ item1: 0, item2: "全部" });

    var vm = new Vue({
        el: "#app",
        data: {
            report: [],
            stores: pageObj.stores.map(function (a) { return { id: a.item1, name: a.item2 }; }),
            selectedItem: 0,
            startDate: pageObj.startDate,
            endDate: pageObj.endDate
        },
        methods: {
            getData: function () {
                var self = this;
                if (!this.startDate || !this.endDate) {
                    $.alert("请选择统计时间");
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
                var query = `startDate=${this.startDate}&endDate=${this.endDate}&businessId=${this.selectedItem}`;
                axios.get(`/Chain/BusinessSummary?${query}`)
                    .then(function (res) {
                        res.data.forEach(function (obj) {
                            obj.amount = +obj.amount.toFixed(2);
                        });
                        self.report = res.data;
                    })
                    .catch(function (err) { $.alert(err); });
            }
        },
        created: function () {
            this.getData();
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


})();

