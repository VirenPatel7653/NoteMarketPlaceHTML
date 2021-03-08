/* ============================================
            Navigation
    =========================================== */
/* Show & Hide White Navigation */
$(function () {

    // Show/hide nav on page load
    showHideNav();

    $(window).scroll(function () {

        // show/hide nav on window's scroll
        showHideNav();
    });

    function showHideNav() {
        if ($(window).scrollTop() > 50) {

            // Show white nav

            $("nav ul li a b").removeClass("home-nav");
            $("nav ul li a button").removeClass("btn-home");
            $("nav").addClass("white-nav-top");
            $("nav ul li a b").addClass("white-nav");
            $("nav ul li a button").addClass("btn-general");
            $("#openmenu").css("color", "#6255a5");

            // Show dark logo
            $(".navbar-brand img").attr("src", "Content/images/home/logo.png");


        } else {

            // Hide white nav
            $("nav").removeClass("white-nav-top");
            $("nav").removeClass("white-nav");
            $("nav ul li a button").removeClass("btn-general");
            $("nav ul li a button").addClass("btn-home");

            $("nav ul li a b").addClass("home-nav");
            $("#openmenu").css("color", "#fff");
            // Show white logo
            $(".navbar-brand img").attr("src", "Content/images/pre-login/top-logo.png");

        }
    }
});
