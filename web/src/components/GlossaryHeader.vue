<template>
  <div :class="$style.info">
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
  data() {
    return {
      glossaryLinkURL: this.$store.app.$env.CLINICAL_ABBREVIATIONS_URL,
    };
  },
};
</script>

<style module scoped lang="scss">
@import '../style/colours';

.info {
  margin-bottom: 0.5em;
  padding: 0.5em 1em 0em 1em;
  :focus {
    outline-color: $focus_highlight;
  }

  p, a, analytics-tracked-tag {
    padding-bottom: 0.5em;
    padding-top: 0.5em;
  }
}
</style>
