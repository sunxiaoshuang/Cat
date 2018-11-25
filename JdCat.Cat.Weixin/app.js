var qcloud = require('./vendor/wafer2-client-sdk/index');
var config = require('./config');
var util = require("./utils/util");
// var isFirstEnter = true;

App({
    onLaunch: function () {
        // 程序启动执行一次
        qcloud.setLoginUrl(config.service.loginUrl);
    },
    onShow: function (option) {
        // 从后台进入前台执行
        // 每次进入首页，重新加载商户信息
        // if (isFirstEnter) {
        //     isFirstEnter = false;
        // } else {
        //     qcloud.request({
        //         url: `/user/business/${config.businessId}`,
        //         method: "GET",
        //         success: function (res) {
        //             var session = qcloud.getSession();
        //             if (!session) return;
        //             session.business = res.data;
        //             qcloud.setSession(session);
        //         }
        //     });
        // }
        // 记录进入小程序的模式
        wx.setStorageSync("scene", option.scene);
    
    },
    onHide: function () {
        // 从前台进入后台执行

    },
    onError: function () {
        // 当小程序发生任何错误时执行

    }
    // globalData: {
    //     // 全局数据
    //     userInfo: {},
    //     businessId: 1,
    //     header: {

    //     }
    // }
});