
(function () {
    var now = jdCat.utilMethods.now();

    var app = new Vue({
        el: "#app",
        data: {
            items: [],
            products: null,
            type: 99,
            start: now,
            end: now
        },
        methods: {
            search: function () {
                this.loadData();
            },
            loadData: function () {
                var self = this;
                axios.get(`/Report/Third/GetProducts?start=${this.start}&end=${this.end}&source=${this.type}`)
                    .then(function (res) {
                        var total = { name: '合计', quantity: 0, amount: 0, cancelQuantity: 0, cancelSaleAmount: 0, cancelAmount: 0, saleQuantity: 0, saleAmount: 0, entertainQuantity: 0, entertainAmount: 0, discountAmount: 0, discountQuantity: 0, discountedAmount: 0, actualQuantity: 0, actualAmount: 0};
                        res.data.forEach(item => {
                            for (let key in total) {
                                if (key === "name") continue;
                                total[key] += item[key];
                            }
                            for (let key in total) {
                                if (key === "name") continue;
                                total[key] = +total[key].toFixed(2);
                            }
                        });
                        res.data.push(total);
                        self.items = res.data;
                    })
                    .catch(function (err) { $.alert(err); });
            },
            exportData: function () {
                location = `/Report/Third/ExportProducts?start=${this.start}&end=${this.end}&source=${this.type}`;
            },
            formatData: function (num) {
                if (num === 0) return "-";
                return +num.toFixed(2);
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