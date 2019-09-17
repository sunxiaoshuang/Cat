
(function () {

    var app = new Vue({
        el: "#app",
        data: pageObj,
        methods: {
            save: function () {
                var self = this;
                if (!this.amount || !this.startTime || !this.endTime) {
                    $.alert("请将信息输入完整！");
                    return;
                }
                $.loading();
                axios.post(`/Sale/SaveNewCustom`, this.$data)
                    .then(function (res) {
                        $.loaded();
                        $.alert("保存成功", "success");
                        setTimeout(function () { location.reload(); }, 1000);
                    })
                    .catch(function (err) {
                        $.loaded();
                        $.alert(err);
                    });
            }
        }
    });

    $("#txtStartDate").datetimepicker(jdCat.utilData.dateOptions).on("changeDate", function (e) {
        app.startTime = this.value;
    });
    $("#txtEndDate").datetimepicker(jdCat.utilData.dateOptions).on("changeDate", function (e) {
        app.endTime = this.value;
    });

})()