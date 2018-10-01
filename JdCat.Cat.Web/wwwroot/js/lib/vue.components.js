(function () {
    if (!Vue) return;
    Vue.component("cat-pager", {
        props: ["pageCount", "pageIndex", "pagePosition"],
        template: `
            <div :class="classObj" v-if="pageCount > 1">
                <ul class="pagination cat-pagination pagination-sm">
                    <li v-bind:class="{'disabled': pageIndex === 1}" v-on:click="prev()">
                        <a>
                            <i class="fa fa-chevron-left"></i>
                        </a>
                    </li>
                    <li v-for="item in items" v-bind:class="{'active': item.active}" v-on:click="page(item)">
                        <a>
                            <span v-text="item.index"></span>
                        </a>
                    </li>
                    <li v-bind:class="{'disabled': pageIndex === pageCount}" v-on:click="next()">
                        <a>
                            <i class="fa fa-chevron-right"></i>
                        </a>
                    </li>
                </ul>
            </div>
        `,
        data: function () {
            var classObj = {};
            if (!!this.pagePosition) {
                classObj["text-" + this.pagePosition] = true;
            } else {
                classObj["text-center"] = true;
            }
            return {
                classObj,
                items: []
            };
        },
        methods: {
            prev: function () {
                if (this.pageIndex == 1) return;
                this.$emit("prev");
            },
            next: function () {
                if (this.pageIndex == this.pageCount) return;
                this.$emit("next");
            },
            page: function (item) {
                if (!item.canClick || this.pageIndex == item.index) return;
                this.$emit("page", item.index);
            },
            calc: function () {
                var index = 1, items = [];
                if (this.pageCount <= 10) {
                    for (; index <= this.pageCount; index++) {
                        items.push({ index, active: this.pageIndex == index, canClick: true });
                    }
                } else {
                    if (this.pageIndex < 4) {
                        for (; index < 5; index++) {
                            items.push({ index, active: this.pageIndex == index, canClick: true });
                        }
                        items.push({ index: "...", active: false, canClick: false });
                        items.push({ index: this.pageCount, active: false, canClick: true });
                    } else if (this.pageIndex > this.pageCount - 3) {
                        index = this.pageCount - 3;
                        items.push({ index: 1, active: false, canClick: true });
                        items.push({ index: "...", active: false, canClick: false });
                        for (; index < this.pageCount + 1; index++) {
                            items.push({ index, active: this.pageIndex == index, canClick: true });
                        }
                    } else {
                        var num = 0;
                        items.push({ index: 1, active: false, canClick: true });
                        items.push({ index: "...", active: false, canClick: false });
                        for (; num < 3; num++) {
                            items.push({ index: this.pageIndex + num - 1, active: num === 1, canClick: true });
                        }
                        items.push({ index: "...", active: false, canClick: false });
                        items.push({ index: this.pageCount, active: false, canClick: true });
                    }
                }
                this.items = items;
            }
        },
        created: function () {
            this.calc();
        },
        watch: {
            "pageIndex": function () {
                this.calc();
            },
            "pageCount": function () {
                this.calc();
            }
        }
    });
})();