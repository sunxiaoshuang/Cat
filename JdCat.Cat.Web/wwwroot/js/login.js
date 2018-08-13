(function ($) {

    var $btnLogin = $("#btnLogin"), $user = $("#txtUser"), $pwd = $("#txtPwd"),
        $alert = $("#alert"), $msg = $("#msg"), url = "/Login/Login", isCode = pageObj.isVerifyCode;

    $btnLogin.click(function () {
        $btnLogin.attr("disabled", "disabled");
        $alert.addClass("hide");
        var username = $.trim($user.val());
        var pwd = $.trim($pwd.val());
        var code = $.trim($("#txtCode").val());
        if (isCode && !code) {
            f_alert("请输入验证码");
            $btnLogin.removeAttr("disabled");
            return false;
        }
        if (!username) {
            f_alert("请输入用户名");
            $btnLogin.removeAttr("disabled");
            return false;
        }
        if (!pwd) {
            f_alert("请输入密码");
            $btnLogin.removeAttr("disabled");
            return false;
        }

        $.loading();
        $.post(url, { username: username, pwd: pwd, code: code }, function (data) {
            $.loaded();
            if (!data.success) {
                f_alert(data.msg);
                $btnLogin.removeAttr("disabled");
                isCode = true;
                $("#div_code").removeClass("hide");
                $("#code").attr("src", "/Login/Code?" + (new Date()).valueOf());
                return;
            }
            if (pageObj.type == "print") {
                window.location.href = "/Print";
            } else {
                window.location.href = "/";
            }
        });
        return false;
    });

    function f_alert(msg) {
        $alert.removeClass("hide");
        $msg.text(msg);
    }

    $("#code").click(function () {
        this.src = "/Login/Code?" + (new Date()).valueOf();
    });

    if (isCode) {
        $("#div_code").removeClass("hide");
        $("#code").attr("src", "/Login/Code?" + (new Date()).valueOf());
    }

})(jQuery);