var couponStatus = {
  "1": "未使用",
  "2": "已使用",
  "3": "已过期"
};
Page({

  data: {
    nav: [],
    pageIndex: 1,
    toView: 1
  },

  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function (options) {
    wx.setNavigationBarTitle({
      title: "我的优惠券"
    });
    var myCoupon = wx.getStorageSync("myCoupon");
    var notUse = myCoupon.filter(a => a.status == 1);
    var used = myCoupon.filter(a => a.status == 2);
    var expire = myCoupon.filter(a => a.status == 3);
    this.setData({
      nav: [
        {name: "未使用", sort: 1, num: notUse.length},
        {name: "已使用", sort: 2, num: used.length},
        {name: "已过期", sort: 3, num: expire.length}
      ],
      notUse, used, expire
    });
    console.log(notUse);
  },
  turnPage: function (e) {
    this.setData({
      pageIndex: e.currentTarget.dataset.index
    });
  },
  change: function (e) {
    this.setData({
      pageIndex: e.detail.currentItemId
    });
  },
  use: function(){
    wx.switchTab({
      url: "/pages/main/main"
    });
  }
});