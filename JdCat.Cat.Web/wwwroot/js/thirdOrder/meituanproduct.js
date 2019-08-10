
(function () {
    var template = `
        <div class='row' id="selectProducts">
            <div class='col-xs-12'>
                <div class="input-group">
                    <input type="text" v-model="content" class="form-control" placeholder="输入内容...">
                    <span class="input-group-addon"><i class="fa fa-search"></i> 商品搜索</span>
                </div>
            </div>
            <div class='col-xs-12' style="height: 400px;">
                <ul class="list-group">
                    <li class="list-group-item" v-for="item in items">
                        <a @click="select(item)" class="text-primary">{{item.name + "[" + item.code + "]"}}</a>
                    </li>
                </ul>
            </div>
        </div>`;


    var app = new Vue({
        el: "#app",
        data: {
            offset: 0,              // 跳过的商品数量
            limit: 20,              // 每页商品数量
            products: [],           // 本地商品列表
            mts: [],                // 美团商品列表
            mappings: [],           // 已映射记录
            table: []               // 列表项
        },
        methods: {
            merge: function () {
                var self = this;
                var index = this.offset;
                this.table = this.mts.map(function (item) {
                    var mapping = self.mappings.first(function (a) { return a.thirdProductName === item.name; });
                    var product;
                    if (mapping) {
                        product = self.products.first(function (a) { return a.id === mapping.productId; });
                    } else {
                        product = self.products.first(function (a) { return a.name === item.name; });
                    }
                    return {
                        num: ++index,
                        name: item.name,
                        localName: !product ? "" : product.name,
                        code: !product ? "" : product.code,
                        productId: !product ? 0 : product.id,
                        pic: item.picture,
                        isSave: !!mapping
                    };
                });
            },
            catImg: function (item) {
                window.open(item.pic);
            },
            select: function (mapping) {
                var curVue;
                var $modal = $.view({
                    title: "选中本地商品",
                    template,
                    dialogHeight: 600,
                    load: function () {
                        curVue = new Vue({
                            el: "#selectProducts",
                            data: {
                                content: '',
                                items: []
                            },
                            watch: {
                                content: function (val) {
                                    var list = app.products.filter(a => a.name.indexOf(val) > -1 || a.code.indexOf(val) > -1 || a.pinyin.indexOf(val) > -1 || a.firstLetter.indexOf(val) > -1);
                                    if (list.length > 10) list.length = 10;
                                    this.items = JSON.parse(JSON.stringify(list));
                                }
                            },
                            methods: {
                                select: function (item) {
                                    mapping.localName = item.name;
                                    mapping.code = item.code;
                                    mapping.productId = item.id;
                                    mapping.isSave = false;
                                    $modal.modal("hide");
                                }
                            }
                        });

                        this.on("hidden.bs.modal", function () {
                            if (!curVue) return;
                            curVue.$destroy();
                            curVue = null;
                        });

                        setTimeout(() => { $modal.find("input").focus(); }, 500);
                    }
                });
            },
            save: function () {
                if (this.table.length === 0) {
                    $.alert("没有需要保存的映射商品", "warning");
                    return;
                }
                var items = this.table.filter(function (a) { return !!a.productId && !a.isSave; });
                if (items.length === 0) {
                    $.alert("没有需要保存的映射商品", "warning");
                    return;
                }
                $.loading();
                let self = this;
                var list = items.map(function(obj) { return { productId: obj.productId, thirdProductName: obj.name, thirdSource: 0 }; });
                axios.post(`/thirdOrder/saveMappings`, list)
                    .then(function (res) {
                        $.loaded();
                        $.alert("保存成功", "success");
                        self.mappings = res.data;
                        self.merge();
                    })
                    .catch(function (err) { $.loaded(); $.alert(err); });
            },
            next: function () {
                if (!this.canNext) return;
                this.offset += this.limit;
                this.loadData();
            },
            prev: function () {
                if (!this.canPrev) return;
                this.offset -= this.limit;
                this.loadData();
            },
            loadData: function () {
                var self = this;
                axios.get(`/thirdOrder/mt/getProducts?offset=${this.offset}&limit=${this.limit}`)
                    .then(function (res) {
                        self.mts = res.data.data;
                        self.merge();
                    })
                    .catch(function (err) { $.alert(err); });
            },
            clear: function () {
                $.confirm("提示", "确定清空已绑定的映射关系吗？", function () {
                    axios.get(`/thirdOrder/clear?source=0`)
                        .then(function () { location.reload(); });
                });
            }
        },
        computed: {
            canNext: function () {
                return this.table.length >= this.limit;
            },
            canPrev: function () {
                return this.offset > 0;
            }
        },
        created: function () {
            var self = this;
            axios.all([
                axios.get(`/thirdOrder/mt/getProducts?offset=${this.offset}&limit=${this.limit}`),
                axios.get("/thirdOrder/mappings?source=0"),
                axios.get("/thirdOrder/products")])
                .then(function (res) {
                    self.mts = res[0].data.data;
                    self.mappings = res[1].data;
                    self.products = res[2].data;
                    self.merge();
                })
                .catch(function (err) {
                    $.alert(err);
                });
        }
    });

})();