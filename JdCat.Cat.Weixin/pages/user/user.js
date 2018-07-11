const config = require("../../config");
const util = require("../../utils/util");
var qcloud = require('../../vendor/wafer2-client-sdk/index');
Page({
  data: {
    userInfo: {}
  },
  onLoad: function () {
    this.setData({
      userInfo: qcloud.getSession().userinfo
    });
  },
  bindGetUserInfo: function (e) {
    var self = this;
    wx.getSetting({ // 如果用户允许授权，则将用户信息写入数据库
      success: function (res) {
        if (!res.authSetting["scope.userInfo"]) return;
        util.showBusy("加载中...");
        var userinfo = e.detail.userInfo;
        userinfo.id = self.data.userInfo.id;
        userinfo.skey = self.data.userInfo.skey;
        qcloud.request({
          url: config.service.requestUrl + "/user/info",
          data: userinfo,
          method: "put",
          success: function (res) {
            wx.hideToast();
            if(res.data.code === 0){
              res = res.data;
              qcloud.setSession(res.data);
              self.setData({
                userInfo: res.data.userinfo
              });
            } else {
              util.showModel("授权失败", "请检查网络连接");
            }
          },
          fail: function (error) {
            util.showModel("登录失败", error);
          }
        });
      }
    });
  },
  bindGetPhoneNumber: function (e) {
    if (e.detail.errMsg != "getPhoneNumber:ok") return;
    var self = this;
    var encrytedData = e.detail.encryptedData;
    var iv = e.detail.iv;
    util.showBusy("加载中...");
    qcloud.request({
      url: config.service.requestUrl + "/user/phone",
      method: "put",
      login: true,      // 需要登录
      data: {
        encrytedData: encrytedData,
        iv: iv
      },
      success: function (res) {
        wx.hideToast();
        if (res.data.code == 0) {
          self.data.userInfo.phone = res.data.data;
          self.data.userInfo.isPhone = true;
          self.setData({
            userInfo: self.data.userInfo
          });
          qcloud.setSession(self.data.userInfo);
          util.showSuccess("绑定成功");
        } else {
          util.showModel("提示", res.data.message);
        }
      },
      fail: function(error){
        util.showModel("错误", "绑定失败，请检查网络连接");
      }
    });
  }
})