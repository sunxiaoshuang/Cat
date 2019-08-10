(function () {

    new Vue({
        el: "#app",
        data: pageObj,
        methods: {
            save: function () {
                if (!this.appId || !this.key || !this.secret || !this.poi_id) {
                    $.alert("请将信息输入完整！");
                    return;
                }
                $.loading();
                axios.post(`/thirdorder/eleme/save`, this.$data)
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