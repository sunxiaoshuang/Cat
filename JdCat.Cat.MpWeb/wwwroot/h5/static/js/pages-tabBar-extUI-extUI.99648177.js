(window["webpackJsonp"]=window["webpackJsonp"]||[]).push([["pages-tabBar-extUI-extUI"],{"1a51":function(t,n,e){"use strict";e.r(n);var a=e("b745"),i=e("2167");for(var u in i)"default"!==u&&function(t){e.d(n,t,function(){return i[t]})}(u);e("b7f5");var r=e("2877"),o=Object(r["a"])(i["default"],a["a"],a["b"],!1,null,"2af54375",null);n["default"]=o.exports},"1df8":function(t,n,e){var a=e("f7f4");"string"===typeof a&&(a=[[t.i,a,""]]),a.locals&&(t.exports=a.locals);var i=e("4f06").default;i("58ed018f",a,!0,{sourceMap:!1,shadowMode:!1})},2167:function(t,n,e){"use strict";e.r(n);var a=e("7df0"),i=e.n(a);for(var u in a)"default"!==u&&function(t){e.d(n,t,function(){return a[t]})}(u);n["default"]=i.a},"37bb":function(t,n,e){"use strict";var a=function(){var t=this,n=t.$createElement,e=t._self._c||n;return e("v-uni-view",{staticStyle:{"text-decoration":"underline"},attrs:{href:t.href,inWhiteList:t.inWhiteList},on:{click:function(n){n=t.$handleEvent(n),t.openURL(n)}}},[t._v(t._s(t.text))])},i=[];e.d(n,"a",function(){return a}),e.d(n,"b",function(){return i})},"62da":function(t,n,e){"use strict";Object.defineProperty(n,"__esModule",{value:!0}),n.default=void 0;var a={name:"u-link",props:{href:{type:String,default:""},text:{type:String,default:""},inWhiteList:{type:Boolean,default:!1}},methods:{openURL:function(){window.open(this.href)}}};n.default=a},"7df0":function(t,n,e){"use strict";Object.defineProperty(n,"__esModule",{value:!0}),n.default=void 0;var a=i(e("eabd"));function i(t){return t&&t.__esModule?t:{default:t}}var u={components:{uLink:a.default},data:function(){return{lists:[{name:"Card 卡片",url:"card"},{name:"Collapse 折叠面板",url:"collapse"},{name:"Drawer 抽屉",url:"drawer"},{name:"Grid 宫格",url:"grid"},{name:"List 列表",url:"list"},{name:"NavBar 导航栏",url:"nav-bar"},{name:"Pagination 分页器",url:"pagination"},{name:"SwiperDot 轮播图指示点",url:"swiper-dot"},{name:"Steps 步骤条",url:"steps"},{name:"SegmentedControl 分段器",url:"segmented-control"},{name:"SwipeAction 滑动操作",url:"swipe-action"},{name:"Badge 数字角标",url:"badge"},{name:"CountDown 倒计时",url:"count-down"},{name:"Icon 图标",url:"icon"},{name:"LoadMore 加载更多",url:"load-more"},{name:"NoticeBar 通告栏",url:"notice-bar"},{name:"NumberBox 数字输入框",url:"number-box"},{name:"Popup 弹出层",url:"popup"},{name:"Rate 评分",url:"rate"},{name:"Tag 标签",url:"tag"},{name:"Calendar 日历",url:"calendar"}]}},onLoad:function(){},onReady:function(){},onShareAppMessage:function(){return{title:"欢迎体验uni-app",path:"/pages/tabBar/extUI/extUI"}},onNavigationBarButtonTap:function(t){uni.navigateTo({url:"/pages/about/about"})},methods:{goDetailPage:function(t){uni.navigateTo({url:"/pages/extUI/"+t+"/"+t})}}};n.default=u},a8b3:function(t,n,e){"use strict";e.r(n);var a=e("62da"),i=e.n(a);for(var u in a)"default"!==u&&function(t){e.d(n,t,function(){return a[t]})}(u);n["default"]=i.a},b745:function(t,n,e){"use strict";var a=function(){var t=this,n=t.$createElement,e=t._self._c||n;return e("v-uni-view",{staticClass:"uni-padding-wrap uni-common-pb"},[e("v-uni-view",{staticClass:"uni-header-logo"},[e("v-uni-image",{attrs:{src:"/static/extuiIndex.png"}})],1),e("v-uni-view",{staticClass:"uni-hello-text uni-common-pb"},[t._v("以下是uni-app扩展组件示例，更多组件见插件市场："),e("u-link",{attrs:{href:"https://ext.dcloud.net.cn/",text:"https://ext.dcloud.net.cn",inWhiteList:!0}})],1),t._l(t.lists,function(n,a){return e("v-uni-view",{key:a,staticClass:"uni-card"},[e("v-uni-view",{staticClass:"uni-list"},[e("v-uni-view",{staticClass:"uni-list-cell uni-collapse"},[e("v-uni-view",{staticClass:"uni-list-cell-navigate uni-navigate-right",attrs:{"hover-class":"uni-list-cell-hover"},on:{click:function(e){e=t.$handleEvent(e),t.goDetailPage(n.url)}}},[t._v(t._s(n.name))])],1)],1)],1)})],2)},i=[];e.d(n,"a",function(){return a}),e.d(n,"b",function(){return i})},b7f5:function(t,n,e){"use strict";var a=e("1df8"),i=e.n(a);i.a},eabd:function(t,n,e){"use strict";e.r(n);var a=e("37bb"),i=e("a8b3");for(var u in i)"default"!==u&&function(t){e.d(n,t,function(){return i[t]})}(u);var r=e("2877"),o=Object(r["a"])(i["default"],a["a"],a["b"],!1,null,"d28d3c36",null);n["default"]=o.exports},f7f4:function(t,n,e){n=t.exports=e("2350")(!1),n.push([t.i,"uni-page-body[data-v-2af54375]{height:auto;min-height:100%}.uni-card[data-v-2af54375]{-webkit-box-shadow:none;box-shadow:none}.uni-list[data-v-2af54375]:after{height:0}.uni-list[data-v-2af54375]:before{height:0}.uni-hello-text[data-v-2af54375]{word-break:break-all}",""])}}]);