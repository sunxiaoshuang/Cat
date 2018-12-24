; (function ($) {
    var template = `
            <div class='form-horizontal' id="{0}">
                <div class="form-group">
                    <label class="control-label col-xs-4">
                        <span class='require'>配送距离km（<=）：</span>
                    </label>
                    <div class="col-xs-7">
                        <input class="form-control" type="text" v-model="distance" />
                    </div>
                </div>
                <div class="form-group">
                    <label class="control-label col-xs-4">
                        <span class='require'>配送费用：</span>
                    </label>
                    <div class="col-xs-7">
                        <input class="form-control" type="number" v-model="amount" />
                    </div>
                </div>
            </div>
        `, appVue, vmAdd, vmUpdate;
    appVue = new Vue({
        el: "#app",
        data: {
            freights: pageData.freights
        },
        methods: {
            add: function () {
                $.view({
                    title: "新增运费设置",
                    template: template.format("addView"),
                    footDisplay: "block",
                    load: function () {
                        vmAdd = new Vue({
                            el: "#addView",
                            data: {
                                distance: 0,
                                amount: 0
                            }
                        });

                        this.on("hidden.bs.modal", function () {
                            if (!vmAdd) return;
                            vmAdd.$destroy();
                            vmAdd = null;
                        });
                    },
                    submit: function () {
                        if (vmAdd.distance <= 0) {
                            $.alert("请输入配送距离");
                            return false;
                        }
                        if (vmAdd.amount < 0) {
                            $.alert("请输入配送费用");
                            return false;
                        }
                        axios.post("/Business/CreateFreight", { maxDistance: vmAdd.distance, amount: vmAdd.amount })
                            .then(function (res) {
                                if (!res.data.success) {
                                    $.alert(res.data.msg);
                                    return;
                                }
                                $.alert(res.data.msg, "success");
                                appVue.freights.push(res.data.data);
                            })
                            .catch(function (err) { $.alert(err); });
                        return true;
                    }
                });
            },
            remove: function (item) {
                $.primary("确定删除吗？", function () {
                    axios.get("/Business/RemoveFreight/" + item.id)
                        .then(function (res) {
                            if (!res.data.success) {
                                $.alert(res.data.msg);
                                return;
                            }
                            $.alert(res.data.msg, "success");
                            appVue.freights.remove(item);
                        })
                        .catch(function (err) { $.alert(err); });
                    appVue.freights.remove(item);
                });
            },
            update: function (item) {
                $.view({
                    title: "修改运费设置",
                    template: template.format("updateView"),
                    footDisplay: "block",
                    load: function () {
                        vmUpdate = new Vue({
                            el: "#updateView",
                            data: {
                                distance: item.maxDistance,
                                amount: item.amount
                            }
                        });

                        this.on("hidden.bs.modal", function () {
                            if (!vmUpdate) return;
                            vmUpdate.$destroy();
                            vmUpdate = null;
                        });
                    },
                    submit: function () {
                        if (vmUpdate.distance <= 0) {
                            $.alert("请输入配送距离");
                            return false;
                        }
                        if (vmUpdate.amount <= 0) {
                            $.alert("请输入配送费用");
                            return false;
                        }
                        axios.post("/Business/UpdateFreight", { id: item.id, maxDistance: vmUpdate.distance, amount: vmUpdate.amount })
                            .then(function (res) {
                                if (!res.data.success) {
                                    $.alert(res.data.msg);
                                    return;
                                }
                                $.alert(res.data.msg, "success");
                                item.maxDistance = res.data.data.maxDistance;
                                item.amount = res.data.data.amount;
                                item.modifyTime = res.data.data.modifyTime;
                            })
                            .catch(function (err) { $.alert(err); });
                        return true;
                    }
                });
            }
        }
    });

})(jQuery);

