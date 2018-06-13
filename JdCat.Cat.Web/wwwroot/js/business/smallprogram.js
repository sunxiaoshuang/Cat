; (function ($) {

    new Vue({
        el: "#app",
        data: {
            entity: {

            }
        },
        methods: {

        },
        computed: {

        },
    });

    $('#switch').bootstrapSwitch({
        state: true,
        onSwitchChange: function (e, state) {

        }
    }).bootstrapSwitch("state", true);
})(jQuery);

