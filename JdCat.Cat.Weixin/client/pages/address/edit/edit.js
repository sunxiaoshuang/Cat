// pages/address/edit/edit.js
Page({

  /**
   * 页面的初始数据
   */
  data: {
    genderList: [
      {name: "先生", value: 1, checked: true},
      {name: "女士", value: 2},
    ],
    entity: {
      gender: 0
    }
  },

  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function (options) {
    
  },
  changeGender: function(e){
    this.setData({
      gender: parseInt(e.detail.value)
    });
  }
})