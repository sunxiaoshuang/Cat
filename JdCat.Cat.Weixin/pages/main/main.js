const qcloud = require("../../vendor/wafer2-client-sdk/index");
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
    swiperTitle: [{ // 定义选项卡
      text: "点单",
      id: 1
    }, {
      text: "评价",
      id: 2
    }, {
      text: "商家",
      id: 3
    }],
    logo: "", // 商户logo
    businessId: 0, // 商户id
    freight: 0, // 配送费
    menu: [], // 商品类别
    productList: [], // 商品列表
    cartList: [], // 购物车列表
    cartLoaded: false, // 购物车是否请求加载完成
    currentPage: 0, // 选项卡当前的下标
    selected: 0, // 选择的选项卡下标                
    showCart: false, // 是否显示购物车
    showDetail: false, // 是否显示商品详情
    animationData: "", // 显示商品详情时的动画
    foodToView: "", // 当前跳转到的商品栏目id
    cartQuantity: 0, // 购物车数量
    productIndex: 0, // 选择的商品在列表中的序号
    curQuantity: 0, // 当前选择规格商品的数量
    isShowFoot: true, // 是否显示底部结算栏
    submitText: "去结算", // 结算按钮文本显示 
    isBalance: true, // 是否可以结算
    fullReduceList: [], // 商户满减活动列表
    saleText: "", // 购买商品时，根据满减活动，提示用户的显示信息
    isShowCoupon: false, // 是否显示领取优惠券页面
    isClosedCoupon: false, // 是否显示过领取优惠券
    couponList: [], // 商户优惠券列表
    myCoupon: [], // 我的优惠券列表
    isShowProductDetail: false, // 是否显示商品详情
    packagePrice: 0, // 餐盒费
    comment: {}, // 商户评论

  },
  onLoad: function () {
    var location = wx.getStorageSync("curLocation"),
      hasLocation = false,
      locationName = "";
    if (location) {
      hasLocation = true;
      locationName = "当前位置：" + location.address;
    }
    this.initComment();
    this.setData({
      hasLocation,
      locationName
    });

  },
  // 分享
  onShareAppMessage: function () {
    var session = qcloud.getSession();
    return {
      title: session.business.name,
      path: "/pages/launch/launch?userid=" + session.userinfo.id
    };
  },
  onShow: function () {
    var self = this,
      session = qcloud.getSession(),
      business = session.business,
      user = session.userinfo,
      currentScene = wx.getStorageSync("currentScene");

    if (currentScene === "search-product") { // 如果是从搜索页面返回
      var selectProduct = wx.getStorageSync("selectProduct");
      var scroll = `scroll_${selectProduct.menuId}_${selectProduct.viewIndex}`;

      wx.removeStorageSync("currentScene");
      wx.removeStorageSync("selectProduct");
      this.setData({
        foodToView: scroll
      });
      return;
    }

    // 重新进入店铺
    if (session.reload) {
      session.reload = false;
      this.data.enterAgain = false;
      this.data.isClosedCoupon = false;

      qcloud.setSession(session);
      this.data.initLoaded = false; // 标识数据初始化完成
      // 设置标题
      wx.setNavigationBarTitle({
        title: business.name
      });
      this.initComment();

      qcloud.request({
        url: `/business/init/${business.id}?userId=${user.id}`,
        method: "GET",
        success: function (res) {
          // 商户满减活动
          var couponList = res.data.coupon,
            myCoupon = res.data.userCoupon,
            unreceived = [];
          self.setData({
            fullReduceList: res.data.fullReduct
          });
          self.data.couponList = couponList; // 商户优惠券
          self.data.myCoupon = myCoupon; // 用户优惠券
          self.data.unreceived = unreceived; // 未领取的优惠券
          self.data.discount = res.data.discount; // 商品折扣券
          self.data.cartList = res.data.carts; // 用户购物车
          self.data.freights = res.data.freights; // 商户运费配置
          self.data.initLoaded = true; // 标识数据初始化完成
          wx.setStorageSync("myCoupon", myCoupon); // 将用户优惠券缓存起来
          wx.setStorageSync("cartList", self.data.cartList); // 将用户购物车写入缓存
          wx.setStorageSync("freights", self.data.freights); // 将配送费用写入缓存
          if (couponList.length > 0) {
            self.couponHandler();
          }
          self.calcFreightAmount();
        }
      });

    }

    // 从后台进入小程序或页面切换到点餐页
    if (this.data.enterAgain) {
      var now = Date.now(),
        lastTime = this.data.refreshTime,
        second = Math.floor((now - lastTime) / 1000),
        orderSubmit = wx.getStorageSync('orderSubmit');
      if (second >= 20 || orderSubmit) {
        // 如果距上次刷新时间已过去20秒或者提交过订单，则重新加载商户信息与菜单
        this.data.refreshTime = now;
        qcloud.request({
          url: `/user/business/${business.id}`,
          method: "GET",
          success: function (res) {
            var session = qcloud.getSession();
            session.business = res.data;
            qcloud.setSession(session);
            self.loadProduct();
          }
        });
      }
    } else {
      self.loadProduct();
      this.data.enterAgain = true;
      this.data.refreshTime = Date.now();
    }
  },
  // 加载商品与用户购物车
  loadProduct: function () {
    var self = this,
      business = qcloud.getSession().business;
    this.data.productLoaded = false; // 标识商品是否请求加载完成

    // 加载菜单类别及产品
    qcloud.request({
      url: "/product/menus/" + business.id,
      method: "GET",
      success: function (res) {
        var menuArr = [];
        var productArr = [];
        res.data.forEach(function (menuItem) {
          menuArr.push(menuItem); // 菜单类别初始化
          menuItem.products.forEach(function (product, index) {
            // 产品初始化
            product.quantity = 0; // 设置每个商品在购物车中的数量值为0
            product.viewIndex = index; // 商品序号
            product.menuId = menuItem.id; // 菜单类别id
            product.formats[0].selected = true; // 设置第一个规格为选中状态
            product.price = product.formats[0].price; // 商品的价格为第一个规格的价格
            if (product.attributes.length > 0) {
              product.attributes.forEach(function (attr) { // 格式化属性，默认选中第一个
                var i = 1;
                for (; i < 9;) {
                  if (attr['item' + i]) {
                    attr['item' + i] = {
                      name: attr['item' + i],
                      selected: false
                    };
                    if (!attr.selectedValue) {
                      attr.selectedValue = attr['item' + i].name;
                      attr['item' + i].selected = true;
                    }
                  }
                  i++;
                }
              });
            }
            productArr.push(product);
          });
        });

        self.data.menu = menuArr;
        self.data.productList = productArr;
        self.data.productLoaded = true;
      }
    });

    // 加载数据
    util.method.delayExec(function () {
      // 当购物车、商品、营销数据均加载完成后，才执行数据计算
      if (!self.data.productLoaded || !self.data.initLoaded) return false;
      self.loadData();
      return true;
    }, 200);
  },
  // 当用户每次进入页面时，重新根据购物车加载数据
  loadData: function () {
    var menu = this.data.menu,
      productList = this.data.productList,
      self = this,
      cartList = wx.getStorageSync("cartList") || [],
      cartQuantity = 0,
      packagePrice = 0,
      format,
      existCart = []; // 存储购物车与商品可以匹配的记录
    this.data.cartList = cartList;
    // 商品与折扣券关联
    this.discountHandler();
    // 商品与购物车关联
    if (cartList.length > 0) {
      cartList.forEach(function (obj) {
        var products = productList.filter(a => a.id == obj.productId);
        if (products.length === 0) return;
        var product = products[0];
        obj.product = product;
        product.quantity += obj.quantity; // 记录购物车有多少该类型的产品
        format = self.findCartFormat(obj, product); // 找到购物车对应的商品规格
        if (!format) return;
        obj.format = format;
        packagePrice += format.packingPrice * format.packingQuantity * obj.quantity; // 累加餐盒费
        existCart.push(obj);
        self.cartHandlerForSingle(obj);
      });
    }
    this.cartHandlerForAll();

    // 根据购物车，计算营销文本展示
    this.cartHandlerForSale();

    // 更新商户信息
    var business = qcloud.getSession().business,
      submitText = "去结算",
      isBalance = true;
    if (business.isClose) {
      submitText = "已暂停营业";
      isBalance = false;
    } else {
      var time1 = !!business.businessStartTime && this.calcBusinessStatus(business.businessStartTime, business.businessEndTime);
      var time2 = !!business.businessStartTime2 && this.calcBusinessStatus(business.businessStartTime2, business.businessEndTime2);
      var time3 = !!business.businessStartTime3 && this.calcBusinessStatus(business.businessStartTime3, business.businessEndTime3);
      time1 || time2 || time3 || (submitText = "未到营业时间", isBalance = false);
    }

    this.setData({
      submitText,
      isBalance,
      business,
      businessId: business.id,
      logo: business.logoSrc,
      productList,
      menu,
      cartList: existCart,
      packagePrice
    });
  },
  initComment: function () {
    var business = qcloud.getSession().business;
    let comment = {
      paging: {
        pageIndex: 0,
        pageSize: 10
      },
      comments: [],
      loaded: false,
      score: +(business.score.toFixed(1)),
      delivery: +(business.delivery.toFixed(1)),
      more: true
    };
    this.setData({
      comment
    });
  },
  // 折扣券处理
  discountHandler: function () {
    var products, productList = this.data.productList,
      menus = this.data.menu,
      now = new Date(),
      hour = now.getHours().toString(),
      minus = now.getMinutes().toString(),
      hourMinus = (hour.length > 1 ? hour : ("0" + hour)) + ":" + (minus.length > 1 ? minus : ("0" + minus)),
      weekday = weekObj[now.getDay().toString()];
    // 筛选出当前有效的优惠券
    var validCoupon = [];
    this.data.discount.forEach(function (item) {
      if (!(weekday & item.cycle)) return; // 不在循环周期内
      var isArea = false; // 是否在有效时间区间内
      if (item.startTime1 <= hourMinus && item.endTime1 >= hourMinus) isArea = true;
      else if (item.startTime2 <= hourMinus && item.endTime2 >= hourMinus) isArea = true;
      else if (item.startTime3 <= hourMinus && item.endTime3 >= hourMinus) isArea = true;
      if (!isArea) return;
      validCoupon.push(item);
    });

    if (validCoupon.length === 0) return;
    var discountProducts = [];
    // 将商品与折扣券关联
    validCoupon.forEach(function (coupon) {
      products = productList.filter(a => a.id == coupon.productId);
      if (products.length > 0) {
        products[0].discount = coupon;
        discountProducts.push(products[0]);
        var productIndex = productList.indexOf(products[0]);
        productList.splice(productIndex, 1);
      }
    });
    // 如果折扣商品大于零
    if (discountProducts.length > 0) {
      var menu = {
        name: "折扣优惠",
        id: 0,
        products: discountProducts
      };
      menus.unshift(menu);
      discountProducts.reverse().forEach(product => {
        product.menuId = menu.id;
        productList.unshift(product);
      });
    }
  },
  // 重新加载单个购物车的价格与名称（购物车加载，更新时均需要执行）
  reloadCart: function (cart) {
    var product = cart.product,
      format = cart.format,
      discount = product.discount;
    cart.name = product.name;
    if (discount) {
      var oldQuantity = cart.quantity - discount.upperLimit, // 原价商品数量
        amount = 0,
        oldPrice = cart.quantity * discount.oldPrice;
      if (oldQuantity > 0) {
        amount += discount.upperLimit * discount.price;
        amount += oldQuantity * discount.oldPrice;
        cart.discountProductQuantity = discount.upperLimit;
      } else {
        amount = cart.quantity * discount.price;
        cart.discountProductQuantity = cart.quantity;
      }

      cart.price = qcloud.utils.getNumber(amount, 2);
      cart.saleProductDiscountId = discount.id;
      cart.oldPrice = oldPrice;
      cart.discount = discount;
    } else {
      cart.price = qcloud.utils.getNumber(format.price * cart.quantity, 2);
      cart.oldPrice = cart.price;
    }
  },
  // 因为商品折扣数量限制，重新计算购物车折扣数据（初次加载，购物车改变时均需要执行，同类规格计算）
  reloadCartByDiscount: function () {
    var carts = this.data.cartList;
    if (carts.length === 0) return;
    var groups = this.cartGroupByFormat(carts),
      group, quantity = 0,
      discount, limit;
    for (var key in groups) {
      group = groups[key];
      if (group.length === 1) continue;
      group.some(cart => {
        discount = cart.discount;
        return !!discount;
      });
      if (!discount) continue;
      limit = discount.upperLimit;
      group.forEach(cart => quantity += cart.quantity);
      group.forEach(cart => {
        if (limit <= 0) {
          cart.saleProductDiscountId = null;
          cart.discountProductQuantity = null;
          cart.discount = null;
          cart.price = cart.oldPrice;
          return;
        }
        var count = limit - cart.quantity;
        if (count < 0) {
          cart.saleProductDiscountId = discount.id;
          cart.discountProductQuantity = limit;
          cart.discount = discount;
          cart.price = discount.price * limit - count * discount.oldPrice;
        }
        limit = count;
      });
    }
    wx.setStorageSync("cartList", this.data.cartList);
  },
  // 计算当前商户的营业状态
  calcBusinessStatus: function (start, end) {
    var now = new Date(),
      nowStamp = Date.parse(now);
    var startTime = start.split(":");
    var endTime = end.split(":");
    var startStamp = Date.parse(new Date(now.getFullYear(), now.getMonth(), now.getDate(), +startTime[0], +startTime[1], 0));
    var endStamp = Date.parse(new Date(now.getFullYear(), now.getMonth(), now.getDate(), +endTime[0], +endTime[1], 0));
    return nowStamp >= startStamp && nowStamp <= endStamp;
  },
  // 提示用户领取优惠券
  couponHandler: function () {
    if (this.data.isClosedCoupon) return; // 如果已经关闭过优惠券窗口，则不再弹出
    if (!this.data.couponList || !this.data.myCoupon || !this.data.unreceived) return;

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
  catLicense: function () {
    wx.navigateTo({
      url: "/pages/main/license/license"
    });
  },
  // 查看商户地址
  catAddress: function () {
    var business = qcloud.getSession().business;
    wx.openLocation({
      latitude: business.lat,
      longitude: business.lng,
      name: business.name,
      address: business.address
    });
  },
  // 拨打商户电话
  callPhone: function () {
    var business = qcloud.getSession().business;
    wx.makePhoneCall({
      phoneNumber: business.mobile
    });
  },
  // 点击跳转选项卡
  turnPage: function (e) {
    this.setData({
      currentPage: e.currentTarget.dataset.index,
      isShowFoot: e.currentTarget.dataset.index == 0
    });
  },
  // 滑动跳转选项卡
  turnTitle: function (e) {
    this.setData({
      currentPage: e.detail.current,
      isShowFoot: e.detail.current == 0
    });
    if (e.detail.current != 1) return;
    if (this.data.comment.loaded) return; // 如果已经加载过评论则退出
    this.data.comment.paging.pageIndex = 1;
    this.loadComments();
  },
  loadComments: function () {
    let business = qcloud.getSession().business,
      comment = this.data.comment,
      paging = comment.paging,
      self = this;
    this.data.comment.loaded = true;
    qcloud.request({
      url: `/business/getComments/${business.id}?pageIndex=${paging.pageIndex}&pageSize=${paging.pageSize}`,
      success: function (res) {
        res.data.list.forEach(a => comment.comments.push(a));
        comment.more = res.data.more;
        self.setData({
          comment
        });
      }
    });
  },
  commentTolower: function () {
    if (!this.data.comment.more) return;
    this.data.comment.paging.pageIndex++;
    this.loadComments();
  },
  // 点击类别跳转到指定的商品
  turnMenu: function (e) {
    var menu = this.data.menu[e.currentTarget.dataset.index];
    var currentToView = "scroll_" + menu.id + '_0';
    this.setData({
      selected: e.currentTarget.dataset.index,
      foodToView: currentToView
    });
  },
  // 关闭购物车
  closeCartDialog: function () {
    this.showCartAnimation(false);
  },
  showCart: function () {
    this.showCartAnimation(!this.data.showCart);
  },
  // 清空购物车
  clearCart: function (e) {
    var self = this;
    var session = qcloud.getSession();
    qcloud.request({
      url: `/cart?userId=${session.userinfo.id}&businessId=${session.business.id}`,
      method: "DELETE",
      success: function (res) {
        if (res.data.success) {
          self.reset();
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
  // 所有数据还原
  reset: function () {
    var productList = this.data.productList;
    productList.forEach(a => a.quantity = 0);
    wx.removeStorageSync("cartList");
    this.setData({
      productList,
      cartList: [],
      curQuantity: 0,
      cartQuantity: 0,
      packagePrice: 0
    });
  },
  // 滚动商品列表时，自动切换商品类别选择状态
  scrollFoodList: function (e) {
    var menus = this.data.menu,
      self = this;
    var query = wx.createSelectorQuery().select(".food");
    query._single = false;
    query.boundingClientRect(function (elements) {
      if (elements.length === 0) return;
      var id;
      elements.some(ele => {
        if (ele.top - 150 >= 0) {
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
  // 控制购物车的显示与隐藏
  showCartAnimation: function (isShow) {
    this.setData({
      showCart: isShow
    });
  },
  // 支付
  pay: function () {
    if (!this.data.isBalance) {
      return;
    }
    if (this.data.cartList.length === 0) {
      util.showError("请选择需要的商品再结算");
      return;
    }
    wx.setStorageSync("packagePrice", this.data.packagePrice);
    wx.setStorageSync("cartList", this.data.cartList);
    wx.setStorageSync("orderFreight", this.data.freight);
    wx.navigateTo({
      url: '/pages/pay/pay'
    });
  },
  // 显示规格、属性选择框
  showFormat: function (e) {
    var index = !!e ? e.currentTarget.dataset.index : this.data.productIndex;
    this.data.productIndex = index;
    this.setData({
      curQuantity: this.calcQuantity(),
      showDetail: true,
      productIndex: index
    });
  },
  // 在订单详情里选择规格
  showFormatAtDetail: function () {
    this.setData({
      isShowProductDetail: false
    });
    this.showFormat();
  },
  // 关闭规格、属性选择框
  closeFormat: function () {
    this.setData({
      showDetail: false
    });
  },
  attch1: function () {}, // 不要删除，有用



  
  // 加入购物车
  // add: function (e) {

  //   // wx.navigateToMiniProgram({
  //   //   appId: "wxbfaec382d63e28a8"
  //   // });
  //   // return;

  //   var user = qcloud.getSession().userinfo,
  //     self = this;
  //   if (!user.isRegister) {
  //     wx.showModal({
  //       title: "提示",
  //       content: "请先登录系统，并绑定手机号，才能开始点单哦",
  //       showCancel: false,
  //       confirmText: "去登陆",
  //       success: function () {
  //         wx.switchTab({
  //           url: "/pages/user/user"
  //         });
  //       }
  //     });
  //     return;
  //   }
  //   if (!user.isPhone) {
  //     wx.showModal({
  //       title: "提示",
  //       content: "为了给您提供更优质的服务，本店邀请您授权手机号",
  //       showCancel: false,
  //       confirmText: "去授权",
  //       success: function () {
  //         wx.switchTab({
  //           url: "/pages/user/user"
  //         });
  //       }
  //     });
  //     return;
  //   }
  //   var index = !!e ? e.currentTarget.dataset.index : this.data.productIndex;
  //   if (Object.prototype.toString.call(index) == "[object Number]") {
  //     this.data.productIndex = index;
  //   } else {
  //     this.data.curQuantity++;
  //   }
  //   this.cartHandler("add");
  //   this.setData({
  //     curQuantity: this.data.curQuantity,
  //     productIndex: this.data.productIndex
  //   });
  // },
  // 在商品详情页中添加购物车
  // addAtDetail: function () {
  //   this.setData({
  //     isShowProductDetail: false
  //   });
  //   this.add();
  // },
  // // 删除购物车
  // remove: function (e) {
  //   var index = e.currentTarget.dataset.index;
  //   if (Object.prototype.toString.call(index) == "[object Number]") {
  //     this.data.productIndex = index;
  //   } else {
  //     if (this.data.curQuantity === 0) return;
  //     this.data.curQuantity--;
  //   }
  //   this.cartHandler("remove");
  //   this.setData({
  //     curQuantity: this.data.curQuantity,
  //     productIndex: this.data.productIndex
  //   });
  // },
  // 选择规格
  selectFormat: function (e) {
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
  // 选择属性
  selectAttribute: function (e) {
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
  // 在购物车中增加同类商品数量
  // addCart: function (e) {
  //   var index = e.currentTarget.dataset.index,
  //     self = this,
  //     cart = this.data.cartList[index],
  //     product = this.data.productList.filter(a => a.id == cart.productId)[0],
  //     productQuantity = product.quantity,
  //     packagePrice = self.data.packagePrice,
  //     quantity = cart.quantity + 1;
  //   qcloud.request({
  //     url: "/user/updateCart/" + cart.id + "?quantity=" + quantity,
  //     method: "GET",
  //     success: function (res) {
  //       if (res.data.success) {
  //         cart.quantity = quantity;
  //         product.quantity = productQuantity + 1;
  //         packagePrice += cart.format.packingPrice;
  //         self.reloadCart(cart);
  //         self.reloadCartByDiscount();
  //         self.setData({
  //           productList: self.data.productList,
  //           cartList: self.data.cartList,
  //           packagePrice
  //         });
  //         self.calcCartQuantity();
  //       } else {
  //         util.showError(res.msg);
  //       }
  //     },
  //     fail: function () {
  //       util.showModel("错误", "请求错误，请检查网络连接");
  //     }
  //   });
  // },
  // // 在购物车中减少同类商品数量
  // removeCart: function (e) {
  //   var index = e.currentTarget.dataset.index,
  //     self = this,
  //     cart = this.data.cartList[index],
  //     quantity = cart.quantity - 1,
  //     product = this.data.productList.filter(a => a.id == cart.productId)[0],
  //     productQuantity = product.quantity,
  //     packagePrice = this.data.packagePrice;
  //   qcloud.request({
  //     url: "/user/updateCart/" + cart.id + "?quantity=" + quantity,
  //     method: "GET",
  //     success: function (res) {
  //       if (res.data.success) {
  //         cart.quantity = quantity;
  //         if (quantity <= 0) {
  //           self.data.cartList.splice(index, 1);
  //         } else {
  //           self.reloadCart(cart);
  //         }
  //         product.quantity = productQuantity - 1;
  //         packagePrice -= cart.format.packingPrice;
  //         self.reloadCartByDiscount();
  //         self.setData({
  //           productList: self.data.productList,
  //           cartList: self.data.cartList,
  //           packagePrice
  //         });
  //         self.calcCartQuantity();
  //       } else {
  //         // util.showError(res.msg);
  //       }
  //     },
  //     fail: function () {
  //       util.showModel("错误", "请求错误，请检查网络连接");
  //     }
  //   });
  // },
  // 领取优惠券
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
  // 关闭优惠券窗口
  closeCoupon: function () {
    this.setData({
      isShowCoupon: false,
      isClosedCoupon: true
    });
    wx.showTabBar();
  },
  // 打开商品详情
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
  // 关闭商品详情
  closeProduct: function () {
    this.setData({
      isShowProductDetail: false
    });
  },
  search: function (e) {
    wx.setStorageSync("cartList", this.data.cartList);
    wx.setStorageSync("productList", this.data.productList);
    wx.navigateTo({
      url: '/pages/searchProduct/searchProduct'
    });
  },


  // 以下均为方法，不代表各类事件
  // cartHandler: function (flag) {
  //   var self = this,
  //     business = qcloud.getSession().business,
  //     product = this.data.productList[this.data.productIndex], // 操作的商品
  //     cart, // 购物车
  //     cartIndex = -1, // 当前选择的购物车在列表中的序号
  //     description = "", // 购物车描述
  //     format = product.formats.filter(a => a.selected)[0], // 选中的规格
  //     packagePrice = this.data.packagePrice; // 当前包装费

  //   if (flag === "add") {
  //     product.quantity++;
  //     packagePrice += format.packingPrice;
  //   } else {
  //     product.quantity--;
  //     packagePrice -= format.packingPrice;
  //   }
  //   this.setData({ // 尽早更新商品数量
  //     productList: self.data.productList,
  //   });
  //   // 处理购物车描述
  //   description = this.calcDescription(product);
  //   // 处理购物车
  //   this.data.cartList.forEach((obj, index) => {
  //     if (obj.productId === product.id && obj.description === description) {
  //       cart = obj;
  //       cartIndex = index;
  //       return false;
  //     }
  //   });
  //   if (!cart) { // 不存在相同的购物车信息
  //     var imgSrc = product.images.length > 0 ? (product.images[0].name + "." + product.images[0].extensionName) : null;
  //     var discount = product.discount;
  //     cart = {
  //       name: product.name,
  //       src: imgSrc,
  //       quantity: 1,
  //       productId: product.id,
  //       product: product,
  //       formatId: format.id,
  //       format: format,
  //       packingQuantity: format.packingQuantity,
  //       saleProductDiscountId: discount ? discount.id : null,
  //       description: description,
  //       userId: qcloud.getSession().userinfo.id,
  //       businessId: business.id
  //     };
  //     if (discount) {
  //       util.showError("折扣商品不参与满减活动");
  //     }
  //     this.data.cartList.push(cart);
  //   } else {
  //     if (flag === "add") cart.quantity++;
  //     else cart.quantity--;
  //   }
  //   this.reloadCart(cart);
  //   var cartClone = qcloud.utils.extend({}, cart);
  //   cartClone.product = null;
  //   cartClone.discount = null;
  //   cartClone.format = null;
  //   // 请求服务器
  //   qcloud.request({
  //     url: "/user/carthandler",
  //     method: "POST",
  //     data: cartClone,
  //     success: function (res) {
  //       cart.id = res.data.id;
  //       if (cart.quantity <= 0) {
  //         self.data.cartList.splice(cartIndex, 1);
  //       }
  //       self.reloadCartByDiscount();
  //       self.setData({
  //         cartList: self.data.cartList,
  //         packagePrice
  //       });
  //       self.calcCartQuantity();
  //     },
  //     fail: function (error) {
  //       util.showModel("错误", "请求错误，请检查网络连接");
  //     }
  //   });
  // },
  // 选择规格或属性时，计算当前已经添加同类商品的数量
  calcQuantity: function () {
    var description = this.calcDescription(),
      productId = this.data.productList[this.data.productIndex].id,
      cart = this.data.cartList.filter(a => a.description == description && a.productId == productId);
    if (cart.length > 0) {
      return cart[0].quantity;
    }
    return 0;
  },
  // 拿到商品的规格、属性组合
  calcDescription: function (product) {
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
  // 计算购物车商品总数和满减显示文本
  // calcCartQuantity: function () {
  //   var cartQuantity = 0;
  //   this.data.cartList.forEach(a => cartQuantity += a.quantity);
  //   this.setData({
  //     cartQuantity: cartQuantity
  //   });
  //   this.calcSaleText();
  // },
  // 获取购物车数量
  // getCartQuantity: function () {
  //   var quantity = 0;
  //   this.data.cartList.forEach(a => quantity += a.quantity);
  //   return quantity;
  // },
  // 获取包装费
  getPackageAmount: function () {
    var amount = 0;
    this.data.cartList.forEach(a => {
      amount += a.format.packingPrice * a.format.packingQuantity * a.quantity;
    });
    return amount;
  },
  // 根据购物车内容，计算满减活动的文本
  calcSaleText: function () {
    var fullReduceList = this.data.fullReduceList.slice(),
      nowItem;
    wx.removeStorageSync("nowFullReduce"); // 移除当前实现的满减活动
    if (this.data.cartList.length === 0 || fullReduceList.length === 0) return;
    // 折扣商品不参与满减
    var total = 0,
      list = fullReduceList.reverse(),
      text = "";
    total = qcloud.utils.getNumber(this.fullAmount() + this.data.packagePrice, 2);
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
  // 计算购物车参与满减的价格（折扣商品不参与满减）
  fullAmount: function () {
    var carts = this.data.cartList;
    if (carts.length === 0) return;
    // 按规格分组
    var groups = this.cartGroupByFormat(carts);
    // 计算总额
    var arr, quantity, amount = 0,
      overQuantity;
    for (var key in groups) {
      arr = groups[key];
      quantity = 0;
      if (!arr[0].discount) {
        arr.forEach(function (cart) {
          amount += cart.price;
        });
        continue;
      }
      arr.forEach(function (cart) {
        quantity += cart.quantity;
      });
      overQuantity = quantity - arr[0].discount.upperLimit;
      if (overQuantity > 0) {
        amount += overQuantity * arr[0].discount.oldPrice;
      }
    }
    return amount;
  },
  cartGroupByFormat: function (carts) {
    var groups = {};
    carts.forEach(function (cart) {
      var group = groups[cart.formatId];
      if (group) {
        group.push(cart);
      } else {
        group = [cart];
        groups[cart.formatId] = group;
      }
    });
    return groups;
  },
  // 找到购物车中对应商品的规格
  findCartFormat: function (cart, product) {
    var formats = product.formats.filter(a => a.id === cart.formatId);
    if (!formats.length) return null;
    return formats[0];
  },
  calcFreightAmount: (function () {
    function getFreight(lng, lat) {
      var business = qcloud.getSession().business,
        freight = business.freight;
      var distance = util.calcDistance({
        lat: lat,
        lng: lng
      }, {
        lat: business.lat,
        lng: business.lng
      });
      var result = this.data.freights.some(function (item) {
        if (item.maxDistance * 1000 >= distance) {
          freight = item.amount;
          return true;
        }
        return false;
      });
      if (!result) {
        freight = this.data.freights[this.data.freights.length - 1].amount;
      }
      return freight;
    }

    return function () {
      var that = this,
        business = qcloud.getSession().business,
        address = wx.getStorageSync("selectAddress"); // 用户选择的默认地址
      // 配送费计算
      var freight;
      if (this.data.freights.length > 0) {
        if (address) {
          freight = getFreight.call(this, address.lng, address.lat);
          this.setData({
            freight: freight
          });
        } else {
          wx.getLocation({
            type: "wgs84",
            success: function (res) {
              var lat = res.latitude,
                lng = res.longitude;
              freight = getFreight.call(that, lng, lat);
            },
            fail: function (err) {
              util.showError(err);
            },
            complete: function () {
              that.setData({
                freight: freight
              });
            }
          });
        }
      } else {
        this.setData({
          freight: business.freight
        });
      }
    };
  })(),

  loginValidate: function () {
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
      return false;
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
      return false;
    }
    return true;
  },




  /* 
    新版购物车操作
  */
  // 增加商品
  addProduct: function (e) {
    if (!this.loginValidate()) return; // 验证用户登录状态
    var user = qcloud.getSession().userinfo,
      product;
    if (!e.currentTarget) {
      product = e;
    } else {
      product = this.data.productList[e.currentTarget.dataset.index];
    }
    var self = this,
      description = this.calcDescription(product), // 拿到商品当前已选中的描述
      cart; // 购物车变量
    this.data.cartList.forEach(obj => {
      if (obj.productId === product.id && obj.description === description) { // 购物车中商品id与描述一致的商品，归类为一条购物车记录
        cart = obj;
        return false;
      }
    });
    if (!cart) {
      let format = product.formats.filter(a => a.selected)[0];
      let imgSrc = product.images.length > 0 ? (product.images[0].name + "." + product.images[0].extensionName) : null;
      let discount = product.discount;
      cart = {
        id: 0,
        name: product.name,
        src: imgSrc,
        quantity: 0,
        productId: product.id,
        product: product,
        formatId: format.id,
        format: format,
        packingQuantity: format.packingQuantity,
        saleProductDiscountId: discount ? discount.id : null,
        description: description,
        userId: user.id,
        businessId: product.businessId
      };
      this.data.cartList.push(cart);
    } else {
      if (!cart.id) return; // 如果购物车还没有获取到id，则本次操作取消
    }
    product.quantity++; // 添加后的商品数量
    cart.quantity++;
    this.cartChange(cart);

    if (!cart.id) { // 购物车不存在于数据库中，则新增购物车
      var cartClone = qcloud.utils.extend({}, cart, {
        product: null,
        format: null,
        discount: null
      });
      // 请求服务器
      qcloud.request({
        url: "/cart",
        method: "POST",
        data: cartClone,
        success: function (res) {
          self.getCart(cart).id = res.data;
        },
        fail: function (error) {
          util.showModel("错误", "请求错误，请检查网络连接");
        }
      });
    } else { // 购物车已存在于数据库中，则修改购物车数量
      util.method.throttle(this.updateCartHttp, 500, this, cart);
    }
  },
  deleteProduct: function (e) {
    var product;
    if (!e.currentTarget) {
      product = e;
    } else {
      product = this.data.productList[e.currentTarget.dataset.index];
    }
    var description = this.calcDescription(product), // 拿到商品当前已选中的描述
      cart, cartIndex;
    this.data.cartList.forEach((obj, num) => {
      if (obj.productId === product.id && obj.description === description) { // 购物车中商品id与描述一致的商品，归类为一条购物车记录
        cart = obj;
        cartIndex = num;
        return false;
      }
    });
    if (!cart.id) return; // 如果购物车还没有获取到id，则本次操作取消

    product.quantity--;
    cart.quantity--;
    if (cart.quantity === 0) {
      this.data.cartList.splice(cartIndex, 1);
    }
    this.cartChange(cart);

    util.method.throttle(this.updateCartHttp, 500, this, cart);
  },
  // 在购物车中增加同类商品数量
  increaseCart: function (e) {
    var cart = this.data.cartList[e.currentTarget.dataset.index],
      product = this.data.productList.filter(a => a.id == cart.productId)[0];

    if (!cart.id) return; // 如果购物车还没有获取到id，则本次操作取消

    product.quantity++;
    cart.quantity++;
    this.cartChange(cart);

    util.method.throttle(this.updateCartHttp, 500, this, cart);
  },
  // 在购物车中减少同类商品数量
  decreaseCart: function (e) {
    var cart = this.data.cartList[e.currentTarget.dataset.index],
      product = this.data.productList.filter(a => a.id == cart.productId)[0];

    if (!cart.id) return; // 如果购物车还没有获取到id，则本次操作取消

    product.quantity--;
    if (product.quantity < 0) product.quantity = 0;
    cart.quantity--;
    if (cart.quantity <= 0) {
      this.data.cartList.splice(e.currentTarget.dataset.index, 1);
      cart.quantity = 0;
    }
    this.cartChange(cart);

    util.method.throttle(this.updateCartHttp, 500, this, cart);
  },
  // 在商品详情页中添加购物车
  addProductOnInfo: function (e) {
    this.setData({
      isShowProductDetail: false
    });
    var product = this.data.productList.filter(a => a.id === e.currentTarget.dataset.id)[0];
    this.addProduct(product);
  },
  // 在规格选择页减少商品
  reduceProductOnFormat: function (e) {
    var index = e.currentTarget.dataset.index,
      product = this.data.productList[index],
      quantity = this.data.curQuantity;
    quantity--;
    if (quantity < 0) return;
    this.deleteProduct(product);
    this.setData({
      curQuantity: quantity
    });
  },
  // 在规格选择页增加商品
  increaseProductOnFormat: function (e) {
    var index = e.currentTarget.dataset.index,
      product = this.data.productList[index],
      quantity = this.data.curQuantity;
    quantity++;
    this.addProduct(product);
    this.setData({
      curQuantity: quantity
    });
  },
  // 购物车改变之后更新视图
  cartChange: function (cart) {
    this.cartHandlerForSingle(cart); // 重新计算单个购物车数据指标（针对购物车中折扣商品计算）
    this.cartHandlerForAll(); // 重新计算整体购物车数据指标（针对购物车中存在多个同类折扣商品的情况处理）
    this.cartHandlerForSale(); // 计算满减活动

    this.setData({
      packagePrice: this.getPackageAmount(),
      cartList: this.data.cartList,
      productList: this.data.productList
    });
  },
  // 单个购物车处理（购物车初次加载与更新时需要执行）
  cartHandlerForSingle: function (cart) {
    var product = cart.product,
      format = cart.format,
      discount = product.discount;
    cart.name = product.name;
    if (discount) {
      var oldQuantity = cart.quantity - discount.upperLimit, // 原价商品数量
        amount = 0,
        oldPrice = cart.quantity * discount.oldPrice;
      if (oldQuantity > 0) {
        amount += discount.upperLimit * discount.price;
        amount += oldQuantity * discount.oldPrice;
        cart.discountProductQuantity = discount.upperLimit;
      } else {
        amount = cart.quantity * discount.price;
        cart.discountProductQuantity = cart.quantity;
      }

      cart.price = qcloud.utils.getNumber(amount, 2);
      cart.saleProductDiscountId = discount.id;
      cart.oldPrice = oldPrice;
      cart.discount = discount;
    } else {
      cart.price = qcloud.utils.getNumber(format.price * cart.quantity, 2);
      cart.oldPrice = cart.price;
    }
  },
  // 每次购物车发生变化时，对所有的购物车记录重新计算处理一次（购物车初次加载与更新时需要执行）
  cartHandlerForAll: function () {
    var carts = this.data.cartList;
    if (carts.length === 0) return;
    var groups = this.cartGroupByFormat(carts),
      group, quantity = 0,
      discount, limit;
    for (var key in groups) {
      group = groups[key];
      if (group.length === 1) continue;
      group.some(cart => {
        discount = cart.discount;
        return !!discount;
      });
      if (!discount) continue;
      limit = discount.upperLimit;
      group.forEach(cart => quantity += cart.quantity);
      group.forEach(cart => {
        if (limit <= 0) {
          cart.saleProductDiscountId = null;
          cart.discountProductQuantity = null;
          cart.discount = null;
          cart.price = cart.oldPrice;
          return;
        }
        var count = limit - cart.quantity;
        if (count < 0) {
          cart.saleProductDiscountId = discount.id;
          cart.discountProductQuantity = limit;
          cart.discount = discount;
          cart.price = discount.price * limit - count * discount.oldPrice;
        }
        limit = count;
      });
    }
    // 处理完成后，保存到缓存
    wx.setStorageSync("cartList", this.data.cartList);
  },
  // 处理购物车营销活动
  cartHandlerForSale: function () {
    var fullReduceList = this.data.fullReduceList.slice(),
      nowItem;
    wx.removeStorageSync("nowFullReduce"); // 移除当前实现的满减活动
    if (this.data.cartList.length === 0 || fullReduceList.length === 0) return;
    // 折扣商品不参与满减
    var total = 0,
      list = fullReduceList.reverse(),
      text = "";
    total = qcloud.utils.getNumber(this.fullAmount() + this.data.packagePrice, 2);
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
  // 更新服务器购物车数量
  updateCartHttp: function (cart) {
    qcloud.request({
      url: "/cart",
      method: "PUT",
      data: {
        id: cart.id,
        quantity: cart.quantity,
        price: cart.price
      }
    });
  },
  // 根据cart对象，获取页面缓存中的cart
  getCart: function (cart) {
    return this.data.cartList.filter(a => a.productId === cart.productId && a.description === cart.description)[0];
  }


});