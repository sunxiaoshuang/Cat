const formatTime = date => {
    const year = date.getFullYear()
    const month = date.getMonth() + 1
    const day = date.getDate()
    const hour = date.getHours()
    const minute = date.getMinutes()
    const second = date.getSeconds()

    return [year, month, day].map(formatNumber).join('/') + ' ' + [hour, minute, second].map(formatNumber).join(':')
};

const formatNumber = n => {
    n = n.toString()
    return n[1] ? n : '0' + n
};


// 显示繁忙提示
var showBusy = text => wx.showToast({
    title: text,
    icon: 'loading',
    duration: 10000
});

// 显示成功提示
var showSuccess = text => wx.showToast({
    title: text,
    icon: 'success'
});
// 显示失败提示
var showError = text => wx.showToast({
    title: text,
    icon: 'none'
});

// 显示失败提示
var showModel = (title, content) => {
    wx.hideToast();

    wx.showModal({
        title,
        content: JSON.stringify(content),
        showCancel: false
    });
};

/**
 * 计算滑动角度
 * @param {Object} start 起点坐标
 * @param {Object} end 终点坐标
 */
var angle = (start, end) => {
    var _X = end.X - start.X,
        _Y = end.Y - start.Y
    //返回角度 /Math.atan()返回数字的反正切值
    return 360 * Math.atan(_Y / _X) / (2 * Math.PI);
}

var PI = Math.PI;

function getRad(d) {
    return d * Math.PI / 180.0;
}
/**
 * 根据经纬度计算距离
 * @param {Object} 坐标点
 * @param {Object} 目标点
 */
var calcDistance = (function () {
    var EARTH_RADIUS = 6378137.0; //单位M
    var PI = Math.PI;

    function getRad(d) {
        return d * Math.PI / 180.0;
    }
    return function (pointer1, pointer2) {
        var radLat1 = getRad(pointer1.lat);
        var radLat2 = getRad(pointer2.lat);
        var a = radLat1 - radLat2;
        var b = getRad(pointer1.lng) - getRad(pointer2.lng);
        var s = 2 * Math.asin(Math.sqrt(Math.pow(Math.sin(a / 2), 2) + Math.cos(radLat1) * Math.cos(radLat2) * Math.pow(Math.sin(b / 2), 2)));
        s = s * EARTH_RADIUS;
        s = Math.round(s * 10000) / 10000.0;
        return s;
    };
})();

// 正则
var regExp = {
    phone: /^(13[0-9]|14[579]|15[0-3,5-9]|16[6]|17[0135678]|18[0-9]|19[89])\d{8}$/
};

// 
var method = {
    delayExec: function (func, delay, params, times) { // 延迟执行，返回true时退出
        var flag = false,
            num = 0,
            interval;
        if (!delay) {
            delay = 200;
        }
        if (!times) {
            times = 100;
        }
        interval = setInterval(function () {
            while (flag || num > times) {
                clearInterval(interval);
                return;
            }
            try {
                flag = func(params);
                num++;
            } catch (e) {
                // 如果执行出错，则退出轮询
                clearInterval(interval);
            }
        }, delay);
    },
    throttle: function(method, time, context){
        clearTimeout(method.tId);
        time = time || 1000;
        method.tId = setTimeout(function(){
            method.call(context);
        }, time);
    }
}

module.exports = {
    formatTime,
    showBusy,
    showSuccess,
    showError,
    showModel,
    regExp,
    angle,
    calcDistance,
    method
};