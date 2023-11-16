$(document).ready(function () {
    $.ajaxSetup({ cache: false });// stop ajax cache for IE.
    setPage();

    function setPage() {
        hrefString = document.location.href ? document.location.href : document.location;

        if (document.getElementById("bootstrapMenu") != null)
            setActiveMenu(document.getElementById("bootstrapMenu").getElementsByTagName("a"),
                             extractPageName(hrefString));
    }

    function extractPageName(hrefString) {
   
        var arr = hrefString.split('/');
        return arr[arr.length - 1].toLowerCase() + '/' + arr[arr.length - 2].toLowerCase();
    }

    function setActiveMenu(arr, crtPage) {
        for (var i = 0; i < arr.length; i++) {
            var link = $($(arr[i])[0]).attr('href');
          
            if ($($(arr[i])[0]).attr('href') !=null)
                {
                if (extractPageName($($(arr[i])[0]).attr('href')) == crtPage) {
                    if (arr[i].parentNode.tagName == "LI") {
                        if (arr[i].parentNode.parentNode.className == 'dropdown-menu') {
                            var temp = arr[i].parentNode.parentNode.parentElement.className = "activeNav"
                        } else {
                            arr[i].className = "activeNav";
                        }
                    }
                }
            }
        }
    }

    $("button[type='Reset']").click(function () {
        $("input[type='text']").val("");
        $("select").val($("select option:first").val());
        $("input[type='Email']").val("");
        return false;
    });


    $("a[title='Logout']").prepend('<span class="glyphicon glyphicon-log-out"></span>');

    $('body').css('height', window.innerHeight + "px");
});
