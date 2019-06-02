var jdCat = {};
(function () {

    // escape、unescape

    ["String", "Object", "Number", "Boolean", "Array", "Date", "Null", "Undefined"].forEach(function (obj) {
        if (!$["is" + obj]) {
            $["is" + obj] = function (val) {
                return Object.prototype.toString.call(val) === "[object " + obj + "]";
            }
        }
    });

    // 字符串格式化工具
    String.prototype.format = function (args) {
        var result = this;
        if (arguments.length > 0) {
            if (arguments.length == 1 && typeof (args) == "object") {
                for (var key in args) {
                    if (args[key] != undefined) {
                        var reg = new RegExp("({" + key + "})", "g");
                        result = result.replace(reg, args[key]);
                    }
                }
            }
            else {
                for (var i = 0; i < arguments.length; i++) {
                    if (arguments[i] != undefined) {
                        var reg = new RegExp("({)" + i + "(})", "g");
                        result = result.replace(reg, arguments[i]);
                    }
                }
            }
        }
        return result;
    }

    // 日期格式化工具
    Date.prototype.format = function (fmt) {
        var o = {
            "M+": this.getMonth() + 1,
            "d+": this.getDate(),
            "h+": this.getHours(),
            "m+": this.getMinutes(),
            "s+": this.getSeconds(),
            "q+": Math.floor((this.getMonth() + 3) / 3),
            "S": this.getMilliseconds()
        };
        if (/(y+)/.test(fmt)) fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
        for (var k in o)
            if (new RegExp("(" + k + ")").test(fmt)) fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
        return fmt;
    };

    // html特殊字符转义
    window.HTMLEncode = function (html) {
        var temp = document.createElement("div");
        (temp.textContent != null) ? (temp.textContent = html) : (temp.innerText = html);
        var output = temp.innerHTML; temp = null;
        return output;
    }

    // html特殊字符返转义
    window.HTMLDecode = function (text) {
        var temp = document.createElement("div");
        temp.innerHTML = text;
        var output = temp.innerText || temp.textContent;
        temp = null;
        return output;
    }

    // 数组扩展
    Array.prototype.first = function (fn) {
        var i = 0, len = this.length;
        for (; i < len; i++) {
            if (fn.call(this[i], this[i]) === true) {
                return this[i];
            }
        }
        return null;
    };
    Array.prototype.remove = function (obj) {
        var i = 0, len = this.length;
        for (; i < len;) {
            if (this[i] === obj) {
                break;
            }
            i++;
        }
        this.splice(i, 1);
        return this;
    };
    Array.prototype.select = function (fn) {
        var arr = [];
        this.forEach(function(obj) {
            arr.push(fn(obj));
        });
        return arr;
    }
    Array.prototype.replace = function (obj, entity) {
        var i = 0, len = this.length;
        for (; i < len;) {
            if (this[i] === obj) {
                break;
            }
            i++;
        }
        this.splice(i, 1, entity);
        return this;
    };
    Array.prototype.sum = function (fn) {
        var total = 0;
        this.forEach(function (obj) {
            total += fn(obj);
        });
        return total;
    };
    Array.prototype.group = function (fn) {
        var groups = [];
        this.forEach(function (obj) {
            var key = fn(obj);
            var group = groups.first(a => a.key === key) || { key, list: [] };
            group.list.push(obj);
            if (groups.indexOf(group) > -1) return;
            groups.push(group);
        });
        return groups;
    };

    // Vue过滤器
    if (Vue) {
        Vue.filter("currency", function (s, n) {
            n = n > 0 && n <= 20 ? n : 2;
            s = parseFloat((s + "").replace(/[^\d\.-]/g, "")).toFixed(n) + "";
            var l = s.split(".")[0].split("").reverse(),
                r = s.split(".")[1];
            var t;
            t = "";
            var i;
            for (i = 0; i < l.length; i++) {
                t += l[i] + ((i + 1) % 3 === 0 && (i + 1) !== l.length ? "," : "");
            }
            return t.split("").reverse().join("") + "." + r;   
        });
    }

    jdCat.utilData = {
        dateOptions: {
            format: 'yyyy-mm-dd',
            autoclose: true,
            maxView: 1,
            minView: 2,
            todayBtn: true,
            todayHighlight: true,
            language: "zh-CN"
        }
    };

    jdCat.utilMethods = {
        now: function (format) {
            var obj = { date: true, time: false };
            $.extend(obj, format);
            var time = new Date();
            var year = time.getFullYear(), month = time.getMonth() + 1, day = time.getDate(), hour = time.getHours(), minute = time.getMinutes(), second = time.getSeconds();
            if (obj.date && obj.time) return [year, month, day].map(this.formatNumber).join('-') + ' ' + [hour, minute, second].map(formatNumber).join(':');
            if (obj.date) return [year, month, day].map(this.formatNumber).join('-');
            if (obj.time) return [hour, minute, second].map(formatNumber).join(':');
        },
        formatNumber: function (num) {
            num = num.toString();
            return num[1] ? num : '0' + num;
        },
        ipValid: function (ip) {
            return /((25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d)))\.){3}(25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d)))/.test(ip);
        },
        diffDay: function (start, end) {
            var timespan = new Date(end) - new Date(start);
            var days = +(timespan / (1000 * 60 * 60 * 24)).toFixed();
            return days;
        }
    };

})();


