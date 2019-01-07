$(document).ready(function() {

    $('.carousel').height($('header.home-header').innerHeight() - $('.nhso_logo').outerHeight());

    $('.carousel').slick({
        dots: true,
        infinite: false,
        speed: 400,
        arrows: false
    });

    $('.left').click(function() {
        $('.carousel').slick('slickPrev');
    });

    $('.right').click(function() {

        var slickObject = $('.carousel').slick('getSlick');

        if (slickObject.currentSlide === slickObject.slideCount - 1) {
            nativeApp.completeAppIntro();
        }

        $('.carousel').slick('slickNext');

    });

    var firstSlide = $('.carousel').slick('slickCurrentSlide');
    if (firstSlide == 0) {
        $('.left').css({
           'opacity': '0.3'
        }).children('button').prop('disabled', true);

        $('.right').click(function() {
            $('.carousel').slick('slickNext');
        });
    }

    $('.carousel').on('afterChange', function(event, slick, currentSlide) {

        if (currentSlide > 0) {
            $('.left').css({
                'opacity': '1'
            }).children('button').prop('disabled', false);

            if(currentSlide == 2) {
                $('.right button').text("Start")
            } else {
                $('.right button').text("Next")
            }
        } else {
            $('.left').css({
               'opacity': '0.3'
            }).children('button').prop('disabled', true);

            $('.right button').text("Next")
        }
    });

});