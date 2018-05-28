//app.js
var qcloud = require('./vendor/wafer2-client-sdk/index');
var config = require('./config');
var util = require("./utils/util");

App({
    onLaunch: function () {
        // 程序启动执行一次
        // qcloud.setLoginUrl(config.service.loginUrl);
        var self = this;
        wx.login({
            success: function(e){
              console.log(e);
                wx.request({
                    url: config.service.loginUrl,
                    data: {
                        code: e.code,
                        businessId: self.globalData.businessId
                    },
                    success: function(res){
                        self.globalData.userInfo = res.data;
                        self.globalData.logged = res.data.isRegister;
                    }
                });
            },
            fail: function(err){
                util.showModel('错误', "请检查网络连接");
            }
        });
    },
    onShow: function(){
        // 从后台进入前台执行

    },
    onHide: function(){
        // 从前台进入后台执行

    },
    onError: function(){
        // 当小程序发生任何错误时执行

    },
    globalData: {
        // 全局数据
        userInfo: undefined,
        logged: false,
        businessId: 1
    }
});