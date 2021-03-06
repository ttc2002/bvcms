﻿$(function () {
    $("#membergroups .ckbox").live("click", function (ev) {
        $.post($(this).attr("href"), {
            ck: $(this).is(":checked")
        });
        return true;
    });
    $("#dropmember").live("click", function (ev) {
        var f = $(this).closest('form');
        var href = this.href;
        bootbox.confirm("are you sure?", function (confirmed) {
            if (confirmed) {
                $.post(href, null, function(ret) {
                    f.modal("hide");
                    self.parent.RebindMemberGrids();
                });
            }
        });
        return false;
    });
    $('#OrgSearch').live("keydown", function (event) {
        if (event.keyCode == 13) {
            event.preventDefault();
            $("#orgsearchbtn").click();
        }
    });
    $("a.movemember").live('click', function (ev) {
        ev.preventDefault();
        var f = $(this).closest('form');
        var href = $(this).attr("href");
        bootbox.confirm("are you sure?", function (confirmed) {
            if (confirmed) {
                $.post(href, null, function(ret) {
                    f.modal("hide");
                    self.parent.RebindMemberGrids();
                });
            }
        });
        return false;
    });
});

