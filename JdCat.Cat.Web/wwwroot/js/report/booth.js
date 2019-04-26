﻿
(function () {
    var now = jdCat.utilMethods.now();

    var app = new Vue({
        el: "#app",
        data: {
            items: [],
            products: null,
            type: 0,
            start: now,
            end: now
        },
        methods: {
            search: function () {
                this.loadData();
            },
            cat: function (item) {
                if (item.selected) return;
                var self = this;
                this.items.forEach(function (obj) { obj.selected = false; });
                item.selected = true;
                $.loading();
                axios.get(`/report/getSingleBoothData/${item.id}?start=${this.start}&end=${this.end}&type=${this.type}`)
                    .then(function (res) {
                        $.loaded();
                        self.products = res.data;
                        drawChart();
                    })
                    .catch(function (err) { $.alert(err); });
            },
            loadData: function () {
                var self = this;
                axios.get(`/report/getBoothData?start=${this.start}&end=${this.end}&type=${this.type}`)
                    .then(function (res) {
                        self.items = res.data.map(function (obj) {
                            obj.amount = parseFloat(obj.amount.toFixed(2));
                            return $.extend(obj, { selected: false });
                        });
                    })
                    .catch(function (err) { $.alert(err); });
            },
            exportData: function () {
                location = `/report/exportBoothData?start=${this.start}&end=${this.end}&type=${this.type}`;
            }
        },
        created: function () {
            this.loadData();
        }
    });

    // 销售额饼图
    var drawChart = (function () {
        var option = {
            title: {
                text: '产品销售额',
                subtext: '档口统计图',
                x: 'center'
            },
            tooltip: {
                trigger: 'item',
                formatter: "{a} <br/>{b} : {c} ({d}%)"
            },
            legend: {
                orient: 'vertical',
                x: 'left'
            },
            toolbox: {
                show: true,
                feature: {
                    mark: { show: true },
                    dataView: { show: true, readOnly: false },
                    magicType: {
                        show: true,
                        type: ['pie', 'funnel'],
                        option: {
                            funnel: {
                                x: '25%',
                                width: '50%',
                                funnelAlign: 'left',
                                max: 1548
                            }
                        }
                    },
                    restore: { show: true },
                    saveAsImage: { show: true }
                }
            },
            calculable: true,
            series: [
                {
                    name: '销售统计',
                    type: 'pie',
                    radius: '55%',
                    center: ['50%', '60%']
                }
            ]
        };
        var myChart = echarts.init(document.getElementById('chart'));
        return function () {
            //if (!app.products || app.products.length === 0) return;
            var products = JSON.parse(JSON.stringify(app.products));
            var items = products.slice(0, 10);
            var top10 = items.map(function (obj) {
                return {
                    name: obj.name, value: obj.amount
                };
            });
            var others = products.slice(10);
            if (others.length > 0) {
                var otherTotal = 0;
                others.forEach(function (obj) { otherTotal += obj.amount; });
                top10.push({
                    name: "其他", value: otherTotal
                });
            }
            option.legend.data = top10.map(function (obj) { return obj.name; });
            option.series[0].data = top10;
            myChart.setOption(option);
        };
    })();

    $("#txtStartDate").datetimepicker(jdCat.utilData.dateOptions).on("changeDate", function (e) {
        app.start = this.value;
    });
    $("#txtEndDate").datetimepicker(jdCat.utilData.dateOptions).on("changeDate", function (e) {
        app.end = this.value;
    });

})();