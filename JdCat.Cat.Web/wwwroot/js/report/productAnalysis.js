;
(function () {

    Vue.prototype.currency = function (input) {
        return "￥ " + input;
    };

    new Vue({
        el: "#app",
        data: {
            products: pageData.products,
            productPrices: pageData.productPrices
        }
    });

})();


