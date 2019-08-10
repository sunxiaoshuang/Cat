var preSettle = false;
var orderNo = "";
var rechargeLimit = $("#rechargeLimit").val();
var merchant_json;
var payType = "1"
$(function () {
    if (/MicroMessenger/.test(window.navigator.userAgent)) {
        payType = "5"
    } else if (/AlipayClient/.test(window.navigator.userAgent)) {
        payType = "4";
    } else {
        payType = "1";
    }
    //显示充值规则
    $('.activity_rule').click(function () {
        $(this).siblings('a').removeClass('active');
        $(this).toggleClass('active');
        var money = $(this).attr("data-money");
        $('#topval').val(money);
        $("#czbtn").removeClass("grayColour");
        $(".recharge-btn").text("充值 " + money + "元")

        // 显示奖励
        prizeShow(money);

        // 显示规则
        var id = $(this).attr("data-id");
        $(".activity_desc div").hide();
        $("#ri_" + id).show();
    });

    //充值获取焦点
    $('#topval').focus(function () {
        var money = $(this).val();
        if (money == "") {
            $("#czbtn").addClass("grayColour");
        }
    });

    // 充值改变
    $('#topval').on('input propertychange', function () {
        var money = $(this).val();
        if (money != '') {
            $(".activity_rule.active").removeClass("active");
        }
        prizeShow(money);
    });
    var coreRechargeStatus = $("#coreRechargeStatus").val();
    

    // 隐藏门店列表
    $(".sidenav-overlay").click(function () {
        $("#change-m-btn").select("close");
    });

    // 充值记录
    $("#mobile_recharge").click(function () {
        $(".pageload").show();
    })

    //立即充值
    $(document).on("touchstart", "#czbtn", function (event) {
        if ($('#czbtn').hasClass("grayColour")) {
            return;
        }
        if ($("#czbtn").attr("auto") == "auto") {
            return;
        }
        var topvalMemony = $('#topval').val();
        if (topvalMemony == undefined || '' == topvalMemony || topvalMemony <= 0) {
            msg("充值金额必须大于0");
            return;
        }
        $("#czbtn").attr("auto", "auto");
        $(".pageload").show();
        jQuery.ajax({
            type: "POST",
            url: "/ShopCashier/shopCashierMicro/rechargeMemberBalance.in",
            data: {
                rechargeMemony: $('#topval').val(),
                openId: $('#mr_openId').val(),
                merchantNo: $('#mr_merchantNo').val(),
                memberLevelId: $('#mr_memberLevelId').val(),
                memberCA: $('#mr_accountNo').val(),
                memberId: $('#mr_memberId').val(),
                buyerId: $('#mr_aliUserId').val(),
                shopNo: $('#mr_shopNo').val(),
                terminalId: $('#mr_terminalId').val(),
                employeeId: $('#mr_employeeId').val()
            },
            dataType: "json",
            success: function (data) {
                $(".pageload").hide();
                if (data.YESORNO == 'YES') {
                    orderNo = data.orderNo;
                    preSettle = data.preSettle;
                    if (data.payType == 'alpay') {//支付宝3.0会员充值
                        aliCallPayV3(data);
                    } else {
                        callpay(data);
                    }
                } else {
                    msg("系统繁忙，暂时无法充值");
                    $("#czbtn").removeAttr("auto");
                }
            },
            error: function () {
                $(".pageload").hide();
            }
        });
    });

    // 显示规则
    if ($(".activity_desc div").length > 0) {
        var html = $(".activity_desc div").eq(0).html().trim();
        if (html == '') {
            $("#coupinfoscroll").hide()
        }
    }

});
/**礼物展示*/
function prizeShow(money) {
    var give_exp = 0;
    var give_point = 0;
    var give_card = '';
    var give_timescard = '';
    var give_bag = '';
    var recharge_rules_txt = ''
    var mr_shopNoId = $("#mr_shopNoId").val();
    if (typeof (recharge_rules) != 'undefined') {
        var pointRule = null;
        var expRule = null;

        var couponRule = null;
        var couponMap = {};
        var couponInitMoney = 0;

        var timescardInitMoney = 0;
        var timescardRule = null;
        var timescardMap = {};

        var bagInitMoney = 0;
        var bagRule = null;
        var bagMap = {};
        for (var key in recharge_rules) {
            var temp = recharge_rules[key];
            var suitShops = temp.suitShops;
            if (!temp.hasOwnProperty("rechargeMoney") || temp.rechargeMoney == null || temp.rechargeMoney == "") {
                temp.rechargeMoney = 0;
            }
            if (temp.payType.indexOf(payType) > -1 && (suitShops == null || suitShops == "" || suitShops.indexOf(mr_shopNoId) > -1) && money >= temp.rechargeMoney) {
                if (temp.prizeType == 1) {
                    if (pointRule == null || temp.rechargeMoney > pointRule.rechargeMoney) {
                        pointRule = temp;
                    }
                } else if (temp.prizeType == 2) {
                    if (expRule == null || temp.rechargeMoney > expRule.rechargeMoney) {
                        expRule = temp;
                    }
                } else if (temp.prizeType == 0) {
                    couponInitMoney = temp.rechargeMoney;
                    if (couponMap.hasOwnProperty(temp.rechargeMoney)) {
                        var coupons = couponMap[temp.rechargeMoney];
                        coupons.push(temp);
                    } else {
                        couponMap[temp.rechargeMoney] = new Array();
                        couponMap[temp.rechargeMoney].push(temp);
                    }
                } else if (temp.prizeType == 7) {
                    timescardInitMoney = temp.rechargeMoney;
                    if (timescardMap.hasOwnProperty(temp.rechargeMoney)) {
                        var timescards = timescardMap[temp.rechargeMoney];
                        timescards.push(temp);
                    } else {
                        timescardMap[temp.rechargeMoney] = new Array();
                        timescardMap[temp.rechargeMoney].push(temp);
                    }
                } else if (temp.prizeType == 9) {
                    bagInitMoney = temp.rechargeMoney;
                    if (bagMap.hasOwnProperty(temp.rechargeMoney)) {
                        var bags = bagMap[temp.rechargeMoney];
                        bags.push(temp);
                    } else {
                        bagMap[temp.rechargeMoney] = new Array();
                        bagMap[temp.rechargeMoney].push(temp);
                    }
                }
            }
        }
        if (couponInitMoney > 0) {
            couponRule = couponMap[couponInitMoney];
            for (key in couponMap) {
                if (key > couponInitMoney) {
                    couponRule = couponMap[key];
                }
            }
        }
        if (timescardInitMoney >= 0) {
            timescardRule = timescardMap[timescardInitMoney];
            for (key in timescardMap) {
                if (key > timescardInitMoney) {
                    timescardRule = timescardMap[key];
                }
            }
        }
        if (bagInitMoney >= 0) {
            bagRule = bagMap[bagInitMoney];
            for (key in bagMap) {
                if (key > bagInitMoney) {
                    bagRule = bagMap[key];
                }
            }
        }
        if (pointRule != null) {
            if (pointRule.giveType == 1) {
                give_point = pointRule.givePoint;
            } else {
                var count = floor(money / pointRule.rechargeMoney);
                give_point = count * pointRule.givePoint;
            }
        }
        if (expRule != null) {
            if (expRule.giveType == 1) {
                give_exp = expRule.giveExp;
            } else {
                var count = floor(money / expRule.rechargeMoney);
                give_exp = count * expRule.giveExp;
            }
        }
        if (timescardRule != null) {
            for (var i = 0; i < timescardRule.length; i++) {
                var c = timescardRule[i];
                give_timescard = give_timescard + c.giveCardName;
                if (i < timescardRule.length - 1) {
                    give_timescard = give_timescard + '<b> + </b>';
                }
            }
        }
        if (couponRule != null) {
            if (couponRule.length > 0) {
                for (var i = 0; i < couponRule.length; i++) {
                    var c = couponRule[i];
                    give_card = give_card + c.giveCardName + "(" + c.giveCardCount + "张)";
                    if (i < couponRule.length - 1) {
                        give_card = give_card + '<b> + </b>';
                    }
                }
            }
        }
        if (bagRule != null) {
            for (var i = 0; i < bagRule.length; i++) {
                var c = bagRule[i];
                give_bag = give_bag + c.giveGiftBagName;
                if (i < bagRule.length - 1) {
                    give_bag = give_bag + '<b> + </b>';
                }
            }
        }
    }
    if (card_recharge_rules) {
        for (var key in card_recharge_rules) {
            var rule = card_recharge_rules[key];
            if (money >= rule.rechargeMoney) {
                if (rule.fullGiveExp != undefined) {
                    var exp = rule.fullGiveExp * floor(money / rule.rechargeMoney);
                    give_exp = exp + give_exp;
                }
                if (rule.fullGivePoint != undefined) {
                    var point = rule.fullGivePoint * floor(money / rule.rechargeMoney);
                    if (rule.maxIncreaseBonus != null && point > rule.maxIncreaseBonus) {
                        point = rule.maxIncreaseBonus;
                    }
                    give_point = point + give_point;
                }
            }
        }
    }

    if (give_exp != 0 || give_point != 0 || give_card != '' || give_timescard != '' || give_bag != '') {
        var r = '';
        recharge_rules_txt = " 赠送";
        if (give_point != 0) {
            recharge_rules_txt = recharge_rules_txt + " " + give_point + "积分";
            r = '<b> + </b>';
        }
        if (give_exp != 0) {
            recharge_rules_txt = recharge_rules_txt + r + give_exp + "经验";
            r = '<b> + </b>';
        }
        if (give_card != '') {
            recharge_rules_txt = recharge_rules_txt + '<span class="giveCardn">' + r + give_card + "</span>";
        }
        if (give_timescard != '') {
            recharge_rules_txt = recharge_rules_txt + '<span class="giveCardn">' + r + give_timescard + "</span>";
        }
        if (give_bag != '') {
            recharge_rules_txt = recharge_rules_txt + '<span class="giveCardn">' + r + give_bag + "</span>";
        }
        if (recharge_rules_txt == ' 赠送') {
            recharge_rules_txt = '';
        }
    }
    if (recharge_rules_txt != '') {
        $(".checkAllRule").show();
        $(".checkAllRule").html('<i class="ico-jifen-s"></i>' + recharge_rules_txt);
    } else {
        $(".checkAllRule").hide();
    }
}

