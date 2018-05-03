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
    }

})();


