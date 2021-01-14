<template>
  <tab-focused-icon id="helpIcon" :icon-classes="[$style.help]" view-box="0 0 116 116" >
    <title>{{ $t('navigation.helpAndSupport') }}</title>
    <desc>{{ $t('navigation.accessHelpAndSupport') }}</desc>
    <g>
      <!--eslint-disable-next-line -->
      <path :class="$style.path" d="M35.74,42.23q1-8.7,6.38-13.43t14-4.74q8.82,0,14.5,4.86a15.45,15.45,0,0,1,5.67,12.24,16.47,16.47,0,0,1-1.65,7.15Q73,51.77,66.8,57.22c-3.15,2.8-5.12,5-5.94,6.62s-1.26,4.51-1.35,8.68H52.14a51.7,51.7,0,0,1,.37-7.27A14.47,14.47,0,0,1,54,60.78a18.18,18.18,0,0,1,3-4.12c1.22-1.28,2.92-2.88,5.12-4.79q4.2-3.73,5.25-5.85a9.64,9.64,0,0,0,1-4.31,10.62,10.62,0,0,0-3.55-7.9,12.08,12.08,0,0,0-8.66-3.41q-10.37,0-12.51,12.68Z"/>
      <rect :class="$style.rect" x="51.63" y="82.16" width="8.78" height="8.78"/>
      <circle :class="$style.circle" cx="56" cy="56" r="52.5"/>
    </g>
  </tab-focused-icon>
</template>

<script>

import TabFocusedIcon from '@/components/widgets/TabFocusedIcon';

export default {
  name: 'HelpIcon',
  components: {
    TabFocusedIcon,
  },
};

</script>

<style module lang="scss" scoped>
  @import "@/style/custom/help-icon";
</style>
