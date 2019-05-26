
<<<<<<< HEAD
var host = 'https://api.jiandanmao.cn';
=======
var host = 'https://api.whliupangzi.cn';
>>>>>>> f743e3f2ac082ba87680efae926468da09eab0f0
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
<<<<<<< HEAD
      imageUrl: `http://f.jiandanmao.cn`
=======
      imageUrl: `http://f.whliupangzi.cn`
>>>>>>> f743e3f2ac082ba87680efae926468da09eab0f0
    },
    globalData: {
        openText: "简单猫\r\n让您的生活更简单"
    },
    businessId: 2
    
};

module.exports = config;

