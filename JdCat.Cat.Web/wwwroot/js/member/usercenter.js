var h = $('header'), f=$('footer'), p=$('.panel');
document.addEventListener('touchmove',function (e){ e.preventDefault();},false);
var lpage = {
	gridList: function(){
		$('.gridclsit i').each(function(){
		});
	},
	headerHide:function(){
		if(p.data('header')==="none"){
			h.hide();
		}
	},
	addScroll:function(scont){
		var myScroll = new iScroll(scont,{scrollbars:false});
	},
	init:function(scont){
		this.headerHide();
	}
}
$(function(){
	if(document.referrer==''){
       $(".back").hide();
    }else{
       $(".back").show();
    }
})
function errolayer(msg){
	$('body').append("<div class='msglay'><span>"+msg+"</span></div>");
	setTimeout(function(){
		$('.msglay').addClass('fadeOutDown').remove();
	},2000);
}
//是否是微信游览器
function isWeiXin(){
    var ua = window.navigator.userAgent.toLowerCase();
    if(ua.match(/MicroMessenger/i) == 'micromessenger'){
        return true;
    }else{
        return false;
    }
}
//是否是支付宝浏览器
function isAlipay(){
    var ua = window.navigator.userAgent.toLowerCase();
    if(ua.indexOf("alipay")!=-1){
        return true;
    }else{
        return false;
    }
}