
(function () {


    new Vue({
        el: "#app",
        data: {
            price: pageData.list.length > 0 ? pageData.list[pageData.list.length - 1].price : 0,
            quantity: pageData.list.length > 0 ? pageData.list[pageData.list.length - 1].quantity : 0,
            products: pageData.products
        }
    });

    var time = pageData.list.map(a => a.createTime.substring(5));
    var priceList = pageData.list.map(a => a.price);
    var option = {
        color: ['#3398DB'],
        tooltip: {
            trigger: 'axis',
            axisPointer: {
                type: 'shadow'
            }
        },
        grid: {
            left: '3%',
            right: '4%',
            bottom: '3%',
            containLabel: true
        },
        xAxis: [
            {
                type: 'category',
                data: time,
                axisTick: {
                    alignWithLabel: true
                }
            }
        ],
        yAxis: [
            {
                type: 'value'
            }
        ],
        series: [
            {
                name: '营业额',
                type: 'bar',
                barWidth: '60%',
                data: priceList,
            }
        ]
    };

    var myChart = echarts.init(document.getElementById('chart'));
    myChart.setOption(option);


})();

