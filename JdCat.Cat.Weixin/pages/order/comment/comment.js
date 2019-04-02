const qcloud = require("../../../vendor/wafer2-client-sdk/index");
const util = require("../../../utils/util");
const levelText = {
  "1": "很差",
  "2": "一般",
  "4": "满意",
  "8": "非常满意",
  "16": "无可挑剔"
};

Page({

  data: {
    business: {},
    achieveTime: "",
    levelText,
    level: [1, 2, 4, 8, 16],
    delivery: {
      level: 0,
      result: "",
      options: [],
      text: ""
    },
    comment: {
      level: 0,
      result: "",
      content: "",
      options: [],
      text: ""
    },
    products: [],
    order: {},
    disabled: false,
    isSubmit: false
  },
  onLoad: function (options) {
    var business = qcloud.getSession().business;
    var order = wx.getStorageSync("orderComment");
    this.data.order = order;
    
    var products = order.products.map(a => {
      return {
        id: a.id,
        name: a.name,
        zan: false,
        cai: false
      };
    });
    var achieveTime = util.formatTime(new Date(+/\d+/g.exec(order.achieveTime)[0]));
    this.setData({
      products,
      achieveTime,
      business
    });
  },
  deliveryScore: function (e) {
    var delivery = this.data.delivery;
    delivery.level = e.currentTarget.dataset.index;
    delivery.options = util.appData.deliveryComment[delivery.level].map(a => {
      return {
        text: a,
        check: false
      };
    });
    delivery.result = "";
    delivery.text = levelText[delivery.level] || "";
    this.setData({
      delivery
    });
  },
  orderScore: function (e) {
    var comment = this.data.comment;
    comment.level = e.currentTarget.dataset.index;
    comment.options = util.appData.orderComment[comment.level].map(a => {
      return {
        text: a,
        check: false
      };
    });
    comment.result = "";
    comment.text = levelText[comment.level] || "";
    this.setData({
      comment
    });
  },
  input: function (e) {
    var comment = this.data.comment;
    comment.content = e.detail.value;
  },
  deliveryCheck: function (e) {
    let delivery = this.data.delivery;
    this.checkOption(delivery, e.currentTarget.dataset.item);
    this.setData({
      delivery
    });
  },
  orderCheck: function (e) {
    let comment = this.data.comment;
    this.checkOption(comment, e.currentTarget.dataset.item);
    this.setData({
      comment
    });
  },
  zan: function (e) {
    var products = this.data.products;
    var product = products.filter(a => a.id == e.currentTarget.dataset.productid)[0];
    product.zan = true;
    product.cai = false;
    this.setData({
      products
    });
  },
  cai: function (e) {
    var products = this.data.products;
    var product = products.filter(a => a.id == e.currentTarget.dataset.productid)[0];
    product.zan = false;
    product.cai = true;
    this.setData({
      products
    });
  },
  submit: function () {
    if(this.data.isSubmit) return;
    this.data.isSubmit = true;
    let order = this.data.order, user = qcloud.getSession().userinfo, self = this;
    order.status = 512;
    let post = {
      userName: user.nickName,
      face: user.avatarUrl,
      phone: user.phone,
      arrivedTime: this.data.order.achieveTime,
      deliveryScore: this.data.delivery.level,
      deliveryType: this.data.order.logisticsType,
      deliveryResult: this.data.delivery.result,
      commentContent: this.data.comment.content,
      commentResult: this.data.comment.result,
      orderScore: this.data.comment.level,
      businessId: this.data.order.businessId,
      orderId: this.data.order.id,
      userId: user.id,
      orderProducts: this.data.products.map(a => {
        return {
          id: a.id,
          commentResult: a.zan ? 1 : (a.cai ? 2 : 0)
        };
      })
    };

    if(!post.deliveryScore || !post.orderScore) {
      util.showError("请评分后再提交");
      return;
    }
    
    qcloud.request({
      url: `/order/comment`,
      data: post,
      method: "POST",
      success: function (res) {
        var res = res.data;
        if(!res.success) {
          util.showError(res.msg);
          self.data.isSubmit = false;
          return;
        }
        wx.setStorageSync("orderComment", order);

        util.showSuccess("谢谢您的评价！");
        setTimeout(function(){
          wx.navigateBack({
            delta: 1
          });
        }, 2000);
      }
    });
  },
  checkOption: function (obj, item) {
    let option = obj.options.filter(a => a.text === item.text)[0];
    option.check = !option.check;
    let selecteds = obj.options.filter(a => a.check);
    obj.result = "";
    if (selecteds.length > 0) {
      selecteds.forEach(a => {
        obj.result += a.text + ",";
      });
      obj.result = obj.result.substring(0, obj.result.length - 1);
    }
  }
})