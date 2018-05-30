// pages/user/user.js
const config = require("../../config");
const util = require("../../utils/util");
Page({
  data: {
    
  },
  onReady: function () {
  
  },
  onShow: function () {
    var app = getApp();
    this.setData({
      userInfo: app.globalData.userInfo
    });
  },
  onHide: function () {
  
  },
  onUnload: function () {
  
  },
  bindGetUserInfo: function(e){
    var self = this;
    var app = getApp();
    wx.getSetting({           // 如果用户允许授权，则将用户信息写入数据库
      success: function(res){
        if(!res.authSetting["scope.userInfo"])return;
        util.showBusy("正在登录...");
        var userinfo = e.detail.userInfo;
        userinfo.id = app.globalData.userInfo.id
        wx.request({
          url: config.service.requestUrl + "/user/info",
          method: "put",
          data: userinfo,
          success: function(res){
            wx.hideToast();
            self.setData({
              userInfo: res.data
            });
            app.globalData.userInfo = res.data;
            util.showSuccess("登录成功");
          }
        });
      }
    });
  }
})