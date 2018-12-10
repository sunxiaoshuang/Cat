; (function () {

    new Vue({
        el: "#app",
        data: {

        },
        methods: {
            auth: function () {
                $.loading();
                axios.get("/Business/PreAuthCode")
                    .then(function (res) {
                        if (!res.data.success) {
                            $.loaded();
                            $.alert(res.data.msg);
                            return;
                        }
                        window.location.href = res.data.data;
                    })
                    .catch(function (err) {
                        $.alert(err);
                    });
            }
        }
    });

})();

