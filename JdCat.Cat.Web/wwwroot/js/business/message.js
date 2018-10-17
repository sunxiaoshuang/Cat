(function () {

    $("#list").on("click", ".delete", function () {
        var self = this;
        var id = $(self).data("id");
        var name = $(self).data("name");
        $.primary(`是否确定删除用户【${name}】？`, function () {
            $.get("DeleteWxUser/" + id, function (res) {
                $(self).closest("li").remove();
            });
            return true;
        });
    });
    $("#btnReload").click(function () {
        window.location.reload();
    });

})();

