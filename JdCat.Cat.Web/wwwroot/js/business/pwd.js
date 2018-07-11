; (function ($) {

    new Vue({
        el: "#app",
        data: {
            oldPwd: "",
            newPwd1: "",
            newPwd2: "",
        },
        methods: {
            ret: function () {
                history.go(-1);
            },
            save: function () {
                var self = this;
                if (!this.oldPwd || !this.newPwd1 || !this.newPwd2) {
                    $.alert("请将表单输入完整");
                    return;
                }
                if (this.oldPwd == this.newPwd1) {
                    $.alert("新密码不能与原始密码相同");
                    return;
                }
                $.loading();
                axios.post(`/business/modifypassword?oldPwd=${this.oldPwd}&newPwd=${this.newPwd1}`)
                    .then(function (res) {
                        $.loaded();
                        $.alert(res.data.msg, !res.data.success ? "" : "success");
                        if (res.data.success) {
                            self.oldPwd = "";
                            self.newPwd1 = "";
                            self.newPwd2 = "";
                        }
                    })
                    .catch(function (err) {
                        $.loaded();
                        $.alert(err);
                    });
            }
        }
    });

})(jQuery);

