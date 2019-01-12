const util = require("../../utils/util");
const config = require('../../config');
const qcloud = require("../../vendor/wafer2-client-sdk/index");
Page({
  data: {
    openText: ""
  },
  onLoad: function () {
    this.setData({
      openText: config.globalData.openText
    });
    // util.showBusy("Loading");
    qcloud.login({
      data: {
        businessId: config.businessId
      },
      success: function () {
        qcloud.request({
          url: `/user/business/${config.businessId}`,
          method: "GET",
          success: function (res) {
            var obj = qcloud.getSession();
            var business = res.data;
            obj.reload = true;
            if (business.category === 1) {
              obj.business = null;
              obj.chain = business;
              qcloud.setSession(obj);
              wx.redirectTo({
                url: "/pages/chain/select/select"
              });
            } else {
              obj.business = business;
              qcloud.setSession(obj);
              wx.switchTab({
                url: "/pages/main/main"
              });
            }
          }
        });
      }
    });
  }

})