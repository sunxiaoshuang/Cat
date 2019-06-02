(function () {
    var template = `
        <div class="row form-horizontal" id="add-mark">
            <div class="form-group">
                <label class="control-label col-xs-3">
                    <span class="require">名称：</span>
                </label>
                <div class="col-xs-8">
                    <input class="form-control" autocomplete="off" id="markName" />
                </div>
            </div>
        </div>
    `;
    new Vue({
        el: "#app",
        data: {
            flavor: null,
            orderMark: null,
            refundFoodReason: null,
            payRemark: null,
            goodRemark: null
        },
        methods: {
            add: function (category, list) {
                $.view({
                    title: "新增标签",
                    footDisplay: "block",
                    template,
                    submit: function () {
                        var name = $("#markName").val();
                        if (!$.trim(name)) {
                            $.alert("请输入名称");
                            return;
                        }
                        var body = { name, category };
                        axios.post("/tang/addMark", body)
                            .then(function (res) {
                                if (!res.data.success) {
                                    $.alert(res.data.msg);
                                    return;
                                }
                                $.alert(res.data.msg, "success");
                                list.push(res.data.data);
                            })
                            .catch(function (err) { $.alert(err); });
                        return true;
                    }
                });
            },
            remove: function (item, list) {
                $.primary(`确定删除标签[${item.name}]吗？`, function () {
                    axios.delete("/tang/removeMark/" + item.id)
                        .then(function (res) {
                            if (!res.data.success) {
                                $.alert(res.data.msg);
                                return;
                            }
                            $.alert(res.data.msg, "success");
                            list.remove(item);
                        })
                        .catch(function (err) { $.alert(err); });
                    return true;
                });
            }
        },
        created: function () {
            var self = this;
            axios.get("/tang/getMarks")
                .then(function (res) {
                    self.flavor = res.data.flavor;
                    self.orderMark = res.data.orderMark;
                    self.refundFoodReason = res.data.refundFoodReason;
                    self.payRemark = res.data.payRemark;
                    self.goodRemark = res.data.goodRemark;
                });
        }
    });

})();

