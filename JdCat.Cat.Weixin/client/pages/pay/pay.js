Page({
  data: {
    address: [],
    products:[],
  },
  onShow: function () {
    var addresses = (wx.getStorageSync('addresses') || []);
    var products = (wx.getStorageSync('products') || []);
    this.setData({
      address:addresses[0],
      products:products
    })
  }

});