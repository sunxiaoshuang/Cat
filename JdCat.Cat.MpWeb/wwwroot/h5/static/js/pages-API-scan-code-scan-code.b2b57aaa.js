(window["webpackJsonp"]=window["webpackJsonp"]||[]).push([["pages-API-scan-code-scan-code"],{1120:function(t,n,e){"use strict";e.r(n);var i=e("b4cd"),a=e("e88f");for(var s in a)"default"!==s&&function(t){e.d(n,t,function(){return a[t]})}(s);e("2ac3");var u=e("2877"),c=Object(u["a"])(a["default"],i["a"],i["b"],!1,null,"2d90544b",null);n["default"]=c.exports},"2ac3":function(t,n,e){"use strict";var i=e("99bd"),a=e.n(i);a.a},"74f2":function(t,n,e){n=t.exports=e("2350")(!1),n.push([t.i,".scan-result[data-v-2d90544b]{min-height:%?50?%;line-height:%?50?%}",""])},9913:function(t,n,e){"use strict";Object.defineProperty(n,"__esModule",{value:!0}),n.default=void 0;var i={data:function(){return{title:"scanCode",result:""}},methods:{scan:function(){var t=this;uni.scanCode({success:function(n){t.result=n.result}})}}};n.default=i},"99bd":function(t,n,e){var i=e("74f2");"string"===typeof i&&(i=[[t.i,i,""]]),i.locals&&(t.exports=i.locals);var a=e("4f06").default;a("d07d6258",i,!0,{sourceMap:!1,shadowMode:!1})},b4cd:function(t,n,e){"use strict";var i=function(){var t=this,n=t.$createElement,e=t._self._c||n;return e("v-uni-view",[e("page-head",{attrs:{title:t.title}}),e("v-uni-view",{staticClass:"uni-padding-wrap uni-common-mt"},[e("v-uni-view",{staticClass:"uni-title"},[t._v("扫码结果：")]),t.result?e("v-uni-view",{staticClass:"uni-list"},[e("v-uni-view",{staticClass:"uni-cell"},[e("v-uni-view",{staticClass:"scan-result"},[t._v(t._s(t.result))])],1)],1):t._e(),e("v-uni-view",{staticClass:"uni-btn-v"},[e("v-uni-button",{attrs:{type:"primary"},on:{click:function(n){n=t.$handleEvent(n),t.scan(n)}}},[t._v("扫一扫")])],1)],1)],1)},a=[];e.d(n,"a",function(){return i}),e.d(n,"b",function(){return a})},e88f:function(t,n,e){"use strict";e.r(n);var i=e("9913"),a=e.n(i);for(var s in i)"default"!==s&&function(t){e.d(n,t,function(){return i[t]})}(s);n["default"]=a.a}}]);