var aa;
(function () {

    aa = new Vue({
        el: "#app",
        data: {
            entity: {},
            password: null
        },
        methods: {
            registe: function () {
                var self = this;
                if (!this.entity.code) {
                    $.alert("请输入登录帐号");
                    return;
                } else if (!this.entity.password) {
                    $.alert("请输入密码");
                    return;
                } else if (!this.entity.name) {
                    $.alert("请输入总店名称");
                    return;
                } else if (!this.entity.contact) {
                    $.alert("请输入联系人");
                    return;
                } else if (!this.entity.mobile) {
                    $.alert("请输入联系电话");
                    return;
                } else if (this.entity.password !== this.password) {
                    $.alert("两次密码输入不一致");
                    return;
                }

                axios.post("/Registe/Registe2", this.entity)
                    .then(function (res) {
                        if (!res.data.success) {
                            $.alert(res.data.msg);
                            return;
                        }
                        $.alert(res.data.msg, "success");
                        self.entity = {};
                        self.password = null;
                    })
                    .catch(function (err) {
                        $.alert(err);
                    });
            }
        }
    });

})();