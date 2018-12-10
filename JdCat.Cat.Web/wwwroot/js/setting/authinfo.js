(function () {
    var infoVue, smallVue, wxauth;
    var qrTemplate = `
        <div class="qrcode">
            <img src="{0}" />
        </div>
    `;
    $('a[data-toggle="tab"]').on('show.bs.tab', function (e) {
        var activeTab = $(e.target).text();
        if (activeTab === "小程序管理") {
            if (!smallVue) {
                initSmall();
            }
        } else if (activeTab === "基本信息") {
            if (!infoVue) {
                initInfo();
            }
        }
    });

    initInfo();

    function initSmall() {

    }

    function initInfo() {
        axios.all([
            axios.get("/setting/getauthinfo"),
            axios.get("/setting/authinfopage"),
            axios.get("/data/wxauth.json")
        ])
            .then(function (res) {
                if (res[0].data.errcode > 0) {
                    window.location.href = "/Setting/AuthPage";
                    return;
                }
                $("#info").html(res[1].data);
                wxauth = res[2].data;
                infoVue = new Vue({
                    el: "#info",
                    data: {
                        info: res[0].data.authorizer_info,
                        auth: res[0].data.authorization_info,
                        wxauth
                    },
                    methods: {
                        getQrcode: function () {
                            $.view({
                                title: " ",
                                template: qrTemplate.format(this.info.qrcode_url)
                            });
                        }
                    },
                    filters: {
                        funcScope: function (type) {
                            return wxauth[type];
                        }
                    }
                });
            });
    }

})();