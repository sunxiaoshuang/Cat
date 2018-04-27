(function ($) {

    var $btnLogin = $("#btnLogin"), $user = $("#txtUser"), $pwd = $("#txtPwd"),
        $alert = $("#alert"), $msg = $("#msg"), url = "/Login/Login";

    $btnLogin.click(function () {
        $btnLogin.attr("disabled", "disabled");
        $alert.addClass("hide");
        var username = $user.val();
        var pwd = $pwd.val();
        if (!$.trim(username)) {
            f_alert("请输入用户名");
            $btnLogin.removeAttr("disabled");
            return false;
        }
        if (!$.trim(pwd)) {
            f_alert("请输入密码");
            $btnLogin.removeAttr("disabled");
            return false;
        }

        $.loading();
        $.post(url, { username: username, pwd: pwd }, function (data) {
            $.loaded();
            if (!data.success) {
                f_alert(data.msg);
                $btnLogin.removeAttr("disabled");
                return;
            }
            window.location.href = "/";
        });
        return false;
    });

    function f_alert(msg) {
        $alert.removeClass("hide");
        $msg.text(msg);
    }

})(jQuery);