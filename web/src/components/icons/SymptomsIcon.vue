<template>
  <svg :class="[$style.menu, selected?$style.selected:undefined]" xmlns="http://www.w3.org/2000/svg"
       viewBox="0 0 110.39 110.39">
    <def/>
    <title>Symptoms checker</title>
    <g>
      <rect :class="$style.canvas" width="110.39" height="110.39" />
      <g>
        <circle :class="[$style['menu_stroke'], $style.active]" cx="53.99" cy="19.59" r="4.32"/>
        <circle :class="[$style['menu_stroke'], $style.active]" cx="76.79" cy="19.59" r="4.32"/>
        <circle :class="[$style['menu_stroke'], $style.active]" cx="31.09" cy="53.84" r="9.37"/>
        <path :class="$style['menu_stroke']" d="M52,50.27c-3-4.62-8.44-13.16-9.73-17.71-1.82-6.43,
              1.28-12.74,7.3-12.74" />
        <path :class="$style['menu_stroke']" d="M81.05,19.82c6,0,8.58,5,7.29,12.74-.94,
              5.68-6.64,13.71-9.73,17.88" />
        <path :class="$style['menu_stroke']" d="M30.51,63.52V76.76c0,10.1,7.65,18.36,17,
              18.36h.81c9.36,0,17-8.26,17-18.36V64.26" />
        <path :class="$style['symptoms_middle']" d="M76.52,51.33a1.87,1.87,0,0,1,1.14.39,
              1.84,1.84,0,0,1,.3,2.59L77,55.49c-3,3.83-7.53,6.86-11.69,6.86h-.92c-3.5,
              0-7.56-2.39-10-5.69C54,56,53.4,55.15,52.73,54.31a1.85,1.85,0,0,1,.3-2.59,
              1.87,1.87,0,0,1,1.14-.39,1.83,1.83,0,0,1,1.45.7c.71.91,1.31,1.74,1.84,
              2.47a9.7,9.7,0,0,0,7,4.17h.92c2.4-.21,5.88-1.7,8.78-5.44l1-1.2a1.83,1.83,0,
              0,1,1.44-.7m0-4h0a5.8,5.8,0,0,0-4.58,2.22c-.3.37-.59.74-.86,1.09l-.11.14a9.6,
              9.6,0,0,1-5.83,3.89h-.56a5.71,5.71,0,0,1-3.87-2.51c-.52-.71-1.16-1.61-2-2.61a5.84,
              5.84,0,1,0-9.17,7.23c.61.79,1.16,1.54,1.64,2.21,3.18,4.41,8.49,7.36,13.2,
              7.36h.92c6.14,0,11.7-4.34,14.85-8.41l.9-1.15a5.83,5.83,0,0,0-4.58-9.46Z" />
      </g>
    </g>
  </svg>
</template>

<script>
export default {
  props: {
    selected: {
      type: Boolean,
      default: false,
    },
  },
};
</script>

<style lang="scss" module scoped>
@import "../../style/colours";
@import "../../style/navmenuicons";
svg.menu {
  .symptoms_middle {
    fill: $dark_grey;
  }
  &.selected {
    .active {
      fill: $nhs_blue;
    }
    .symptoms_middle {
      fill: $nhs_blue;
    }
  }
}

</style>
