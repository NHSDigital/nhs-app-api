<template>
  <svg id="accountIcon" :class="$style.account" xmlns="http://www.w3.org/2000/svg"
       width="16.5" height="22" viewBox="0 0 83.63 111.84" >
    <defs/>
    <title>{{ $t('icons.accountIcon.title') }}</title>
    <path :class="$style['icon-account']"
          d="M31.07,54.29s1.09,11.65-19.51,16.13C4.89,72.51,2,79.5,2,84.75v9.16c0,
              5.26,4.05,8.33,9.56,10.35a84.84,84.84,0,0,0,60.52,0c5.3-2.52,9.55-5.09,
              9.55-10.35V84.75c0-5.25-2.69-12.44-9.55-14.33C53.26,65.64,52.17,54.29,
              52.17,54.29"/>
    <rect :class="$style['icon-account']" x="21.07" y="2"
          width="43.18" height="54.55" rx="21.59" ry="21.59"/>
  </svg>
</template>

<style module lang="scss" scoped>
@import "../../style/colours";
.account {
  margin: 1em;
  display: inline-block;
  height: 1.375em;
  position: fixed;
  right: 0em;
  top: 0em;
  cursor: pointer;

  .icon-account {
    fill: none;
    stroke: $white;
    stroke-linecap: round;
    stroke-linejoin: round;
    stroke-width: 0.4em;
  }
}

</style>
