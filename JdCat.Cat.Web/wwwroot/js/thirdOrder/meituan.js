(function () {

    new Vue({
        el: "#app",
        data: pageObj,
        methods: {
            save: function() {
                if (!this.appId || !this.key || !this.poi_id) {
                    $.alert("请将信息输入完整！");
                    return;
                }
                if (this.isDelivery && !this.deliveryMode) {
                    $.alert("请选择配送方式！");
                    return;
                }
                $.loading();
                axios.post(`/thirdorder/mt/save`, this.$data)
                    .then(function (res) {
                        $.loaded();
                        $.alert("保存成功", "success");
                    })
                    .catch(function (err) {
                        $.loaded();
                        $.alert(err);
                    });
            }
        }
    });

})()