function msg(msginfo, def) {
    var opt = def || {};
    $('.msglay').remove();
    $('body').stop().append("<div class='msglay'><span>" + msginfo + "</span></div>");
    setTimeout(function () {
        $('.msglay').remove();
        opt.cback && opt.cback();
    }, 5000);
    return false;
}

//去空
function trim(string) {
    var str = string.replace(/\s+/g, "");
    if (str == '') {
        return false;
    } else {
        return str;
    }
}

//只能输入数字
function onlyNum() {
    if (!(event.keyCode == 46) && !(event.keyCode == 8) && !(event.keyCode == 37) && !(event.keyCode == 39))
        if (!((event.keyCode >= 48 && event.keyCode <= 57) || (event.keyCode >= 96 && event.keyCode <= 105)))
            event.returnValue = false;
    $(".recharge-btn").text("充值");
}

//校验值是否正确
function validateValue(obj) {
    var rachargeMoney = trim($(obj).val());
    if (rachargeMoney == false) {
        $("#czbtn").addClass("grayColour");
        return false;
    } else if (/^([1-9]\d*|0)(\.\d*[1-9])?$/.test(rachargeMoney)) {
        $("#czbtn").removeClass("grayColour");
        return true;
    } else {
        $("#czbtn").addClass("grayColour");
        return false;
    }
}

