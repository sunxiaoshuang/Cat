Page({
  data: {
    address: []
  },
  onShow: function () {
    var addresses = (wx.getStorageSync('addresses') || []);
    this.setData({
      address:addresses[0]
    })
  }

});