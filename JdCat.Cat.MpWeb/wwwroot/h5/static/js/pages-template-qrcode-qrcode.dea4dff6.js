(window["webpackJsonp"]=window["webpackJsonp"]||[]).push([["pages-template-qrcode-qrcode"],{"276a":function(r,t,e){"use strict";var n=e("3487"),o=e.n(n);o.a},"2bbf":function(r,t,e){"use strict";var n=function(){var r=this,t=r.$createElement,e=r._self._c||t;return e("v-uni-view",{staticClass:"qrcode"},[r.img?e("v-uni-image",{staticClass:"image",style:{width:r.sizeSync+"px",height:r.sizeSync+"px"},attrs:{src:r.img}}):r._e()],1)},o=[];e.d(t,"a",function(){return n}),e.d(t,"b",function(){return o})},3487:function(r,t,e){var n=e("9e8e");"string"===typeof n&&(n=[[r.i,n,""]]),n.locals&&(r.exports=n.locals);var o=e("4f06").default;o("1bcc5da0",n,!0,{sourceMap:!1,shadowMode:!1})},"603b":function(r,t,e){"use strict";function n(r,t){if("undefined"==typeof r.length)throw new Error(r.length+"/"+t);var e=function(){for(var e=0;e<r.length&&0==r[e];)e+=1;for(var n=new Array(r.length-e+t),o=0;o<r.length-e;o+=1)n[o]=r[o+e];return n}(),o={getAt:function(r){return e[r]},getLength:function(){return e.length},multiply:function(r){for(var t=new Array(o.getLength()+r.getLength()-1),e=0;e<o.getLength();e+=1)for(var i=0;i<r.getLength();i+=1)t[e+i]^=c.gexp(c.glog(o.getAt(e))+c.glog(r.getAt(i)));return n(t,0)},mod:function(r){if(o.getLength()-r.getLength()<0)return o;for(var t=c.glog(o.getAt(0))-c.glog(r.getAt(0)),e=new Array(o.getLength()),i=0;i<o.getLength();i+=1)e[i]=o.getAt(i);for(i=0;i<r.getLength();i+=1)e[i]^=c.gexp(c.glog(r.getAt(i))+t);return n(e,0).mod(r)}};return o}var o=function(r,t){var e=236,o=17,i=r,u=a[t],c=null,g=0,h=null,d=new Array,w={},y=function(r,t){g=4*i+17,c=function(r){for(var t=new Array(r),e=0;e<r;e+=1){t[e]=new Array(r);for(var n=0;n<r;n+=1)t[e][n]=null}return t}(g),E(0,0),E(g-7,0),E(0,g-7),B(),A(),b(r,t),i>=7&&C(r),null==h&&(h=k(i,u,d)),M(h,t)},E=function(r,t){for(var e=-1;e<=7;e+=1)if(!(r+e<=-1||g<=r+e))for(var n=-1;n<=7;n+=1)t+n<=-1||g<=t+n||(c[r+e][t+n]=0<=e&&e<=6&&(0==n||6==n)||0<=n&&n<=6&&(0==e||6==e)||2<=e&&e<=4&&2<=n&&n<=4)},m=function(){for(var r=0,t=0,e=0;e<8;e+=1){y(!0,e);var n=f.getLostPoint(w);(0==e||r>n)&&(r=n,t=e)}return t},A=function(){for(var r=8;r<g-8;r+=1)null==c[r][6]&&(c[r][6]=r%2==0);for(var t=8;t<g-8;t+=1)null==c[6][t]&&(c[6][t]=t%2==0)},B=function(){for(var r=f.getPatternPosition(i),t=0;t<r.length;t+=1)for(var e=0;e<r.length;e+=1){var n=r[t],o=r[e];if(null==c[n][o])for(var a=-2;a<=2;a+=1)for(var u=-2;u<=2;u+=1)c[n+a][o+u]=-2==a||2==a||-2==u||2==u||0==a&&0==u}},C=function(r){for(var t=f.getBCHTypeNumber(i),e=0;e<18;e+=1){var n=!r&&1==(t>>e&1);c[Math.floor(e/3)][e%3+g-8-3]=n}for(e=0;e<18;e+=1){n=!r&&1==(t>>e&1);c[e%3+g-8-3][Math.floor(e/3)]=n}},b=function(r,t){for(var e=u<<3|t,n=f.getBCHTypeInfo(e),o=0;o<15;o+=1){var i=!r&&1==(n>>o&1);o<6?c[o][8]=i:o<8?c[o+1][8]=i:c[g-15+o][8]=i}for(o=0;o<15;o+=1){i=!r&&1==(n>>o&1);o<8?c[8][g-o-1]=i:o<9?c[8][15-o-1+1]=i:c[8][15-o-1]=i}c[g-8][8]=!r},M=function(r,t){for(var e=-1,n=g-1,o=7,i=0,a=f.getMaskFunction(t),u=g-1;u>0;u-=2)for(6==u&&(u-=1);;){for(var s=0;s<2;s+=1)if(null==c[n][u-s]){var l=!1;i<r.length&&(l=1==(r[i]>>>o&1));var v=a(n,u-s);v&&(l=!l),c[n][u-s]=l,o-=1,-1==o&&(i+=1,o=7)}if(n+=e,n<0||g<=n){n-=e,e=-e;break}}},T=function(r,t){for(var e=0,o=0,i=0,a=new Array(t.length),u=new Array(t.length),c=0;c<t.length;c+=1){var s=t[c].dataCount,l=t[c].totalCount-s;o=Math.max(o,s),i=Math.max(i,l),a[c]=new Array(s);for(var v=0;v<a[c].length;v+=1)a[c][v]=255&r.getBuffer()[v+e];e+=s;var g=f.getErrorCorrectPolynomial(l),h=n(a[c],g.getLength()-1),d=h.mod(g);u[c]=new Array(g.getLength()-1);for(v=0;v<u[c].length;v+=1){var w=v+d.getLength()-u[c].length;u[c][v]=w>=0?d.getAt(w):0}}var p=0;for(v=0;v<t.length;v+=1)p+=t[v].totalCount;var y=new Array(p),E=0;for(v=0;v<o;v+=1)for(c=0;c<t.length;c+=1)v<a[c].length&&(y[E]=a[c][v],E+=1);for(v=0;v<i;v+=1)for(c=0;c<t.length;c+=1)v<u[c].length&&(y[E]=u[c][v],E+=1);return y},k=function(r,t,n){for(var i=s.getRSBlocks(r,t),a=l(),u=0;u<n.length;u+=1){var c=n[u];a.put(c.getMode(),4),a.put(c.getLength(),f.getLengthInBits(c.getMode(),r)),c.write(a)}var v=0;for(u=0;u<i.length;u+=1)v+=i[u].dataCount;if(a.getLengthInBits()>8*v)throw new Error("code length overflow. ("+a.getLengthInBits()+">"+8*v+")");for(a.getLengthInBits()+4<=8*v&&a.put(0,4);a.getLengthInBits()%8!=0;)a.putBit(!1);for(;;){if(a.getLengthInBits()>=8*v)break;if(a.put(e,8),a.getLengthInBits()>=8*v)break;a.put(o,8)}return T(a,i)};return w.addData=function(r){var t=v(r);d.push(t),h=null},w.isDark=function(r,t){if(r<0||g<=r||t<0||g<=t)throw new Error(r+","+t);return c[r][t]},w.getModuleCount=function(){return g},w.make=function(){y(!1,m())},w.createTableTag=function(r,t){r=r||2,t="undefined"==typeof t?4*r:t;var e="";e+='<table style="',e+=" border-width: 0upx; border-style: none;",e+=" border-collapse: collapse;",e+=" padding: 0upx; margin: "+t+"upx;",e+='">',e+="<tbody>";for(var n=0;n<w.getModuleCount();n+=1){e+="<tr>";for(var o=0;o<w.getModuleCount();o+=1)e+='<td style="',e+=" border-width: 0upx; border-style: none;",e+=" border-collapse: collapse;",e+=" padding: 0upx; margin: 0upx;",e+=" width: "+r+"upx;",e+=" height: "+r+"upx;",e+=" background-color: ",e+=w.isDark(n,o)?"#000000":"#ffffff",e+=";",e+='"/>';e+="</tr>"}return e+="</tbody>",e+"</table>"},w.createImgTag=function(r,t,e){r=r||2,t="undefined"==typeof t?4*r:t;var n=t,o=w.getModuleCount()*r+t;return p(e,e,function(t,e){if(n<=t&&t<o&&n<=e&&e<o){var i=Math.floor((t-n)/r),a=Math.floor((e-n)/r);return w.isDark(a,i)?0:1}return 1})},w};o.stringToBytes=function(r){for(var t=new Array,e=0;e<r.length;e+=1){var n=r.charCodeAt(e);t.push(255&n)}return t},o.createStringToBytes=function(r,t){var e=function(){for(var e=d(r),n=function(){var r=e.read();if(-1==r)throw new Error;return r},o=0,i={};;){var a=e.read();if(-1==a)break;var u=n(),f=n(),c=n(),s=String.fromCharCode(a<<8|u),l=f<<8|c;i[s]=l,o+=1}if(o!=t)throw new Error(o+" != "+t);return i}(),n="?".charCodeAt(0);return function(r){for(var t=new Array,o=0;o<r.length;o+=1){var i=r.charCodeAt(o);if(i<128)t.push(i);else{var a=e[r.charAt(o)];"number"==typeof a?(255&a)==a?t.push(a):(t.push(a>>>8),t.push(255&a)):t.push(n)}}return t}};var i={MODE_NUMBER:1,MODE_ALPHA_NUM:2,MODE_8BIT_BYTE:4,MODE_KANJI:8},a={L:1,M:0,Q:3,H:2},u={PATTERN000:0,PATTERN001:1,PATTERN010:2,PATTERN011:3,PATTERN100:4,PATTERN101:5,PATTERN110:6,PATTERN111:7},f=function(){var r=[[],[6,18],[6,22],[6,26],[6,30],[6,34],[6,22,38],[6,24,42],[6,26,46],[6,28,50],[6,30,54],[6,32,58],[6,34,62],[6,26,46,66],[6,26,48,70],[6,26,50,74],[6,30,54,78],[6,30,56,82],[6,30,58,86],[6,34,62,90],[6,28,50,72,94],[6,26,50,74,98],[6,30,54,78,102],[6,28,54,80,106],[6,32,58,84,110],[6,30,58,86,114],[6,34,62,90,118],[6,26,50,74,98,122],[6,30,54,78,102,126],[6,26,52,78,104,130],[6,30,56,82,108,134],[6,34,60,86,112,138],[6,30,58,86,114,142],[6,34,62,90,118,146],[6,30,54,78,102,126,150],[6,24,50,76,102,128,154],[6,28,54,80,106,132,158],[6,32,58,84,110,136,162],[6,26,54,82,110,138,166],[6,30,58,86,114,142,170]],t=1335,e=7973,o=21522,a={},f=function(r){for(var t=0;0!=r;)t+=1,r>>>=1;return t};return a.getBCHTypeInfo=function(r){for(var e=r<<10;f(e)-f(t)>=0;)e^=t<<f(e)-f(t);return(r<<10|e)^o},a.getBCHTypeNumber=function(r){for(var t=r<<12;f(t)-f(e)>=0;)t^=e<<f(t)-f(e);return r<<12|t},a.getPatternPosition=function(t){return r[t-1]},a.getMaskFunction=function(r){switch(r){case u.PATTERN000:return function(r,t){return(r+t)%2==0};case u.PATTERN001:return function(r,t){return r%2==0};case u.PATTERN010:return function(r,t){return t%3==0};case u.PATTERN011:return function(r,t){return(r+t)%3==0};case u.PATTERN100:return function(r,t){return(Math.floor(r/2)+Math.floor(t/3))%2==0};case u.PATTERN101:return function(r,t){return r*t%2+r*t%3==0};case u.PATTERN110:return function(r,t){return(r*t%2+r*t%3)%2==0};case u.PATTERN111:return function(r,t){return(r*t%3+(r+t)%2)%2==0};default:throw new Error("bad maskPattern:"+r)}},a.getErrorCorrectPolynomial=function(r){for(var t=n([1],0),e=0;e<r;e+=1)t=t.multiply(n([1,c.gexp(e)],0));return t},a.getLengthInBits=function(r,t){if(1<=t&&t<10)switch(r){case i.MODE_NUMBER:return 10;case i.MODE_ALPHA_NUM:return 9;case i.MODE_8BIT_BYTE:return 8;case i.MODE_KANJI:return 8;default:throw new Error("mode:"+r)}else if(t<27)switch(r){case i.MODE_NUMBER:return 12;case i.MODE_ALPHA_NUM:return 11;case i.MODE_8BIT_BYTE:return 16;case i.MODE_KANJI:return 10;default:throw new Error("mode:"+r)}else{if(!(t<41))throw new Error("type:"+t);switch(r){case i.MODE_NUMBER:return 14;case i.MODE_ALPHA_NUM:return 13;case i.MODE_8BIT_BYTE:return 16;case i.MODE_KANJI:return 12;default:throw new Error("mode:"+r)}}},a.getLostPoint=function(r){for(var t=r.getModuleCount(),e=0,n=0;n<t;n+=1)for(var o=0;o<t;o+=1){for(var i=0,a=r.isDark(n,o),u=-1;u<=1;u+=1)if(!(n+u<0||t<=n+u))for(var f=-1;f<=1;f+=1)o+f<0||t<=o+f||0==u&&0==f||a==r.isDark(n+u,o+f)&&(i+=1);i>5&&(e+=3+i-5)}for(n=0;n<t-1;n+=1)for(o=0;o<t-1;o+=1){var c=0;r.isDark(n,o)&&(c+=1),r.isDark(n+1,o)&&(c+=1),r.isDark(n,o+1)&&(c+=1),r.isDark(n+1,o+1)&&(c+=1),0!=c&&4!=c||(e+=3)}for(n=0;n<t;n+=1)for(o=0;o<t-6;o+=1)r.isDark(n,o)&&!r.isDark(n,o+1)&&r.isDark(n,o+2)&&r.isDark(n,o+3)&&r.isDark(n,o+4)&&!r.isDark(n,o+5)&&r.isDark(n,o+6)&&(e+=40);for(o=0;o<t;o+=1)for(n=0;n<t-6;n+=1)r.isDark(n,o)&&!r.isDark(n+1,o)&&r.isDark(n+2,o)&&r.isDark(n+3,o)&&r.isDark(n+4,o)&&!r.isDark(n+5,o)&&r.isDark(n+6,o)&&(e+=40);var s=0;for(o=0;o<t;o+=1)for(n=0;n<t;n+=1)r.isDark(n,o)&&(s+=1);var l=Math.abs(100*s/t/t-50)/5;return e+10*l},a}(),c=function(){for(var r=new Array(256),t=new Array(256),e=0;e<8;e+=1)r[e]=1<<e;for(e=8;e<256;e+=1)r[e]=r[e-4]^r[e-5]^r[e-6]^r[e-8];for(e=0;e<255;e+=1)t[r[e]]=e;var n={glog:function(r){if(r<1)throw new Error("glog("+r+")");return t[r]},gexp:function(t){for(;t<0;)t+=255;for(;t>=256;)t-=255;return r[t]}};return n}(),s=function(){var r=[[1,26,19],[1,26,16],[1,26,13],[1,26,9],[1,44,34],[1,44,28],[1,44,22],[1,44,16],[1,70,55],[1,70,44],[2,35,17],[2,35,13],[1,100,80],[2,50,32],[2,50,24],[4,25,9],[1,134,108],[2,67,43],[2,33,15,2,34,16],[2,33,11,2,34,12],[2,86,68],[4,43,27],[4,43,19],[4,43,15],[2,98,78],[4,49,31],[2,32,14,4,33,15],[4,39,13,1,40,14],[2,121,97],[2,60,38,2,61,39],[4,40,18,2,41,19],[4,40,14,2,41,15],[2,146,116],[3,58,36,2,59,37],[4,36,16,4,37,17],[4,36,12,4,37,13],[2,86,68,2,87,69],[4,69,43,1,70,44],[6,43,19,2,44,20],[6,43,15,2,44,16],[4,101,81],[1,80,50,4,81,51],[4,50,22,4,51,23],[3,36,12,8,37,13],[2,116,92,2,117,93],[6,58,36,2,59,37],[4,46,20,6,47,21],[7,42,14,4,43,15],[4,133,107],[8,59,37,1,60,38],[8,44,20,4,45,21],[12,33,11,4,34,12],[3,145,115,1,146,116],[4,64,40,5,65,41],[11,36,16,5,37,17],[11,36,12,5,37,13],[5,109,87,1,110,88],[5,65,41,5,66,42],[5,54,24,7,55,25],[11,36,12],[5,122,98,1,123,99],[7,73,45,3,74,46],[15,43,19,2,44,20],[3,45,15,13,46,16],[1,135,107,5,136,108],[10,74,46,1,75,47],[1,50,22,15,51,23],[2,42,14,17,43,15],[5,150,120,1,151,121],[9,69,43,4,70,44],[17,50,22,1,51,23],[2,42,14,19,43,15],[3,141,113,4,142,114],[3,70,44,11,71,45],[17,47,21,4,48,22],[9,39,13,16,40,14],[3,135,107,5,136,108],[3,67,41,13,68,42],[15,54,24,5,55,25],[15,43,15,10,44,16],[4,144,116,4,145,117],[17,68,42],[17,50,22,6,51,23],[19,46,16,6,47,17],[2,139,111,7,140,112],[17,74,46],[7,54,24,16,55,25],[34,37,13],[4,151,121,5,152,122],[4,75,47,14,76,48],[11,54,24,14,55,25],[16,45,15,14,46,16],[6,147,117,4,148,118],[6,73,45,14,74,46],[11,54,24,16,55,25],[30,46,16,2,47,17],[8,132,106,4,133,107],[8,75,47,13,76,48],[7,54,24,22,55,25],[22,45,15,13,46,16],[10,142,114,2,143,115],[19,74,46,4,75,47],[28,50,22,6,51,23],[33,46,16,4,47,17],[8,152,122,4,153,123],[22,73,45,3,74,46],[8,53,23,26,54,24],[12,45,15,28,46,16],[3,147,117,10,148,118],[3,73,45,23,74,46],[4,54,24,31,55,25],[11,45,15,31,46,16],[7,146,116,7,147,117],[21,73,45,7,74,46],[1,53,23,37,54,24],[19,45,15,26,46,16],[5,145,115,10,146,116],[19,75,47,10,76,48],[15,54,24,25,55,25],[23,45,15,25,46,16],[13,145,115,3,146,116],[2,74,46,29,75,47],[42,54,24,1,55,25],[23,45,15,28,46,16],[17,145,115],[10,74,46,23,75,47],[10,54,24,35,55,25],[19,45,15,35,46,16],[17,145,115,1,146,116],[14,74,46,21,75,47],[29,54,24,19,55,25],[11,45,15,46,46,16],[13,145,115,6,146,116],[14,74,46,23,75,47],[44,54,24,7,55,25],[59,46,16,1,47,17],[12,151,121,7,152,122],[12,75,47,26,76,48],[39,54,24,14,55,25],[22,45,15,41,46,16],[6,151,121,14,152,122],[6,75,47,34,76,48],[46,54,24,10,55,25],[2,45,15,64,46,16],[17,152,122,4,153,123],[29,74,46,14,75,47],[49,54,24,10,55,25],[24,45,15,46,46,16],[4,152,122,18,153,123],[13,74,46,32,75,47],[48,54,24,14,55,25],[42,45,15,32,46,16],[20,147,117,4,148,118],[40,75,47,7,76,48],[43,54,24,22,55,25],[10,45,15,67,46,16],[19,148,118,6,149,119],[18,75,47,31,76,48],[34,54,24,34,55,25],[20,45,15,61,46,16]],t=function(r,t){var e={};return e.totalCount=r,e.dataCount=t,e},e={},n=function(t,e){switch(e){case a.L:return r[4*(t-1)+0];case a.M:return r[4*(t-1)+1];case a.Q:return r[4*(t-1)+2];case a.H:return r[4*(t-1)+3];default:return}};return e.getRSBlocks=function(r,e){var o=n(r,e);if("undefined"==typeof o)throw new Error("bad rs block [url=home.php?mod=space&uid=5302]@[/url] typeNumber:"+r+"/errorCorrectLevel:"+e);for(var i=o.length/3,a=new Array,u=0;u<i;u+=1)for(var f=o[3*u+0],c=o[3*u+1],s=o[3*u+2],l=0;l<f;l+=1)a.push(t(c,s));return a},e}(),l=function(){var r=new Array,t=0,e={getBuffer:function(){return r},getAt:function(t){var e=Math.floor(t/8);return 1==(r[e]>>>7-t%8&1)},put:function(r,t){for(var n=0;n<t;n+=1)e.putBit(1==(r>>>t-n-1&1))},getLengthInBits:function(){return t},putBit:function(e){var n=Math.floor(t/8);r.length<=n&&r.push(0),e&&(r[n]|=128>>>t%8),t+=1}};return e},v=function(r){for(var t=i.MODE_8BIT_BYTE,e=r,n=[],o={},a=0,u=e.length;a<u;a++){var f=[],c=e.charCodeAt(a);c>65536?(f[0]=240|(1835008&c)>>>18,f[1]=128|(258048&c)>>>12,f[2]=128|(4032&c)>>>6,f[3]=128|63&c):c>2048?(f[0]=224|(61440&c)>>>12,f[1]=128|(4032&c)>>>6,f[2]=128|63&c):c>128?(f[0]=192|(1984&c)>>>6,f[1]=128|63&c):f[0]=c,n.push(f)}n=Array.prototype.concat.apply([],n),n.length!=e.length&&(n.unshift(191),n.unshift(187),n.unshift(239));var s=n;return o.getMode=function(){return t},o.getLength=function(r){return s.length},o.write=function(r){for(var t=0;t<s.length;t+=1)r.put(s[t],8)},o},g=function(){var r=new Array,t={writeByte:function(t){r.push(255&t)},writeShort:function(r){t.writeByte(r),t.writeByte(r>>>8)},writeBytes:function(r,e,n){e=e||0,n=n||r.length;for(var o=0;o<n;o+=1)t.writeByte(r[o+e])},writeString:function(r){for(var e=0;e<r.length;e+=1)t.writeByte(r.charCodeAt(e))},toByteArray:function(){return r},toString:function(){var t="";t+="[";for(var e=0;e<r.length;e+=1)e>0&&(t+=","),t+=r[e];return t+"]"}};return t},h=function(){var r=0,t=0,e=0,n="",o={},i=function(r){n+=String.fromCharCode(a(63&r))},a=function(r){if(r<0);else{if(r<26)return 65+r;if(r<52)return r-26+97;if(r<62)return r-52+48;if(62==r)return 43;if(63==r)return 47}throw new Error("n:"+r)};return o.writeByte=function(n){for(r=r<<8|255&n,t+=8,e+=1;t>=6;)i(r>>>t-6),t-=6},o.flush=function(){if(t>0&&(i(r<<6-t),r=0,t=0),e%3!=0)for(var o=3-e%3,a=0;a<o;a+=1)n+="="},o.toString=function(){return n},o},d=function(r){var t=r,e=0,n=0,o=0,i={read:function(){for(;o<8;){if(e>=t.length){if(0==o)return-1;throw new Error("unexpected end of file./"+o)}var r=t.charAt(e);if(e+=1,"="==r)return o=0,-1;r.match(/^\s$/)||(n=n<<6|a(r.charCodeAt(0)),o+=6)}var i=n>>>o-8&255;return o-=8,i}},a=function(r){if(65<=r&&r<=90)return r-65;if(97<=r&&r<=122)return r-97+26;if(48<=r&&r<=57)return r-48+52;if(43==r)return 62;if(47==r)return 63;throw new Error("c:"+r)};return i},w=function(r,t){var e=r,n=t,o=new Array(r*t),i={setPixel:function(r,t,n){o[t*e+r]=n},write:function(r){r.writeString("GIF87a"),r.writeShort(e),r.writeShort(n),r.writeByte(128),r.writeByte(0),r.writeByte(0),r.writeByte(0),r.writeByte(0),r.writeByte(0),r.writeByte(255),r.writeByte(255),r.writeByte(255),r.writeString(","),r.writeShort(0),r.writeShort(0),r.writeShort(e),r.writeShort(n),r.writeByte(0);var t=2,o=u(t);r.writeByte(t);for(var i=0;o.length-i>255;)r.writeByte(255),r.writeBytes(o,i,255),i+=255;r.writeByte(o.length-i),r.writeBytes(o,i,o.length-i),r.writeByte(0),r.writeString(";")}},a=function(r){var t=r,e=0,n=0,o={write:function(r,o){if(r>>>o!=0)throw new Error("length over");for(;e+o>=8;)t.writeByte(255&(r<<e|n)),o-=8-e,r>>>=8-e,n=0,e=0;n|=r<<e,e+=o},flush:function(){e>0&&t.writeByte(n)}};return o},u=function(r){for(var t=1<<r,e=1+(1<<r),n=r+1,i=f(),u=0;u<t;u+=1)i.add(String.fromCharCode(u));i.add(String.fromCharCode(t)),i.add(String.fromCharCode(e));var c=g(),s=a(c);s.write(t,n);var l=0,v=String.fromCharCode(o[l]);for(l+=1;l<o.length;){var h=String.fromCharCode(o[l]);l+=1,i.contains(v+h)?v+=h:(s.write(i.indexOf(v),n),i.size()<4095&&(i.size()==1<<n&&(n+=1),i.add(v+h)),v=h)}return s.write(i.indexOf(v),n),s.write(e,n),s.flush(),c.toByteArray()},f=function(){var r={},t=0,e={add:function(n){if(e.contains(n))throw new Error("dup key:"+n);r[n]=t,t+=1},size:function(){return t},indexOf:function(t){return r[t]},contains:function(t){return"undefined"!=typeof r[t]}};return e};return i},p=function(r,t,e,n){for(var o=w(r,t),i=0;i<t;i+=1)for(var a=0;a<r;a+=1)o.setPixel(a,i,e(a,i));var u=g();o.write(u);for(var f=h(),c=u.toByteArray(),s=0;s<c.length;s+=1)f.writeByte(c[s]);f.flush();var l="";return l+="data:image/gif;base64,",l+f},y=function(r,t){t=t||{};var e,n=t.typeNumber||4,i=t.errorCorrectLevel||"M",a=t.size||500;try{e=o(n,i||"M"),e.addData(r),e.make()}catch(t){if(n>=40)throw new Error("Text too long to encode");return gen(r,{size:a,errorCorrectLevel:i,typeNumber:n+1})}var u=parseInt(a/e.getModuleCount()),f=parseInt((a-e.getModuleCount()*u)/2);return e.createImgTag(u,f,a)};r.exports={createQrCodeImg:y}},"9e8e":function(r,t,e){t=r.exports=e("2350")(!1),t.push([r.i,".qrcode[data-v-91a0c81a]{display:-webkit-box;display:-webkit-flex;display:-ms-flexbox;display:flex;-webkit-box-pack:center;-webkit-justify-content:center;-ms-flex-pack:center;justify-content:center}",""])},"9f3c3":function(r,t,e){"use strict";e.r(t);var n=e("d782"),o=e.n(n);for(var i in n)"default"!==i&&function(r){e.d(t,r,function(){return n[r]})}(i);t["default"]=o.a},a934:function(r,t,e){"use strict";var n=function(){var r=this,t=r.$createElement,e=r._self._c||t;return e("v-uni-view",{staticClass:"container"},[e("page-head",{attrs:{title:r.title}}),e("v-uni-view",[e("qrcode",{ref:"qrcode",attrs:{val:r.qrval,size:r.qrsize}})],1),e("v-uni-view",{staticClass:"uni-padding-wrap"},[e("v-uni-view",{staticClass:"uni-title"},[r._v("请输入要生成的二维码内容")])],1),e("v-uni-view",{staticClass:"uni-list"},[e("v-uni-view",{staticClass:"uni-list-cell"},[e("v-uni-input",{staticClass:"uni-input",attrs:{placeholder:"请输入要生成的二维码内容",value:r.qrval},on:{input:function(t){t=r.$handleEvent(t),r.bindClearInput(t)}}}),r.showClearIcon?e("v-uni-view",{staticClass:"uni-icon uni-icon-clear",on:{click:function(t){t=r.$handleEvent(t),r.clearIcon(t)}}}):r._e()],1)],1),e("v-uni-view",{staticClass:"uni-padding-wrap uni-common-mt"},[e("v-uni-view",{staticClass:"uni-title"},[r._v("设置二维码大小")])],1),e("v-uni-view",{staticClass:"body-view"},[e("v-uni-slider",{attrs:{value:r.qrsize,min:"50",max:"500","show-value":""},on:{change:function(t){t=r.$handleEvent(t),r.sliderchange(t)}}})],1),e("v-uni-view",{staticClass:"uni-padding-wrap"},[e("v-uni-view",{staticClass:"uni-btn-v uni-common-mt"},[e("v-uni-button",{attrs:{type:"primary"},on:{click:function(t){t=r.$handleEvent(t),r.creatQrcode(t)}}},[r._v("生成二维码")]),e("v-uni-button",{attrs:{type:"warn"},on:{click:function(t){t=r.$handleEvent(t),r.clearQrcode(t)}}},[r._v("清除二维码")])],1)],1),e("page-foot",{attrs:{name:r.name}})],1)},o=[];e.d(t,"a",function(){return n}),e.d(t,"b",function(){return o})},ac85:function(r,t,e){"use strict";e.r(t);var n=e("2bbf"),o=e("d950");for(var i in o)"default"!==i&&function(r){e.d(t,r,function(){return o[r]})}(i);e("276a");var a=e("2877"),u=Object(a["a"])(o["default"],n["a"],n["b"],!1,null,"91a0c81a",null);t["default"]=u.exports},d45c:function(r,t,e){"use strict";Object.defineProperty(t,"__esModule",{value:!0}),t.default=void 0;var n=o(e("603b"));function o(r){return r&&r.__esModule?r:{default:r}}var i={name:"number-box",props:{val:{type:String,default:""},size:{type:Number,default:100}},data:function(){return{img:"",sizeSync:100}},methods:{creatQrcode:function(){var r=this.val+"";if(r){var t=n.default.createQrCodeImg(r,{size:parseInt(this.size)});this.img=t}},clearQrcode:function(){this.img=""}},watch:{size:function(r,t){r!=t&&(this.sizeSync=r,this.creatQrcode())}},created:function(){this.sizeSync=this.size}};t.default=i},d782:function(r,t,e){"use strict";Object.defineProperty(t,"__esModule",{value:!0}),t.default=void 0;var n=o(e("ac85"));function o(r){return r&&r.__esModule?r:{default:r}}var i={data:function(){return{title:"二维码生成",name:"诗小柒",showClearIcon:!1,qrval:"",qrsize:100}},methods:{bindClearInput:function(r){this.qrval=r.target.value,r.target.value.length>0?this.showClearIcon=!0:this.showClearIcon=!1},clearIcon:function(){this.qrval="",this.showClearIcon=!1},sliderchange:function(r){this.qrsize=r.detail.value},creatQrcode:function(){this.qrval?this.$refs.qrcode.creatQrcode():uni.showToast({title:"请输入二维码内容",icon:"none"})},clearQrcode:function(){this.$refs.qrcode.clearQrcode(),this.clearIcon()}},components:{qrcode:n.default}};t.default=i},d950:function(r,t,e){"use strict";e.r(t);var n=e("d45c"),o=e.n(n);for(var i in n)"default"!==i&&function(r){e.d(t,r,function(){return n[r]})}(i);t["default"]=o.a},dfd3:function(r,t,e){"use strict";e.r(t);var n=e("a934"),o=e("9f3c3");for(var i in o)"default"!==i&&function(r){e.d(t,r,function(){return o[r]})}(i);var a=e("2877"),u=Object(a["a"])(o["default"],n["a"],n["b"],!1,null,null,null);t["default"]=u.exports}}]);