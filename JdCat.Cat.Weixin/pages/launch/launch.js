const util = require("../../utils/util");
const config = require('../../config');
const qcloud = require("../../vendor/wafer2-client-sdk/index");
Page({
  onLoad: function () {
    wx.setNavigationBarTitle({
      title: "正在启动"
    });
    util.showBusy("Loading");
    qcloud.login({
      data: {
        businessId: config.businessId
      },
      success: function (userinfo) {
        // setTimeout(() => {


          qcloud.request({
            url: `/user/business/${config.businessId}`,
            method: "GET",
            success: function (res) {
              var business = res.data;

              if(business.isPublish) {
                wx.switchTab({
                  url: "/pages/main/main"
                });
              } else {
                wx.redirectTo({
                  url: "/pages/showPage/showPage"
                });
              }

            }
          });
          // wx.navigateTo({
          //   url: "/pages/showPage/showPage"
          // });
        // }, 100);
      }
    });
  },
})