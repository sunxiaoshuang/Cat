/**
 * 小程序配置文件
 */

// 此处主机域名修改成腾讯云解决方案分配的域名
var host = 'https://www.jiandanmao.cn';

var config = {

    // 下面的地址配合云端 Demo 工作
    service: {
        host,

        // 登录地址，用于建立会话
        loginUrl: `${host}/api/User`,

        // 测试的请求地址，用于测试会话
        requestUrl: `${host}/api`,

        // 测试的信道服务地址
        tunnelUrl: `${host}/api/tunnel`,

        // 上传图片接口
        uploadUrl: `${host}/api/upload`,

        // 图片地址
        imageUrl: `http://f.jiandanmao.cn`
    },
    businessId: 1
};

module.exports = config;
