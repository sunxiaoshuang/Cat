
/**
 * 拓展对象
 */
exports.extend = function extend(target) {
    var sources = Array.prototype.slice.call(arguments, 1);

    for (var i = 0; i < sources.length; i += 1) {
        var source = sources[i];
        for (var key in source) {
            if (source.hasOwnProperty(key)) {
                target[key] = source[key];
            }
        }
    }

    return target;
};

exports.getUuid = function(len) {
    var s = [], len = len || 36;
    var hexDigits = "0123456789abcdef";
    for (var i = 0; i < len; i++) {
        s[i] = hexDigits.substr(Math.floor(Math.random() * 0x10), 1);
    }
    // s[14] = "4";  
    // s[19] = hexDigits.substr((s[19] & 0x3) | 0x8, 1);
    if(len === 36) {
        s[8] = s[13] = s[18] = s[23] = "-";
    }

    var uuid = s.join("");
    return uuid;
}