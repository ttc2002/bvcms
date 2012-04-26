﻿$(function () {
    $("table.grid > tbody > tr:even").addClass("alt");
    $("a.deltran").live("click", function (ev) {
        ev.preventDefault();
        if (confirm("are you sure"))
            $.post($(this).attr("href"), {}, function (ret) {
                $("#history").replaceWith(ret);
                $("#history > tbody > tr:even").addClass("alt");
            });
        return false;
    });
    $("span.date").editable('/TransactionHistory/Edit/', {
        tooltip: 'click to edit...',
        event: 'click',
        submit: 'OK',
        cancel: 'Cancel',
        width: '100px',
        height: 25
    });
});
