const qcloud = require("../../../vendor/wafer2-client-sdk/index");
Page({

  /**
   * 页面的初始数据
   */
  data: {
  
  },

  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function (options) {
    wx.setNavigationBarTitle({
      title: "商家资质"
    });
    var business = qcloud.getSession().business;
    this.setData({
      business: business
    });
  }
})