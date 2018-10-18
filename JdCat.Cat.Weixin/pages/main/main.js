const qcloud = require("../../vendor/wafer2-client-sdk/index");
const config = require("../../config");
const util = require("../../utils/util");
const weekObj = {
  "1": 1,
  "2": 2,
  "3": 4,
  "4": 8,
  "5": 16,
  "6": 32,
  "0": 64
};
Page({

  /**
   * 页面的初始数据
   */
  data: {
    swiperTitle: [{
      text: "点单",
      id: 1
    }, {
      text: "评价",
      id: 2
    }, {
      text: "商家",
      id: 3
    }],
    logo: "",
    businessId: config.businessId,
    freight: 0,
    menu: [],
    productList: [],
    cartList: [],
    currentPage: 0,
    selected: 0,
    showCart: false,
    showDetail: false,
    pullBar: false,
    animationData: "",
    location: "",
    foodToView: "scroll21",
    cartQuantity: 0,
    scrollTop: 0,
    formatIndex: 0,
    productIndex: 0, // 选择的商品在列表中的序号
    curQuantity: 0,
    isShowFoot: true,
    submitText: "去结算",
    isBalance: true, // 是否可以结算
    fullReduceList: [], // 商户满减活动列表
    saleText: "", // 购买商品时，根据满减活动，提示用户的显示信息
    isShowCoupon: false, // 是否显示领取优惠券页面
    isClosedCoupon: false, // 是否显示过领取优惠券
    couponList: [], // 商户优惠券列表
    myCoupon: [], // 我的优惠券列表
    isShowProductDetail: false
  },
  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function (options) {
    var that = this;
    var session = qcloud.getSession(),
      business = session.business;
    wx.setNavigationBarTitle({
      title: business.name
    });
    util.showBusy("loading");
    // 首次加载时，将获取商品信息，并将其缓存
    qcloud.request({
      url: "/product/menus/" + config.businessId,
      method: "GET",
      success: function (res) {
        // 1. 首先加载商户产品
        var menuArr = [];
        var productArr = [];
        for (var i in res.data) {
          var menuItem = res.data[i];
          menuArr.push(menuItem);
          menuItem.products.forEach(function (product, index) {
            product.quantity = 0; // 购物车中该商品的总数
            product.viewIndex = index;
            product.menuId = menuItem.id;
            productArr.push(product);
          });
        }
        productArr.forEach(function (product, index) {
          // 初始化时，默认规格、属性均选择第一个
          product.formats[0].selected = true;
          product.price = product.formats[0].price;
          if (product.attributes.length > 0) {
            product.attributes.forEach(function (attr) {
              if (attr.item1) {
                attr.selectedValue = attr.item1;
                attr.item1 = {
                  name: attr.item1,
                  selected: true
                };
              }
              attr.item2 && (attr.item2 = {
                name: attr.item2,
                selected: false
              });
              attr.item3 && (attr.item3 = {
                name: attr.item3,
                selected: false
              });
              attr.item4 && (attr.item4 = {
                name: attr.item4,
                selected: false
              });
              attr.item5 && (attr.item5 = {
                name: attr.item5,
                selected: false
              });
              attr.item6 && (attr.item6 = {
                name: attr.item6,
                selected: false
              });
              attr.item7 && (attr.item7 = {
                name: attr.item7,
                selected: false
              });
              attr.item8 && (attr.item8 = {
                name: attr.item8,
                selected: false
              });
            });
          }
        });
        wx.setStorageSync("businessMenu", menuArr);
        wx.setStorageSync("businessProduct", productArr);

        var user = qcloud.getSession().userinfo;
        qcloud.request({
          url: `/user/carts/${user.id}`,
          method: "GET",
          success: function (res) {
            wx.hideToast();
            wx.setStorageSync("cartList", res.data);
            that.loadData();
          }
        });
      }
    });

    qcloud.request({
      url: "/business/sale/" + config.businessId,
      method: "GET",
      success: function (res) {
        // 满减
        that.setData({
          fullReduceList: res.data.fullReduct
        });
        // 优惠券
        var couponList = res.data.coupon;
        qcloud.request({
          url: `/user/userCoupon/${session.userinfo.id}`,
          method: "GET",
          success: function (res) {
            var myCoupon = res.data,
              unreceived = [];
            wx.setStorageSync("myCoupon", myCoupon);
            that.data.couponList = couponList;
            that.data.myCoupon = myCoupon;
            that.data.unreceived = unreceived;
            if (couponList.length === 0) return;
            that.couponHandler();
          }
        });
        // 商品打折
        that.data.discount = res.data.discount;
        that.discountHandler();
      }
    });

  },
  onShareAppMessage: function () {
    var session = qcloud.getSession();
    return {
      title: session.business.name,
      path: "/pages/launch/launch?userid=" + session.userinfo.id
    };
  },
  onShow: function () {
    var self = this;

    // 每次显示，均重新计算购物车数据
    setTimeout(function () {
      self.loadData();
    }, 1000);
    this.couponHandler();
  },
  loadData: function () {
    var menuArr = wx.getStorageSync("businessMenu");
    if (!menuArr) return;
    var productArr = wx.getStorageSync("businessProduct");
    var cartList = wx.getStorageSync("cartList");
    var cartQuantity = 0;
    var existCart = []; // 存储购物车与商品可以匹配的记录
    if (cartList.length > 0) {
      cartList.forEach(function (obj) {
        var products = productArr.filter(a => a.id == obj.productId);
        if (products.length === 0) return;
        obj.product = products[0];
        existCart.push(obj);
        products[0].quantity += obj.quantity;
        cartQuantity += obj.quantity;
      });
    }
    wx.setStorageSync("cartList", existCart);

    this.calcSaleText();

    // 每次重新加载数据时，都重新更新一遍商户信息显示
    var business = qcloud.getSession().business;
    var submitText = "去结算",
      isBalance = true;
    if (business.isClose) {
      submitText = "已暂停营业";
      isBalance = false;
    } else {
      var time1 = !!business.businessStartTime && this.calcBusinessStatus(business.businessStartTime, business.businessEndTime);
      var time2 = !!business.businessStartTime2 && this.calcBusinessStatus(business.businessStartTime2, business.businessEndTime2);
      var time3 = !!business.businessStartTime3 && this.calcBusinessStatus(business.businessStartTime3, business.businessEndTime3);
      time1 || time2 || time3 || (submitText = "已暂停营业", isBalance = false);
    }

    this.setData({
      submitText: submitText,
      isBalance: isBalance,
      business: business,
      freight: business.freight,
      logo: business.logoSrc,
      productList: productArr,
      menu: menuArr,
      cartList: existCart,
      cartQuantity: cartQuantity
    });
    // this.discountHandler();
    // this.cartSync();
  },
  calcBusinessStatus: function (start, end) {
    var now = new Date(), nowStamp = Date.parse(now);
    var startTime = start.split(":");
    var endTime = end.split(":");
    var startStamp = Date.parse(new Date(now.getFullYear(), now.getMonth(), now.getDate(), +startTime[0], +startTime[1], 0));
    var endStamp = Date.parse(new Date(now.getFullYear(), now.getMonth(), now.getDate(), +endTime[0], +endTime[1], 0));
    return nowStamp >= startStamp && nowStamp <= endStamp;
  },
  couponHandler: function () {
    if (this.data.isClosedCoupon) return; // 如果已经关闭过优惠券窗口，则不再弹出
    if (!this.data.couponList || !this.data.myCoupon || !this.data.unreceived) return;
    var userinfo = qcloud.getSession().userinfo;
    // if (!userinfo.isRegister) return; // 用户没有注册，则不弹出

    var self = this;
    this.data.couponList.forEach(a => {
      var coupons = this.data.myCoupon.filter(b => b.couponId == a.id);
      if (coupons.length === 0) {
        self.data.unreceived.push(a);
      }
    });
    if (self.data.unreceived.length > 0) {
      self.setData({
        isShowCoupon: true
      });
      wx.hideTabBar();
    }
  },
  discountHandler: function () {
    if (!this.data.productList || !this.data.discount) return;
    if (this.data.productList.length === 0 || this.data.discount.length === 0) return;
    var self = this,
      now = new Date(),
      product, hour = now.getHours().toString(),
      minus = now.getMinutes().toString(),
      hourMinus = (hour.length > 1 ? hour : ("0" + hour)) + ":" + (minus.length > 1 ? minus : ("0" + minus)),
      weekday = weekObj[now.getDay().toString()];
    this.data.discount.forEach(function (item) {
      if (!(weekday & item.cycle)) return; // 不在循环周期内
      var isArea = false; // 是否在有效时间区间内
      if (item.startTime1 <= hourMinus && item.endTime1 >= hourMinus) isArea = true;
      else if (item.startTime2 <= hourMinus && item.endTime2 >= hourMinus) isArea = true;
      else if (item.startTime3 <= hourMinus && item.endTime3 >= hourMinus) isArea = true;
      if (!isArea) return;
      product = self.data.productList.filter(a => a.id == item.productId);
      if (product.length === 0) return;
      product[0].discount = item;
    });

    this.setData({
      productList: this.data.productList,
    });
    this.cartSync(); // 同步购物车商品
  },
  cartSync: function () {
    var productList = this.data.productList,
      cartList = this.data.cartList,
      product;
    if (!cartList || cartList.length === 0) return;
    cartList.forEach(cart => {
      product = productList.filter(a => a.id == cart.productId)[0];
      cart.product = product;
    });
    this.setData({
      cartList
    });
  },
  catLicense: function () {
    wx.navigateTo({
      url: "/pages/main/license/license"
    });
  },
  catAddress: function () {
    var business = qcloud.getSession().business;
    wx.openLocation({
      latitude: business.lat,
      longitude: business.lng,
      name: business.name,
      address: business.address
    });
  },
  callPhone: function () {
    var business = qcloud.getSession().business;
    wx.makePhoneCall({
      phoneNumber: business.mobile
    });
  },
  pullBar: function () {
    this.setData({
      pullBar: !this.data.pullBar
    });
  },
  turnPage: function (e) {
    this.setData({
      currentPage: e.currentTarget.dataset.index,
      isShowFoot: e.currentTarget.dataset.index == 0
    });
  },
  turnTitle: function (e) {
    if (e.detail.source == "touch") {
      this.setData({
        currentPage: e.detail.current,
        isShowFoot: e.detail.current == 0
      });
    }
  },
  turnMenu: function (e) {
    var menu = this.data.menu[e.currentTarget.dataset.index];
    var currentToView = "scroll_" + menu.id + '_0';
    this.setData({
      selected: e.currentTarget.dataset.index,
      foodToView: currentToView
    });
  },
  closeCartDialog: function () {
    this.showCartAnimation(false);
  },
  showCart: function () {
    this.showCartAnimation(!this.data.showCart);
  },
  clearCart: function (e) {
    var self = this;
    var session = qcloud.getSession();
    qcloud.request({
      url: `/user/clearCart/${session.userinfo.id}?businessId=${session.business.id}`,
      method: "DELETE",
      success: function (res) {
        if (res.data.success) {
          self.data.productList.forEach(a => a.quantity = 0);
          wx.setStorageSync("cartList", []);
          self.setData({
            productList: self.data.productList,
            cartList: [],
            curQuantity: 0,
            cartQuantity: 0
          });
        } else {
          util.showModel("提示", res.data.msg);
        }
        self.showCartAnimation(false);
      },
      fail: function (error) {
        util.showModel("错误", "请检查网络连接");
      }
    });
  },
  scrollFoodList: function (e) {
    var menus = this.data.menu,
      self = this;
    var query = wx.createSelectorQuery().select(".food");
    query._single = false;
    query.boundingClientRect(function (elements) {
      if (elements.length === 0) return;
      var id;
      elements.some(ele => {
        if (ele.top - 70 >= 0) {
          id = ele.id.split('_')[1];
          menus.some((a, index) => {
            if (a.id == id) {
              if (self.data.selected != index) {
                self.setData({
                  selected: index
                });
              }
              return true;
            }
          });
          return true;
        }
      });
    }).exec();

  },
  showFoodDetail: function () {

  },
  showCartAnimation: function (isShow) {
    this.setData({
      showCart: isShow
    });
    // var animation = wx.createAnimation({
    //   duration: 200,
    //   timingFunction: "ease-out",
    //   delay: 0
    // });

    // this.animation = animation;

    // animation.opacity(0).translateX(-100).step();

    // this.setData({
    //   animationData: animation.export()
    // })

    // setTimeout(function () {

    //   animation.opacity(1).translateX(0).step();

    //   this.setData({
    //     animationData: animation
    //   })

    //   if (!isShow) {
    //     this.setData({
    //       showCart: false
    //     });
    //   }
    // }.bind(this), 200)

    // if (isShow) {
    //   this.setData({
    //     showCart: true
    //   });
    // }
  },
  pay: function () {
    if (!this.data.isBalance) {
      return;
    }
    if (this.data.cartList.length === 0) {
      util.showError("请选择需要的商品再结算");
      return;
    }
    wx.navigateTo({
      url: '/pages/pay/pay'
    });
  },
  showFormat: function (e) { // 显示规格、属性选择框
    this.setData({
      curQuantity: this.calcQuantity(),
      showDetail: true,
      productIndex: !!e ? e.currentTarget.dataset.index : this.data.productIndex
    });
  },
  showFormatAtDetail: function () {
    this.setData({
      isShowProductDetail: false
    });
    this.showFormat();
  },
  closeFormat: function () { // 关闭规格、属性选择框
    this.setData({
      showDetail: false
    });
  },
  attch1: function () {},
  add: function (e) { // 添加商品
    var user = qcloud.getSession().userinfo;
    if (!user.isRegister) {
      wx.showModal({
        title: "提示",
        content: "请先登录系统，并绑定手机号，才能开始点单哦",
        showCancel: false,
        confirmText: "去登陆",
        success: function () {
          wx.switchTab({
            url: "/pages/user/user"
          });
        }
      });
      return;
    }
    if (!user.isPhone) {
      wx.showModal({
        title: "提示",
        content: "为了给您提供更优质的服务，本店邀请您授权手机号",
        showCancel: false,
        confirmText: "去授权",
        success: function () {
          wx.switchTab({
            url: "/pages/user/user"
          });
        }
      });
      return;
    }
    var index = !!e ? e.currentTarget.dataset.index : this.data.productIndex;
    if (Object.prototype.toString.call(index) == "[object Number]") {
      this.data.productIndex = index;
    } else {
      this.data.curQuantity++;
    }
    this.cartHandler("add");
    this.setData({
      curQuantity: this.data.curQuantity,
      productIndex: this.data.productIndex
    });
  },
  addAtDetail: function () {
    this.setData({
      isShowProductDetail: false
    });
    this.add();
  },
  remove: function (e) {
    var index = e.currentTarget.dataset.index;
    if (Object.prototype.toString.call(index) == "[object Number]") {
      this.data.productIndex = index;
    } else {
      if (this.data.curQuantity === 0) return;
      this.data.curQuantity--;
    }
    this.cartHandler("remove");
    this.setData({
      curQuantity: this.data.curQuantity,
      productIndex: this.data.productIndex
    });
  },
  selectFormat: function (e) { // 选择规格事件
    var product = this.data.productList[this.data.productIndex]; // 取到列表中的产品对象
    product.formats.forEach(a => a.selected = false);
    var format = product.formats[e.currentTarget.dataset.index];
    format.selected = true;
    product.price = format.price;
    this.setData({
      productList: this.data.productList,
      curQuantity: this.calcQuantity()
    });
  },
  selectAttribute: function (e) { // 选择属性事件
    var product = this.data.productList[this.data.productIndex]; // 取到列表中的产品对象
    var attr = product.attributes[e.currentTarget.dataset.num],
      i = 1,
      item;
    for (; i < 9;) {
      item = attr["item" + i++];
      item && (item.selected = false);
    }
    item = attr["item" + e.currentTarget.dataset.index];
    item.selected = true;
    attr.selectedValue = item.name;
    this.setData({
      productList: this.data.productList,
      curQuantity: this.calcQuantity()
    });
  },
  addCart: function (e) { // 增加购物车数量
    var index = e.currentTarget.dataset.index,
      self = this,
      cart = this.data.cartList[index],
      quantity = cart.quantity + 1;
    qcloud.request({
      url: "/user/updateCart/" + cart.id + "?quantity=" + quantity,
      method: "GET",
      success: function (res) {
        if (res.data.success) {
          cart.quantity = quantity;
          var product = self.data.productList.filter(a => a.id == cart.productId);
          if (product && product.length > 0) {
            product[0].quantity++;
          }
          wx.setStorageSync("cartList", self.data.cartList);
          self.setData({
            productList: self.data.productList,
            cartList: self.data.cartList,
          });
          self.calcCartQuantity();
        } else {
          util.showError(res.msg);
        }
      },
      fail: function () {
        util.showModel("错误", "请求错误，请检查网络连接");
      }
    });
  },
  removeCart: function (e) { // 减少购物车数量                 // 增加购物车数量
    var index = e.currentTarget.dataset.index,
      self = this,
      cart = this.data.cartList[index],
      quantity = cart.quantity - 1;
    qcloud.request({
      url: "/user/updateCart/" + cart.id + "?quantity=" + quantity,
      method: "GET",
      success: function (res) {
        if (res.data.success) {
          cart.quantity = quantity;
          if (quantity <= 0) {
            self.data.cartList.splice(index, 1);
          }
          var product = self.data.productList.filter(a => a.id == cart.productId);
          if (product && product.length > 0) {
            product[0].quantity--;
          }
          wx.setStorageSync("cartList", self.data.cartList);
          self.setData({
            productList: self.data.productList,
            cartList: self.data.cartList,
          });
          self.calcCartQuantity();
        } else {
          util.showError(res.msg);
        }
      },
      fail: function () {
        util.showModel("错误", "请求错误，请检查网络连接");
      }
    });
  },
  openCoupon: function () {
    if (!this.data.unreceived || this.data.unreceived.length == 0) return;
    var ids = this.data.unreceived.map(a => a.id);
    util.showBusy("请稍等...");
    var user = qcloud.getSession().userinfo,
      self = this;
    qcloud.request({
      url: "/user/receiveCoupons/" + user.id,
      data: this.data.unreceived,
      method: "POST",
      success: function (res) {
        wx.hideToast();
        res.data.forEach(a => {
          self.data.myCoupon.push(a);
        });
        wx.setStorageSync("myCoupon", self.data.myCoupon);
        self.closeCoupon();
        wx.navigateTo({
          url: "/pages/user/mycoupon/mycoupon"
        });
      }
    });
  },
  closeCoupon: function () {
    this.setData({
      isShowCoupon: false,
      isClosedCoupon: true
    });
    wx.showTabBar();
  },
  productDetail: function (e) {
    var index = e.currentTarget.dataset.index,
      self = this;
    var product = this.data.productList[index];
    this.data.productIndex = index;

    var animation = wx.createAnimation({
      duration: 200,
      timingFunction: "ease",
      delay: 0
    });

    animation.scale(0.01, 0.01).step();

    this.setData({
      isShowProductDetail: true,
      animationData: animation.export(),
      curProduct: product
    });
    setTimeout(function () {
      animation.scale(1, 1).step();
      self.setData({
        animationData: animation.export()
      });
    }, 100);
  },
  closeProduct: function () {
    this.setData({
      isShowProductDetail: false
    });
  },
  // 以下均为方法，不代表各类事件
  cartHandler: function (flag) {
    var self = this,
      product = this.data.productList[this.data.productIndex], // 操作的商品
      cart, // 购物车
      cartIndex = -1, // 当前选择的购物车在列表中的序号
      description = "", // 购物车描述
      format; // 选中的规格

    if (flag === "add") product.quantity++;
    else product.quantity--;

    format = product.formats.filter(a => a.selected)[0];
    // 处理购物车描述
    description = this.calcDescription(product);

    // 处理购物车
    this.data.cartList.forEach((obj, index) => {
      if (obj.productId === product.id && obj.description === description) {
        cart = obj;
        cartIndex = index;
        return false;
      }
    });
    if (!cart) { // 不存在相同的购物车信息
      var imgSrc = product.images.length > 0 ? (product.images[0].name + "." + product.images[0].extensionName) : null;
      cart = {
        name: product.name,
        src: imgSrc,
        quantity: 1,
        price: qcloud.utils.getNumber(format.price + format.packingPrice, 2),
        productId: product.id,
        product: product,
        formatId: format.id,
        packingQuantity: format.packingQuantity,
        description: description,
        userId: qcloud.getSession().userinfo.id,
        businessId: config.businessId
      };
      this.data.cartList.push(cart);
    } else {
      if (flag === "add") cart.quantity++;
      else cart.quantity--;
    }
    var cartClone = qcloud.utils.extend({}, cart);
    cartClone.product = null;
    // 请求服务器
    qcloud.request({
      url: "/user/carthandler",
      method: "POST",
      data: cartClone,
      success: function (res) {
        cart.id = res.data.id;
        if (cart.quantity <= 0) {
          self.data.cartList.splice(cartIndex, 1);
        }
        wx.setStorageSync("cartList", self.data.cartList);
        self.setData({
          productList: self.data.productList,
          cartList: self.data.cartList
        });
        self.calcCartQuantity();
      },
      fail: function (error) {
        util.showModel("错误", "请求错误，请检查网络连接");
      }
    });
  },
  calcQuantity: function () { // 选择规格或属性时，计算当前已经添加同类商品的数量
    var description = this.calcDescription(),
      productId = this.data.productList[this.data.productIndex].id,
      cart = this.data.cartList.filter(a => a.description == description && a.productId == productId);
    if (cart.length > 0) {
      return cart[0].quantity;
    }
    return 0;
  },
  calcDescription: function (product) { // 拿到商品的规格、属性组合
    product = product || this.data.productList[this.data.productIndex];
    var format = product.formats.filter(a => a.selected)[0],
      attrs = product.attributes,
      description = "";

    // 处理商品规格
    if (product.formats.length > 1) {
      description += format.name;
    }
    // 处理商品属性
    if (attrs.length > 0) {
      attrs.forEach(attr => {
        description && (description += "|");
        description += attr.selectedValue;
      });
    }
    return description;
  },
  calcCartQuantity: function () {
    var cartQuantity = 0;
    this.data.cartList.forEach(a => cartQuantity += a.quantity);
    this.setData({
      cartQuantity: cartQuantity
    });
    this.calcSaleText();
  },
  calcSaleText: function () {
    var cartList = this.data.cartList,
      self = this,
      fullReduceList = this.data.fullReduceList.slice(),
      nowItem;
    wx.removeStorageSync("nowFullReduce");
    if (cartList.length === 0 || fullReduceList.length === 0) return;
    var total = 0,
      list = fullReduceList.reverse(),
      text = "";
    cartList.forEach(function (a) {
      // total += a.quantity * a.price;
      total += self.calcCartProductPrice(a);
    });
    total = qcloud.utils.getNumber(total, 2);
    list.some(function (item, index) {
      if (total >= item.minPrice) {
        text += "已满" + item.minPrice + "元，结算减" + item.reduceMoney + "元";
        nowItem = item;
        if (index > 0) {
          var pre = list[index - 1];
          var addMoney = qcloud.utils.getNumber(pre.minPrice - total, 2);
          text += "；再加" + addMoney + "元，减" + pre.reduceMoney + "元";
        }
        return true;
      }
    });
    if (!text) {
      var lastItem = list[list.length - 1];
      text += "已购金额" + total + "元，再加" + qcloud.utils.getNumber(lastItem.minPrice - total, 2) + "元，减" + lastItem.reduceMoney + "元";
    }
    wx.setStorageSync("nowFullReduce", nowItem);
    this.setData({
      saleText: text
    });
  },
  calcCartProductPrice: function (cart) {
    var price = cart.price,
      quantity = cart.quantity,
      product = cart.product,
      discount = product.discount;
    if (!discount) return price * quantity;
    if (discount.upperLimit == -1) {
      return discount.price * quantity;
    }
    if (discount.upperLimit >= quantity) {
      return discount.price * quantity;
    }
    return (quantity - discount.upperLimit) * price + discount.upperLimit * discount.price;
  }
});