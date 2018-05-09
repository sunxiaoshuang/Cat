(function ($) {

    var $btnRegiste = $("#btnRegiste"), $user = $("#txtUser"), $pwd = $("#txtPwd"),
        $invitationCode = $("#txtInvitationCode"), $name = $("#txtName"), $email = $("#txtEmail"), $address = $("#txtAddress"),
        $phone = $("#txtPhone"), $mark = $("#txtMark"), $license = $("#txtLicense"),
        $alert = $("#alert"), $alertMsg = $("#alertMsg"), $info = $("#info"), $infoMsg = $("#infoMsg"), url = "/Registe/Registe";

    $btnRegiste.click(function () {
        $btnRegiste.attr("disabled", "disabled");
        $alert.addClass("hide");
        $info.addClass("hide");
        var username = $user.val();
        var pwd = $pwd.val();
        var invitationCode = $invitationCode.val();
        var name = $name.val();
        var email = $email.val();
        var address = $address.val();
        var phone = $phone.val();
        var mark = $mark.val();
        var license = $license.val();
        if (!$.trim(username)) {
            f_alert("请输入用户名");
            $btnRegiste.removeAttr("disabled");
            return false;
        }
        if (!$.trim(name)) {
            f_alert("请输入商户名");
            $btnRegiste.removeAttr("disabled");
            return false;
        }
        if (!$.trim(pwd)) {
            f_alert("请输入密码");
            $btnRegiste.removeAttr("disabled");
            return false;
        }
        if (!$.trim(address)) {
            f_alert("请输入详细地址");
            $btnRegiste.removeAttr("disabled");
            return false;
        }
        if (!$.trim(license)) {
            f_alert("请输入营业执照");
            $btnRegiste.removeAttr("disabled");
            return false;
        }
        if (phone.match(/^1([358][0-9]|4[579]|66|7[0135678]|9[89])[0-9]{8}$/) == null) {
            f_alert("手机号码格式错误");
            $btnRegiste.removeAttr("disabled");
            return false;
        }
        if (email.match(/^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$/) == null) {
            f_alert("邮箱格式错误");
            $btnRegiste.removeAttr("disabled");
            return false;
        }
        $.loading();
        $.post(url, {
            name: name,
            pwd: pwd,
            code: username,
            invitationCode: invitationCode,
            email: email,
            address: address,
            phone: phone,
            mark: mark,
            license: license
        }, function (data) {
            $.loaded();
            if (!data.success) {
                f_alert(data.msg);
                $btnRegiste.removeAttr("disabled");
                return;
            } else {
                $pwd.val("");
                $invitationCode.val("");
                $name.val("");
                $email.val("");
                $address.val("");
                $phone.val("");
                $mark.val("");
                $user.val("");
                $license.val("");
                f_info(data.msg);
                return;
            }
            window.location.href = "/";
        });
        return false;
    });

    function f_alert(msg) {
        $alert.removeClass("hide");
        $alertMsg.text(msg);
    }

    function f_info(msg) {
        $info.removeClass("hide");
        $infoMsg.text(msg);
    }

})(jQuery);