
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
                axios.get(`/report/getSetmealData?start=${this.start}&end=${this.end}&type=${this.type}`)
                    .then(function (res) {
                        self.items = res.data.map(function (obj) {
                            return $.extend(obj, { selected: false });
                        });
                    })
                    .catch(function (err) { $.alert(err); });
            },
            exportData: function () {
                location = `/report/exportSetmealData?start=${this.start}&end=${this.end}&type=${this.type}`;
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