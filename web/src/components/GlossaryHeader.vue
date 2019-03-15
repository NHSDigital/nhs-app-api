<template>
  <div :class="[!$store.state.device.isNativeApp && $style.desktopWeb,
                $style.info, ...extraClasses]">
    <p>{{ $t('rp01.glossary.headerText') }}</p>
    <analytics-tracked-tag :href="glossaryLinkURL"
                           :text="$t('rp01.glossary.linkText')"
                           tag="a"
                           target="_blank">
      <abbreviations-arrow-right-icon />
      {{ $t('rp01.glossary.linkText') }}
    </analytics-tracked-tag>
    <hr aria-hidden="true">
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import AbbreviationsArrowRightIcon from './icons/AbbreviationsArrowRightIcon';
import AnalyticsTrackedTag from './widgets/AnalyticsTrackedTag';

export default {
  components: {
    AbbreviationsArrowRightIcon,
    AnalyticsTrackedTag,
  },
  props: {
    extraClasses: {
      type: Array,
      default: () => [],
    },
  },
  data() {
    return {
      glossaryLinkURL: this.$store.app.$env.CLINICAL_ABBREVIATIONS_URL,
    };
  },
};
</script>

<style module scoped lang="scss">
@import '../style/colours';
@import '../style/textstyles';
@import '../style/fonts';
@import '../style/desktopWeb/accessibility';

.info {
  margin-bottom: 0.5em;
  &.desktopWeb {
  a {
   margin-bottom: 1em;
   &:focus {
    @include outlineStyle
   }
   &:hover{
    background: #ffcd60;
    outline: none;
    box-sizing: border-box;
    background-clip: content-box;
   }
  }
  p {
   font-family: $default_web;
   font-weight: normal;
   cursor: default;
  }
 }
 hr {
   opacity: unset;
  }
  p, a, analytics-tracked-tag {
    padding-bottom: 0.5em;
    padding-top: 0.5em;
  }
}
</style>
