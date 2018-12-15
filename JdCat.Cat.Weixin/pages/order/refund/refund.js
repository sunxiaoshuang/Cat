const util = require("../../../utils/util");
const qcloud = require("../../../vendor/wafer2-client-sdk/index");

Page({

  /**
   * 页面的初始数据
   */
  data: {
    reasons: ["订单长时间没有配送", "订单信息错误", "订单地址错误", "不想要了", "其他"],
    index: 0,
    other: ""
  },

  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function (options) {

  },
  bindPickerChange(e) {
    this.setData({
      index: e.detail.value
    });
  },
  bindTextAreaInput(e) {
    this.data.reason = e.detail.value;
  },
  sure(){
    var reason = this.data.reasons[this.data.index];
    if(this.data.index === this.data.reasons.length - 1) {
      reason = this.data.reason;
    }
    if(!reason) {
      util.showModel("提示", "请输入取消原因");
      return;
    }
    var order = wx.getStorageSync("refundOrder");
    if(!order) {
      util.showModel("提示", '申请失败，请退出后重试');
      return;
    }
    qcloud.request({
      url: `/order/applyRefund/${order.id}?reason=${reason}`,
      method: "GET",
      success: function (res) {
        
      }
    });
  }
})