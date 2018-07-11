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
    remainderTime: 0

  },

  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function (options) {
    var order = wx.getStorageSync("orderDetail");
    var business = qcloud.getSession().business;
    this.setData({
      order: order,
      businessId: business.id
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
  onShow: function () {
    // 每次显示界面，重新计算剩余时间
    clearInterval(this.data.timeId);
    var overSecond = Math.floor(+/\d+/g.exec(this.data.order.createTime)[0] / 1000) + 60 * 300;
    var nowSecond = Date.parse(new Date()) / 1000;
    var remainderTime = overSecond - nowSecond;
    var self = this;
    var timeId = setInterval(function () {
      remainderTime--;
      if (remainderTime < 0) { // 支付超时
        clearInterval(self.data.timeId);

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
  submit: function () {
    util.showBusy("loading");
    var user = qcloud.getSession().userinfo;
    qcloud.request({
      url: `/order/unifiePayment/${this.data.order.id}?businessId=${this.data.businessId}&userId=${user.id}`,
      success: function (res) {
        wx.hideToast();
        if (res.data.success) {
          wx.requestPayment(qcloud.utils.extend({}, res.data.data, {
            success: function (res) {
              console.log(res);
            },
            fail: function(err){
              util.showError(err);
            }
          }));

        } else {
          util.showError(res.data.msg);
        }
      }
    });
  }
})