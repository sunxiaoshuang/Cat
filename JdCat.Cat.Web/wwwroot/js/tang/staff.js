(function () {

    var app = new Vue({
        el: "#app",
        data: {
            items: []
        },
        methods: {
            add: function () {
                edit();
            },
            modify: function (staff) {
                edit(staff);
            },
            remove: function (staff) {
                $.primary(`确定删除员工[${staff.name}]吗？`, function () {
                    axios.delete(`/tang/deleteStaff/${staff.id}`)
                        .then(function (res) {
                            if (!res.data.success) {
                                $.alert(res.data.msg);
                                return;
                            }
                            app.items.remove(staff);
                            $.alert(res.data.msg, "success");
                        })
                        .catch(function (err) { $.alert(err); });
                    return true;
                });
            },
            resetPwd: (function () {
                var vueObj, template = `
                    <div class="row form-horizontal" id="resetPwd">
                        <div class="form-group">
                            <label class="control-label col-xs-3">
                                <span class="require">新密码：</span>
                            </label>
                            <div class="col-xs-8">
                                <input class="form-control" type="password" v-model.trim="password" />
                            </div>
                        </div>
                        <div class="form-group">
                            <label class="control-label col-xs-3">
                                <span class="require">再次输入：</span>
                            </label>
                            <div class="col-xs-8">
                                <input class="form-control" type="password" v-model.trim="password1" />
                            </div>
                        </div>
                    </div>
                `;
                return function (staff) {
                    $.view({
                        title: "重置密码",
                        footDisplay: "block",
                        template,
                        load: function () {
                            vueObj = new Vue({
                                el: "#resetPwd",
                                data: {
                                    password: "",
                                    password1: ""
                                }
                            });
                            // 隐藏时销毁
                            destroyVue.call(this, vueObj);
                        },
                        submit: function () {
                            if (!vueObj.password) {
                                $.alert("请输入新密码");
                                return;
                            }
                            if (vueObj.password !== vueObj.password1) {
                                $.alert("两次输入的密码不一致");
                                return;
                            }
                            axios.put(`/tang/resetPassword/${staff.id}?pwd=${vueObj.password}`)
                                .then(function (res) {
                                    if (!res.data.success) {
                                        $.alert(res.data.msg);
                                        return;
                                    }
                                    $.alert(res.data.msg, "success");
                                })
                                .catch(function (err) { $.alert(err); });
                            return true;
                        }
                    });
                };
            })(),
            bindProduct: function (cook) {
                if ((cook.staffPost.authority & 4) === 0) return;
                bindProducts(cook);
            },


            genderFormat: function (gender) {
                if (gender === 1) return "男";
                if (gender === 2) return "女";
                return "未知";
            },
            timeFormat: function (time) {
                if (!time) return "";
                return time.substring(0, 10);
            }
        },
        created: function () {
            var self = this;
            axios.get("/tang/getStaffs")
                .then(function (res) {
                    self.items = res.data.list;
                });
        }
    });


    var edit = function (obj) {
        var template, posts, vueObj;
        $.loading();
        axios.all([axios.get("/tang/editStaff"), axios.get("/tang/getStaffPosts")])
            .then(function (res) {
                template = res[0].data;
                posts = res[1].data.staffPosts || [];
                posts = posts.map(function (obj) { return { id: obj.id, name: obj.name }; });
            })
            .then(function () {
                $.loaded();
                edit = function (staff) {
                    var isAdd = !staff, employee = staff || {};
                    $.view({
                        title: isAdd ? "新增员工" : "编辑员工",
                        footDisplay: "block",
                        template: template,
                        load: function () {
                            var $model = this;
                            vueObj = new Vue({
                                el: "#edit-staff",
                                data: {
                                    isAdd: isAdd,
                                    name: employee.name,
                                    alise: employee.alise,
                                    password: "",
                                    password1: "",
                                    gender: employee.gender || 0,
                                    enterTime: employee.enterTime ? employee.enterTime.substring(0, 10) : "",
                                    birthday: employee.birthday ? employee.birthday.substring(0, 10) : "",
                                    cardId: employee.cardId,
                                    staffPostId: employee.staffPostId || 0,
                                    posts: JSON.parse(JSON.stringify(posts)),
                                    genderList: [{ name: "男", value: 1 }, { name: "女", value: 2 }]
                                },
                                created: function () {
                                }
                            });
                            $model.find(".birthday").datetimepicker($.dateOptions).on("changeDate", function (e) {
                                vueObj.birthday = this.value;
                            });
                            $model.find(".enterTime").datetimepicker($.dateOptions).on("changeDate", function (e) {
                                vueObj.enterTime = this.value;
                            });
                            // 隐藏时销毁
                            destroyVue.call(this, vueObj);
                        },
                        submit: function () {
                            if (!vueObj.staffPostId) {
                                $.alert("请选择所属岗位");
                                return;
                            }
                            if (!vueObj.name) {
                                $.alert("请输入员工姓名");
                                return;
                            }
                            if (!vueObj.alise) {
                                $.alert("请输入登录帐号");
                                return;
                            }
                            if (isAdd) {
                                if (!vueObj.password) {
                                    $.alert("请输入登录密码");
                                    return;
                                }
                                if (vueObj.password !== vueObj.password1) {
                                    $.alert("两次输入密码不一致");
                                    return;
                                }
                            }
                            if (!vueObj.gender) {
                                $.alert("请选择员工性别");
                                return;
                            }
                            var url = isAdd ? "/tang/addStaff" : "/tang/updateStaff";
                            var body = {
                                id: employee.id || 0,
                                name: vueObj.name,
                                alise: vueObj.alise,
                                password: vueObj.password,
                                gender: vueObj.gender,
                                birthday: vueObj.birthday,
                                enterTime: vueObj.enterTime,
                                cardId: vueObj.cardId,
                                staffPostId: vueObj.staffPostId
                            };
                            axios.post(url, body)
                                .then(function (res) {
                                    if (!res.data.success) {
                                        $.alert(res.data.msg);
                                        return;
                                    }
                                    $.alert(res.data.msg, "success");
                                    if (isAdd) {
                                        app.items.push(res.data.data);
                                    } else {
                                        app.items.replace(staff, res.data.data);
                                    }
                                })
                                .catch(function (err) { $.alert(err); });
                            return true;
                        }
                    });
                };
                edit(obj);
            });
    };

    var bindProducts = function (obj) {
        var template, types;
        $.loading();
        axios.all([axios.get(`/product/getProductTree?isSetMeal=1`), axios.get(`/tang/selectProductForCook`)])
            .then(function (res) {
                template = res[1].data;
                types = res[0].data.map(function (type) { type.checked = false; return type; });
                $.loaded();

                bindProducts = function (cook) {
                    var vueObj, typeList = JSON.parse(JSON.stringify(types)), productList = [], allProducts = [];
                    typeList.forEach(function (type) { allProducts = allProducts.concat(type.list); });
                    allProducts.forEach(function (product) { product.checked = false; });

                    axios.get(`/tang/getProductIdsByCook/${cook.id}`)
                        .then(function (res) {
                            var productIds = res.data;

                            if (productIds && productIds.length > 0) {
                                productIds.forEach(function (id) {
                                    var product = allProducts.first(function (item) { return item.id === id; });
                                    if (!product) return;
                                    product.checked = true;
                                    productList.push(product);
                                });
                            }
                            $.view({
                                title: `厨师[${cook.name}]绑定菜单`,
                                footDisplay: "block",
                                template,
                                dialogWidth: 800,
                                load: function () {
                                    vueObj = new Vue({
                                        el: "#bindCook",
                                        data: {
                                            typeList,
                                            productList
                                        },
                                        methods: {
                                            open: function (type) {
                                                type.checked = !type.checked;
                                            },
                                            choice: function (product) {
                                                if (product.checked) return;
                                                product.checked = true;
                                                this.productList.push(product);
                                            },
                                            remove: function (product) {
                                                product.checked = false;
                                                this.productList.remove(product);
                                            }
                                        }
                                    });
                                },
                                submit: function () {
                                    var relative = vueObj.productList.map(function (obj) { return { staffId: cook.id, productId: obj.id }; });
                                    var foodIds = JSON.stringify(vueObj.productList.map(function (obj) { return obj.id; }));
                                    axios.post(`/tang/bindProductsForCook/${cook.id}`, relative)
                                        .then(function (res) {
                                            $.alert(res.data.msg, "success");
                                        })
                                        .catch(function (err) { $.alert(err); });
                                    return true;
                                }


                            });
                        })
                        .catch(function (err) { $.alert(err); });
                };
                bindProducts(obj);
            })
            .catch(function (err) { $.alert(err); });
    };


    // 销毁vue对象
    function destroyVue(obj) {
        this.on("hidden.bs.modal", function () {
            if (!obj) return;
            obj.$destroy();
            obj = null;
        });
    }

})();

