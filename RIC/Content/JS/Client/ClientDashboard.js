
$(document).ready(function () {
    changeButtonState()
    var months = ["January", "February", "March", "April", "May", "June",
           "July", "August", "September", "October", "November", "December"];
    var month = $("#HidMonth").val();
    var jobRepartPartialUrl = '../Client/ClientTablePartial';
    var options = { "backdrop": "static", keyboard: true };
    $.ajax({
        type: "GET",
        url: jobRepartPartialUrl,
        contentType: "application/json; charset=utf-8",
        data: { "month": month },
        datatype: "json",
        success: function (data) {
            $('#ClientTablePartial').html(data);
        },
        error: function () {
           // alert("Dynamic content load failed.");
        }
    });
    var date = new Date();
    var m = date.getMonth();
    $('#monthTextLable').text(months[m]);

    $("#previousMonthBtn").click(function () {

        //disable the buttons.
        $("#nextMontBtn").prop('disabled', true);
        $("#previousMontBtn").prop('disabled', true);

        var month = $("#HidMonth").val();
        var monthInt = parseInt(month) - 1;
        $("#HidMonth").val(monthInt);

        var options = { "backdrop": "static", keyboard: true };
        $.ajax({
            type: "GET",
            url: jobRepartPartialUrl,
            contentType: "application/json; charset=utf-8",
            data: { "month": monthInt },
            datatype: "json",
            success: function (data) {
                $('#ClientTablePartial').html(data);


            },
            error: function () {
              //  alert("Dynamic content load failed.");
            }
        });
        var date = new Date();
        date.setMonth(date.getMonth() + monthInt);
        var m = date.getMonth();
        $('#monthTextLable').text(months[m]);
        changeButtonState()
        return false;
    })//, 1000);      

    $("#nextMonthBtn").click(function () {
        //disable the buttons.
        $("#nextMontBtn").prop('disabled', true);
        $("#previousMontBtn").prop('disabled', true);
        var month = $("#HidMonth").val();
        var monthInt = parseInt(month) + 1;
        $("#HidMonth").val(monthInt);


        var options = { "backdrop": "static", keyboard: true };
        $.ajax({
            type: "GET",
            url: jobRepartPartialUrl,
            contentType: "application/json; charset=utf-8",
            data: { "month": monthInt },
            datatype: "json",
            success: function (data) {
                $('#ClientTablePartial').html(data);
            },
            error: function () {
               // alert("Dynamic content load failed.");
            }
        });
        var date = new Date();
        date.setMonth(date.getMonth() + monthInt);
        var m = date.getMonth();
        $('#monthTextLable').text(months[m]);
        changeButtonState()
        return false;
    })//, 1000);







});
function changeButtonState() {
    var month = $("#HidMonth").val();
    if (month == 0) {
        $("#nextMonthBtn").prop('disabled', true);
        $("#previousMonthBtn").prop('disabled', false);
    }
    else if (month == -2) {
        $("#nextMonthBtn").prop('disabled', false);
        $("#previousMonthBtn").prop('disabled', true);

    }
    else {
        $("#nextMonthBtn").prop('disabled', false);
        $("#previousMonthBtn").prop('disabled', false);
    }
}
