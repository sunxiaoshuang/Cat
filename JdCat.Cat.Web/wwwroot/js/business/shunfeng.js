(function () {

    new Vue({
        el: "#app",
        data: pageObj,
        methods: {
            save: function () {
                if (!this.devId || !this.key || !this.shopId) {
                    $.alert("请将信息输入完整！");
                    return;
                }
                $.loading();
                axios.post(`/business/saveShunfeng`, this.$data)
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