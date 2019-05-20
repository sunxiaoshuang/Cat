;
(function () {

    Vue.prototype.currency = function (input) {
        return "￥ " + input;
    };

    var vm = new Vue({
        el: "#app",
        data: {
            price: pageData.list.length > 0 ? +pageData.list[pageData.list.length - 1].price.toFixed(2) : 0,
            todayQuantity: pageData.list.length > 0 ? pageData.list[pageData.list.length - 1].quantity : 0,
            list: [],
            start: undefined,
            end: undefined,
            productOriginalAmount: 0,
            productAmount: 0,
            freightAmount: 0,
            packageAmount: 0,
            discountAmount: 0,
            activityAmount: 0,
            benefitAmount: 0,
            quantity: 0,
            total: 0,
            actualTotal: 0
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
                axios.get(`/Report/GetSaleStatistics?start=${this.start}&end=${this.end}`)
                    .then(function (res) {
                        self.list = res.data;
                        self.total = 0;
                        self.quantity = 0;
                        self.list.forEach(item => {
                            self.quantity += item.quantity;
                            self.productOriginalAmount += item.productOriginalAmount;
                            self.productAmount += item.productAmount;
                            self.freightAmount += item.freightAmount;
                            self.packageAmount += item.packageAmount;
                            self.discountAmount += item.discountAmount;
                            self.activityAmount += item.activityAmount;
                            self.benefitAmount += item.benefitAmount;
                            self.total += item.total;
                            self.actualTotal += item.actualTotal;

                            item.productOriginalAmount = +item.productOriginalAmount.toFixed(2);
                            item.productAmount = +item.productAmount.toFixed(2);
                            item.freightAmount = +item.freightAmount.toFixed(2);
                            item.discountAmount = +item.discountAmount.toFixed(2);
                            item.activityAmount = +item.activityAmount.toFixed(2);
                            item.benefitAmount = +item.benefitAmount.toFixed(2);
                            item.total = +item.total.toFixed(2);
                            item.actualTotal = +item.actualTotal.toFixed(2);
                        });
                        self.quantity = +self.quantity.toFixed(2);
                        self.productOriginalAmount = +self.productOriginalAmount.toFixed(2);
                        self.productAmount = +self.productAmount.toFixed(2);
                        self.freightAmount = +self.freightAmount.toFixed(2);
                        self.packageAmount = +self.packageAmount.toFixed(2);
                        self.discountAmount = +self.discountAmount.toFixed(2);
                        self.activityAmount = +self.activityAmount.toFixed(2);
                        self.benefitAmount = +self.benefitAmount.toFixed(2);
                        self.total = +self.total.toFixed(2);
                        self.actualTotal = +self.actualTotal.toFixed(2);
                    })
                    .catch(function (msg) {
                        $.alert(msg);
                    });
            },
            clearTotal: function () {
                this.quantity = this.productOriginalAmount = this.productAmount = this.freightAmount = this.packageAmount = this.discountAmount = this.activityAmount = this.benefitAmount = this.total = this.actualTotal = 0;
            },
            report: function () {
                window.open(`/Report/ExportSaleStatistics?start=${this.start}&end=${this.end}`);
            }
        },
        created: function () {
            this.start = $("#txtStartDate").val();
            this.end = $("#txtEndDate").val();
            this.search();
        }
    });

    (function () {

        var time = pageData.list.map(a => a.createTime.substring(5));
        var priceList = pageData.list.map(a => +a.price.toFixed(2));
        var option = {
            color: ['#3398DB'],
            tooltip: {
                trigger: 'axis',
                axisPointer: {
                    type: 'shadow'
                }
            },
            grid: {
                left: '3%',
                right: '4%',
                bottom: '3%',
                containLabel: true
            },
            xAxis: [
                {
                    type: 'category',
                    data: time,
                    axisTick: {
                        alignWithLabel: true
                    }
                }
            ],
            yAxis: [
                {
                    type: 'value'
                }
            ],
            series: [
                {
                    name: '营业额',
                    type: 'bar',
                    barWidth: '60%',
                    data: priceList
                }
            ]
        };

        var myChart = echarts.init(document.getElementById('chart'));
        myChart.setOption(option);

    })();


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


