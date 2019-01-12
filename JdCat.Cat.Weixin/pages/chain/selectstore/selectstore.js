const qcloud = require("../../../vendor/wafer2-client-sdk/index");
const util = require("../../../utils/util");

Page({

  data: {
    top10: [], // 主要城市
    type: [], // 可选的城市
    viewName: "store", // city
    arrow: "down", // 箭头
    searchcontent: "搜索...", // 搜索框提示
    location: {}, // 定位
    cityName: "", // 选择的城市
    inputContent: "", // 输入的内容
    filterCity: [], // 筛选后的城市
    hasCitySearch: false, // 正处于城市搜索
    nearbyStores: [], // 附近的门店
    markers: [],  // 地图标记点
    mapObj: {}
  },
  // 周期函数
  onLoad: function (options) {
    this.loadCity();    // 加载所有的可选择城市
    this.loadUserSelect();  // 加载用户选择数据
    this.loadNearby();  // 加载所选城市最近门店

  },
  // 事件函数
  changeCity: function () {
    var viewName = this.data.viewName,
      arrow, isCity = viewName !== "store";
    viewName = isCity ? "store" : "city";
    arrow = isCity ? "down" : "up";
    this.setData({
      viewName,
      arrow,
      inputContent: "",
      hasCitySearch: false,
    });
  },
  bindKeyInput: function (e) {
    this.data.inputContent = e.detail.value;
    var time = this.data.viewName == "store" ? 1000 : 200;
    util.method.throttle(this.search, time, this);
  },
  selectCity: function(e) {
    var name = e.currentTarget.dataset.name;
    wx.setStorageSync("userSelectCity", name);

    this.setData({
      cityName: name,
      inputContent: "",
      hasCitySearch: false,
      viewName: "store",
      arrow: "down"
    });
    this.loadNearby();
  },
  goMap: function(e){
    var store = e.currentTarget.dataset.store;
    var mapObj = {
      longitude: store.lng,
      latitude: store.lat
    };
    this.setData({
      mapObj
    });
    // wx.openLocation({
    //   latitude: store.lat,
    //   longitude: store.lng,
    //   name: store.name,
    //   address: store.address
    // });
  },
  enterStore: function(e){
    var store = e.currentTarget.dataset.store, session = qcloud.getSession();
    session.business = store;
    session.reload = true;
    qcloud.setSession(session);
    
    wx.switchTab({
      url: "/pages/main/main"
    });
  },
  clickMaker: function(e){
    // console.log(e);
    var store, session = qcloud.getSession();
    store = this.data.nearbyStores.filter(function(obj){ return obj.id == e.markerId})[0];
    session.business = store;
    session.reload = true;
    qcloud.setSession(session);
    
    wx.switchTab({
      url: "/pages/main/main"
    });
  },

  // 处理函数
  loadCity: function () {
    var self = this,
      cityJson = wx.getStorageSync("cityJson");
    if (!cityJson) {
      qcloud.request({
        url: "/app/getCity",
        method: "GET",
        success: function (res) {
          wx.setStorageSync("cityJson", res.data);
          self.handleCity(res.data);
        }
      });
    } else {
      this.handleCity(cityJson);
    }
  },
  handleCity: function (cityJson) {
    // 前十个城市归为主要城市
    var top10 = cityJson.slice(0, 10);
    // 按首字母分类
    var type = [],
      arr = "ABCDEFGHIJKLMNOPQRSTUVWXYZ",
      len = arr.length,
      index = 0;
    for (; index < len;) {
      type.push({
        key: arr[index++],
        items: []
      });
    }
    cityJson.forEach(city => {
      var first = city.pinyin.substring(0, 1);
      var typeArr = type.filter(a => a.key === first);
      if (typeArr.length === 0) return;
      typeArr[0].items.push(city);
    });
    this.setData({
      top10,
      type
    });
  },
  loadUserSelect: function(){
    var location = wx.getStorageSync("curLocation");    // 当前用户定位
    var cityName = wx.getStorageSync("userSelectCity");   // 选择的城市
    if(!cityName) {
      if (!location) {
        cityName = "未知";
      } else {
        cityName = location.address_component.city.replace("市", "");
      }
    }

    this.setData({
      cityName,
      location: location || {}
    });
  },
  loadNearby: function(key){
    var self = this, chain = qcloud.getSession().chain, cityName = this.data.cityName, location = this.data.location, position;
    if(!cityName) return;
    if(!location) {
      position = "0,0";
    } else {
      position = location.location.lat + "," + location.location.lng;
    }
    qcloud.request({
      url: `/app/getNearbyStore/${chain.id}?city=${cityName}&key=${key || ""}&location=${position}`,
      method: "GET",
      success: function(res){
        var markers = [], mapObj = {};
        res.data.forEach(function(obj){
          markers.push({
            id: obj.id,
            latitude: obj.lat,
            longitude: obj.lng,
            title: obj.name,
            iconPath: "/images/other/logo.png",
            width: 30,
            height: 30
          });
        });
        if(res.data.length > 0){
          mapObj.longitude = res.data[0].lng;
          mapObj.latitude = res.data[0].lat;
        }
        self.setData({
          nearbyStores: res.data,
          markers,
          mapObj
        });
      }
    });
  },
  search: function(){
    var inputContent = this.data.inputContent, viewName = this.data.viewName;

    switch(viewName) {
      case "store":
        this.searchStore(inputContent);
        break;
      case "city":
        this.searchCity(inputContent);
        break;
      default:
        break;
    }
  },
  searchStore: function(inputContent){
    this.loadNearby(inputContent);
  },
  // 城市搜索
  searchCity: function(inputContent){
    if(!inputContent) {
      this.setData({
        hasCitySearch: false
      });
      return;
    }
    var cityJson = wx.getStorageSync("cityJson");
    var filterCity = cityJson.filter(function(city){
      return city.label.toLowerCase().indexOf(inputContent) > -1;
    });
    if(filterCity.length === 0) {
      util.showError("找不到城市：" + inputContent);
    }
    this.setData({
      filterCity,
      hasCitySearch: true
    });
  }

});