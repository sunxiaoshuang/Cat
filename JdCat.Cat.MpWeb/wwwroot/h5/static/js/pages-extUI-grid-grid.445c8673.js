(window["webpackJsonp"]=window["webpackJsonp"]||[]).push([["pages-extUI-grid-grid"],{"354d":function(i,t,e){"use strict";var n=e("ff76"),r=e.n(n);r.a},"3caf":function(i,t,e){var n=e("86ba");"string"===typeof n&&(n=[[i.i,n,""]]),n.locals&&(i.exports=n.locals);var r=e("4f06").default;r("2cf0dbfc",n,!0,{sourceMap:!1,shadowMode:!1})},"6cc6":function(i,t,e){"use strict";Object.defineProperty(t,"__esModule",{value:!0}),t.default=void 0;var n={name:"UniGrid",props:{options:{type:Array,default:function(){return[]}},type:{type:String,default:"square"},columnNum:{type:[Number,String],default:3},showOutBorder:{type:Boolean,default:!0},showBorder:{type:Boolean,default:!0}},data:function(){return{}},computed:{gridGroup:function(){var i=this,t=[],e=[];if(this.options&&this.options.forEach(function(n,r){e.push(n),r%i.columnNum===i.columnNum-1&&(t.push(e),e=[])}),e.length>0){if(this.columnNum>e.length)for(var n=0,r=e.length;n<this.columnNum-r;n++)e.push({seize:!0});t.push(e)}return e=null,t}},created:function(){this.columnNumber=this.gridGroup[0].length},methods:{onClick:function(i,t){this.$emit("click",{index:i*this.columnNumber+t})}}};t.default=n},"85b8":function(i,t,e){"use strict";e.r(t);var n=e("6cc6"),r=e.n(n);for(var o in n)"default"!==o&&function(i){e.d(t,i,function(){return n[i]})}(o);t["default"]=r.a},"86ba":function(i,t,e){t=i.exports=e("2350")(!1),t.push([i.i,'.uni-grid[data-v-2fb8901e]{position:relative;display:-webkit-box;display:-webkit-flex;display:-ms-flexbox;display:flex;-webkit-box-orient:vertical;-webkit-box-direction:normal;-webkit-flex-direction:column;-ms-flex-direction:column;flex-direction:column}.uni-grid__flex[data-v-2fb8901e]{display:-webkit-box;display:-webkit-flex;display:-ms-flexbox;display:flex;-webkit-box-orient:horizontal;-webkit-box-direction:normal;-webkit-flex-direction:row;-ms-flex-direction:row;flex-direction:row}.uni-grid-item[data-v-2fb8901e]{display:-webkit-box;display:-webkit-flex;display:-ms-flexbox;display:flex;position:relative;-webkit-box-orient:vertical;-webkit-box-direction:normal;-webkit-flex-direction:column;-ms-flex-direction:column;flex-direction:column;-webkit-box-flex:1;-webkit-flex:1;-ms-flex:1;flex:1}.uni-grid-item[data-v-2fb8901e]:before{display:block;content:" ";padding-bottom:100%}.uni-grid-item[data-v-2fb8901e]:after{content:"";position:absolute;z-index:1;-webkit-transform-origin:center;-ms-transform-origin:center;transform-origin:center;-webkit-box-sizing:border-box;box-sizing:border-box;top:-50%;left:-50%;right:-50%;bottom:-50%;border-color:#c8c7cc;border-style:solid;border-width:1px;-webkit-transform:scale(.5);-ms-transform:scale(.5);transform:scale(.5);border-top-width:0;border-left-width:0}.uni-grid-item__content[data-v-2fb8901e]{position:absolute;left:0;top:0;width:100%;height:100%;display:-webkit-box;display:-webkit-flex;display:-ms-flexbox;display:flex;-webkit-box-orient:vertical;-webkit-box-direction:normal;-webkit-flex-direction:column;-ms-flex-direction:column;flex-direction:column;-webkit-box-pack:center;-webkit-justify-content:center;-ms-flex-pack:center;justify-content:center;-webkit-box-align:center;-webkit-align-items:center;-ms-flex-align:center;align-items:center}.uni-grid-item-text[data-v-2fb8901e]{font-size:%?32?%;color:#333;margin-top:%?12?%}.uni-grid-item-hover[data-v-2fb8901e]{background-color:#f1f1f1}.uni-grid-item-image[data-v-2fb8901e]{width:%?80?%;height:%?80?%}.uni-grid .uni-grid__flex:first-child .uni-grid-item[data-v-2fb8901e]:after{border-top-width:1px}.uni-grid .uni-grid__flex .uni-grid-item[data-v-2fb8901e]:first-child:after{border-left-width:1px}.uni-grid.uni-grid-no-out-border .uni-grid__flex:first-child .uni-grid-item[data-v-2fb8901e]:after{border-top-width:0}.uni-grid.uni-grid-no-out-border .uni-grid__flex:last-child .uni-grid-item[data-v-2fb8901e]:after{border-bottom-width:0}.uni-grid.uni-grid-no-out-border .uni-grid__flex .uni-grid-item[data-v-2fb8901e]:first-child:after{border-left-width:0}.uni-grid.uni-grid-no-out-border .uni-grid__flex .uni-grid-item[data-v-2fb8901e]:last-child:after{border-right-width:0}.uni-grid.uni-grid-no-border .uni-grid-item[data-v-2fb8901e]:after{border-width:0}.uni-grid.uni-grid-no-border .uni-grid__flex:first-child .uni-grid-item[data-v-2fb8901e]:after{border-top-width:0}.uni-grid.uni-grid-no-border .uni-grid__flex .uni-grid-item[data-v-2fb8901e]:first-child:after{border-left-width:0}.uni-grid-item-oblong.uni-grid-item[data-v-2fb8901e]:before{padding-bottom:60%}.uni-grid-item-oblong .uni-grid-item__content[data-v-2fb8901e]{-webkit-box-orient:horizontal;-webkit-box-direction:normal;-webkit-flex-direction:row;-ms-flex-direction:row;flex-direction:row}.uni-grid-item-oblong .uni-grid-item-image[data-v-2fb8901e]{width:%?52?%;height:%?52?%}.uni-grid-item-oblong .uni-grid-item-text[data-v-2fb8901e]{margin-top:0;margin-left:%?12?%}',""])},"9cc4":function(i,t,e){"use strict";var n=e("3caf"),r=e.n(n);r.a},a12d:function(i,t,e){"use strict";Object.defineProperty(t,"__esModule",{value:!0}),t.default=void 0;var n=r(e("d05f"));function r(i){return i&&i.__esModule?i:{default:i}}var o={components:{uniGrid:n.default},data:function(){return{swiperGridHeight:"0px",swiperGridWidth:"100%",data1:[{image:"/static/c1.png",text:"Grid"},{image:"/static/c2.png",text:"Grid"},{image:"/static/c3.png",text:"Grid"},{image:"/static/c4.png",text:"Grid"},{image:"/static/c5.png",text:"Grid"},{image:"/static/c6.png",text:"Grid"},{image:"/static/c7.png",text:"Grid"},{image:"/static/c8.png",text:"Grid"},{image:"/static/c9.png",text:"Grid"}],data2:[{image:"/static/c1.png",text:"Grid"},{image:"/static/c2.png",text:"Grid"},{image:"/static/c3.png",text:"Grid"},{image:"/static/c4.png",text:"Grid"},{image:"/static/c5.png",text:"Grid"},{image:"/static/c6.png",text:"Grid"},{image:"/static/c7.png",text:"Grid"},{image:"/static/c8.png",text:"Grid"}],data3:[{image:"/static/c1.png",text:"Grid"},{image:"/static/c2.png",text:"Grid"},{image:"/static/c3.png",text:"Grid"},{image:"/static/c4.png",text:"Grid"},{image:"/static/c5.png",text:"Grid"},{image:"/static/c6.png",text:"Grid"}]}},onReady:function(){var i=this;uni.createSelectorQuery().select(".grid-view").boundingClientRect().exec(function(t){i.swiperGridHeight=t[0].height+1+"px"})},methods:{onClick:function(i){console.log("点击grid:"+JSON.stringify(i))}}};t.default=o},a41e:function(i,t,e){"use strict";e.r(t);var n=e("a12d"),r=e.n(n);for(var o in n)"default"!==o&&function(i){e.d(t,i,function(){return n[i]})}(o);t["default"]=r.a},a6e8:function(i,t,e){"use strict";var n=function(){var i=this,t=i.$createElement,e=i._self._c||t;return e("v-uni-view",{staticClass:"uni-grid",class:{"uni-grid-no-border":!i.showBorder,"uni-grid-no-out-border":i.showBorder&&!i.showOutBorder}},i._l(i.gridGroup,function(t,n){return e("v-uni-view",{key:n,staticClass:"uni-grid__flex"},i._l(t,function(t,r){return e("v-uni-view",{key:r,staticClass:"uni-grid-item",class:[r==i.columnNum?"uni-grid-item-last":"","uni-grid-item-"+i.type],style:{visibility:t.seize?"hidden":"inherit"},attrs:{"hover-start-time":20,"hover-stay-time":70,"hover-class":"uni-grid-item-hover"},on:{click:function(t){t=i.$handleEvent(t),i.onClick(n,r)}}},[t.seize?i._e():e("v-uni-view",{staticClass:"uni-grid-item__content"},[e("v-uni-image",{staticClass:"uni-grid-item-image",attrs:{src:t.image}}),e("v-uni-text",{staticClass:"uni-grid-item-text"},[i._v(i._s(t.text))])],1)],1)}),1)}),1)},r=[];e.d(t,"a",function(){return n}),e.d(t,"b",function(){return r})},beb1:function(i,t,e){t=i.exports=e("2350")(!1),t.push([i.i,"uni-page-body[data-v-1c9fdf28]{display:-webkit-box;display:-webkit-flex;display:-ms-flexbox;display:flex;-webkit-box-orient:vertical;-webkit-box-direction:normal;-webkit-flex-direction:column;-ms-flex-direction:column;flex-direction:column;-webkit-box-sizing:border-box;box-sizing:border-box;background-color:#fff}uni-view[data-v-1c9fdf28]{font-size:%?28?%;line-height:inherit}.example[data-v-1c9fdf28]{padding:0 %?30?% %?30?%}.example-title[data-v-1c9fdf28]{font-size:%?32?%;line-height:%?32?%;color:#777;margin:%?40?% %?25?%;position:relative}.example .example-title[data-v-1c9fdf28]{margin:%?40?% 0}.example-body[data-v-1c9fdf28]{padding:0 %?40?%}.grid-view[data-v-1c9fdf28]{\n\tpadding:0 .5px;\n\t-webkit-box-sizing:border-box;box-sizing:border-box}body.?%PAGE?%[data-v-1c9fdf28]{background-color:#fff}",""])},bfcc:function(i,t,e){"use strict";e.r(t);var n=e("da07"),r=e("a41e");for(var o in r)"default"!==o&&function(i){e.d(t,i,function(){return r[i]})}(o);e("354d");var a=e("2877"),d=Object(a["a"])(r["default"],n["a"],n["b"],!1,null,"1c9fdf28",null);t["default"]=d.exports},d05f:function(i,t,e){"use strict";e.r(t);var n=e("a6e8"),r=e("85b8");for(var o in r)"default"!==o&&function(i){e.d(t,i,function(){return r[i]})}(o);e("9cc4");var a=e("2877"),d=Object(a["a"])(r["default"],n["a"],n["b"],!1,null,"2fb8901e",null);t["default"]=d.exports},da07:function(i,t,e){"use strict";var n=function(){var i=this,t=i.$createElement,e=i._self._c||t;return e("v-uni-view",{staticClass:"page"},[e("v-uni-view",{staticClass:"example"},[e("v-uni-view",{staticClass:"example-title"},[i._v("默认样式")]),e("uni-grid",{attrs:{options:i.data1},on:{click:function(t){t=i.$handleEvent(t),i.onClick(t)}}}),e("v-uni-view",{staticClass:"example-title"},[i._v("可滑动宫格组件")]),e("v-uni-swiper",{style:{height:i.swiperGridHeight,width:i.swiperGridWidth},attrs:{"indicator-dots":!0}},[e("v-uni-swiper-item",[e("v-uni-view",{staticClass:"grid-view"},[e("uni-grid",{attrs:{options:i.data1},on:{click:function(t){t=i.$handleEvent(t),i.onClick(t)}}})],1)],1),e("v-uni-swiper-item",[e("v-uni-view",{staticClass:"grid-view"},[e("uni-grid",{attrs:{options:i.data1},on:{click:function(t){t=i.$handleEvent(t),i.onClick(t)}}})],1)],1)],1),e("v-uni-view",{staticClass:"example-title"},[i._v("无外边框")]),e("uni-grid",{attrs:{options:i.data3,"show-out-border":!1}}),e("v-uni-view",{staticClass:"example-title"},[i._v("无所有框")]),e("uni-grid",{attrs:{options:i.data3,"show-border":!1}}),e("v-uni-view",{staticClass:"example-title"},[i._v("一行四个")]),e("uni-grid",{attrs:{options:i.data2,"show-out-border":!1,"column-num":4}}),e("v-uni-view",{staticClass:"example-title"},[i._v("矩形案例")]),e("uni-grid",{attrs:{options:i.data3,type:"oblong"}})],1)],1)},r=[];e.d(t,"a",function(){return n}),e.d(t,"b",function(){return r})},ff76:function(i,t,e){var n=e("beb1");"string"===typeof n&&(n=[[i.i,n,""]]),n.locals&&(i.exports=n.locals);var r=e("4f06").default;r("297aeca5",n,!0,{sourceMap:!1,shadowMode:!1})}}]);