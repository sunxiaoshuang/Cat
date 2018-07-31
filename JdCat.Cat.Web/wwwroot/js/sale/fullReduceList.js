
(function () {

    new Vue({
        el: "#app",
        data: {
            list: pageData.list
        },
        methods: {
            create: function () {
                window.location = "/Sale/FullReduce";
            },
            timeFilter: function (item) {
                if (item.isForeverValid) {
                    return "永久";
                }
                return item.startDate.substring(0, 10) + " 至 " + item.endDate.substring(0, 10);
            },
            stateFilter: function (item) {
                if (item.isForeverValid) {
                    return "进行中";
                }
                var now = new Date().format("yyyy-MM-dd");

                if (now > item.endDate.substring(0, 10)) {
                    return "已过期";
                }
                if (now < item.startDate.substring(0, 10)) {
                    return "尚未开始";
                }
                return "进行中";
            },
            edit: function (item) {
                window.location = "/Sale/FullReduce/" + item.id;
            },
            remove: function (item, index) {
                var self = this;
                $.primary("确定删除该活动？", function () {
                    axios.delete("/Sale/DeleteFullReduce/" + item.id)
                        .then(function (res) {
                            if (!res.data.success) {
                                $.alert(res.data.msg);
                                return;
                            }
                            $.alert(res.data.msg, "success");
                            self.list.splice(index, 1);
                        })
                        .catch(function (err) {
                            $.alert(err);
                        });
                    return true;
                });
            }
        }
    });
    

})();
