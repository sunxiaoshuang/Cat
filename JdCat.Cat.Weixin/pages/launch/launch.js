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
            var obj = qcloud.getSession();
            obj.business = res.data;
            qcloud.setSession(obj);
            wx.switchTab({
              url: "/pages/main/main"
            });

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