//支付宝网页版调起支付
function aliCallPayV3(data) {
    AlipayJSBridge.call("tradePay", {
        tradeNO: data.tradeNo
    }, function (result) {
        $("#czbtn").removeAttr("auto");
        if (result.resultCode == "9000") {
            window.location.href = data.redirectUrl;
        } else if (result.resultCode == "4000") {
            msg("充值失败,请稍后再试!");
        } else if (result.resultCode == "6002") {
            msg("网络异常!");
        }
    });
}

function callpay(data) {
    var readyPay = function () {
        jsApiCall(data);
    };
    if (typeof WeixinJSBridge == "undefined") {
        if (document.addEventListener) {
            document.addEventListener('WeixinJSBridgeReady', readyPay, false);
        } else if (document.attachEvent) {
            document.attachEvent('WeixinJSBridgeReady', readyPay);
            document.attachEvent('onWeixinJSBridgeReady', readyPay);
        }
    } else {
        jsApiCall(data);
    }
}

//调用微信JS api 支付
function jsApiCall(data) {
    WeixinJSBridge.invoke('getBrandWCPayRequest', {
        "appId": data.appId,
        "timeStamp": data.timeStamp,
        "signType": data.signType,
        "package": data.package,
        "nonceStr": data.nonceStr,
        "paySign": data.paySign
    }, function (res) {
        $("#czbtn").removeAttr("auto");
        if (res.err_msg == "get_brand_wcpay_request:ok") {
            jQuery.ajax({
                type: "POST",
                url: "/ShopCashier/newShopCashierMicro/getClubCoupon.in",
                data: {
                    orderNo: data.orderNo,
                    type: 0
                },
                async: false,
                dataType: "json",
                success: function (datas) {
                    if (datas.code == "S") {
                        addCoupon(datas.couponJson, data.redirectUrl);
                    } else {
                        window.location.href = data.redirectUrl;
                    }
                },
                error: function () {
                    window.location.href = data.redirectUrl;
                }
            });
        }
    });
}

//领取优惠券
function addCoupon(couponJson, redirectUrl) {
    if (typeof (wxJsUtil) != 'undefined') {
        wxJsUtil.bind = function () {
            var merchantId = couponJson.superMerchantId;
            var wechatCardNo = couponJson.couponNo;
            var wechatCardId = couponJson.wechatCardId;
            // 微信领取
            wxJsUtil.addCard(merchantId, wechatCardId, wechatCardNo, function (data) {
                if (data != 'cancel' && data != 'error') {
                    show();
                } else {
                    window.location.href = redirectUrl;
                }
            });
        }
    } else {
        window.location.href = redirectUrl;
    }
}

function floor(num) {
    return Math.floor(Number(num.toFixed(3)));
}
