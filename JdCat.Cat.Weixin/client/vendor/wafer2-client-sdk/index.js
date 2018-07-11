var constants = require('./lib/constants');
var login = require('./lib/login');
var Session = require('./lib/session');
var request = require('./lib/request');
var Tunnel = require('./lib/tunnel');
var Utils = require('./lib/utils');

var exports = module.exports = {
    login: login.login,
    setLoginUrl: login.setLoginUrl,
    LoginError: login.LoginError,
    requestLogin: login.requestLogin,
    
    clearSession: Session.clear,
    getSession: Session.get,
    setSession: Session.set,
    request: request.request,
    payment: request.payment,
    RequestError: request.RequestError,

    Tunnel: Tunnel,

    utils: Utils
};

// 导出错误类型码
Object.keys(constants).forEach(function (key) {
    if (key.indexOf('ERR_') === 0) {
        exports[key] = constants[key];
    }
});