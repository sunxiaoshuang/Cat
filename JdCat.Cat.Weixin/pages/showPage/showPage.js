const qcloud = require("../../vendor/wafer2-client-sdk/index")
var config = require('../../config')
Page({

  /**
   * 页面的初始数据
   */
  data: {
    
  },

  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function () {
    var self = this;
    qcloud.request({
        url: `/user/business/${config.businessId}`,
        method: "GET",
        success: function (res) {
            var business = res.data;
            self.setData({
              business
            });
        }
    });
  },
  address: function(){
    wx.openLocation({
      latitude: this.data.business.lat,
      longitude: this.data.business.lng,
      name: this.data.business.name,
      address: this.data.business.address
    });
  },
  phone: function(){
    wx.makePhoneCall({
      phoneNumber: this.data.business.mobile
    });
  }
})