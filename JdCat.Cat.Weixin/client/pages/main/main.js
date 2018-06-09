Page({

  /**
   * 页面的初始数据
   */
  data: {
    swiperTitle: [{
      text: "点菜",
      id: 1
    }, {
      text: "评价",
      id: 2
    }, {
      text: "商家",
      id: 3
    }],
    menu: [],
    productList: [],
    cartList: [],
    currentPage: 0,
    selected: 0,
    foodSelected: 0,
    howMuch: 12,
    totalPrice: 0,
    hideCart: true,
    hideItem: true,
    pullBar: false,
    animationData: "",
    location: "",
    foodToView: "scroll21",
    scrollTop: 0
  },

  finish: function () {
    var that = this;
    wx.request({
      url: "https://www.easy-mock.com/mock/5afff442c20d695226befb23/JdCat/api/business/filter",
      method: "GET",
      success: function (res) {
        that.setData({
          restaurant: res.data.data.restaurant,
        })
      }
    });
  },
  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function (options) {
    var that = this;
    wx.request({
      url: "https://www.jiandanmao.cn/api/product/menus?businessId=" + 1,
      method: "GET",
      success: function (res) {
        var menuArr = new Array();
        var productArr = new Array();
        for (var i in res.data) {
          var menuItem = res.data[i];
          var menu = {};
          menu.ID = menuItem.ID;
          menu.Name = menuItem.Name;
          menu.Description = menuItem.Description;
          menu.Sort = menuItem.Sort;
          menuArr.push(menu);

          for (var j in menuItem.Products) {
            var productItem = menuItem.Products[j];
            var product = {};
            product.ID = productItem.ID;
            product.MenuId = menuItem.ID;
            product.Name = productItem.Name;
            product.Description = productItem.Description;
            product.Formats = productItem.Formats;
            product.Attributes = productItem.Attributes;
            product.Count = 0;
            product.ViewIndex = j;
            productArr.push(product);
          }
        };
        that.setData({
          productList: productArr,
          menu: menuArr,
          location: wx.getStorageSync('location')
        });
      }
    });
  },

  /**
   * 生命周期函数--监听页面初次渲染完成
   */
  onReady: function () {

  },

  /**
   * 生命周期函数--监听页面显示
   */
  onShow: function () {

  },

  /**
   * 生命周期函数--监听页面隐藏
   */
  onHide: function () {

  },

  /**
   * 生命周期函数--监听页面卸载
   */
  onUnload: function () {

  },

  /**
   * 页面相关事件处理函数--监听用户下拉动作
   */
  onPullDownRefresh: function () {

  },

  /**
   * 页面上拉触底事件的处理函数
   */
  onReachBottom: function () {

  },

  /**
   * 用户点击右上角分享
   */
  onShareAppMessage: function () {

  },
  pullBar: function () {
    this.setData({
      pullBar: !this.data.pullBar
    })
  }
  ,
  addFood: function (e) {
    var productItems = this.data.productList;
    productItems[e.currentTarget.dataset.index].Count++;
    var productItem = productItems[e.currentTarget.dataset.index];
    var cartItems = this.data.cartList;
    if (cartItems.length == 0) {
      cartItems.push(productItem)
    } else {
      var exist = false;
      for (var i in cartItems) {
        if (cartItems[i].ID == productItem.ID) {
          cartItems[i].Count++;
          exist = true;
          break;
        }
      }
      if (!exist) {
        cartItems.push(productItem);
      }
    }
    this.setData({
      productList: productItems,
      cartList: cartItems
    })
  },
  removeFood: function (e) {
    var productItems = this.data.productList;
    if (productItems[e.currentTarget.dataset.index].Count != 0) {
      productItems[e.currentTarget.dataset.index].Count--;
      var productItem = productItems[e.currentTarget.dataset.index];
      var cartItems = this.data.cartList;
      if (cartItems.length == 0) {
        return;
      } else {
        for (var i in cartItems) {
          if (cartItems[i].ID == productItem.ID) {
            cartItems[i].Count--;
          }
          if (cartItems[i].Count == 0) {
            cartItems.splice(i, 1);
          }
        }
      }
      this.setData({
        productList: productItems,
        cartList: cartItems
      })
    }
  },
  turnPage: function (e) {
    this.setData({
      currentPage: e.currentTarget.dataset.index
    })
  },
  turnTitle: function (e) {
    if (e.detail.source == "touch") {
      this.setData({
        currentPage: e.detail.current
      })
    }
  },
  turnMenu: function (e) {
    var menu = this.data.menu[e.currentTarget.dataset.index];
    var currentToView = "scroll" + menu.ID + 0;
    this.setData({
      selected: e.currentTarget.dataset.index,
      foodToView: currentToView
    })
    console.log(e.currentTarget.dataset.index);
  },
  maskCancel: function () {
    this.setData({
      hideCart: true
    })
  },
  showCart: function () {
    this.setData({
      hideCart: false
    })
  },
  showFoodDetail: function (e) {
    this.setData({
      foodSelected: e.currentTarget.dataset.index
    })
    this.setData({
      hideItem: false
    })
  },
  hideItem: function (e) {
    this.setData({
      hideItem: true
    })
  },
  addFoodInCart: function (e) {
    var cartItems = this.data.cartList;
    cartItems[e.currentTarget.dataset.index].Count++;
    var cartItem = cartItems[e.currentTarget.dataset.index];
    var productItems = this.data.productList;
    for (var i in productItems) {
      if (productItems[i].ID == cartItem.ID) {
        productItems[i].Count++;
      }
    }
    this.setData({
      productList: productItems,
      cartList: cartItems
    })
  },
  removeFoodInCart: function (e) {
    var cartItems = this.data.cartList;
    var cartItem = cartItems[e.currentTarget.dataset.index];
    var productItems = this.data.productList;
    for (var i in cartItems) {
      if (cartItems[i].ID == cartItem.ID) {
        cartItems[i].Count--;

        if (cartItems[i].Count == 0) {
          cartItems.splice(i, 1);
        }
      }
    }
    for (var i in productItems) {
      if (productItems[i].ID == cartItem.ID) {
        productItems[i].Count--;
      }
    }
    this.setData({
      productList: productItems,
      cartList: cartItems
    })
  },
  clearCart: function (e) {
    var productItems = this.data.productList;
    for (var i in productItems) {
      if (productItems[i].Count != 0) {
        productItems[i].Count = 0;
      }
    }
    this.setData({
      productList: productItems,
      cartList: []
    })
  },
  addFoodInDetail: function (e) {
    var foodSelected = this.data.foodSelected;
    var cartItems = this.data.cartList;
    var productItems = this.data.productList;
    var product = productItems[foodSelected];

    for (var i in productItems) {
      if (productItems[i].ID == product.ID) {
        productItems[i].Count++;
        break;
      }
    }

    if (cartItems.length == 0) {
      cartItems.push(product)
    } else {
      var exist = false;
      for (var i in cartItems) {
        if (cartItems[i].ID == product.ID) {
          cartItems[i].Count++;
          exist = true;
          break;
        }
      }
      if (!exist) {
        cartItems.push(product);
      }
    }
    this.setData({
      productList: productItems,
      cartList: cartItems
    })
  },
  removeFoodInDetail: function (e) {
    var foodSelected = this.data.foodSelected;
    var cartItems = this.data.cartList;
    var productItems = this.data.productList;
    var product = productItems[foodSelected];

    for (var i in productItems) {
      if (productItems[i].ID == product.ID) {
        productItems[i].Count--;
      }
    }
    for (var i in cartItems) {
      if (cartItems[i].ID == product.ID) {
        cartItems[i].Count--;
        if (cartItems[i].Count == 0) {
          cartItems.splice(i, 1);
        }
      }
    }
    this.setData({
      productList: productItems,
      cartList: cartItems
    })
  }
})