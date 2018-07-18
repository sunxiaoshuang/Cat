const util = require("../../utils/util");
const config = require('../../config');
const qcloud = require("../../vendor/wafer2-client-sdk/index");
Page({
  onLoad: function () {
    util.showBusy("Loading");
    qcloud.login({
      data: {
        businessId: config.businessId
      },
      success: function (userinfo) {
        setTimeout(() => {
          wx.switchTab({
            url: "/pages/main/main"
          });
          // wx.navigateTo({
          //   url: "/pages/test/test"
          // });
        }, 100);
      }
    });
  },
})