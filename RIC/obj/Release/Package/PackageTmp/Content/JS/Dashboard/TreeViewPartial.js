$(document).ready(function () {
    $('#divnav').click(function (event) {
        event.stopPropagation();
    });
});
function openNavi() {
    document.getElementById("mySidenav").style.width = "250px";
}
function closeNav() {
    document.getElementById("mySidenav").style.width = "0";
}
$('html').click(function () {
    //Hide the menus if visible
    document.getElementById("mySidenav").style.width = "0";
});



//function myFunction() {
//    //   debugger;
//    var input, filter, ul, li, a, i;
//    input = document.getElementById("searchTextbox");
//    filter = input.value.toUpperCase();
//    ul = document.getElementById("MyNav");
//    li = ul.getElementsByTagName("li");

//    //disable all the ul inside nav.
//    $("#MyNav").find("ul").each(function () {
//        $(this).css("display", "none");
//    });
//    for (i = 0; i < li.length; i++) {
//        a = li[i].getElementsByTagName("a")[0];
//        if (a.innerHTML.toUpperCase().indexOf(filter) > -1) {

//            // a.closest('ul').style.display = "block";
//            $(a).closest('ul').css("display", "block")
//            $(a).closest('.has-children').find('[type=checkbox]').prop('checked', true)
//            //  a.style.display = "";   
//            $(a).css("display", "")

//        }
//        else {
//            // a.style.display = "none";
//            $(a).css("display", "none")
//        }
//    }
//    $('.has-children').each(function () {

//        // if child ul display is none then disable the li.
//        if ($(this).find('ul').css('display') == 'none') {
//            $(this).css("display", "none");
//        }
//        else {
//            $(this).css("display", "block")
//        }
//    })
//}