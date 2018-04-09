<template>
  <svg xmlns="http://www.w3.org/2000/svg" width="25" height="30" viewBox="0 0 94.96 112.5">
    <defs>
    </defs>
    <title>{{ $t('icons.symptomsIcon.title') }}</title>
    <circle class="menu_stroke active" cx="45.84" cy="7.86" r="5.86" />
    <circle class="menu_stroke active" cx="76.82" cy="7.86" r="5.86" />
    <circle class="menu_stroke active" cx="14.73" cy="54.4" r="12.73" />
    <path class="menu_stroke" d="M43.15,49.56C39,43.28,31.68,31.67,
          29.93,25.49c-2.47-8.74,1.74-17.31,9.92-17.31" />
    <path class="menu_stroke" d="M82.61,8.18c8.18,0,11.66,6.8,9.91,
          17.31-1.29,7.72-9,18.63-13.23,24.3" />
    <path class="menu_stroke" d="M13.93,67.56v18c0,13.72,10.41,25,23.12,
          25h1.1c12.72,0,23.12-11.23,23.12-25v-17" />
    <path class="symptoms_middle" d="M76.46,51a2.51,2.51,0,0,1,2,4l-1.26,1.61C73.12,
          61.86,66.93,66,61.27,66H60c-4.76,0-10.28-3.25-13.52-7.75-.69-.95-1.48-2-2.38-3.19A2.5,
          2.5,0,1,1,48.06,52c1,1.23,1.78,2.36,2.5,3.36A13.21,13.21,0,0,0,60,61h1.24c3.27-.29,
          8-2.32,11.94-7.4l1.28-1.64a2.53,2.53,0,0,1,2-.95m0-4h0a6.47,6.47,0,0,0-5.11,
          2.47l-1.14,1.47-.16.2c-3,3.92-6.59,5.58-9,5.85h-.91a9.21,9.21,0,0,
          1-6.36-4c-.7-1-1.56-2.17-2.61-3.5A6.5,6.5,0,0,0,41,57.52c.85,1.08,1.61,
          2.13,2.27,3C47.32,66.2,54.06,70,60,70h1.24c5.13,0,12.83-2.86,
          19-10.88l1.24-1.58A6.51,6.51,0,0,0,76.46,47Z" />
  </svg>
</template>

<style lang="scss" scoped>
  @import "../../style/icons";
  @import "../../style/colours";
  .active {
    svg {
      .active {
        fill: $nhs_blue;
      }
      .symptoms_middle {
        fill: $nhs_blue;
      }
    }
  }

</style>
