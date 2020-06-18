
// var host = 'https://t.api.jiandanmao.cn';
var host = 'https://api.whliupangzi.cn';
// var host = 'http://localhost:5000';
// e.jiandanmao.cn
// t.e.jiandanmao.cn
// e.whliupangzi.cn

var config = {

    service: {
        host,

        // 登录地址，用于建立会话
        loginUrl: `${host}/api/user/login`,

        // 测试的请求地址，用于测试会话
        requestUrl: `${host}/api`,

        // 测试的信道服务地址
        tunnelUrl: `${host}/api/tunnel`,

        // 上传图片接口
        uploadUrl: `${host}/api/upload`,

        // 图片地址
      imageUrl: `http://f.whliupangzi.cn`
    },
    globalData: {
        openText: "简单猫\r\n让您的生活更简单"
    },
    businessId: 2
    
};

module.exports = config;

