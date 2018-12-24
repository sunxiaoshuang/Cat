const qcloud = require("../../../vendor/wafer2-client-sdk/index");
const util = require("../../../utils/util");
Page({

  /**
   * 页面的初始数据
   */
  data: {
    order: {},
    businessId: "",
    timeId: 0,
    remainderTime: 0,
    scene: 0
  },

  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function (options) {
    if (!options) return;
    this.loadData(options.id);

    // 当界面是从模版消息进来时，需要显示返回按钮
    var scene = wx.getStorageSync("scene");
    wx.setStorageSync("scene", 0);
    this.setData({
      scene: scene
    });
  },

  /**
   * 生命周期函数--监听页面初次渲染完成
   */
  onReady: function () {

  },

  /**
   * 生命周期函数--监听页面显示
   */
  onShow: function (a) {
    this.calcTime();
    var refundResult = wx.getStorageSync("refundResult");
    if (refundResult && this.data.order) {
      this.loadData(this.data.order.id);
    }
    wx.removeStorageSync("refundResult");
  },
  loadData: function (id) {
    var order = wx.getStorageSync("orderDetail"),
      self = this;
    var business = qcloud.getSession().business;
    if (order) {
      this.setData({
        order: order,
        businessId: business.id
      });
      wx.removeStorageSync("orderDetail");
      return;
    }

    qcloud.request({
      url: "/order/single/" + id,
      method: "GET",
      success: function (res) {
        self.setData({
          order: res.data,
          businessId: business.id
        });
        self.calcTime();
      }
    });
  },
  onPullDownRefresh: function () {
    this.loadData(this.data.order.id);
    wx.stopPullDownRefresh();
  },
  calcTime: function () {
    // 每次显示界面，重新计算剩余时间
    if (!this.data.order) return;
    clearInterval(this.data.timeId);
    if (this.data.order.status != 256) return;
    var overSecond = Math.floor(+/\d+/g.exec(this.data.order.createTime)[0] / 1000) + 60 * 15;
    var nowSecond = Date.parse(new Date()) / 1000;
    var remainderTime = overSecond - nowSecond;
    var self = this;
    var timeId = setInterval(function () {
      remainderTime--;
      if (remainderTime < 0) { // 支付超时
        clearInterval(self.data.timeId);
        self.onLoad();
      }
      self.setData({
        remainderTime: remainderTime
      });
    }, 1000);
    this.setData({
      timeId: timeId,
      remainderTime: remainderTime
    });
  },
  sure: function (e) {
    wx.navigateTo({
      url: "/pages/pay/sure/sure?id=" + this.data.order.id
    });
  },
  refund: function () {
    wx.setStorageSync("refundOrder", this.data.order);
    wx.navigateTo({
      url: "/pages/order/refund/refund"
    });
  },

  copy: function () {
    wx.setClipboardData({
      data: this.data.order.orderCode,
      success: function () {
        util.showSuccess("复制成功");
      }
    });
  },
  cancel: function () {
    wx.switchTab({
      url: '/pages/main/main'
    });
  }
})