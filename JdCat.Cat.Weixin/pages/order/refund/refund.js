const util = require("../../../utils/util");
const qcloud = require("../../../vendor/wafer2-client-sdk/index");

Page({

  /**
   * 页面的初始数据
   */
  data: {
    reasons: ["请选择", "订单长时间没有配送", "订单信息错误", "订单地址错误", "不想要了", "其他"],
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
    this.data.other = e.detail.value;
  },
  sure(){
    if(this.data.index == 0) {
      util.showError("请选择退款原因");
      return;
    }
    var reason = this.data.reasons[this.data.index];
    if(this.data.index == (this.data.reasons.length - 1)) {
      reason = this.data.other;
    }
    if(!reason) {
      util.showError("请输入取消原因");
      return;
    }
    var order = wx.getStorageSync("refundOrder");
    if(!order) {
      util.showError('申请失败，请退出后重试');
      return;
    }
    util.showBusy("请稍等...");
    qcloud.request({
      url: `/order/applyRefund/${order.id}?reason=${reason}`,
      method: "GET",
      success: function (res) {
        wx.hideToast();
        if(res.data.success) {
          util.showSuccess("申请成功");
          wx.setStorageSync("refundResult", true);
          setTimeout(() => wx.navigateBack({
            delta: 1
          }), 1500);
        } else {
          util.showError(res.data.msg);
        }
      }
    });
  }
})