
(function () {
    var now = jdCat.utilMethods.now();

    var app = new Vue({
        el: "#app",
        data: {
            items: [],
            orders: [],
            start: now,
            end: now
        },
        methods: {
            search: function () {
                this.loadData();
            },
            cat: function (item, index) {
                if (item.selected) return;
                if (index === this.items.length - 1) return;
                var self = this;
                this.items.forEach(function (obj) { obj.selected = false; });
                item.selected = true;
                $.loading();
                axios.get(`/report/getSingleBenetifData?name=${item.name}&start=${this.start}&end=${this.end}`)
                    .then(function (res) {
                        $.loaded();
                        self.orders = res.data;
                    })
                    .catch(function (err) { $.alert(err); });
            },
            loadData: function () {
                var self = this;
                var days = jdCat.utilMethods.diffDay(this.start, this.end);
                if (days > 31) {
                    $.alert('查询时间间隔请不要超过31天');
                    return;
                }
                axios.get(`/report/getBenefitData?start=${this.start}&end=${this.end}`)
                    .then(function (res) {
                        var total = { name: '合计', quantity: 0, amount: 0, orderAmount: 0};
                        self.items = res.data.map(function (obj) {
                            obj.selected = false;
                            total.amount += obj.amount;
                            total.orderAmount += obj.orderAmount;
                            total.quantity += obj.quantity;
                            return obj;
                        });
                        self.items.push(total);
                    })
                    .catch(function (err) { $.alert(err); });
            },
            exportData: function () {
                location = `/report/exportBenefitData?start=${this.start}&end=${this.end}`;
            },
            info: function (item) {
                $.view({
                    title: "订单详情",
                    url: `/Store/OrderDetail/${item.id}`,
                    dialogWidth: 700
                });
            }
        },
        created: function () {
            this.loadData();
        }
    });

    $("#txtStartDate").datetimepicker(jdCat.utilData.dateOptions).on("changeDate", function (e) {
        app.start = this.value;
    });
    $("#txtEndDate").datetimepicker(jdCat.utilData.dateOptions).on("changeDate", function (e) {
        app.end = this.value;
    });

})();