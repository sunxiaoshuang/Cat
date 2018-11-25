const config = require("../../config");
const util = require("../../utils/util");
var qcloud = require('../../vendor/wafer2-client-sdk/index');
Page({
  data: {
    userInfo: {},
    animationData: "",
    isShowPhoneWindow: false
  },
  onLoad: function () {
    var user = qcloud.getSession().userinfo;
    this.setData({
      userInfo: user
    });
    if(user.isRegister && !user.isPhone){
      this.bindPhone();
    }
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
              var session = qcloud.getSession();
              session.userinfo = res.data.userinfo;
              qcloud.setSession(session);
              self.setData({
                userInfo: res.data.userinfo
              });
              
              self.bindPhone();

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
            userInfo: self.data.userInfo,
            isShowPhoneWindow: false
          });
          var session = qcloud.getSession();
          session.userinfo = self.data.userInfo;
          qcloud.setSession(session);
          util.showSuccess("绑定成功");
          wx.showTabBar();
          wx.switchTab({
            url: "/pages/main/main"
          });
        } else {
          util.showModel("提示", res.data.message);
        }
      },
      fail: function(error){
        util.showModel("错误", "绑定失败，请检查网络连接");
      }
    });
  },
  bindPhone: function(){
    var self = this;
    wx.hideTabBar();
    var animation = wx.createAnimation({
      duration: 200,
      timingFunction: "ease",
      delay: 0
    });

    animation.scale(0.01, 0.01).step();

    this.setData({
      isShowPhoneWindow: true,
      animationData: animation.export()
    });
    setTimeout(function () {
      animation.scale(1, 1).step();
      self.setData({
        animationData: animation.export()
      });
    }, 100);
  },
  callPhone: function(){
    var business = qcloud.getSession().business;
    wx.makePhoneCall({
      phoneNumber: business.mobile
    });
  }
});