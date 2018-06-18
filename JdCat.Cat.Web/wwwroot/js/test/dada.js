
(function () {

    new Vue({
        el: "#app",
        data: {
            orderCode: null
        },
        methods: {
            receive: function () {
                this.fn("/Test/DadaReceive?orderCode=" + this.orderCode);
            },
            fetch: function () {
                this.fn("/Test/DadaFetch?orderCode=" + this.orderCode);
            },
            finish: function () {
                this.fn("/Test/DadaFinish?orderCode=" + this.orderCode);
            },
            cancel: function () {
                this.fn("/Test/DadaCancel?orderCode=" + this.orderCode);
            },
            expire: function () {
                this.fn("/Test/DadaExpire?orderCode=" + this.orderCode);
            },
            fn: function (url) {
                axios.get(url)
                    .then(function (res) {
                        $.alert("操作成功", "success");
                        console.log(res);
                    })
                    .catch(function (err) {
                        $.alert(err);
                    });
            }
        }
    });

})();