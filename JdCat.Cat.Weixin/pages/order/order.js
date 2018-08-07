const qcloud = require("../../vendor/wafer2-client-sdk/index");
const util = require("../../utils/util");

Page({

  /**
   * 页面的初始数据
   */
  data: {
    query: {
      pageIndex: 1,
      pageSize: 10
    },
    list: [], // 订单列表
    isEnd: false // 订单列表是否已经全部加载完成
  },

  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function (options) {
    wx.setNavigationBarTitle({
      title: "我的订单"
    });
    this.loadData(function (list) {
      this.setData({
        list: list
      });
    });
  },

  /**
   * 页面相关事件处理函数--监听用户下拉动作
   */
  onPullDownRefresh: function () {
    // 下拉刷新后，重新加载列表
    this.data.query = {
      pageIndex: 1,
      pageSize: 10
    };
    this.setData({
      query: this.data.query
    });
    this.loadData(function (list) {
      this.setData({
        list: list
      });
    }, function () {
      wx.stopPullDownRefresh();
    });
  },

  /**
   * 页面上拉触底事件的处理函数
   */
  onReachBottom: function () {
    if (this.data.isEnd) return;
    var self = this;
    this.data.query.pageIndex++;
    this.loadData(function (list) {
      if (list.length === 0) return;
      var arr = [];
      list.forEach(function (order) {
        if (self.data.list.some(function (obj) {
            return obj.id == order.id
          })) return; // 如果原列表中已经存在，则不加载
        arr.push(order);
      });
      arr.forEach(function (order) {
        self.data.list.push(order);
      });
      this.setData({
        list: this.data.list
      });
    });
  },
  loadData: function (callback1, callback2) {
    var self = this,
      session = qcloud.getSession();
    qcloud.request({
      url: `/order/getOrder/${session.userinfo.id}?businessId=${session.business.id}`,
      data: this.data.query,
      method: "POST",
      success: function (res) {
        var res = res.data,
          isEnd = false;
        if (res.success) {
          if (res.data.rows === 0 || Math.ceil(res.data.rows / self.data.query.pageSize) === self.data.query.pageIndex) {
            isEnd = true;
          }
          self.setData({
            isEnd: isEnd
          });
          callback1.call(self, res.data.list);
        } else {
          util.showError(res.data.msg);
        }
        callback2 && callback2.call(self);
      }
    })
  },
  navigate: function (e) {
    var order = this.data.list[e.currentTarget.dataset.index];
    // wx.setStorageSync("orderDetail", order);
    wx.navigateTo({
      url: "/pages/order/orderInfo/orderInfo?id=" + order.id
    });
  },
  mainPage: function(){
    wx.switchTab({
      url: "/pages/main/main"
    });
  },
  sure: function(e){
    var order = this.data.list[e.currentTarget.dataset.index];
    wx.navigateTo({
      url: "/pages/pay/sure/sure?id=" + order.id
    });
  }
})