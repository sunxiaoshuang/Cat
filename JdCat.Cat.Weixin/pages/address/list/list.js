const config = require("../../../config");
const util = require("../../../utils/util");
const qcloud = require('../../../vendor/wafer2-client-sdk/index');
Page({

  /**
   * 页面的初始数据
   */
  data: {
    list: [],
    startX: 0,
    startY: 0, 
    items: []
  },

  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function (options) {
    wx.setNavigationBarTitle({
      title: "地址列表"
    });
    this.data.flag = options.flag;
  },

  /**
   * 生命周期函数--监听页面显示
   */
  onShow: function () {
    var userinfo = qcloud.getSession().userinfo;
    var self = this, addressList = wx.getStorageSync("addressList");
    if(!addressList) {
      qcloud.request({
        url: `/user/getAddress/${userinfo.id}`,
        success: function (res) {
          self.setData({
            list: res.data
          });
          wx.setStorageSync('addressList', res.data);
        },
        fail: function (err) {
          util.showModel("错误", "请检查网络连接");
        }
      });
    } else {
      self.setData({
        list: addressList
      });
    }
  },

  /**
   * 页面相关事件处理函数--监听用户下拉动作
   */
  onPullDownRefresh: function () {

  },

  touchstart: function (e) {
    this.data.list.forEach(function (v, i) {
      if (v.isTouchMove)
        v.isTouchMove = false;
    })
    this.setData({
      startX: e.changedTouches[0].clientX,
      startY: e.changedTouches[0].clientY,
      list: this.data.list
    })
  },
  //滑动事件处理
  touchmove: function (e) {
    var that = this,
      index = e.currentTarget.dataset.index, //当前索引
      startX = that.data.startX, //开始X坐标
      startY = that.data.startY, //开始Y坐标
      touchMoveX = e.changedTouches[0].clientX, //滑动变化坐标
      touchMoveY = e.changedTouches[0].clientY, //滑动变化坐标
      //获取滑动角度
      angle = util.angle({
        X: startX,
        Y: startY
      }, {
        X: touchMoveX,
        Y: touchMoveY
      });
    that.data.list.forEach(function (v, i) {
      v.isTouchMove = false
      //滑动超过30度角 return
      if (Math.abs(angle) > 30) return;
      if (i == index) {
        if (touchMoveX > startX) //右滑
          v.isTouchMove = false
        else //左滑
          v.isTouchMove = true
      }
    })
    that.setData({
      list: that.data.list
    })
  },
  del: function (e) {
    var self = this;
    var address = self.data.list.splice(e.currentTarget.dataset.index, 1)[0];
    util.showBusy("删除中");
    qcloud.request({
      url: config.service.requestUrl + "/user/delAddress/" + address.id,
      method: "delete",
      success: function(res){
        wx.hideToast();
        if(res.data != "ok") {
          util.showError("删除失败，请重试");
          return;
        }
        wx.setStorageSync("addressList", self.data.list);
        self.setData({
          list: self.data.list
        });
      }
    });
  },
  navigateEdit: function(e){
    var id = e.currentTarget.dataset.id;
    wx.navigateTo({
      url: "/pages/address/edit/edit?id=" + id
    })
  },
  select: function(e){
    if(this.data.flag != "select") return;
    var business = qcloud.getSession().business;
    var address = this.data.list[e.currentTarget.dataset.index];
    if(business.range > 0) { 
      var distance = util.calcDistance(address, business);
      if(business.range - distance / 1000 < 0) {
        util.showError("地址超出商家配送范围");
        return;
      }
    }
    wx.setStorageSync("selectAddress", address);
    wx.navigateBack({
      delta: 1
    });
  }

})