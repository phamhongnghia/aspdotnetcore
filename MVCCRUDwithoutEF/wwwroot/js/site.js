﻿
$(document).ready(function () {
    var owl = $('.owl-carousel');
    owl.owlCarousel({
        navigation: true,
        loop: true,
        margin: 10,
        autoplay: true,
        autoplayTimeout: 1000,
        autoplayHoverPause: true,
        nav: true,
        responsive:
        {
            0:
            {
                items: 1
            },
            600:
            {
                items: 2
            },
            1000:
            {
                items: 3
            }
        }
    });
})