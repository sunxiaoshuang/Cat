const qcloud = require("../../vendor/wafer2-client-sdk/index");
const util = require("../../utils/util");

Page({
  data: {
    address: {},
    cartList: [],
    freight: 0,
    total: 0,
    oldPrice: 0,
    remark: "",
    productCost: 0,
    tablewareQuantity: 0,
    tablewareQuantitys: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10],
    couponQuantity: 0,
    coupon: {},
    saleFullReduce: {}
  },
  onLoad: function () {
    wx.setNavigationBarTitle({
      title: "订单下单"
    });
    /** 页面加载时操作：
     * 1. 查询购物车信息
     * 2. 计算购物车总价格，餐盒数
     * 3. 计算出可使用的优惠券
     * 4. 初始化配送费
     */
    var cartList = wx.getStorageSync('cartList') || [],
      business = qcloud.getSession().business,
      productCost = 0,
      tablewareQuantity = 0;
    cartList.forEach(a => {
      productCost += a.price * a.quantity;
      tablewareQuantity += a.packingQuantity * a.quantity;
    });

    this.data.productCost = qcloud.utils.getNumber(productCost, 2); // 产品费用总和
    this.data.saleFullReduce = wx.getStorageSync("nowFullReduce") || {}; // 当前使用的满减活动

    var coupons = wx.getStorageSync("myCoupon").filter(a => { // 优惠券列表
      if (a.status != 1) return false;
      if (productCost >= a.minConsume) return true;
      return false;
    });

    var freight = business.freightMode == 1 ? 0 : business.freight;

    this.setData({
      cartList,
      tablewareQuantity,
      freight,
      couponQuantity: coupons.length,
      saleFullReduce: this.data.saleFullReduce
    });
  },
  onShow: function () {
    var business = qcloud.getSession().business;
    /** 页面每次展示时，执行的操作：
     * 1. 载入正在使用的优惠券
     * 2. 载入正在使用的地址
     * 3. 如有必要，计算订单的阶梯运费
     * 4. 重新计算订单费用
     */

    var coupon = wx.getStorageSync("selectCoupon") || {};
    wx.removeStorageSync("selectCoupon"); // 载入之后删除缓存

    var address = wx.getStorageSync("selectAddress") || {},
      oldAddress = this.data.address;
    this.setData({
      address,
      coupon
    });

    if (business.freightMode == 1 && !!address && !!oldAddress && address.id != oldAddress.id) {
      this.changeAddress();
    } else {
      this.calcCost();
    }
  },
  changeAddress: function () {                    // 改变地址后，重新计算配送费
    var business = qcloud.getSession().business;
    this.data.freight = business.freight;
    this.calcCost();
  },
  calcCost: function () {
    /** 计算订单各种费用：
     * 1. 涉及的费用：产品总价、配送费、满减、优惠券
     * 2. 计算总价，原价
     * 3. 载入视图
     */
    var productCost = this.data.productCost,
      freight = this.data.freight,
      fullReduce = this.data.saleFullReduce.reduceMoney || 0,
      coupon = this.data.coupon.value || 0;

    var total = qcloud.utils.getNumber(productCost + freight - fullReduce - coupon, 2);
    var oldPrice = qcloud.utils.getNumber(productCost + freight, 2);
    this.setData({
      total,
      freight,
      oldPrice
    });
  },
  selectAddress: function () {
    wx.navigateTo({
      url: "/pages/address/list/list?flag=select"
    });
  },
  selectCoupon: function () {
    wx.navigateTo({
      url: `/pages/user/selectcoupon/selectcoupon?total=${this.data.productCost}&id=${this.data.coupon.id}`
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
    var distance = util.calcDistance(this.data.address, business);
    if (business.range > 0 && (business.range - distance / 1000 < 0)) {
      util.showError("地址超出商家配送范围");
      return;
    }
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
      saleFullReduceMoney: this.data.saleFullReduce.reduceMoney,
      saleCouponUserId: this.data.coupon.id,
      saleCouponUserMoney: this.data.coupon.value,
      products: [],
      openId: user.openId,
      distance: +distance.toFixed(0)
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
  useCoupon: function (id) {
    if (!id) return;
    var myCoupon = wx.getStorageSync("myCoupon");
    myCoupon.forEach(a => {
      if (a.id == id) {
        a.status = 2;
      }
    });
    wx.setStorageSync("myCoupon", myCoupon);
  }

});