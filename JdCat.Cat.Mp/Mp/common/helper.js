import config from '../config.js'

const request = function(obj){
	obj.url = config.apiUrl + obj.url;
	if(!obj.fail) {
		obj.fail = function(err){
			uni.showToast({
				title: '网络请求错误',
				icon: 'none'
			})
		}
	}
	uni.request(obj);
}

export default {  
    request
}  