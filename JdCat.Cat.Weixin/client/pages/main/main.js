var config = require('../../config');
Page({
  onLoad: function(){
  },
  onShow: function () {
    var app = getApp();
    this.setData({
      userInfo: app.globalData.userInfo,
      logged: true
    });
  },
  data: {
    userInfo: undefined,
    logged: false
  },
  bindGetUserInfo: function (e) {
    var self = this;
    var app = getApp();
    wx.getSetting({
      success: function(res){
        var userinfo = e.detail.userInfo;
        userinfo.id = app.globalData.userInfo.id
        wx.request({
          url: config.service.requestUrl + "/user/info",
          method: "put",
          data: userinfo,
          success: function(res){
            self.setData({
              userInfo: res.data,
              logged: true
            });
          }
        });
      }
    });
  }
});