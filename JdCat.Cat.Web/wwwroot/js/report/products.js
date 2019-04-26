
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
            loadData: function () {
                var self = this;
                axios.get(`/report/getProductsData?start=${this.start}&end=${this.end}&type=${this.type}`)
                    .then(function (res) {
                        self.items = res.data;
                    })
                    .catch(function (err) { $.alert(err); });
            },
            exportData: function () {
                location = `/report/exportProductsData?start=${this.start}&end=${this.end}&type=${this.type}`;
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