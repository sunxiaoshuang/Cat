const qcloud = require("../../vendor/wafer2-client-sdk/index");
const util = require("../../utils/util");

Page({
  data: {
    address: "",
    cartList: [],
    freight: 0,
    total: 0,
    remark: "",
    tablewareQuantity: 0,
    tablewareQuantitys: [1,2,3,4,5,6,7,8,9,10]
  },
  onLoad: function () {
    var cartList = wx.getStorageSync('cartList') || [],
      total = 0, tablewareQuantity = 0,
      freight = qcloud.getSession().business.freight;
    cartList.forEach(a => {
      total += a.price * a.quantity;
      tablewareQuantity += a.packingQuantity * a.quantity;
    });
    var saleFullReduce = wx.getStorageSync("nowFullReduce"), isSaleFullReduce = true;
    if(!saleFullReduce) {
      saleFullReduce = {};
      isSaleFullReduce = false;
      total = qcloud.utils.getNumber(total + freight);
    } else {
      total = qcloud.utils.getNumber(total - saleFullReduce.reduceMoney + freight, 2);
    }
    this.setData({
      cartList: cartList,
      freight: freight,
      total: total,
      tablewareQuantity: tablewareQuantity,
      saleFullReduce: saleFullReduce,
      isSaleFullReduce: isSaleFullReduce
    });
  },
  onShow: function () {
    var address = wx.getStorageSync("selectAddress");
    if (address) {
      this.setData({
        address: address
      });
    }
  },
  selectAddress: function () {
    wx.navigateTo({
      url: "/pages/address/list/list?flag=select"
    });
  },
  blurRemark: function(e){
    this.data.remark = e.detail.value;
  },
  bindPickerChange: function(e) {
    this.setData({
      tablewareQuantity: this.data.tablewareQuantitys[e.detail.value]
    });
  },
  submit: function () {
    if (!this.data.address) {
      util.showError("请选择收货地址");
      return;
    }
    util.showBusy("loading");
    var self = this, business = qcloud.getSession().business, user = qcloud.getSession().userinfo;
    var order = {
      price: this.data.total,
      freight: this.data.freight,
      receiverName: this.data.address.receiver,
      receiverAddress: this.data.address.mapInfo + " " + this.data.address.detailInfo,
      lat: this.data.address.lat,
      lng: this.data.address.lng,
      phone: this.data.address.phone,
      remark: this.data.remark,
      tablewareQuantity: this.data.tablewareQuantity,
      cityCode: business.cityCode,
      userId: user.id,
      businessId: business.id,
      saleFullReduceId: this.data.saleFullReduce.id,
      products: []
    };
    this.data.cartList.forEach(a => {
      order.products.push({
        name: a.name,
        quantity: a.quantity,
        price: a.price,
        src: a.src,
        description: a.description,
        productId: a.productId,
        formatId: a.formatId
      });
    });
    qcloud.request({
      url: `/order/createOrder`,
      method: "POST",
      data: order,
      success: function (res) {
        if(res.data.success){
          self.data.cartList = [];
          wx.setStorageSync('cartList', []);
          wx.setStorageSync("orderDetail", res.data.data);
          wx.redirectTo({
            url: '/pages/pay/sure/sure'
          });
        } else {
          util.showError(res.data.msg);
        }
      },
      fail: function (err) {
        util.showModel("错误", "请检查网络连接");
      }
    });
  }

});
