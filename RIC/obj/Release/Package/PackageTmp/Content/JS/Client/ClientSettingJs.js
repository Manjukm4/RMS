$(function () {

    $('body').on('click', '.list-group .list-group-item', function () {
        $(this).toggleClass('active');
    });
    $('.list-arrows button').click(function () {
        var $button = $(this), actives = '';
       // debugger;
        if ($button.hasClass('move-left')) {
            actives = $('.list-right ul li.active');
            actives.clone().appendTo('.list-left ul');
            actives.remove();
           
            saveClientData()

        } else if ($button.hasClass('move-right')) {
            actives = $('.list-left ul li.active');
            actives.clone().appendTo('.list-right ul');
            actives.remove();          
            //var quotedCSV = '"' + optionTexts.join('", "') + '"';
            saveClientData()
        }
    });
    $('.dual-list .selector').click(function () {
        var $checkBox = $(this);
        if (!$checkBox.hasClass('selected')) {
            $checkBox.addClass('selected').closest('.well').find('ul li:not(.active)').addClass('active');
            $checkBox.children('i').removeClass('glyphicon-unchecked').addClass('glyphicon-check');
        } else {
            $checkBox.removeClass('selected').closest('.well').find('ul li.active').removeClass('active');
            $checkBox.children('i').removeClass('glyphicon-check').addClass('glyphicon-unchecked');
        }
    });
    $('[name="SearchDualList"]').keyup(function (e) {
        var code = e.keyCode || e.which;
        if (code == '9') return;
        if (code == '27') $(this).val(null);
        var $rows = $(this).closest('.dual-list').find('.list-group li');
        var val = $.trim($(this).val()).replace(/ +/g, ' ').toLowerCase();
        $rows.show().filter(function () {
            var text = $(this).text().replace(/\s+/g, ' ').toLowerCase();
            return !~text.indexOf(val);
        }).hide();
    });

});

function saveClientData()
{
    var optionTexts = [];
    $(".list-right ul li").each(function () { optionTexts.push($(this).text()) });
    var clientsJson = JSON.stringify(optionTexts);

    $.ajax({
        type: "POST",
        url: '../Client/SaveClient',
        datatype: "json",
        data: { "clients": clientsJson },
        success: function (data) {

            //$('#myModalContent').html(data);
            //$('#myModal').modal(options);
            //$('#myModal').modal('show');
            //$("#myModal").dialog();
        },
        error: function () {
            // alert("Dynamic content load failed.");
        }
    });

}