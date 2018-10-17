
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
    }
    pageObj.method.hashchange();

    // 店铺营业状态
    $('#cbClose').bootstrapSwitch({
        onSwitchChange: function (e, state) {
            state = !state;
            if (pageData.business.isClose === state) return;
            pageData.business.isClose = state;
            $.loading();
            axios.get(`/business/changeClose?isClose=${state}`)
                .then(function (res) {
                    $.loaded();
                    if (!res.data.success) {
                        $.alert(res.data.msg);
                        return;
                    }
                    $.alert(state ? "暂停营业" : "营业中", "success");
                });
        }
    }).bootstrapSwitch("state", !pageData.business.isClose);

    // 新订单提醒
    var ws, $wsModal;
    var newOrder = document.getElementById("newOrder");
    var autoOrder = document.getElementById("autoOrder");
    var exceptionOrder = document.getElementById("exceptionOrder");

    function connect() {

        ws = new WebSocket(pageData.orderUrl + "?id=" + pageData.business.id);
        //ws = new WebSocket("ws://203.195.205.143:5084/ws?id=" + pageData.business.id);

        ws.onmessage = function (res) {
            var time = new Date(), data = res.data.split("|"), code = data[0], state = data[1];

            if (state == 4 || state == 8 || state == 16) {
                autoOrder.play();
            } else if (state == 32) {
                exceptionOrder.play();
            } else {
                newOrder.play();
            }

            msg.list.push({ code: code, time: time.getHours() + ":" + time.getMinutes(), tip: state !== 32 });

            //axios.get("/order/MessageHandler?code=" + code)
            //    .then(function (res) {
            //        if (res.data.data === 1) {
            //            newOrder.play();
            //        } else if (res.data.data === 2) {
            //            autoOrder.play();
            //        } else if (res.data.data === 3) {
            //            exceptionOrder.play();
            //        }

            //        msg.list.push({ code: code, time: time.getHours() + ":" + time.getMinutes(), tip: res.data.success });
            //    })
            //    .catch(function (msg) {
            //        $.alert(msg);
            //    });

        };
        ws.onopen = function (a) {
            console.log("服务已连接", a);
            if ($wsModal) {
                $wsModal.modal("hide");
                $wsModal = null;
            }
        };
        ws.onclose = function (a) {
            if (a.reason) {
                $.primary(a.reason);
                return;
            }
            if (!$wsModal) {
                $wsModal = $.primary("新订单提醒异常，请检查网络连接");
            }
            setTimeout(function () {
                connect();
            }, 3000);
        };
    }
    connect();

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
                                <div :class="msg.tip ? '' : 'text-danger'">
                                    <i class="fa" :class="msg.tip ? 'fa-info-circle fa-fw text-primary' : 'fa-warning text-danger'"></i> {{msg.tip ? '您有一个新订单' : '异常订单'}}
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
        autoOrder.play();
        exceptionOrder.play();
        setTimeout(function () {
            newOrder.pause();
            newOrder.currentTime = 0;
            autoOrder.pause();
            autoOrder.currentTime = 0;
            exceptionOrder.pause();
            exceptionOrder.currentTime = 0;
        }, 20);
    });
    //}

}(jQuery));
