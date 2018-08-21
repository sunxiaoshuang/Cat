(function () {
    var genderList = {
        '0': "未知", '1': "男", '2': "女"
    };
    new Vue({
        el: "#app",
        data: {
            pageIndex: 1,
            list: [],
            pageCount: undefined,
            pageSize: 20
        },
        methods: {
            phoneFilter: function (item) {
                return !item.phone ? "未绑定" : item.phone;
            },
            genderFilter: function (item) {
                return genderList[item.gender];
            },
            loadData: function () {
                var self = this;
                axios.get(`/User/GetList?pageIndex=${this.pageIndex}&pageSize=${this.pageSize}`)
                    .then(function (res) {
                        self.list = res.data.list;
                        self.pageCount = Math.ceil(res.data.rows / self.pageSize);
                    });
            },
            prev: function () {
                if (this.pageIndex == 1) return;
                this.pageIndex--;
                this.loadData();
            },
            next: function () {
                if (this.pageIndex == this.pageCount) return;
                this.pageIndex++;
                this.loadData();
            },
            page: function (num) {
                this.pageIndex = num;
                this.loadData();
            }
        },
        created: function () {
            this.loadData();
        }
    });
    
})();