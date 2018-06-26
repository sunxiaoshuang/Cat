; (function ($) {
    var bindView, 
    app = new Vue({
        el: "#app",
        data: {
            showError: false,
            modifyState: false,
            entity: pageObj.business,
            list: pageObj.printers
        },
        methods: {
            modify: function () {
                this.modifyState = true;
            },
            save: function () {
                var self = this;
                if (!this.entity.feyinApiKey || !this.entity.feyinMemberCode) {
                    $.alert("请将开发设置信息输入完整");
                    this.showError = true;
                    return;
                }
                this.showError = false;
                axios.get(`/business/savefeyin?feyinApiKey=${this.entity.feyinApiKey}&feyinMemberCode=${this.entity.feyinMemberCode}`)
                    .then(function (res) {
                        if (res.data.success) {
                            $.alert(res.data.msg, "success");
                            self.modifyState = false;
                        } else {
                            $.alert(res.data.msg);
                        }
                    })
                    .catch(function (msg) {
                        $.alert(msg);
                    });
            },
            cancel: function () {
                this.modifyState = false;
            },
            addBind: function () {
                $.view({
                    title: "打印机信息",
                    footDisplay: true,
                    template: `
                        <div class='row form-horizontal' id='bindView'>
                            <label class='col-md-3 col-xs-12 control-label'>打印机编号：</label>
                            <div class='col-md-8 col-xs-12' style='margin-bottom: 10px;'>
                                <input type='text' class='form-control' v-model.trim='code' />
                            </div>
                            <label class='col-md-3 col-xs-12 control-label'>打印机名称：</label>
                            <div class='col-md-8 col-xs-12'>
                                <input type='text' class='form-control' v-model.trim='name' />
                            </div>
                        </div>`,
                    load: function () {
                        bindView = new Vue({
                            el: "#bindView",
                            data: {
                                code: '',
                                name: ''
                            }
                        });
                        this.on("hidden.bs.modal", function () {
                            if (!bindView) return;
                            bindView.$destroy();
                            bindView = null;
                        });
                    },
                    submit: function (e, $modal) {
                        var code = bindView.code;
                        var name = bindView.name;
                        if (!code || !name) {
                            $.alert("请将表单输入完整");
                            return false;
                        }
                        axios.get(`/business/addbind?code=${code}&name=${name}`)
                            .then(function (res) {
                                if (res.data.success) {
                                    $.alert(res.data.msg, "success");
                                    app.list.push(res.data.data);
                                } else {
                                    $.alert(res.data.msg);
                                }
                            })
                            .catch(function (msg) {
                                $.alert(msg);
                            });
                        return true;
                    }
                });
            },
            unbind: function (printer) {
                var self = this;
                $.primary("解除绑定后，打印机将不可打印小票，确定解除吗？", function () {
                    axios.get("/business/unbind/" + printer.id)
                        .then(function (res) {
                            if (res.data.success) {
                                $.alert(res.data.msg, "success");
                                self.list.remove(printer);
                            } else {
                                $.alert(res.data.msg);
                            }
                        })
                        .catch(function (msg) {
                            $.alert(msg);
                        });
                    return true;
                });
            }
        }
    });

})(jQuery);

