; (function () {

    new Vue({
        el: "#app",
        data: {
            ycfkKey: pageObj.business.ycfkKey,
            ycfkSecret: pageObj.business.ycfkSecret
        },
        methods: {
            ycfk_save: function () {
                axios.get(`/business/ycfkSave?key=${this.ycfkKey}&secret=${this.ycfkSecret}`)
                    .then(function (res) {
                        if (!res.data.success) {
                            $.alert(res.data.msg);
                            return;
                        }
                        $.alert("保存成功", "success");
                    });
            }
        }
    });

})();

