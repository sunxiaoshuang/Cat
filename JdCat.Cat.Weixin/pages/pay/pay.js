const qcloud = require("../../vendor/wafer2-client-sdk/index");
const util = require("../../utils/util");

Page({
  data: {
    address: "",
    cartList: [],
    freight: 0,
    total: 0,
    oldPrice: 0,
    remark: "",
    tablewareQuantity: 0,
    tablewareQuantitys: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10],
    couponQuantity: 0,
    coupon: {}
  },
  onLoad: function () {
    wx.setNavigationBarTitle({
      title: "订单下单"
    });
    var cartList = wx.getStorageSync('cartList') || [],
      total = 0,
      oldPrice = 0,
      tablewareQuantity = 0,
      freight = qcloud.getSession().business.freight;
    cartList.forEach(a => {
      oldPrice += a.price * a.quantity;
      tablewareQuantity += a.packingQuantity * a.quantity;
    });
    oldPrice = qcloud.utils.getNumber(oldPrice + freight, 2);
    var saleFullReduce = wx.getStorageSync("nowFullReduce"),
      isSaleFullReduce = true;
    if (!saleFullReduce) {
      saleFullReduce = {};
      isSaleFullReduce = false;
      total = oldPrice;
    } else {
      total = qcloud.utils.getNumber(oldPrice - saleFullReduce.reduceMoney, 2);
    }

    if(total < 0)total = 0;
    // 优惠券
    var myCoupon = wx.getStorageSync("myCoupon");
    var notUse = myCoupon.filter(a => {
      if (a.status != 1) return false;
      if (oldPrice >= a.minConsume) return true;
      return false;
    });

    this.setData({
      cartList: cartList,
      freight: freight,
      total: total,
      oldPrice: oldPrice,
      tablewareQuantity: tablewareQuantity,
      saleFullReduce: saleFullReduce,
      isSaleFullReduce: isSaleFullReduce,
      couponQuantity: notUse.length
    });
  },
  onShow: function () {
    var address = wx.getStorageSync("selectAddress") || "";
    // 优惠券
    var coupon = wx.getStorageSync("selectCoupon") || "";
    wx.removeStorageSync("selectCoupon");
    
    var oldPrice = this.data.oldPrice, fullReducePrice = this.data.saleFullReduce.reduceMoney || 0;
    var total = qcloud.utils.getNumber(oldPrice - fullReducePrice - (coupon.value || 0), 2);
    if(total < 0)total = 0;
    this.setData({
      address,
      coupon,
      total
    });
  },
  selectAddress: function () {
    wx.navigateTo({
      url: "/pages/address/list/list?flag=select"
    });
  },
  selectCoupon: function () {
    wx.navigateTo({
      url: `/pages/user/selectcoupon/selectcoupon?total=${this.data.oldPrice}&id=${this.data.coupon.id}`
    });
  },
  blurRemark: function (e) {
    this.data.remark = e.detail.value;
  },
  bindPickerChange: function (e) {
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
    var self = this,
      business = qcloud.getSession().business,
      user = qcloud.getSession().userinfo;
    var order = {
      price: this.data.total,
      oldPrice: this.data.oldPrice,
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
      saleCouponUserId: this.data.coupon.id,
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
        if (res.data.success) {
          self.data.cartList = [];
          wx.setStorageSync('cartList', []);
          wx.setStorageSync("orderDetail", res.data.data);
          self.useCoupon(order.saleCouponUserId);
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
  },
  useCoupon: function(id){
    if(!id)return;
    var myCoupon = wx.getStorageSync("myCoupon");
    myCoupon.forEach(a => {
      if(a.id == id) {
        a.status = 2;
      }
    });
    wx.setStorageSync("myCoupon", myCoupon);
  }

});