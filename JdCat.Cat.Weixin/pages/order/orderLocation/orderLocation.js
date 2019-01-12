const qcloud = require("../../../vendor/wafer2-client-sdk/index");
const util = require("../../../utils/util");

Page({

  data: {
    longitude: 0,
    latitude: 0,
    orderId: 0,
    markers: []
  },

  onLoad: function (options) {
    var markers = [{
      latitude: options.lat,
      longitude: options.lng,
      iconPath: "/images/icon/map-user.png",
      callout: {
        content: "用户地址", color: "#fff", display: "ALWAYS", bgColor: "#f00", padding: 5, borderRadius: 10, fontSize: 12
      },
      width: 30,
      height: 30
    }];
    this.setData({
      orderId: options.id,
      longitude: options.lng,
      latitude: options.lat,
      markers
    });
  },

  onShow: function () {
    var self = this,
      markers = this.data.markers,
      business = qcloud.getSession().business;
    qcloud.request({
      url: "/order/orderLocation/" + this.data.orderId,
      method: "GET",
      success: function (res) {
        var lng, lat;
        if (!res.data) {
          lng = business.lng;
          lat = business.lat;
        } else {
          lng = res.data.lng;
          lat = res.data.lat;
        }
        markers.push({
          latitude: lat,
          longitude: lng,
          iconPath: "/images/other/logo.png",
          callout: {
            content: "骑手位置", color: "#fff", display: "ALWAYS", bgColor: "#4caf50", padding: 5, borderRadius: 10, fontSize: 12
          },
          width: 30,
          height: 30
        });
        self.setData({
          markers
        });
      }
    });
  }

});