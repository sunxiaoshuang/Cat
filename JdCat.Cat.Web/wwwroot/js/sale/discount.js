
(function () {
    var dialogName = "product", vmProduct, vm, settingVm,
        cycleList = { "1": "周一", "2": "周二", "4": "周三", "8": "周四", "16": "周五", "32": "周六", "64": "周日" }, editTemplate;

    // 定义时间选择组件
    Vue.component("cat-time", {
        props: ["time"],
        data: function () {
            var hour = "00", minus = "00", hourList = [], minusList = [], i = 0, sign;
            for (; i < 24;) {
                sign = i.toString();
                if (i < 10) sign = "0" + sign;
                hourList.push(sign);
                i++;
            }
            i = 0;
            for (; i < 60;) {
                sign = i.toString();
                if (i < 10) sign = "0" + sign;
                minusList.push(sign);
                i += 5;
            }
            minusList.push("59");

            if (!!this.time) {
                hour = this.time.split(":")[0];
                minus = this.time.split(":")[1];
            }
            return {
                hour, minus, hourList, minusList
            };
        },
        template: `
            <div class='cat-time'>
                <select class='form-control' v-model='hour'>
                    <option v-for='item in hourList' :value='item'>{{item}}</option>
                </select>
                <span class='split'>:</span>
                <select class='form-control' v-model='minus'>
                    <option v-for='item in minusList' :value='item'>{{item}}</option>
                </select>
            </div>`,
        methods: {

        },
        watch: {
            hour: function (a) {
                var time = this.hour + ":" + this.minus;
                this.$emit("change", time);
            }
        }
    });

    // 主界面列表对象
    vm = new Vue({
        el: "#app",
        data: {
            isShow: true,
            discountList: JSON.parse(pageData.discountList)
        },
        methods: {
            select: function () {
                $.view({
                    url: "/Sale/DiscountProduct",
                    name: dialogName,
                    title: "选择商品",
                    footDisplay: "block",
                    submit: function () {
                        if (vmProduct.productList.length === 0) return true;

                        var data = vmProduct.productList.map(product => {
                            return {
                                name: product.name, oldPrice: product.formats[0].price, productId: product.id
                            };
                        });

                        axios.post("/Sale/CreateDiscount", data)
                            .then(function (res) {
                                res.data.forEach(a => vm.discountList.push(a));
                                $.alert("创建成功", "success");
                            })
                            .catch(function () {
                                $.alert("系统错误，请刷新后重试");
                            });

                        return true;
                    },
                    load: function () {
                        initModal.call(this);
                    }
                });
            },
            edit: function (entity) {
                if (!editTemplate) {
                    $.loading();
                    axios.get("/Sale/DiscountDetail")
                        .then(function (res) {
                            editTemplate = res.data;
                            editModal(entity);
                            $.loaded();
                        });
                } else {
                    editModal(entity);
                }
            },
            remove: function (entity, index) {
                var self = this;
                $.primary(`确定删除商品<b>[${entity.name}]</b>的折扣活动吗？`, function () {
                    axios.delete("/Sale/DeleteDiscount/" + entity.id)
                        .then(function (res) {
                            if (res.data.success) {
                                $.alert(res.data.msg, "success");
                                self.discountList.splice(index, 1);
                            } else {
                                $.alert(res.data.msg);
                            }
                        });
                    return true;
                });
            }
        },
        filters: {
            price: function (price) {
                if (price == 0) return "-";
                return "￥" + price;
            },
            discount: function (discount) {
                if (!discount) return "-";
                return discount + "折";
            },
            upperLimit: function (num) {
                if (num === -1) return "不限";
                return num;
            },
            activityTime: function (item) {
                if (!item.startDate || !item.endDate) return "-";
                return item.startDate.substring(0, 10) + " 至 " + item.endDate.substring(0, 10)
            },
            cycle: function (cycle) {
                var result = "";
                if (cycle == 127) return "全部";
                for (var key in cycleList) {
                    if (cycle & key) {
                        result += cycleList[key] + "|";
                    }
                }
                if (result.length > 0) {
                    result = result.substring(0, result.length - 1);
                }
                return result;
            },
            validTime: function (item) {
                var result = "";
                if (item.startTime1 == "00:00" && item.endTime1 == "23:59") return "全天";
                if (!!item.startTime1) {
                    result += item.startTime1 + "-" + item.endTime1;
                }
                if (!!item.startTime2) {
                    result += " | " + item.startTime2 + "-" + item.endTime2;
                }
                if (!!item.startTime3) {
                    result += " | " + item.startTime3 + "-" + item.endTime3;
                }
                return result;
            },
            status: function (status) {
                if (status == 0) return "";
                return "生效"
            }
        }
    });

    // 初始化选择菜品模态框
    function initModal() {
        var self = this, typeList = JSON.parse(pageData.typeList);
        typeList.forEach(function (item) {
            item.checked = false;
            // 仅保留只有一个规格的商品
            item.products = item.products.filter(product => product.formats.length === 1);
            // 去掉已经存在的打折活动商品
            item.products = item.products.filter(product => vm.discountList.filter(a => a.productId === product.id).length === 0);
        });
        vmProduct = new Vue({
            el: `#discount`,
            data: {
                typeList: typeList,
                productList: []
            },
            methods: {
                open: function (item) {
                    var state = item.checked;
                    this.typeList.forEach(function (type) { type.checked = false; });
                    item.checked = !state;
                },
                choice: function (product) {
                    var item = this.productList.filter(a => a.id === product.id);
                    if (item.length > 0) return;
                    this.productList.push(product);
                },
                remove: function (index) {
                    this.productList.splice(index, 1);
                }
            }
        });
        // 隐藏时销毁
        this.on("hidden.bs.modal", function () {
            if (!vmProduct) return;
            vmProduct.$destroy();
            vmProduct = null;
        });
    }

    // 初始化设置折扣商品模态框
    function editModal(entity) {
        $.view({
            template: editTemplate,
            title: "设置折扣商品-" + entity.name,
            name: "view_setting",
            footDisplay: "block",
            load: function () {
                var obj = JSON.parse(JSON.stringify(entity));
                obj.startDate && (obj.startDate = obj.startDate.substring(0, 10));
                obj.endDate && (obj.endDate = obj.endDate.substring(0, 10));
                settingVm = new Vue({
                    el: "#setting",
                    data: {
                        entity: obj,
                        cycleList: [
                            { name: "周一", value: 1, checked: true },
                            { name: "周二", value: 2, checked: true },
                            { name: "周三", value: 4, checked: true },
                            { name: "周四", value: 8, checked: true },
                            { name: "周五", value: 16, checked: true },
                            { name: "周六", value: 32, checked: true },
                            { name: "周日", value: 64, checked: true }
                        ]
                    },
                    methods: {
                        changeTime: function (sign, a) {
                            this.entity[sign] = a;
                        },
                        addTime: function () {
                            if (!this.entity.startTime2) {
                                this.entity.startTime2 = "00:00";
                                this.entity.endTime2 = "00:00";
                                return;
                            }
                            if (!this.entity.startTime3) {
                                this.entity.startTime3 = "00:00";
                                this.entity.endTime3 = "00:00";
                            }
                        },
                        removeTime: function () {
                            if (!!this.entity.startTime3) {
                                this.entity.startTime3 = null;
                                this.entity.endTime3 = null;
                                return;
                            }
                            if (!!this.entity.startTime2) {
                                this.entity.startTime2 = null;
                                this.entity.endTime2 = null;
                            }
                        },
                        discountChange: function () {
                            this.entity.price = parseFloat((this.entity.oldPrice * this.entity.discount / 10).toFixed(2));
                        },
                        priceChange: function () {
                            this.entity.discount = parseFloat((this.entity.price / this.entity.oldPrice * 10).toFixed(2));
                        }
                    },
                    created: function () {
                        this.cycleList.forEach(cycle => {
                            if (!(this.entity.cycle & cycle.value)) {
                                cycle.checked = false;
                            }
                        });
                    },
                    watch: {
                        "cycleList": {
                            handler: function (val) {
                                var num = 0;
                                val.forEach(cycle => {
                                    if (cycle.checked) {
                                        num = num | cycle.value;
                                    }
                                });
                                this.entity.cycle = num;
                            },
                            deep: true
                        }
                    }
                });

                $("#txtStartDate").datetimepicker(dateOptions).on("changeDate", function (e) {
                    settingVm.entity.startDate = this.value;
                });
                $("#txtEndDate").datetimepicker(dateOptions).on("changeDate", function (e) {
                    settingVm.entity.endDate = this.value;
                });
                // 隐藏时销毁
                this.on("hidden.bs.modal", function () {
                    if (!settingVm) return;
                    settingVm.$destroy();
                    settingVm = null;
                });
            },
            submit: function () {
                if (!settingVm.entity.startDate || !settingVm.entity.endDate) {
                    $.alert("请选择活动日期");
                    return false;
                }
                if (settingVm.entity.startDate > settingVm.entity.endDate) {
                    $.alert("活动开始时间不能大于活动结束时间");
                    return false;
                }
                if (settingVm.entity.startTime1 > settingVm.entity.endTime1) {
                    $.alert("生效时段[1]：开始时间不能大于结束时间");
                    return false;
                }
                // 判断时间是否有重叠
                if (!!settingVm.entity.startTime2) {
                    if (settingVm.entity.startTime2 > settingVm.entity.endTime2) {
                        $.alert("生效时段[2]：开始时间不能大于结束时间");
                        return false;
                    }
                    if (settingVm.entity.startTime2 < settingVm.entity.endTime1) {
                        $.alert("生效时段[2]必须大于生效时段[1]");
                        return false;
                    }
                }
                if (!!settingVm.entity.startTime3) {
                    if (settingVm.entity.startTime3 > settingVm.entity.endTime3) {
                        $.alert("生效时段[3]：开始时间不能大于结束时间");
                        return false;
                    }
                    if (settingVm.entity.startTime3 < settingVm.entity.endTime2) {
                        $.alert("生效时段[3]必须大于生效时段[2]");
                        return false;
                    }
                }
                if (settingVm.entity.upperLimit <= 0) {
                    $.alert("每单限购必须大于0");
                    return false;
                }
                axios.post("/Sale/UpdateDiscount", settingVm.entity)
                    .then(function (res) {
                        if (!res.data.success) {
                            $.alert(res.data.msg);
                            return;
                        }
                        $.alert(res.data.msg, "success");
                        // 替换原来的对象
                        var entity = res.data.data, index;
                        vm.discountList.some((a, i) => {
                            if (a.id != entity.id) return false;
                            index = i;
                            return true;
                        });
                        vm.discountList.splice(index, 1, entity);
                    })
                    .catch(function (err) {
                        $.alert(err);
                    });
                return true;
            }
        });
    }

    // 停止加载
    $.loaded();

    var dateOptions = {
        format: 'yyyy-mm-dd',
        autoclose: true,
        maxView: 1,
        minView: 2,
        todayBtn: true,
        todayHighlight: true,
        language: "zh-CN"
    };
})();
