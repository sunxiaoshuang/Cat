;
(function ($) {
    var postAuthName = { 1: "收银", 2: "管理", 4: "厨师", 8: "服务员" };

    var app = new Vue({
        el: "#app",
        data: {
            post: {
                items: []
            },
            staff: {
                items: []
            }
        },
        methods: {
            addPost: function () {
                editPost();
            },
            modifyPost: function (post) {
                editPost(post);
            },
            removePost: function (post) {
                $.primary(`确定删除岗位[${post.name}]吗？`, function () {
                    axios.delete(`/tang/deleteStaffPost/${post.id}`)
                        .then(function (res) {
                            if (!res.data.success) {
                                $.alert(res.data.msg);
                                return;
                            }
                            app.post.items.remove(post);
                            $.alert(res.data.msg, "success");
                        })
                        .catch(function (err) { $.alert(err); });
                    return true;
                });
            },


            postAuthFormat: function (auth) {
                var name = [];
                for (var key in postAuthName) {
                    if ((key & auth) === 0) continue;
                    name.push(postAuthName[key]);
                }
                return name.join("，");
            }
        },
        created: function () {
            var self = this;
            axios.get("/tang/getStaffPosts")
                .then(function (res) {
                    self.post.items = res.data.staffPosts;
                });
        }
    });

    var editPost = function (obj) {
        var template = null,
            vueObj = null,
            post = null,
            isAdd = false,
            authList = [{ name: "收银", value: 1, checked: false }, { name: "管理", value: 2, checked: false }, { name: "厨师", value: 4, checked: false }, { name: "服务员", value: 8, checked: false }];
        axios.get("/tang/editStaffPost")
            .then(function (res) {
                template = res.data;

                editPost = function () {
                    post = arguments[0];
                    isAdd = !post;
                    $.view({
                        title: isAdd ? "新增岗位" : "修改岗位",
                        template: template,
                        footDisplay: "block",
                        load: function () {
                            var auths = JSON.parse(JSON.stringify(authList));
                            if (!isAdd) {
                                auths.forEach(function (obj) {
                                    if ((post.authority & obj.value) === 0) return;
                                    obj.checked = true;
                                });
                            }
                            vueObj = new Vue({
                                el: "#edit-post",
                                data: {
                                    name: isAdd ? "" : post.name,
                                    auths
                                }
                            });
                            // 隐藏时销毁
                            destroyVue.call(this, vueObj);
                        },
                        submit: function () {
                            var name = vueObj.name, auth = 0;
                            if (!name) {
                                $.alert("请输入岗位名称");
                                return;
                            }
                            vueObj.auths.forEach(function (item) {
                                if (!item.checked) return;
                                auth += item.value;
                            });
                            if (!auth) {
                                $.alert("请选择岗位权限");
                                return;
                            }
                            var url = isAdd ? "/tang/addStaffPost" : "/tang/updateStaffPost";
                            var body = { id: isAdd ? 0 : post.id, name: name, authority: auth };
                            $.loading();
                            axios.post(url, body)
                                .then(function (res) {
                                    $.loaded(); 
                                    if (!res.data.success) {
                                        $.alert(res.data.msg);
                                        return;
                                    }
                                    if (isAdd) {
                                        app.post.items.push(res.data.data);
                                    }
                                    else {
                                        post.name = name;
                                        post.authority = auth;
                                    }
                                })
                                .catch(function (err) { $.loaded(); $.alert(err); });
                            return true;
                        }
                    });
                };
                editPost(obj);
            });
    };


    // 销毁vue对象
    function destroyVue(obj) {
        this.on("hidden.bs.modal", function () {
            if (!obj) return;
            obj.$destroy();
            obj = null;
        });
    }

})(jQuery);

