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
    productCost: 0,
    productOldCost: 0,
    tablewareQuantity: 0,
    tablewareQuantitys: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10],
    couponQuantity: 0,
    coupon: {},
    saleFullReduce: {},
    saleNewCustom: {},
    packagePrice: 0,
    isInvoice: false,
    invoiceName: "",
    invoiceTax: "",
    isSelfTaking: false,
    takePerson: '',
    phoneNumber: '',
    phoneNumberFocus: false,
    canCoupon: true // 是否可以使用优惠券
  },
  onLoad: function () {
    /** 页面加载时操作：
     * 1. 查询购物车信息
     * 2. 将购物车中折扣商品与原价商品分开
     * 3. 计算购物车总价格，餐盒数
     * 4. 计算出可使用的优惠券
     * 5. 初始化配送费
     */
    var cartList = wx.getStorageSync('cartList') || [],
      carts = [],
      productCost = 0,
      productOldCost = 0,
      tablewareQuantity = 0,
      packagePrice = +wx.getStorageSync("packagePrice");

    cartList.forEach(cart => {
      if (cart.quantity <= 0) return;
      if (cart.discount && cart.discountProductQuantity < cart.quantity) { // 当商品存在折扣，商品数量大于折扣商品数量时
        var discountProduct = qcloud.utils.extend({}, cart);
        discountProduct.quantity = cart.discountProductQuantity;
        discountProduct.price = +(cart.discount.price * discountProduct.quantity).toFixed(2);
        discountProduct.oldPrice = +(cart.discount.oldPrice * discountProduct.quantity).toFixed(2);
        carts.push(discountProduct);
        var oldProduct = qcloud.utils.extend({}, cart);
        oldProduct.discountProductQuantity = null;
        oldProduct.discount = null;
        oldProduct.quantity = cart.quantity - cart.discountProductQuantity;
        oldProduct.price = +(oldProduct.quantity * cart.discount.oldPrice).toFixed(2);
        oldProduct.oldPrice = oldProduct.price;
        carts.push(oldProduct);
        return;
      }
      carts.push(cart);
    });

    carts.forEach(a => {
      productCost += a.price;
      productOldCost += a.oldPrice;
      tablewareQuantity += a.packingQuantity * a.quantity;
    });

    this.data.productCost = qcloud.utils.getNumber(productCost, 2); // 产品费用总和
    this.data.productOldCost = qcloud.utils.getNumber(productOldCost, 2); // 产品费用总和（原价）
    this.data.saleFullReduce = wx.getStorageSync("nowFullReduce") || {}; // 当前使用的满减活动
    var saleNewCustom = wx.getStorageSync("newCustom"); // 新客户立减活动

    var coupons = wx.getStorageSync("myCoupon").filter(a => { // 优惠券列表
      if (a.status != 1) return false;
      if (productCost >= a.minConsume) return true;
      return false;
    });

    var freight = wx.getStorageSync("orderFreight"),
      isInvoice = wx.getStorageSync("isInvoice"),
      invoiceName = wx.getStorageSync("invoiceName"),
      invoiceTax = wx.getStorageSync("invoiceTax");


    this.setData({
      cartList: carts,
      tablewareQuantity,
      freight,
      couponQuantity: coupons.length,
      saleFullReduce: this.data.saleFullReduce,
      saleNewCustom: saleNewCustom || {},
      packagePrice,
      invoiceName,
      invoiceTax,
      isInvoice: !!isInvoice,
      canCoupon: !saleNewCustom
    });
  },
  onShow: function () {
    /** 页面每次展示时，执行的操作：
     * 1. 载入正在使用的优惠券
     * 2. 载入正在使用的地址
     * 3. 如有必要，计算订单的阶梯运费
     * 4. 重新计算订单费用
     */

    var coupon = wx.getStorageSync("selectCoupon") || {};
    wx.removeStorageSync("selectCoupon"); // 载入之后删除缓存

    var address = wx.getStorageSync("selectAddress") || "",
      oldAddress = this.data.address;

    this.setData({
      address,
      coupon,
      takePerson: address ? address.receiver : '',
      phoneNumber: address ? address.phone : ''
    });

    if (address && (!oldAddress || address.id != oldAddress.id)) {
      this.changeAddress();
    } else {
      this.calcCost();
    }
  },
  /**
   * 地址改变后，重新计算配送费和订单综合
   */
  changeAddress: function () {
    var business = qcloud.getSession().business,
      freights = wx.getStorageSync("freights")
    if (!freights || freights.length === 0) {
      this.data.freight = business.freight;
    } else {
      var freight = 0;
      var distance = util.calcDistance({
        lat: this.data.address.lat,
        lng: this.data.address.lng
      }, {
        lat: business.lat,
        lng: business.lng
      });
      freights.some(function (item) {
        if (item.maxDistance * 1000 >= distance) {
          freight = item.amount;
          return true;
        }
        return false;
      });
      this.data.freight = freight;
    }
    this.calcCost();
  },
  /**
   * 计算订单总额
   */
  calcCost: function () {
    /** 计算订单各种费用：
     * 1. 涉及的费用：产品总价、配送费、新客户立减、满减、优惠券
     * 2. 计算总价，原价
     * 3. 载入视图
     */
    var productCost = this.data.productCost,
      productOldCost = this.data.productOldCost,
      freight = this.data.freight,
      fullReduce = this.data.saleFullReduce.reduceMoney || 0,
      newCustom = this.data.saleNewCustom.amount || 0,
      coupon = this.data.coupon.value || 0;

    if (this.data.isSelfTaking) { // 如果是自提，则配送费为零
      freight = 0
    }

    var total = qcloud.utils.getNumber(productCost + freight - newCustom - fullReduce - coupon + this.data.packagePrice, 2);
    if (total <= 0) total = 0.01;
    var oldPrice = qcloud.utils.getNumber(productOldCost + freight + this.data.packagePrice, 2);
    this.setData({
      total,
      freight,
      oldPrice
    });
  },
  radioChange(e) {
    this.setData({
      deliveryMode: +e.detail.value
    })
  },
  selectAddress: function () {
    wx.navigateTo({
      url: "/pages/address/list/list?flag=select"
    })
  },
  selectCoupon: function () {
    wx.navigateTo({
      url: `/pages/user/selectcoupon/selectcoupon?total=${this.data.productCost}&id=${this.data.coupon.id}`
    });
  },
  bindPickerChange: function (e) {
    this.setData({
      tablewareQuantity: this.data.tablewareQuantitys[e.detail.value]
    });
  },
  sure() {
    let self = this, business = qcloud.getSession().business
    wx.requestSubscribeMessage({
      tmplIds: [business.templateNotifyId],
      complete() {
        self.submit()
      }
    })
  },
  submit: function () {
    var self = this,
      business = qcloud.getSession().business,
      user = qcloud.getSession().userinfo,
      address = this.data.isSelfTaking ? {} : this.data.address

    if (this.data.isSelfTaking) {
      // 如果是自提
      if (!this.data.phoneNumber) {
        util.showError('请输入联系电话')
        return
      }
    } else {
      // 需要配送地址
      if (!address) {
        util.showError("请选择收货地址")
        return
      }
      // 验证是否超过范围
      var distance = util.calcDistance(address, business)
      if (business.range > 0 && (business.range - distance / 1000 < 0)) {
        util.showError("地址超出商家配送范围")
        return
      }
    }

    var remark = this.data.remark || "";
    // 验证发票信息
    if (this.data.isInvoice) {
      if (!this.data.invoiceName || !this.data.invoiceTax) {
        util.showError("请填写公司名称、纳税识别码")
        return
      }
      wx.setStorageSync("invoiceName", this.data.invoiceName);
      wx.setStorageSync("invoiceTax", this.data.invoiceTax);
      remark += `(开票公司：${this.data.invoiceName}，识别码：${this.data.invoiceTax})`;
    }
    wx.setStorageSync("isInvoice", this.data.isInvoice);

    util.showBusy("loading")

    // 提交订单
    var order = {
      price: this.data.total,
      oldPrice: this.data.oldPrice,
      freight: this.data.freight,
      receiverName: address.receiver || '',
      receiverAddress: address.mapInfo + " " + address.detailInfo,
      lat: address.lat || business.lat,
      lng: address.lng || business.lng,
      phone: address.phone,
      gender: address.gender || 0,
      remark: remark,
      tablewareQuantity: this.data.tablewareQuantity,
      cityCode: business.cityCode,
      userId: user.id,
      businessId: business.id,
      saleFullReduceId: this.data.saleFullReduce.id,
      saleFullReduceMoney: this.data.saleFullReduce.reduceMoney,
      saleCouponUserId: this.data.coupon.couponId > 0 ? this.data.coupon.id : null,
      saleCouponUserMoney: this.data.coupon.couponId > 0 ? this.data.coupon.value : null,
      products: [],
      openId: user.openId,
      distance: ~~distance,
      deliveryMode: this.data.isSelfTaking ? 2 : 0,
      packagePrice: +this.data.packagePrice,
      invoiceName: this.data.isInvoice ? this.data.invoiceName : '',
      invoiceTax: this.data.isInvoice ? this.data.invoiceTax: ''
    };

    if (this.data.isSelfTaking) {
      order.receiverAddress = '自提'
      order.phone = this.data.phoneNumber
      order.receiverName = this.data.takePerson || ''
    }

    this.data.cartList.forEach(a => {
      order.products.push({
        name: a.name,
        quantity: a.quantity,
        price: a.price,
        src: a.src,
        description: a.description,
        productId: a.productId,
        formatId: a.formatId,
        saleProductDiscountId: a.saleProductDiscountId,
        discountProductQuantity: a.discountProductQuantity,
        oldPrice: a.oldPrice,
        discount: a.product.discount ? a.product.discount.discount : 10,
        feature: a.product.feature,
        productIdSet: a.product.productIdSet
      });
    });
    // 分析订单活动
    var activities = [];
    // 1. 立减
    let newCustom = this.data.saleNewCustom;
    if (newCustom.amount > 0) {
      activities.push({
        amount: newCustom.amount,
        type: 1,
        activityId: newCustom.id,
        remark: `新客户立减${newCustom.amount}元`
      });
    }
    // 2. 满减
    let fullReduce = this.data.saleFullReduce;
    if (fullReduce.reduceMoney > 0) {
      activities.push({
        amount: fullReduce.reduceMoney,
        type: 2,
        activityId: fullReduce.id,
        remark: `满${fullReduce.minPrice}元减${fullReduce.reduceMoney}元`
      });
    }
    // 3. 优惠券
    let coupon = this.data.coupon;
    if (coupon.value > 0) {
      activities.push({
        amount: coupon.value,
        type: 3,
        activityId: coupon.id,
        remark: `${coupon.value}元优惠券`,
        tag: coupon.returnCouponId ? 'retCoupon' : null
      });
    }
    // 4. 商品折扣
    order.products.forEach(product => {
      if (!product.discount) return;
      let amount = +(product.oldPrice - product.price).toFixed(2);
      if (amount === 0) return;
      activities.push({
        amount: amount,
        type: 4,
        activityId: product.discount.id,
        remark: `${product.name}折扣优惠${amount}元`
      });
    });
    order.orderActivities = activities;
    qcloud.request({
      url: `/order/createOrder`,
      method: "POST",
      data: order,
      success: function (res) {
        if (res.data.success) {
          self.data.cartList = [];
          wx.setStorageSync('cartList', []);
          wx.setStorageSync('orderSubmit', true);
          wx.setStorageSync("orderDetail", res.data.data);
          self.useCoupon(coupon.id);
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
  },
  changeDelivery(e) {
    this.setData({
      isSelfTaking: e.detail.value,
      phoneNumberFocus: e.detail.value
    })
    this.changeAddress()
  },
  changeInvoice: function () {
    var result = !this.data.isInvoice;
    this.setData({
      isInvoice: result,
      invoiceNameFocus: result
    });

  },
  inputField: function (e) {
    var field = e.currentTarget.dataset.fieldname;
    this.data[field] = e.detail.value;
  },
  completeName: function () {
    this.setData({
      invoiceTaxFocus: true
    });
  }

});