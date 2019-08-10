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
                if (!entity.appId || !entity.secret || !entity.mchId || !entity.mchKey || !entity.weChatAppId || !entity.weChatSecret || !entity.payServerKey || !entity.payServerMchId || !entity.certFile || !entity.payServerAppId) {
                    $.alert("请将信息填写完整");
                    this.showError = true;
                    return;
                }
                this.showError = false;
                $.loading();
                axios.post("/business/savesmall", { id: entity.id, appId: entity.appId, secret: entity.secret, mchId: entity.mchId, mchKey: entity.mchKey, templateNotifyId: entity.templateNotifyId, appQrCode: entity.appQrCode, weChatAppId: entity.weChatAppId, weChatSecret: entity.weChatSecret, payServerKey: entity.payServerKey, payServerMchId: entity.payServerMchId, certFile: entity.certFile, payServerAppId: entity.payServerAppId })
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
            }
        }
    });

})(jQuery);

