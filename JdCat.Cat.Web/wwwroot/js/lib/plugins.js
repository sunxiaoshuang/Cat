(function ($) {
    var appObj = {
        $loading: null
    };
    $.extend({
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
        loaded: function () {
            appObj.$loading.hide();
        }
    });

})(jQuery);