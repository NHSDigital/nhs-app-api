function removeAttrs() {
  if(typeof isIos !== 'undefined') {
    setTimeout(function() {
      $('.slick-dots').removeAttr("role");
      $("#slick-slide-control00").removeAttr("aria-label");
      $("#slick-slide-control01").removeAttr("aria-label");
      $("#slick-slide-control02").removeAttr("aria-label");
    }, 10);
  }
}

$(document).ready(function() {
  $(".carousel").height(
    $("header.home-header").innerHeight() - $(".nhso_logo").outerHeight()
  );

  $(".carousel").slick({
    dots: true,
    infinite: false,
    speed: 400,
    arrows: false,
    accessibility: true
  });

  $(".left").click(function() {
    $(".carousel").slick("slickPrev");
  });

  $(".right").click(function() {
    var slickObject = $(".carousel").slick("getSlick");

    if (slickObject.currentSlide === slickObject.slideCount - 1) {
      nativeApp.completeAppIntro();
    }

    $(".carousel").slick("slickNext");
  });

  var firstSlide = $(".carousel").slick("slickCurrentSlide");
  if (firstSlide == 0) {
    $(".left")
      .css({
        opacity: "0.3"
      })
      .children("button")
      .prop("disabled", true);

    $(".right").click(function() {
      $(".carousel").slick("slickNext");
    });
  }

  $("#slide-one-content").hide();
  $("#slide-two-content").hide();
  $("#slide-three-content").hide();

  $(".carousel").on("beforeChange", function(
    event,
    slick,
    currentSlide,
    nextSlide
  ) {
    if (nextSlide === 0) {
      $("#slide-two-content").hide();
      $("#slide-three-content").hide();
      $("#slide-one-content").show();
    } else if (nextSlide === 1) {
      $("#slide-one-content").hide();
      $("#slide-three-content").hide();
      $("#slide-two-content").show();
    } else if (nextSlide === 2) {
      $("#slide-one-content").hide();
      $("#slide-two-content").hide();
      $("#slide-three-content").show();
    }
  });

  $(".carousel").on("afterChange", function(event, slick, currentSlide) {
    switch (currentSlide) {
      case 0:
        $('#nhs_logo_one').focus();
        if(typeof isIos !== 'undefined') {
          nativeApp.focusElement("#nhs_logo_one");
        }
        break;
      case 1:
        $('#nhs_logo_two').focus();
        if(typeof isIos !== 'undefined') {
          nativeApp.focusElement("#nhs_logo_two");
        }
        break;
      case 2:
        $('#nhs_logo_three').focus();
        if(typeof isIos !== 'undefined') {
          nativeApp.focusElement("#nhs_logo_three");
        }
        break;
    }

    if (currentSlide > 0) {
      $(".left")
        .css({
          opacity: "1"
        })
        .children("button")
        .prop("disabled", false);

      if (currentSlide == 2) {
        $(".right button").text("Start");
      } else {
        $(".right button").text("Next");
      }
    } else {
      $(".left").css({
        opacity: "0.3"
      });
      $(".right button").text("Next");
    }

    removeAttrs();
  });

  $("#slide-one-content").show();
});
