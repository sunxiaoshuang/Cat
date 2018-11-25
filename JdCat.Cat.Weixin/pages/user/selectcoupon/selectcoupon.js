// pages/user/selectcoupon/selectcoupon.js
Page({

  /**
   * 页面的初始数据
   */
  data: {
    noChecked: true
  },

  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function (options) {
    var total = parseFloat(options.total), self = this;
    
    var myCoupon = wx.getStorageSync("myCoupon");
    var notUse = myCoupon.filter(a => {
      if(a.status != 1) return false;
      if(total >= a.minConsume) return true;
      return false;
    });
    notUse.forEach(a => {
      if(a.id == options.id) {
        a.checked = true;
        self.data.noChecked = false;
      } else {
        a.checked = false;
      }
    });
    this.setData({
      coupons: notUse,
      quantity: notUse.length,
      noChecked: self.data.noChecked
    });
  },
  noSelect: function(){
    wx.navigateBack({
      delta: 1
    });
  },
  select: function(e){
    var coupon = this.data.coupons[e.currentTarget.dataset.index];
    wx.setStorageSync("selectCoupon", coupon);
    wx.navigateBack({
      delta: 1
    });
  }

})