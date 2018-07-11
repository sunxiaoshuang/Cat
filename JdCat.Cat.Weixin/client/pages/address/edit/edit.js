const config = require("../../../config");
const util = require("../../../utils/util");
const qcloud = require('../../../vendor/wafer2-client-sdk/index');
Page({

  /**
   * 页面的初始数据
   */
  data: {
    genderList: [{
        name: "先生",
        value: 1
      },
      {
        name: "女士",
        value: 2
      },
    ],
    isModify: false,
    phoneFocus: false,
    entity: {
      id: 0,
      receiver: "",
      provinceName: "",
      cityName: "",
      areaName: "",
      mapInfo: "",
      lng: 0,
      lat: 0,
      detailInfo: "",
      phone: "",
      gender: 1,
      postalCode: "",
      userId: 0
    }
  },

  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function (options) {
    var id = options.id, 
        self = this, 
        addressList = wx.getStorageSync("addressList");
    if(id){
      self.setData({
        entity: addressList.filter(a => a.id == id)[0],
        isModify: true
      });
    }
  },
  blurReceiver: function(e){
    var entity = this.data.entity;
    entity.receiver = e.detail.value;
    this.setData({
      entity: entity
    });
  },
  blurPhone: function(e){
    if(!e.detail.value) return;
    // 验证手机号
    if(!util.regExp.phone.test(e.detail.value)){
      util.showError("请输入正确的手机号码");
      this.setData({
        phoneFocus: true
      });
      return;
    }
    var entity = this.data.entity;
    entity.phone = e.detail.value;
    this.setData({
      entity: entity,
      phoneFocus: false
    });
  },
  blurDetailInfo: function(e){
    var entity = this.data.entity;
    entity.detailInfo = e.detail.value;
    this.setData({
      entity: entity
    });
  },
  changeGender: function (e) {
    var entity = this.data.entity;
    entity.gender = e.detail.value;
    this.setData({
      entity: entity
    });
  },
  selectAddress: function () {
    var self = this;
    wx.chooseLocation({
      success: function (res) {
        if (res.errMsg != "chooseLocation:ok") return;
        var address = self.data.entity;
        address.mapInfo = res.address;
        address.lng = res.longitude;
        address.lat = res.latitude;
        self.setData({
          entity: address
        });
      }
    });
  },
  saveAddress: function () {
    var self = this, userId = qcloud.getSession().skey, entity = this.data.entity, url;
    if(!entity.receiver){
      util.showError("请输入联系人姓名");
      return;
    }
    if(!entity.phone){
      util.showError("请输入联系人手机号码");
      return;
    }
    if(!entity.mapInfo){
      util.showError("请选择收货地址");
      return;
    }
    if(!entity.detailInfo){
      util.showError("请输入门牌号");
      return;
    }
    util.showBusy("loading");
    if(this.data.isModify){
      url = "/user/updateAddress/" + self.data.entity.id;
    } else {
      url = "/user/address/" + userId;
    }
    qcloud.request({
      url: url,
      method: this.data.isModify ? "put" : "post",
      data: self.data.entity,
      login: !this.data.isModify,
      success: function(res){
        wx.hideToast();
        if(res.data.success){
          util.showSuccess("保存成功");
          var addressList = wx.getStorageSync("addressList"), index, address;
          addressList.forEach((a, b) => {
            if(a.id == res.data.data.id){
              address = a;
              index = b;
              return false;
            }
          });
          if(!address){
            addressList.push(res.data.data);
          } else {
            addressList.splice(index, 1, res.data.data);
          }
          wx.setStorageSync("addressList", addressList);
          setTimeout(() => wx.navigateBack({
            delta: 1
          }), 1500);
        } else {
          util.showModel("保存失败", "原因：" + JSON.stringify(res.data));
        }
      }
    });
  }
})