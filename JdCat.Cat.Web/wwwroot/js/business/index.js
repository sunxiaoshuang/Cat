; (function ($) {

    var data = new Vue({
        el: "#app",
        data: {
            entity: pageObj.business,
            cityList: pageObj.cityList,
            showError: false,
            isChangeLogo: false,
            startHour: !pageObj.business.businessStartTime ? "06" : pageObj.business.businessStartTime.split(":")[0],
            startMinus: !pageObj.business.businessStartTime ? "00" : pageObj.business.businessStartTime.split(":")[1],
            endHour: !pageObj.business.businessEndTime ? "21" : pageObj.business.businessEndTime.split(":")[0],
            endMinus: !pageObj.business.businessEndTime ? "00" : pageObj.business.businessEndTime.split(":")[1],
        },
        methods: {
            ret: function () {
                history.go(-1);
            },
            save: function () {
                var entity = this.entity;
                if (!entity.logoSrc) {
                    $.alert("请上传商户LOGO");
                    return;
                }
                if (!entity.name || !entity.address || !entity.contact || !entity.mobile || !entity.freight || !entity.businessLicense || !entity.businessLicenseImage || !entity.lng || !entity.lat) {
                    $.alert("请将信息填写完整");
                    this.showError = true;
                    return;
                }
                entity.businessStartTime = this.startHour + ":" + this.startMinus;
                entity.businessEndTime = this.endHour + ":" + this.endMinus;

                this.showError = false;
                $.loading();
                axios.post("/business/savebase?isChangeLogo=" + (this.isChangeLogo ? 1 : 0), entity)
                    .then(function (res) {
                        $.loaded();
                        if (!res.data.success) {
                            $.alert(res.data.msg);
                            return;
                        }
                        $.alert("保存成功", "success");
                    })
                    .catch(function (err) {
                        $.loaded();
                        $.alert(err);
                    });
            },
            selectFile: function () {
                $("#file").click();
            },
            remove: function () {
                this.entity.logoSrc = null;
            },
            uploadBusinessLicense: function () {
                $("#fileLicense").click();
            },
            uploadSpecialImage: function () {
                $("#fileSpecialImage").click();
            },
            changeBusinessLicense: function (e) {
                var file = e.target.files[0], self = this;
                if (!file) {
                    this.entity.businessLicenseImage = null;
                    return;
                }
                var read = new FileReader();
                read.onload = function (e) {
                    self.entity.businessLicenseImage = e.target.result;
                };
                read.readAsDataURL(file);
            },
            changeSpecialImage: function (e) {
                var file = e.target.files[0], self = this;
                if (!file) {
                    this.entity.specialImage = null;
                    return;
                }
                var read = new FileReader();
                read.onload = function (e) {
                    self.entity.specialImage = e.target.result;
                };
                read.readAsDataURL(file);
            }
        },
        computed: {
            logo: function () {
                var input = this.entity.logoSrc;
                if (!input) return input;
                if (input.indexOf("data:image") > -1) return input;
                return `${appConfig.apiUrl}/File/Logo/${pageObj.business.id}/${input}`;
            },
            license: function () {
                var input = this.entity.businessLicenseImage;
                if (!input) return input;
                if (input.indexOf("data:image") > -1) return input;
                return `${appConfig.apiUrl}/File/License/${pageObj.business.id}/${input}`;
            },
            specialImage: function () {
                var input = this.entity.specialImage;
                if (!input) return input;
                if (input.indexOf("data:image") > -1) return input;
                return `${appConfig.apiUrl}/File/License/${pageObj.business.id}/${input}`;
            }

        },
        filters: {

        }
    });


    $('#switch').bootstrapSwitch({
        onSwitchChange: function (e, state) {
            if (data.entity.isAutoReceipt === state) return;
            data.entity.isAutoReceipt = state;
            $.loading();
            axios.get(`/business/changeAutoReceipt?isAutoReceipt=${state}`)
                .then(function (res) {
                $.loaded();
                if (!res.data.success) {
                    $.alert(res.data.msg);
                    return;
                }
                $.alert(state ? "开启自动接单成功" : "关闭自动接单成功", "success");
            });
        }
    }).bootstrapSwitch("state", data.entity.isAutoReceipt);

    $('#cbClose').bootstrapSwitch({
        onSwitchChange: function (e, state) {
            if (data.entity.isClose === state) return;
            data.entity.isClose = state;
            $.loading();
            axios.get(`/business/changeClose?isClose=${state}`)
                .then(function (res) {
                    $.loaded();
                    if (!res.data.success) {
                        $.alert(res.data.msg);
                        return;
                    }
                    $.alert(state ? "暂停营业中" : "开启营业模式成功", "success");
                });
        }
    }).bootstrapSwitch("state", data.entity.isClose);


    // 裁剪图片
    var clipArea = new bjj.PhotoClip("#clipArea", {
        size: [400, 300], // 截取框的宽和高组成的数组。默认值为[260,260]
        outputSize: [400, 300], // 输出图像的宽和高组成的数组。默认值为[0,0]，表示输出图像原始大小
        //outputType: "jpg", // 指定输出图片的类型，可选 "jpg" 和 "png" 两种种类型，默认为 "jpg"
        file: "#file", // 上传图片的<input type="file">控件的选择器或者DOM对象
        // view: "#view", // 显示截取后图像的容器的选择器或者DOM对象
        ok: "#clipBtn", // 确认截图按钮的选择器或者DOM对象
        loadStart: function () {
            // 开始加载的回调函数。this指向 fileReader 对象，并将正在加载的 file 对象作为参数传入
            $('.cover-wrap').fadeIn();
        },
        loadComplete: function () {
            // 加载完成的回调函数。this指向图片对象，并将图片地址作为参数传入
            //console.log(this);
        },
        //loadError: function(event) {}, // 加载失败的回调函数。this指向 fileReader 对象，并将错误事件的 event 对象作为参数传入
        clipFinish: function (dataURL) {
            // 裁剪完成的回调函数。this指向图片对象，会将裁剪出的图像数据DataURL作为参数传入
            $('.cover-wrap').fadeOut();
            data.entity.logoSrc = dataURL;
            data.isChangeLogo = true;
        }
    });
    $("#btn-closeArea").click(function () {
        $('.cover-wrap').fadeOut();
    });
})(jQuery);

