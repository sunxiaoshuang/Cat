// pages/userinfo/userinfo.js
let videoAd = null

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
    // if (wx.createRewardedVideoAd) {
    //   videoAd = wx.createRewardedVideoAd({
    //     adUnitId: 'adunit-acda8ee29bef6c81'
    //   })
    //   videoAd.onLoad(() => {})
    //   videoAd.onError((err) => {})
    //   videoAd.onClose((res) => {})
    // }
  
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
    // if (videoAd) {
    //   videoAd.show().catch(() => {
    //     // 失败重试
    //     videoAd.load()
    //       .then(() => videoAd.show())
    //       .catch(err => {
    //         console.log('激励视频 广告显示失败')
    //       })
    //   })
    // }
  },

  /**
   * 生命周期函数--监听页面隐藏
   */
  onHide: function () {
  
  },

  /**
   * 生命周期函数--监听页面卸载
   */
  onUnload: function () {
  
  },

  /**
   * 页面相关事件处理函数--监听用户下拉动作
   */
  onPullDownRefresh: function () {
  
  },

  /**
   * 页面上拉触底事件的处理函数
   */
  onReachBottom: function () {
  
  },

  /**
   * 用户点击右上角分享
   */
  onShareAppMessage: function () {
  
  }
})