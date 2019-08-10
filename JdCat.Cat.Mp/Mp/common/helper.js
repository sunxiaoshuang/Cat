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
	return uni.request(obj);
}

const extend = function(obj, ...objArr){
	objArr.forEach(a => {
		for(var key in a) {
			obj[key] = a[key];
		}
	});
	return obj;
}

const firstLetterUpper = function (str) {
	return str.slice(0, 1).toUpperCase() + str.slice(1);
}

export default {  
    request, extend, firstLetterUpper
}  