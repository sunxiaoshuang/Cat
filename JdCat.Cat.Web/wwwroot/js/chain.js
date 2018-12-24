
(function ($) {
    "use strict";
    var mainApp = {

        initFunction: function () {
            // 菜单
            mainApp.$menu = $('#main-menu');
            mainApp.$menu.metisMenu();
            mainApp.$menu.on("click", "a", function () {
                var self = $(this);
                if (self.siblings()[0]) return;

                mainApp.$menu.find("a.active-menu").removeClass("active-menu");
                self.addClass("active-menu");
            });

            $(window).bind("load resize", function () {
                if ($(this).width() < 768) {
                    $('div.sidebar-collapse').addClass('collapse')
                } else {
                    $('div.sidebar-collapse').removeClass('collapse')
                }
            });
        },

        initialization: function () {
            mainApp.initFunction();
        }
    };

    $(document).ready(function () {
        mainApp.initFunction();
        $("#sideNav").click(function () {
            if ($(this).hasClass('closed')) {
                $('.navbar-side').animate({ left: '0px' });
                $(this).removeClass('closed');
                $('#page-wrapper').animate({ 'margin-left': '260px' });

            }
            else {
                $(this).addClass('closed');
                $('.navbar-side').animate({ left: '-260px' });
                $('#page-wrapper').animate({ 'margin-left': '0px' });
            }
        });
    });

    var pageObj = {
        $frame: $("#mainframe"),
        defaultUrl: "/Report",
        method: {
            hashchange: function () {
                var url = pageObj.defaultUrl;
                if (window.location.href.indexOf("#") > -1) {
                    url = window.location.href.substring(window.location.href.indexOf("#") + 1) || "/home/empty";
                }
                pageObj.$frame.attr("src", url);
            }
        }
    };

    // 菜单切换
    window.onhashchange = function () {
        pageObj.method.hashchange();
    };
    pageObj.method.hashchange();

}(jQuery));
