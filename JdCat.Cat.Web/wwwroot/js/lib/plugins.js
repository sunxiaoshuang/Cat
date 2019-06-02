(function ($) {
    var appObj = {
        $loading: null
    };
    $.extend({
        // 页面加载
        loading: function () {
            var html = '<div class="loading-container">\
                            <div class="loading">\
                                <span></span>\
                                <span></span>\
                                <span></span>\
                                <span></span>\
                                <span></span>\
                                <span></span>\
                                <span></span>\
                                <span></span>\
                                <label>正在加载，请稍等...</label>\
                            </div>\
                            <div class="loading-shadow"></div>\
                        </div> ';
            appObj.$loading = $(html);
            $(document.body).append(appObj.$loading);
            $.loading = function () {
                appObj.$loading.show();
            };
            $.loading();
        },
        // 加载完成
        loaded: function () {
            appObj.$loading && appObj.$loading.hide();
        },
        // 模态框，加载页面，使用template加载视图模版，可以取到模态框返回值
        view: function () {
            var template = '<div class="modal fade" id="{name}" aria-hidden="true">\
                <div class="modal-dialog" style="width: {dialogWidth}px;">\
                    <div class="modal-content">\
                        <div class="modal-header">\
                            <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>\
                            <h5 class="modal-title">{title}</h5>\
                        </div>\
                        <div class="modal-body">{body}</div>\
                        <div class="modal-footer" style="display: {footDisplay}">\
                            <button type="button" class="btn btn-default" data-dismiss="modal">{closeText}</button>\
                            <button type="button" class="btn btn-primary btn-save">{saveText}</button>\
                        </div>\
                    </div>\
                </div >\
            </div >';
            var $body = $(document.body);
            var loadView = function (obj) {
                var html = template.format(obj);
                var $modal = $(html).appendTo($body);
                $modal.modal({ backdrop: "static", keyboard: obj.keyboard === undefined ? true : obj.keyboard });
                $modal.on("hidden.bs.modal", function () {
                    $modal.remove();
                });
                if (obj.submit) {
                    $modal.on("click", ".btn-save", function (e) {
                        var result = obj.submit.call(this, e, $modal);
                        if (result) {
                            $modal.modal("hide");
                        }
                    });
                }
                if (obj.load) {
                    obj.load.call($modal);
                }
                return $modal;
            };
            $.view = function (name, title, url, template) {
                var obj = {
                    title: title, url: url, template: template, name: `modal_${Date.parse(new Date())}`,
                    footDisplay: "none", closeText: "关闭", saveText: "保存", dialogWidth: 600
                };
                if (arguments.length === 1) {
                    $.extend(obj, arguments[0]);
                }
                obj.name = obj.name || "view_jquery";

                if (obj.template) {
                    obj.body = obj.template;
                    return loadView(obj);
                }

                $.get(obj.url, null, function (page) {
                    obj.body = page;
                    loadView(obj);
                });
                return null;
            };
            return $.view.apply(this, arguments);
        },
        confirm: function () {
            var template = '<div class="modal fade modal-alert" aria-hidden="true">\
                <div class="modal-dialog" style="margin-top: 200px;width: 400px;">\
                    <div class="modal-content">\
                        <div class="modal-header {alertColor}">\
                            <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>\
                            <h5 class="modal-title {titleColor}">{pic}{title}</h5>\
                        </div>\
                        <div class="modal-body">{msg}</div>\
                        <div class="modal-footer">\
                            <button type="button" class="btn btn-default" data-dismiss="modal">关闭</button>\
                            <button type="button" class="btn {submitStyle} {submitDisplay}">{submitText}</button>\
                        </div>\
                    </div>\
                </div >\
            </div >';
            var $body = $(document.body);
            $.confirm = function (title, msg, submit, cancel, pic, alertColor, submitStyle, submitText, titleColor) {
                var p_1 = [].slice.call(arguments, 1), obj;
                if ($.isObject(p_1)) {
                    obj = $.extend({}, {
                        title: p_1.title || "提示",
                        titleColor: p_1.titleColor || "text-primary",
                        submitDisplay: !p_1.submit ? "hide" : "",
                        alertColor: p_1.alertColor || "",
                        submitStyle: p_1.submitStyle || "btn-primary",
                        pic: "<i class='fa " + (p_1.pic || "fa-question-circle-o") + "'></i>",
                        submitText: p_1.submitText || "确定"
                    })
                } else {
                    obj = {
                        title: title, msg: msg,
                        titleColor: titleColor || "text-primary",
                        submitDisplay: !submit ? "hide" : "",
                        alertColor: alertColor || "",
                        submitStyle: submitStyle || "btn-primary",
                        pic: "<i class='fa " + (pic || "fa-question-circle-o") + "'></i> ",
                        submitText: submitText || "确定"
                    };
                }
                var html = template.format(obj);
                $modal = $(html).appendTo($body);
                $modal.modal({ backdrop: "static" });
                $modal.on("hidden.bs.modal", function () {
                    if (cancel) {
                        cancel.call($modal);
                    }
                    $modal.remove();
                });
                if (submit) {
                    $modal.find(".btn-primary").click(function () {
                        var result = submit.call($modal);
                        if (result) {
                            $modal.modal("hide");
                        }
                    });
                }
                return $modal;
            };
            return $.confirm.apply(this, arguments);
        },
        warning: function (msg, submit, cancel) {
            return $.confirm("警告", msg, submit, cancel, "fa-warning text-warning", "bg-warning", "btn-warning", null, "text-warning");
        },
        info: function (msg, submit, cancel) {
            return $.confirm("提示", msg, submit, cancel, "fa-info-circle text-info", "bg-info", "btn-info", null, "text-info");
        },
        danger: function (msg, submit, cancel) {
            return $.confirm("错误", msg, submit, cancel, "fa-times-circle text-danger", "bg-danger", "btn-danger", null, "text-danger");
        },
        success: function (msg, submit, cancel) {
            return $.confirm("提示", msg, submit, cancel, "fa-check-circle text-success", "bg-success", "btn-success", null, "text-success");
        },
        primary: function (msg, submit, cancel) {
            return $.confirm("提示", msg, submit, cancel, "fa-check-circle text-primary", "bg-primary", "btn-primary", null, "text-primary");
        },
        alert: function (msg, status) {
            status = status || "error";
            var icon = "fa-times";
            switch (status) {
                case "success":
                    icon = "fa-check";
                    break;
                case "error":
                    icon = "fa-times";
                    break;
                case "warning":
                    icon = "fa-warning";
                    break;
                default:
            }
            var obj = {
                status: status,
                title: msg || "未处理异常",
                icon: icon
            };
            var html = `<div class="alert-status alert-{status}">
                            <div>
                                <span class="icon">
                                    <i class="fa {icon}"></i>
                                </span>
                                <span title="{title}" class="content">
                                    {title}
		                        </span>
	                        </div >
                        </div >`;
            var $alert = $(html.format(obj));
            var $body = $(document.body);
            $body.append($alert);
            var time = setTimeout(() => {
                $alert.css("opacity", 0);
                setTimeout(() => $alert.remove(), 500);
            }, 5000);
            $alert.on("click", function () {
                $alert.remove();
                clearTimeout(time);
            });
        },
        browser: {
            versions: function () {
                var u = navigator.userAgent;
                return { //移动终端浏览器版本信息 
                    trident: u.indexOf('Trident') > -1, //IE内核 
                    presto: u.indexOf('Presto') > -1, //opera内核 
                    webKit: u.indexOf('AppleWebKit') > -1, //苹果、谷歌内核 
                    gecko: u.indexOf('Gecko') > -1 && u.indexOf('KHTML') === -1, //火狐内核 
                    mobile: !!u.match(/AppleWebKit.*Mobile.*/), //是否为移动终端 
                    ios: !!u.match(/\(i[^;]+;( U;)? CPU.+Mac OS X/), //ios终端 
                    android: u.indexOf('Android') > -1 || u.indexOf('Linux') > -1 || u.indexOf('Adr') > -1, //android终端或uc浏览器 
                    iPhone: u.indexOf('iPhone') > -1, //是否为iPhone或者QQHD浏览器 
                    iPad: u.indexOf('iPad') > -1, //是否iPad 
                    webApp: u.indexOf('Safari') === -1,
                    weixin: u.indexOf('MicroMessenger') > -1, //是否微信 （2015-01-22新增）
                    qq: u.indexOf(' QQ') > -1   //是否QQ
                    //是否web应该程序，没有头部与底部 
                };
            }(),
            language: (navigator.browserLanguage || navigator.language)
                .toLowerCase()
        },
        dateOptions: {
            format: 'yyyy-mm-dd',
            autoclose: true,
            maxView: 4,
            minView: 2,
            todayBtn: true,
            todayHighlight: true,
            language: "zh-CN"
        }

    });

})(jQuery);