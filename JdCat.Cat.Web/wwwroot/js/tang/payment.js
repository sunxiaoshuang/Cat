
(function () {
    var template = `
        <div class="row form-horizontal">
            <div class="form-group">
                <label class="control-label col-xs-3">
                    <span class="require">名称：</span>
                </label>
                <div class="col-xs-8">
                    <input class="form-control" id="name" />
                </div>
            </div>
        </div>
    `;
    var editTemplate = `
        <div class="row form-horizontal">
            <div class="form-group">
                <label class="control-label col-xs-3">
                    <span class="require">排序码：</span>
                </label>
                <div class="col-xs-8">
                    <input class="form-control" type="number" id="sort" />
                </div>
            </div>
        </div>
    `;
    var app = new Vue({
        el: "#app",
        data: {
            items: null
        },
        methods: {
            add: function () {
                $.view({
                    title: "新增支付方式",
                    template,
                    footDisplay: "block",
                    submit: function () {
                        var name = $.trim($("#name").val());
                        if (!name) {
                            $.alert("请输入支付名称");
                            return;
                        }
                        axios.post("/tang/addPayment", { name })
                            .then(function (res) {
                                if (!res.data.success) {
                                    $.alert(res.data.msg);
                                    return;
                                }
                                $.alert(res.data.msg, "success");
                                app.items.push(res.data.data);
                            })
                            .catch(function (err) { $.alert(err); });
                        return true;
                    }
                });
            },
            modify: function (payment) {
                var self = this;
                $.view({
                    title: "修改支付方式排序码",
                    template: editTemplate,
                    footDisplay: "block",
                    submit: function () {
                        var sort = $.trim($("#sort").val());
                        if (!sort) {
                            $.alert("请输入排序码");
                            return;
                        }
                        axios.put(`/tang/setPaymentSort/${payment.id}?sort=${sort}`)
                            .then(function (res) {
                                if (!res.data.success) {
                                    $.alert(res.data.msg);
                                    return;
                                }
                                $.alert(res.data.msg, "success");
                                payment.sort = sort;
                                self.items.sort(function (a, b) { return a.sort - b.sort; });
                            })
                            .catch(function (err) { $.alert(err); });
                        return true;
                    }
                });
            },
            remove: function (payment) {
                var self = this;
                $.primary(`确定删除支付方式[${payment.name}]吗？`, function () {
                    axios.delete("/tang/removePayment/" + payment.id)
                        .then(function (res) {
                            if (!res.data.success) {
                                $.alert(res.data.msg);
                                return;
                            }
                            $.alert(res.data.msg, "success");
                            self.items.remove(payment);
                        })
                        .catch(function (err) { $.alert(err); });
                    return true;
                });
            }
        },
        created: function () {
            var self = this;
            axios.get("/tang/getPayments")
                .then(function (res) {
                    self.items = res.data;
                });
        }
    });

})();