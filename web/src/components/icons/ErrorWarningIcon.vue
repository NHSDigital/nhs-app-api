<template>
  <div :class="[iconClass, iconType]">
    <svg width="7px" height="26px" viewBox="0 0 7 26" version="1.1" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink">
      <title>Error</title>
      <desc/>
      <defs/>
      <!--eslint-disable-next-line -->
      <g id="Style-guide" stroke="none" stroke-width="1" fill="none" fill-rule="evenodd" transform="translate(-1166.000000, -3740.000000)">
        <path id="icon_warning" d="M1171.534,3758.62 L1167.43,3758.62 L1166.746,3740.872 L1172.29,
        3740.872 L1171.534,3758.62 Z M1172.038,3766 L1166.998,3766 L1166.998,
        3760.96 L1172.038,3760.96 L1172.038,3766 Z" fill="#FFFFFF"/>
      </g>
    </svg>
  </div>
</template>

<script>
export default {
  props: {
    iconType: {
      default: 'error',
      type: String,
    },
  },
  data() {
    return {
      iconClass: 'icon',
    };
  },
};
</script>


<style lang="scss" scoped="">
@import "../../style/icons";
</style>
