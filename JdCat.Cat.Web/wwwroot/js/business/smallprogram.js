; (function ($) {

    new Vue({
        el: "#app",
        data: {
            entity: pageObj.business,
            showError: false
        },
        methods: {
            ret: function () {
                history.go(-1);
            },
            save: function () {
                var entity = this.entity;
                if (!entity.appId || !entity.secret) {
                    $.alert("请将信息填写完整");
                    this.showError = true;
                    return;
                }
                this.showError = false;
                $.loading();
                axios.post("/business/savesmall", { appId: this.entity.appId, secret: this.entity.secret })
                    .then(function (res) {
                        $.loaded();
                        if (!res.data.success) {
                            $.alert(res.data.msg);
                            return;
                        }
                        $.alert("保存成功", "success");
                    })
                    .catch(function (err) {
                        $.loaded();
                        $.alert(err);
                    });
            },
        }
    });

})(jQuery);

