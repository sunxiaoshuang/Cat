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
            printerType: function (printer) {
                if (printer.type == 0) {
                    return "佳博";
                } else if (printer.type == 1) {
                    return "易联云";
                } else if (printer.type == 2) {
                    return "飞鹅";
                } else {
                    return "外卖管家";
                }
            },
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
                            <label class='col-md-3 col-xs-12 control-label'>打印机名称：</label>
                            <div class='col-md-8 col-xs-12' style='margin-bottom: 10px;'>
                                <input type='text' class='form-control' v-model.trim='name' />
                            </div>
                            <label class='col-md-3 col-xs-12 control-label'>打印机编号：</label>
                            <div class='col-md-8 col-xs-12' style='margin-bottom: 10px;'>
                                <input type='text' class='form-control' v-model.trim='code' />
                            </div>
                            <label class='col-md-3 col-xs-12 control-label'>打印机密钥：</label>
                            <div class='col-md-8 col-xs-12' style='margin-bottom: 10px;'>
                                <input type='text' class='form-control' v-model.trim='apiKey' />
                            </div>
                            <label class='col-md-3 col-xs-12 control-label'>打印机类别：</label>
                            <div class='col-md-8 col-xs-12' style='margin-bottom: 10px;'>
                                <select class='form-control' v-model='type'>
                                    <option value='0'>佳博</option>
                                    <option value='1'>易联云</option>
                                    <option value='2'>飞鹅</option>
                                    <option value='3'>外卖管家</option>
                                </select>
                            </div>
                            <label class='col-md-3 col-xs-12 control-label'>流量卡号（仅限飞鹅）：</label>
                            <div class='col-md-8 col-xs-12'>
                                <input type='text' class='form-control' v-model.trim='remark' />
                            </div>
                        </div>`,
                    load: function () {
                        bindView = new Vue({
                            el: "#bindView",
                            data: {
                                code: '',
                                name: '',
                                type: "1",
                                apiKey: ""
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
                        var type = bindView.type;
                        var apiKey = bindView.apiKey;
                        var remark = bindView.remark;
                        if (!name) {
                            $.alert("请输入打印机名称");
                            return false;
                        }
                        if (!code) {
                            $.alert("请输入打印机编号");
                            return false;
                        }
                        if (type == 1 && !apiKey) {
                            $.alert("易联云打印机需要填写打印机密钥");
                            return false;
                        }
                        if (type == 2 && !remark) {
                            $.alert("飞鹅打印机请在备注栏输入流量卡号码");
                            return false;
                        }
                        
                        axios.get(`/business/addbind?code=${code}&name=${name}&type=${type}&apiKey=${apiKey}&remark=${remark}`)
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
            },
            printerDefault: function (printer) {
                if (printer.isDefault) return;
                var self = this;
                axios.get("/business/setDefaultPrinter/" + printer.id)
                    .then(function (res) {
                        if (res.data.success) {
                            $.alert(res.data.msg, "success");
                            self.list.forEach(function (obj) {
                                obj.isDefault = false;
                            });
                            printer.isDefault = true;
                        } else {
                            $.alert(res.data.msg);
                        }
                    })
                    .catch(function (msg) {
                        $.alert(msg);
                    });
            }
        }
    });

})(jQuery);

