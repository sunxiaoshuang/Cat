const qcloud = require("../../vendor/wafer2-client-sdk/index");
Page({

  /**
   * 页面的初始数据
   */
  data: {
    coupon: {},
    costAmount: 0
  },

  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function () {
    var obj = wx.getStorageSync("receviceReturnCoupon"),
      user = qcloud.getSession().userinfo,
      self = this;
    var coupon = {
      name: obj.name,
      value: obj.value,
      minConsume: obj.minConsume,
      startDate: obj.startDate,
      endDate: obj.endDate,
      validDay: obj.validDay,
      userId: user.id,
      returnCouponId: obj.id,
      status: 1
    };
    qcloud.request({
      url: '/user/receiveReturnCoupon',
      method: 'POST',
      data: [coupon],
      success: function ({
        data
      }) {
        var entity = data.data[0];
        self.setData({
          coupon: entity,
          costAmount: obj.costAmount
        });
        var coupons = wx.getStorageSync("myCoupon") || [];
        coupons.unshift(entity);
        wx.setStorageSync("myCoupon", coupons);
      }
    });
  },
  toCoupon: function () {
    wx.redirectTo({
      url: '/pages/user/mycoupon/mycoupon'
    });
  }
})