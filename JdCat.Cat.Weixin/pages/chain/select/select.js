const qcloud = require("../../../vendor/wafer2-client-sdk/index");
const config = require("../../../config");
const util = require("../../../utils/util");

Page({

  data: {

  },
  onLoad: function (options) {

  },

  onShow: function () {
    util.showBusy("正在定位"); 
    setTimeout(function () {
      wx.getLocation({
        type: "gcj02",
        success: function (res) {
          var location = res.latitude + "," + res.longitude;
          qcloud.request({
            url: `/app/getlocations/${location}`,
            method: "GET",
            success: function (res) {
              if (res.data.status != 0) {
                // 定位失败
                wx.removeStorageSync("curLocation");
              } else {
                wx.setStorageSync("curLocation", res.data.result);
              }
              // 根据经纬度，自动读取距离最近的门店进入
              qcloud.request({
                url: `/app/getNearestStore/${config.businessId}?location=${location}`,
                method: "GET",
                success: function (res) {
                  if (!res.data.data) {
                    util.showError("附近未找到门店，请重新选择地址");
                    return;
                  }
                  var session = qcloud.getSession();
                  session.business = res.data.data;
                  qcloud.setSession(session);
                  wx.switchTab({
                    url: "/pages/main/main"
                  });
                },
                complete: () => wx.hideToast()
              });

            }
          });

        },
        fail: function () {
          wx.hideToast();
          wx.showModal({
            title: "提示",
            content: "为了向您提供更好的服务，请允许我们获得您的地理位置",
            showCancel: false,
            confirmText: "去设置",
            success: function () {
              wx.openSetting();
            }
          });
        }
      });
    }, 1000);
  },
  getNearest: function () {

  }

});