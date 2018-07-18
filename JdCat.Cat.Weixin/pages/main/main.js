const qcloud = require("../../vendor/wafer2-client-sdk/index");
const config = require("../../config");
const util = require("../../utils/util");
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
    isBalance: true,  // 是否可以结算
  },
  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function (options) {
    var that = this;
    var business = qcloud.getSession().business;
    this.setData({
      freight: business.freight,
      logo: business.logoSrc,
      business: business
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
        };
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
  },
  onShow: function () {
    var business = qcloud.getSession().business;
    var submitText = "去结算", isBalance = true, now = new Date();
    var startTime = business.businessStartTime.split(":"), 
      endTime = business.businessEndTime.split(":");
    var nowStamp = Date.parse(now), 
      startStamp = Date.parse(new Date(now.getFullYear(), now.getMonth(), now.getDate(), +startTime[0], +startTime[1], 0)),
      endStamp = Date.parse(new Date(now.getFullYear(), now.getMonth(), now.getDate(), +endTime[0], +endTime[1], 0));
    if(nowStamp <= startStamp || nowStamp >= endStamp || business.isClose) {
      submitText = "已暂停营业";
      isBalance = false;
    }
    console.log(business);
    this.setData({
      submitText: submitText,
      isBalance: isBalance
    });
    this.loadData();
  },
  loadData: function () {
    var menuArr = wx.getStorageSync("businessMenu");
    if (!menuArr) return;
    var productArr = wx.getStorageSync("businessProduct");
    var cartList = wx.getStorageSync("cartList");
    var cartQuantity = 0;
    if (cartList.length > 0) {
      cartList.forEach(function (obj) {
        var products = productArr.filter(a => a.id == obj.productId);
        if (!products.length === 0) return;
        products[0].quantity += obj.quantity;
        cartQuantity += obj.quantity;
      });
    }
    
    this.setData({
      productList: productArr,
      menu: menuArr,
      cartList: cartList,
      cartQuantity: cartQuantity
    });

  },
  catLicense: function(){
    wx.navigateTo({
      url: "/pages/main/license/license"
    });
  },
  callPhone: function(){
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
    var currentToView = "scroll" + menu.id + 0;
    this.setData({
      selected: e.currentTarget.dataset.index,
      foodToView: currentToView
    });
  },
  closeCartDialog: function () {
    this.showCartAnimation(false);
  },
  showCart: function () {
    if(this.data.showCart) return;
    this.showCartAnimation(true);
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

  },
  showCartAnimation: function (isShow) {

    var animation = wx.createAnimation({
      duration: 200,
      timingFunction: "ease-out",
      delay: 0
    });

    this.animation = animation;

    animation.opacity(0).translateX(-100).step();

    this.setData({
      animationData: animation.export()
    })

    setTimeout(function () {

      animation.opacity(1).translateX(0).step();

      this.setData({
        animationData: animation
      })

      if (!isShow) {
        this.setData({
          showCart: false
        });
      }
    }.bind(this), 200)

    if (isShow) {
      this.setData({
        showCart: true
      });
    }
  },
  pay: function () {
    if(!this.data.isBalance) {
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
    this.data.productIndex = e.currentTarget.dataset.index;
    this.setData({
      curQuantity: this.calcQuantity(),
      showDetail: true,
      productIndex: e.currentTarget.dataset.index
    });
  },
  closeFormat: function () { // 关闭规格、属性选择框
    this.setData({
      showDetail: false
    });
  },
  attch1: function () {},
  add: function (e) { // 添加商品
    var user = qcloud.getSession().userinfo;
    if(!user.isRegister) {
      wx.showModal({
        title: "提示",
        content: "请先登录系统，才能开始点单哦",
        showCancel: false,
        confirmText: "去登陆",
        success: function() {
          wx.switchTab({
            url: "/pages/user/user"
          });
        }
      });
      return;
    }
    var index = e.currentTarget.dataset.index;
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
        price: format.price,
        productId: product.id,
        formatId: format.id,
        description: description,
        userId: qcloud.getSession().userinfo.id,
        businessId: config.businessId
      };
      this.data.cartList.push(cart);
    } else {
      if (flag === "add") cart.quantity++;
      else cart.quantity--;
    }

    // 请求服务器
    qcloud.request({
      url: "/user/carthandler",
      method: "POST",
      data: cart,
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
    var product = product || this.data.productList[this.data.productIndex],
      format = product.formats.filter(a => a.selected)[0],
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
  calcCartQuantity: function(){
    var cartQuantity = 0;
    this.data.cartList.forEach(a => cartQuantity += a.quantity);
    this.setData({
      cartQuantity: cartQuantity
    });
  }
})