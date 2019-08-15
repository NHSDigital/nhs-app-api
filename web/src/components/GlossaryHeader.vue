<template>
  <div :class="[!$store.state.device.isNativeApp && $style.desktopWeb,
                $style.info, ...extraClasses]">
    <p>{{ $t('rp01.glossary.headerText') }}</p>
    <analytics-tracked-tag :href="glossaryLinkURL"
                           :text="$t('rp01.glossary.linkText')"
                           tag="a"
                           target="_blank">
      <external-link-arrow-right-icon/>
      {{ $t('rp01.glossary.linkText') }}
    </analytics-tracked-tag>
    <hr aria-hidden="true">
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import AnalyticsTrackedTag from './widgets/AnalyticsTrackedTag';
import ExternalLinkArrowRightIcon from './icons/ExternalLinkArrowRightIcon';

export default {
  name: 'GlossaryHeader',
  components: {
    AnalyticsTrackedTag,
    ExternalLinkArrowRightIcon,
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
        display: inline-block;
        margin-bottom: 1em;
        width: 13em;
        &:focus {
          @include outlineStyle;
          background-color: $focus_highlight;
        }
        &:hover {
          @include linkHoverStyle;
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
