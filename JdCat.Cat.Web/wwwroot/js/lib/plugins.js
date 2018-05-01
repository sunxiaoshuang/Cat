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
            appObj.$loading.hide();
        },
        // 模态框，加载页面
        view: function () {
            var template = '<div class="modal fade" id="{name}" aria-hidden="true">\
                <div class="modal-dialog" >\
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
            $.view = function (name, title, url) {
                var obj = {
                    name: name, title: title, url: url,
                    footDisplay: "none", closeText: "关闭", saveText: "保存"
                }, html, $modal, args;
                if (arguments.length === 1) {
                    $.extend(obj, arguments[0]);
                }

                $.post(obj.url, null, function (page) {
                    obj.body = page;
                    html = template.format(obj);
                    $modal = $(html).appendTo($body);
                    $modal.modal({ backdrop: "static" });
                    $modal.on("hidden.bs.modal", function () {
                        $modal.remove();
                    });
                });
            };
            $.view.apply(this, arguments);
        },
        confirm: function () {

        }
    });

})(jQuery);