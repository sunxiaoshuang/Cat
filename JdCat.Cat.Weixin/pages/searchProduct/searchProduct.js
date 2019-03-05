const qcloud = require("../../vendor/wafer2-client-sdk/index");
const util = require("../../utils/util");
Page({

  /**
   * 页面的初始数据
   */
  data: {
    products: [],
    historys: [],
    businessId: ""
  },

  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function (options) {
    this.data.carts = wx.getStorageSync("cartList");
    this.data.productList = wx.getStorageSync("productList");
    this.data.historys = wx.getStorageSync("historys") || [];
    var businessId = qcloud.getSession().business.id;
    
    this.setData({
      historys: this.data.historys,
      businessId
    });
  },

  onReady: function () {

  },

  onShow: function () {

  },

  inputkey: function(a,b,c){
    this.data.key = a.detail.value;
    
    util.method.throttle(this.search, 200, this);
  },
  search: function(){
    var key = this.data.key, products;
    if(!key) {
      products = [];
    }
    else {
      products = this.data.productList.filter(element => {
        return element.name.indexOf(key) > -1;
      });

      // 搜索有结果的关键字，保存为历史关键字
      if(products.length > 0){
        var exist = this.data.historys.some(item => {
          return item === key;
        });
        if(!exist) {
          this.data.historys.unshift(key);
          if(this.data.historys.length > 10){
            this.data.historys.length = 10;
          }
          wx.setStorageSync("historys", this.data.historys);
          this.setData({
            historys: this.data.historys
          });
        }
      }
    }

    this.setData({
      key: key,
      products: products
    });
  },
  historySearch: function(e){
    this.data.key = e.currentTarget.dataset.key;
    this.search();
  },
  clear: function(){
    this.setData({
      key: "",
      products: []
    });
  },
  remove: function(){
    this.data.historys = [];
    this.data.key = "";
    wx.removeStorageSync("historys");
    this.setData({
      historys: []
    });
  },
  selectProduct: function(e){
    var product = e.currentTarget.dataset.product;
    wx.setStorageSync("selectProduct", product);
    wx.setStorageSync("currentScene", "search-product");
    wx.navigateBack({
      delta: 1
    });
  }
})