(window["webpackJsonp"]=window["webpackJsonp"]||[]).push([["pages-extUI-pagination-pagination"],{"0103":function(t,e,n){"use strict";var i=function(){var t=this,e=t.$createElement,n=t._self._c||e;return n("v-uni-view",[n("v-uni-view",{staticClass:"example-title"},[t._v("默认样式")]),n("uni-pagination",{attrs:{total:20,title:"标题文字"}}),n("v-uni-view",{staticClass:"example-title"},[t._v("修改按钮文字")]),n("uni-pagination",{attrs:{total:20,title:"标题文字","prev-text":"前一页","next-text":"后一页"}}),n("v-uni-view",{staticClass:"example-title"},[t._v("图标样式")]),n("uni-pagination",{attrs:{"show-icon":!0,total:20,title:"标题文字"}}),n("v-uni-view",{staticClass:"example-title"},[t._v("修改数据长度")]),n("uni-pagination",{attrs:{current:t.current,total:t.total,title:"标题文字","show-icon":"true"},on:{change:function(e){e=t.$handleEvent(e),t.change(e)}}}),n("v-uni-view",{staticClass:"btn-view"},[n("v-uni-view",[t._v("当前页："+t._s(t.current)+"，数据总量："+t._s(t.total)+"条，每页数据："+t._s(t.pageSize))]),n("v-uni-button",{attrs:{type:"primary"},on:{click:function(e){e=t.$handleEvent(e),t.add(e)}}},[t._v("增加10条数据")]),n("v-uni-button",{attrs:{type:"default"},on:{click:function(e){e=t.$handleEvent(e),t.reset(e)}}},[t._v("重置数据")])],1)],1)},a=[];n.d(e,"a",function(){return i}),n.d(e,"b",function(){return a})},"06f6":function(t,e,n){"use strict";var i=n("6a91"),a=n.n(i);a.a},"0c18":function(t,e,n){var i=n("5c13");"string"===typeof i&&(i=[[t.i,i,""]]),i.locals&&(t.exports=i.locals);var a=n("4f06").default;a("1c24cea1",i,!0,{sourceMap:!1,shadowMode:!1})},"14eb":function(t,e,n){"use strict";var i=n("0c18"),a=n.n(i);a.a},"1e33":function(t,e,n){"use strict";var i=function(){var t=this,e=t.$createElement,n=t._self._c||e;return n("v-uni-view",{staticClass:"uni-pagination"},[n("v-uni-view",{staticClass:"uni-pagination__btns"},[n("v-uni-view",{class:["uni-pagination__btn",{"uni-pagination--disabled":1===t.currentIndex}],attrs:{"hover-class":1===t.currentIndex?"":"uni-pagination--hover","hover-start-time":20,"hover-stay-time":70},on:{click:function(e){e=t.$handleEvent(e),t.clickLeft(e)}}},[t.showIcon?[n("uni-icon",{attrs:{color:"#000",size:"20",type:"arrowleft"}})]:[t._v(t._s(t.prevText))]],2),n("v-uni-view",{class:["uni-pagination__btn",{"uni-pagination--disabled":t.currentIndex===t.maxPage}],attrs:{"hover-class":t.currentIndex===t.maxPage?"":"uni-pagination--hover","hover-start-time":20,"hover-stay-time":70},on:{click:function(e){e=t.$handleEvent(e),t.clickRight(e)}}},[t.showIcon?[n("uni-icon",{attrs:{color:"#000",size:"20",type:"arrowright"}})]:[t._v(t._s(t.nextText))]],2)],1),n("v-uni-view",{staticClass:"uni-pagination__num"},[n("v-uni-text",{staticClass:"uni-pagination__num-current"},[t._v(t._s(t.currentIndex))]),t._v("/"+t._s(t.maxPage))],1)],1)},a=[];n.d(e,"a",function(){return i}),n.d(e,"b",function(){return a})},3387:function(t,e,n){"use strict";Object.defineProperty(e,"__esModule",{value:!0}),e.default=void 0;var i=a(n("734d"));function a(t){return t&&t.__esModule?t:{default:t}}var o={name:"UniPagination",components:{uniIcon:i.default},props:{prevText:{type:String,default:"上一页"},nextText:{type:String,default:"下一页"},current:{type:[Number,String],default:1},total:{type:[Number,String],default:0},pageSize:{type:[Number,String],default:10},showIcon:{type:Boolean,default:!1}},data:function(){return{currentIndex:1}},computed:{maxPage:function(){var t=1,e=Number(this.total),n=Number(this.pageSize);return e&&n&&(t=Math.ceil(e/n)),t}},watch:{current:function(t){this.currentIndex=+t}},created:function(){this.currentIndex=+this.current},methods:{clickLeft:function(){1!==Number(this.currentIndex)&&(this.currentIndex-=1,this.change("prev"))},clickRight:function(){Number(this.currentIndex)!==this.maxPage&&(this.currentIndex+=1,this.change("next"))},change:function(t){this.$emit("change",{type:t,current:this.currentIndex})}}};e.default=o},4413:function(t,e,n){e=t.exports=n("2350")(!1),e.push([t.i,"uni-page-body[data-v-e621189c]{display:-webkit-box;display:-webkit-flex;display:-ms-flexbox;display:flex;-webkit-box-orient:vertical;-webkit-box-direction:normal;-webkit-flex-direction:column;-ms-flex-direction:column;flex-direction:column;-webkit-box-sizing:border-box;box-sizing:border-box;background-color:#fff}uni-view[data-v-e621189c]{font-size:%?28?%;line-height:inherit}.example[data-v-e621189c]{padding:0 %?30?% %?30?%}.example-title[data-v-e621189c]{font-size:%?32?%;line-height:%?32?%;color:#777;margin:%?40?% %?25?%;position:relative}.example .example-title[data-v-e621189c]{margin:%?40?% 0}.example-body[data-v-e621189c]{padding:0 %?40?%}.btn-view[data-v-e621189c]{margin:%?30?% %?30?% 0;text-align:center}uni-button[data-v-e621189c]{margin-top:%?30?%}body.?%PAGE?%[data-v-e621189c]{background-color:#fff}",""])},"5c13":function(t,e,n){e=t.exports=n("2350")(!1),e.push([t.i,'.uni-pagination[data-v-be19eada]{width:100%;-webkit-box-sizing:border-box;box-sizing:border-box;padding:0 %?40?%;position:relative;overflow:hidden;display:-webkit-box;display:-webkit-flex;display:-ms-flexbox;display:flex;-webkit-box-orient:horizontal;-webkit-box-direction:normal;-webkit-flex-direction:row;-ms-flex-direction:row;flex-direction:row}.uni-pagination__btns[data-v-be19eada]{-webkit-box-flex:1;-webkit-flex:1;-ms-flex:1;flex:1;display:-webkit-box;display:-webkit-flex;display:-ms-flexbox;display:flex;-webkit-box-pack:justify;-webkit-justify-content:space-between;-ms-flex-pack:justify;justify-content:space-between;-webkit-box-align:center;-webkit-align-items:center;-ms-flex-align:center;align-items:center;-webkit-box-orient:horizontal;-webkit-box-direction:normal;-webkit-flex-direction:row;-ms-flex-direction:row;flex-direction:row}.uni-pagination__btn[data-v-be19eada]{width:%?120?%;height:%?60?%;padding:0 %?16?%;line-height:%?60?%;font-size:%?28?%;-webkit-box-sizing:border-box;box-sizing:border-box;position:relative;background-color:#f8f8f8;display:-webkit-box;display:-webkit-flex;display:-ms-flexbox;display:flex;-webkit-box-orient:horizontal;-webkit-box-direction:normal;-webkit-flex-direction:row;-ms-flex-direction:row;flex-direction:row;-webkit-box-pack:center;-webkit-justify-content:center;-ms-flex-pack:center;justify-content:center;-webkit-box-align:center;-webkit-align-items:center;-ms-flex-align:center;align-items:center}.uni-pagination__btn[data-v-be19eada]:after{content:"";width:200%;height:200%;position:absolute;top:0;left:0;border:1px solid #c8c7cc;-webkit-transform:scale(.5);-ms-transform:scale(.5);transform:scale(.5);-webkit-transform-origin:0 0;-ms-transform-origin:0 0;transform-origin:0 0;-webkit-box-sizing:border-box;box-sizing:border-box;border-radius:%?12?%}.uni-pagination__num[data-v-be19eada]{width:%?100?%;height:%?60?%;line-height:%?60?%;font-size:%?28?%;color:#333;position:absolute;left:50%;top:0;-webkit-transform:translateX(-50%);-ms-transform:translateX(-50%);transform:translateX(-50%)}.uni-pagination__num-current[data-v-be19eada]{color:#007aff}.uni-pagination--disabled[data-v-be19eada]{opacity:.3}.uni-pagination--hover[data-v-be19eada]{color:rgba(0,0,0,.6);background-color:#f1f1f1}',""])},"6a91":function(t,e,n){var i=n("4413");"string"===typeof i&&(i=[[t.i,i,""]]),i.locals&&(t.exports=i.locals);var a=n("4f06").default;a("514932ea",i,!0,{sourceMap:!1,shadowMode:!1})},"78e0":function(t,e,n){"use strict";n.r(e);var i=n("8c04"),a=n.n(i);for(var o in i)"default"!==o&&function(t){n.d(e,t,function(){return i[t]})}(o);e["default"]=a.a},"8c04":function(t,e,n){"use strict";Object.defineProperty(e,"__esModule",{value:!0}),e.default=void 0;var i=a(n("ed93"));function a(t){return t&&t.__esModule?t:{default:t}}var o={components:{uniPagination:i.default},data:function(){return{current:1,total:0,pageSize:10}},methods:{add:function(){this.total+=10},reset:function(){this.total=0,this.current=1},change:function(t){console.log(t),this.current=t.current}}};e.default=o},e0b8a:function(t,e,n){"use strict";n.r(e);var i=n("3387"),a=n.n(i);for(var o in i)"default"!==o&&function(t){n.d(e,t,function(){return i[t]})}(o);e["default"]=a.a},ed93:function(t,e,n){"use strict";n.r(e);var i=n("1e33"),a=n("e0b8a");for(var o in a)"default"!==o&&function(t){n.d(e,t,function(){return a[t]})}(o);n("14eb");var r=n("2877"),c=Object(r["a"])(a["default"],i["a"],i["b"],!1,null,"be19eada",null);e["default"]=c.exports},f15a:function(t,e,n){"use strict";n.r(e);var i=n("0103"),a=n("78e0");for(var o in a)"default"!==o&&function(t){n.d(e,t,function(){return a[t]})}(o);n("06f6");var r=n("2877"),c=Object(r["a"])(a["default"],i["a"],i["b"],!1,null,"e621189c",null);e["default"]=c.exports}}]);