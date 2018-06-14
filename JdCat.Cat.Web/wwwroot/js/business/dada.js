; (function ($) {

    new Vue({
        el: "#app",
        data: {
            entity: pageObj.business,
            cityList: pageObj.cityList,
            showError: false
        },
        methods: {
            ret: function () {
                history.go(-1);
            },
            save: function () {
                var entity = this.entity;
                if (!entity.dadaAppKey || !entity.dadaAppSecret || !entity.cityCode) {
                    $.alert("请将信息填写完整");
                    this.showError = true;
                    return;
                }
                this.showError = false;
                $.loading();
                axios.post("/business/savedada", { dadaAppKey: entity.dadaAppKey, dadaAppSecret: entity.dadaAppSecret, cityCode: entity.cityCode, cityName: entity.cityName })
                    .then(function (res) {
                        $.loaded();
                        if (!res.data.success) {
                            $.alert(response.data.msg);
                            return;
                        }
                        $.alert("保存成功", "success");
                    })
                    .catch(function (err) {
                        $.loaded();
                        $.alert(err);
                    });
            },
            changeOption: function (e) {
                if (!e.target.selectedOptions[0]) return;
                this.entity.cityName = e.target.selectedOptions[0].text;
            }
        }
    });

})(jQuery);

