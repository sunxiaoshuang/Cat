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
                if (!entity.appId || !entity.secret || !entity.mchId || !entity.mchKey) {
                    $.alert("请将信息填写完整");
                    this.showError = true;
                    return;
                }
                this.showError = false;
                $.loading();
                axios.post("/business/savesmall", { id: this.entity.id, appId: this.entity.appId, secret: this.entity.secret, mchId: this.entity.mchId, mchKey: this.entity.mchKey, templateNotifyId: this.entity.templateNotifyId })
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

