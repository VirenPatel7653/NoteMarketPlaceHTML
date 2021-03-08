/* ============================================
            Login Page (Show/hide Password)
    =========================================== */

function show_hide_password(input) {
    if (input.attr("type") == "password") {
        input.attr("type", "text");
    } else {
        input.attr("type", "password");
    }
}
$(".toggle-password").click(function () {
    var input = $($(this).attr("toggle"));
    show_hide_password(input);
});

/* ============================================
            Change Password Page (Show/hide Password)
    =========================================== */
$(".old-toggle-password").click(function () {
    var input = $($(this).attr("toggle"));
    show_hide_password(input);

});
$(".new-toggle-password").click(function () {
    var input = $($(this).attr("toggle"));
    show_hide_password(input);
});
$(".confirm-toggle-password").click(function () {
    var input = $($(this).attr("toggle"));
    show_hide_password(input);
});

/* ============================================
           Ratings
    =========================================== */
$.fn.stars = function () {
    return $(this).each(function () {
        const rating = $(this).data("rating");
        const numStars = $(this).data("numStars");
        const fullStar = '<i class="fa fa-star" style="color:#FECC31;"></i>'.repeat(Math.floor(rating));
        const halfStar = (rating % 1 !== 0) ? '<i class="fa fa-star-half-o" style="color:#FECC31;"></i>' : '';
        const noStar = '<i class="fa fa-star-o" style="color:#d1d1d1;"></i>'.repeat(Math.floor(numStars - rating));
        $(this).html(`${fullStar}${halfStar}${noStar}`);
    });
}

/* ============================================
           FAQ
    =========================================== */
$(document).ready(function () {

    $(".collapse.show").each(function () {
        $(this).prev(".card-header").addClass("card-header-white");
        $(this).prev(".card-header").find(".card-collapse-heading").addClass("card-header-white-text").removeClass("card-collapse-heading");
        $(this).prev(".card-header").find(".fa").addClass("fa-minus").removeClass("fa-plus");
    });

    $(".collapse").on('show.bs.collapse', function () {
        $(this).prev(".card-header").addClass("card-header-white");
        $(this).prev(".card-header").find(".card-collapse-heading").addClass("card-header-white-text").removeClass("card-collapse-heading");

        $(this).prev(".card-header").find(".fa").removeClass("fa-plus").addClass("fa-minus");
    }).on('hide.bs.collapse', function () {
        $(this).prev(".card-header").removeClass("card-header-white");
        $(this).prev(".card-header").find(".card-header-white-text").addClass("card-collapse-heading").removeClass("card-header-white-text");

        $(this).prev(".card-header").find(".fa").removeClass("fa-minus").addClass("fa-plus");
    });


});

/* ============================================
            Mobile Menu
    =========================================== */

$(function () {

    //Show mobile nav
    $("#openmenu,#mobile-nav a[data-toggle='dropdown']").click(function () {
        $("#mobile-nav").css("height", "100%");
    });
    //Hide mobile nav
    $("#mobile-nav-close-btn,#mobile-nav a b,#mobile-nav a.dropdown-item").click(function () {
        $("#mobile-nav").css("height", "0%");
    });


});
