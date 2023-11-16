$(document).ready(function () {

    var TeamDetailPostBackURL = '../Dashboard/ViewDetailsPopup';
    // $(function () {
    $(".anchorDetail").click(function () {
        //debugger;
        // var empCD = $("#HdnEmpCD").val();
        var $buttonClicked = $(this);
        var fromDate = $buttonClicked.attr('data-FromDate');
        var toDate = $buttonClicked.attr('data-ToDate');
        var jr = $buttonClicked.attr('data-Jr');
        var empCD = $buttonClicked.attr('data-empCD');
        //var company = $buttonClicked.attr('data-Company')
        var options = { "backdrop": "static", keyboard: true };
        $.ajax({
            type: "GET",
            url: TeamDetailPostBackURL,
            contentType: "application/json; charset=utf-8",
            data: { "fromDate": fromDate, "toDate": toDate, "data_Jr": jr, "empCd": empCD },
            //data: { "fromDate": fromDate, "toDate": toDate, "data_Jr": jr, "company": company },
            datatype: "json",
            success: function (data) {

                $('#myModalContent').html(data);
                $('#myModal').modal(options);
                $('#myModal').modal('show');
                //$("#myModal").dialog();
            },
            error: function () {
                //alert("Dynamic content load failed.");
            }
        });
    });
});