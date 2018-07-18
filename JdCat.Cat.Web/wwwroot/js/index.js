
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

    }

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
        defaultUrl: "/product",
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
    }
    pageObj.method.hashchange();

    // 新订单提醒
    var ws = new WebSocket(pageData.orderUrl + "?id=" + pageData.business.id);
    var newOrder = document.getElementById("newOrder");
    
    ws.onmessage = function (res) {
        var time = new Date();
        var arr = res.data.split("|");
        msg.list.push({ code: arr[0], status: arr[1], time: time.getHours() + ":" + time.getMinutes() });
        newOrder.play();
    };
    ws.onopen = function (a) {
        console.log("服务已连接", a);
    }
    ws.onclose = function (a) {
        $.primary(a.reason || "新订单提醒异常，请刷新后重试");
    }

    var msg = new Vue({
        el: "#msg",
        data: {
            list: [

            ]
        },
        components: {
            "cat-order-msg": {
                data: function () {
                    return {};
                },
                methods: {
                    navigate: function () {
                        msg.list.remove(this.msg);
                    }
                },
                props: ["msg"],
                template: `
                        <li>
                            <a :href="'#/Order?code=' + msg.code" @click="navigate()">
                                <div>
                                    <i class="fa fa-info-circle fa-fw text-primary"></i> 您有一个新订单
                                    <span class="pull-right text-muted small">{{msg.time}}</span>
                                </div>
                            </a>
                        </li>`
            }
        }
    });

    //$.primary($.browser.versions.mobile);
    //if ($.browser.versions.mobile) {
        $('body').one('click', function () {
            newOrder.play();
            setTimeout(function () {
                newOrder.pause();
                newOrder.currentTime = 0;
            }, 20);
        });
    //}
    
}(jQuery));
