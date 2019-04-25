<template>
  <tab-focused-icon id="backIcon" :icon-classes="[$style.back]" view-box="0 0 11 20"
                    height="20px"
                    version="1.1"
                    xmlns:xlink="http://www.w3.org/1999/xlink">
    <title>Back</title>
    <desc/>
    <defs/>
    <g id="Header---Back" stroke="none" stroke-width="1" fill="none" fill-rule="evenodd"
       transform="translate(-17.000000, -15.000000)">
      <g id="icon_back_arrow" transform="translate(17.000000, 15.000000)"
         fill="#FFFFFF" fill-rule="nonzero">
        <path id="Shape" d="M9.76205788,19.7931034 C9.90353698,19.9310345 10.0803859,
        20 10.2749196,20 C10.4694534,20 10.6463023,19.9310345 10.7877814,
        19.7931034 C11.0707395,19.5172414 11.0707395,19.0689655 10.7877814,
        18.7931034 L1.76848875,10 L10.7877814,1.20689655 C11.0707395,
        0.931034483 11.0707395,0.482758621 10.7877814,0.206896552 C10.5048232,
        -0.0689655172 10.0450161,-0.0689655172 9.76205788,0.206896552 L0.21221865,
        9.5 C-0.0707395498,9.77586207 -0.0707395498,10.2241379 0.21221865,
        10.5 L9.76205788,19.7931034 Z" />
      </g>
    </g>
  </tab-focused-icon>
</template>

<script>

import TabFocusedIcon from '@/components/widgets/TabFocusedIcon';

export default {
  name: 'BackIcon',
  components: {
    TabFocusedIcon,
  },
};

</script>

<style module lang="scss" scoped>
@import "../../style/colours";
svg.back {
  margin: 1em;
  margin-top: 0.875em;
  height: 1.375em;
  outline-style:none;
}

</style>
