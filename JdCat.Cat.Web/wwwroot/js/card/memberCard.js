
(function () {

    new Vue({
        el: "#app",
        data: {
            card: {
                member_card: {
                    base_info: {}
                },
                bonusSale: {amount: 0, give: 0, mode: 0},
                bonusCharge: { amount: 0, give: 0, mode: 1 },
                bonusOpen: { amount: 0, give: 0, mode: 2 }
            },
            colors: [
                { name: "Color010", color: "#63b359", selected: false },
                { name: "Color020", color: "#2c9f67", selected: false },
                { name: "Color030", color: "#509fc9", selected: false },
                { name: "Color040", color: "#5885cf", selected: false },
                { name: "Color050", color: "#9062c0", selected: false },
                { name: "Color060", color: "#d09a45", selected: false },
                { name: "Color070", color: "#e4b138", selected: false },
                { name: "Color080", color: "#ee903c", selected: false },
                { name: "Color090", color: "#dd6549", selected: false },
                { name: "Color100", color: "#cc463d", selected: false }
            ]
        },
        computed: {
            cardStatus: function () {
                if (!this.card.member_card.base_info.status) return "未创建";
                if (this.card.member_card.base_info.status === "CARD_STATUS_VERIFY_OK") return "可投放";
                return "审核中";
            }
        },
        methods: {
            selectColor: function (color) {
                if (color.selected) return;
                this.colors.forEach(a => a.selected = false);
                color.selected = true;
                this.card.member_card.base_info.color = color.name;
            },
            setWhiteList: function () {
                axios.post("/Card/SetWhiteList");
            },
            save: function () {
                var baseinfo = this.card.member_card.base_info;
                if (!baseinfo.brand_name) {
                    $.alert("请输入品牌名称");
                    return;
                }
                if (!baseinfo.title) {
                    $.alert("请输入副标题");
                    return;
                }
                if (!baseinfo.notice) {
                    $.alert("请输入操作提示");
                    return;
                }
                if (!this.card.member_card.prerogative) {
                    $.alert("请输入特权说明");
                    return;
                }
                if (!baseinfo.description) {
                    $.alert("请输入使用须知");
                    return;
                }
                if (!baseinfo.service_phone) {
                    $.alert("请输入联系电话");
                    return;
                }
                $.loading();
                if (baseinfo.id) {          // 修改
                    var obj = {
                        "card_id": baseinfo.id,
                        "member_card": {
                            "base_info": {
                                "title": baseinfo.title,
                                "color": baseinfo.color,
                                "notice": baseinfo.notice,
                                "service_phone": baseinfo.service_phone,
                                "description": baseinfo.description,
                                //"center_url": mpUrl + "/member.html?sign=pay",
                                //"custom_url": mpUrl + "/member.html?sign=charge",
                                //"custom_url_name": "会员充值",
                                //"custom_url_sub_title": "",
                                //promotion_url: mpUrl + "/member.html?sign=sale",       // 自定义外链
                                //promotion_url_name: "消费记录"
                            },
                            "prerogative": this.card.member_card.prerogative
                        },
                        chargeList: this.card.chargeList,
                        bonusSale: this.card.bonusSale,
                        bonusCharge: this.card.bonusCharge,
                        bonusOpen: this.card.bonusOpen
                    };
                    axios.post("/Card/UpdateCard", obj)
                        .then(function (res) {
                            $.loaded();
                            if (res.data.success) {
                                window.location.reload();
                                return;
                            }
                            $.alert(res.data.msg);
                        })
                        .catch(function (err) {
                            $.loaded();
                            $.alert(err);
                        });
                } else {                    // 新增
                    var postData = {
                        card: this.card
                    };
                    postData.card.member_card.bonus_rule.init_increase_bonus = this.card.bonusOpen.give;
                    axios.post("/Card/CreateMemberCard", postData)
                        .then(function (res) {
                            $.loaded();
                            if (res.data.success) {
                                window.location.reload();
                                return;
                            }
                            $.alert(res.data.msg);
                        })
                        .catch(function (err) {
                            $.loaded();
                            $.alert(err);
                        });
                }
            },
            qrcode: function () {
                window.open("/Card/CreateMemberQrcode?cardId=" + this.card.member_card.base_info.id);
            },
            addCharge: function () {
                if (this.card.chargeList.length >= 4) return;
                this.card.chargeList.push({amount: 0, give: 0});
            },
            reduceCharge: function (index) {
                this.card.chargeList.splice(index, 1);
            }
        },
        created: function () {
            var self = this;
            $.loading();

            axios.all([axios.get("/Card/GetMemberCard"), axios.get("/Card/GetCardRule")])
                .then(function (res) {
                    $.loaded();
                    var card;
                    if (res[0].data.errcode === 0) {
                        card = res[0].data.card;
                    } else {
                        card = {
                            card_type: "MEMBER_CARD",
                            member_card: {
                                base_info: {                                // 基本信息设置
                                    brand_name: "",                         // 品牌名称
                                    title: "",                              // 副标题
                                    code_type: "CODE_TYPE_BARCODE",         // 点击会员卡后显示条形码
                                    can_give_friend: false,                 // 不能转赠朋友
                                    center_title: "会员支付",               // 中间的按钮文字
                                    center_sub_title: "点击生成二维码",     // 中间的副标题
                                    center_url: mpUrl + "/member.html?sign=pay",       // 中间的跳转链接
                                    color: "#63b359",
                                    custom_url: mpUrl + "/member.html?sign=charge",    // 自定义外链信息
                                    custom_url_name: "会员充值",
                                    custom_url_sub_title: "",
                                    date_info: {
                                        type: "DATE_TYPE_PERMANENT"             // 永久有效
                                    },
                                    description: "会员卡优惠不可与其他优惠同享",
                                    get_limit: 1,                               // 每人可领券的数量限制
                                    location_id_list: [],                       // 门店位置ID。调用 POI门店管理接口 获取门店位置ID。
                                    logo_url: "",                               // 会员卡LOGO
                                    notice: "使用时向服务员出示此券",           // 卡券使用提醒，字数上限为16个汉字
                                    promotion_url: mpUrl + "/member.html?sign=sale",       // 自定义外链
                                    promotion_url_name: "消费记录",
                                    service_phone: "",                          // 客服电话
                                    sku: {                                      // 库存
                                        quantity: 100000000
                                    },
                                    use_custom_code: false                      // 是否使用自定义编码
                                },
                                advanced_info: {                        // 高级信息设置
                                    //business_service: [
                                    //    "BIZ_SERVICE_DELIVER",          // 外卖服务
                                    //    "BIZ_SERVICE_FREE_PARK",        // 停车位
                                    //    "BIZ_SERVICE_WITH_PET",         // 可带宠物
                                    //    "BIZ_SERVICE_FREE_WIFI"         // 免费wifi
                                    //],
                                    //text_image_list: [
                                    //    { image_url: "http://f.jiandanmao.cn/File/Product/a.jpg", text: "独家秘方烹制而成！"},
                                    //    { image_url: "http://f.jiandanmao.cn/File/Product/b.jpg", text: "好吃难忘！" },
                                    //],

                                },
                                auto_activate: false,                       // 不要自动激活
                                bonus_cleared: "",                          // 积分清零规则
                                bonus_rules: "",                            // 积分规则
                                bonus_rule: {                               // 积分规则
                                //    cost_money_unit: 100,                   // 消费金额。以分为单位
                                //    increase_bonus: 1,                      // 对应增加的积分
                                //    max_increase_bonus: 200,                // 用户单次可获取的积分上限
                                //    init_increase_bonus: 10,                // 初始设置积分
                                //    cost_bonus_unit: 5,                     // 每使用5积分
                                //    reduce_money: 100,                      // 抵扣xx元，（这里以分为单位）
                                //    least_money_to_use_bonus: 1000,         // 抵扣条件，满xx元（这里以分为单位）可用
                                //    max_reduce_bonus: 50                    // 抵扣条件，单笔最多使用xx积分。
                                },
                                //discount: 10，                              // 享受折扣
                                //custom_cell1: {                               // 自定义外链，最多三个
                                //    name: "",                                 // 入口名称
                                //    tips: "入口右侧提示语，6个汉字内",        // 入口右侧提示语，6个汉字内
                                //    url: ""
                                //}，
                                //custom_field1: {                              // 会员卡激活后呈现的信息类目，包含积分、余额，最多三个
                                //    name_type: "FIELD_NAME_TYPE_STAMP",
                                //    url: "余额"
                                //},
                                //custom_field2: {
                                //    name_type: "FIELD_NAME_TYPE_ACHIEVEMEN",
                                //    url: "积分"
                                //},
                                prerogative: "",                                // 会员卡特权说明,限制1024汉字。
                                supply_balance: false,                          // 是否支持储值
                                balance_url: mpUrl + "/member.html?sign=charge",
                                supply_bonus: false,                             // 是否显示积分
                                wx_activate: true                               // 使用一键开卡
                            }
                        };
                    }
                    card.chargeList = res[1].data.charge.length === 0 ? [{ amount: 0, give: 0 }] : res[1].data.charge;
                    card.bonusSale = res[1].data.bonus.first(a => a.mode === 0) || { amount: 0, give: 0, mode: 0 };
                    card.bonusCharge = res[1].data.bonus.first(a => a.mode === 1) || { amount: 0, give: 0, mode: 1 };
                    card.bonusOpen = res[1].data.bonus.first(a => a.mode === 2) || { amount: 0, give: 0, mode: 2 };

                    self.card = card;
                    var color = self.colors.first(a => a.color === self.card.member_card.base_info.color);
                    if (color) {
                        self.card.member_card.base_info.color = color.name;
                        color.selected = true;
                    }
                });
        }
    });
})();