const config = require("../../../config");
const util = require("../../../utils/util");
var qcloud = require('../../../vendor/wafer2-client-sdk/index');

Page({

  /**
   * 页面的初始数据
   */
  data: {
    comments: []
  },

  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function (options) {
    var self = this;
    qcloud.request({
      url: "/user/comments/" + qcloud.getSession().userinfo.id,
      method: "GET",
      success: function(res){
        self.setData({
          comments: res.data
        })
      },
      fail: function(err){
        util.showError(err);
      }
    });
  },
  cat: function(e){
    var comment = e.currentTarget.dataset.comment;
    wx.navigateTo({
      url: "/pages/order/orderInfo/orderInfo?id=" + comment.orderId
    });
  }

})