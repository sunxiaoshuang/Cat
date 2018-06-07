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
    showCart: false,
    showDetail: false,
    pullBar: false,
    animationData: "",
    location: "",
    foodToView: "scroll21",
    scrollTop: 0,
    imageUrl: "http://f.jiandanmao.cn/File/Product/",
    formatIndex:0
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
      url: "https://www.jiandanmao.cn/api/Product/menus?businessId="+ 1,
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
            product.Images = productItem.Images;
            product.Count = 0;
            product.ViewIndex = j;
            productArr.push(product);
          }
        };
        for (var m in productArr){
          for (var n in productArr[m].Formats){
            productArr[m].Formats[n].Count = 0;
            productArr[m].Formats[n].Uid = getUuid();
          }
        }
        that.setData({
          productList: productArr,
          menu: menuArr,
          location: wx.getStorageSync('location')
        });
        function getUuid() {
          var s = [];
          var hexDigits = "0123456789abcdef";
          for (var i = 0; i < 36; i++) {
              s[i] = hexDigits.substr(Math.floor(Math.random() * 0x10), 1);
          }
          s[14] = "4";  
          s[19] = hexDigits.substr((s[19] & 0x3) | 0x8, 1);
          s[8] = s[13] = s[18] = s[23] = "-";
       
          var uuid = s.join("");
          return uuid;
        }
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
  },
  addFood: function (e) {
    var formatIndex = this.data.formatIndex;
    var productItems = this.data.productList;
    productItems[e.currentTarget.dataset.index].Count++;
    var productItem = productItems[e.currentTarget.dataset.index];
    if(productItem.Formats[formatIndex].Count == undefined){
      productItem.Formats[formatIndex].Count = 0;
    }
    productItem.Formats[formatIndex].Count++;
    var cartItems = this.data.cartList;
    if (cartItems.length == 0) {
      cartItems.push(productItem)
    } else {
      var exist = false;
      for (var i in cartItems) {
        if (cartItems[i].ID == productItem.ID) {
          cartItems[i].Count++;
          cartItems[i].Formats[formatIndex].Count++;
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
    var formatIndex = this.data.formatIndex;
    var productItems = this.data.productList;
    if (productItems[e.currentTarget.dataset.index].Count != 0) {
      productItems[e.currentTarget.dataset.index].Count--;
      var productItem = productItems[e.currentTarget.dataset.index];
      productItem.Formats[formatIndex].Count--;
      var cartItems = this.data.cartList;
      if (cartItems.length == 0) {
        return;
      } else {
        for (var i in cartItems) {
          if (cartItems[i].ID == productItem.ID) {
            cartItems[i].Formats[formatIndex].Count--;
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
  closeCartDialog: function () {
    this.setData({
      showCart: false
    })
  },
  showCart: function () {
    this.setData({
      showCart: !this.data.showCart
    })
  },
  showFoodDetail: function (e) {
    this.setData({
      foodSelected: e.currentTarget.dataset.index
    })
    this.setData({
      showDetail: true
    })
  },
  closeDetail: function (e) {
    this.setData({
      showDetail: false,
      formatIndex: 0
    })
  },
  addFoodInCart: function (e) {
    var cartItems = this.data.cartList;
    var productItems = this.data.productList;
    var uid = e.currentTarget.dataset.index;

    for (var i in productItems) {
      for (var j in productItems[i].Formats) {
        if(productItems[i].Formats[j].Uid == uid){
          productItems[i].Formats[j].Count++;
          productItems[i].Count++;
        }      
      }
    }

    for (var i in cartItems) {
      for (var j in cartItems[i].Formats) {
        if(cartItems[i].Formats[j].Uid == uid){
          cartItems[i].Formats[j].Count++;
        }      
      }
    }

    this.setData({
      productList: productItems,
      cartList: cartItems
    })
  },
  removeFoodInCart: function (e) {
    var cartItems = this.data.cartList;
    var productItems = this.data.productList;
    var uid = e.currentTarget.dataset.index;

    for (var i in productItems) {
      for (var j in productItems[i].Formats) {
        if(productItems[i].Formats[j].Uid == uid){
          productItems[i].Formats[j].Count--;
          productItems[i].Count--;
        }      
      }
    }

    for (var i in cartItems) {
      for (var j in cartItems[i].Formats) {
        if(cartItems[i].Formats[j].Uid == uid){
          cartItems[i].Formats[j].Count--;
        }      
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
        for (var j in productItems[i].Formats){
          productItems[i].Formats[j].Count = 0;
        }
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
    var formatIndex = this.data.formatIndex;
    var product = productItems[foodSelected];

    for (var i in productItems) {
      if (productItems[i].ID == product.ID) {
        if(productItems[i].Formats[formatIndex].Count == undefined){
          productItems[i].Formats[formatIndex].Count = 0;
        }
        productItems[i].Formats[formatIndex].Count++;
        
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
          if(cartItems[i].Formats[formatIndex].Count == undefined){
            cartItems[i].Formats[formatIndex].Count = 0;
          }
          cartItems[i].Formats[formatIndex].Count++;

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
    var formatIndex = this.data.formatIndex;

    for (var i in productItems) {
      if (productItems[i].ID == product.ID) {
        productItems[i].Formats[formatIndex].Count--;
        productItems[i].Count--;
      }
    }
    for (var i in cartItems) {
      if (cartItems[i].ID == product.ID) {
        cartItems[i].Formats[formatIndex].Count--;
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
  },
  selectFormat: function(e){
    this.setData({
      formatIndex: e.currentTarget.dataset.index
    })
  },
  scrollFoodList: function(e){
    console.log(this.data.foodToView);
  }
})