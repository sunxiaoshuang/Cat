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
            url: "/pages/user/user"
          });
        }, 100);
      }
    });
  },
})