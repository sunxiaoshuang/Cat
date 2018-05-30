const util = require("../../utils/util");
const config = require('../../config');
Page({
  onReady: function(){
    util.showBusy("Loading");
    var app = getApp();
    var self = this;
    wx.login({
        success: function(e){
            wx.request({
                url: config.service.loginUrl,
                data: {
                    code: e.code,
                    businessId: app.globalData.businessId
                },
                success: function(res){
                  app.globalData.userInfo = res.data;
                  setTimeout(() => {
                    wx.switchTab({
                      url: "/pages/user/user"
                    });
                  }, 100);
                }
            });
        },
        fail: function(err){
            util.showModel('错误', "请检查网络连接");
        }
    });
  },
})