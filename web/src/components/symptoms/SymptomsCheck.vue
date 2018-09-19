<template>
  <div v-if="showTemplate" :class="$style['no-padding']" data-purpose="">
    <ul :class="$style['list-menu']" role="list">
      <li role="link">
        <analytics-tracked-tag
          id="btn_choices" :href="conditionsCheckerUrl"
          :class="$style['no-decoration']"
          :text="$t('sy01.a_z.subheader')"
          :aria-label="`${$t('sy01.a_z.subheaderAriaLabel')}. ${$t('sy01.a_z.body')}`"
          tag="a" target="_blank">
          <h2 :aria-label="$t('sy01.a_z.subheaderAriaLabel')">{{ $t('sy01.a_z.subheader') }}</h2>
          <p>{{ $t('sy01.a_z.body') }}</p>
        </analytics-tracked-tag>
      </li>
      <li role="link">
        <analytics-tracked-tag id="btn_111" :href="symptomsCheckerUrl"
                               :class="$style['no-decoration']"
                               :text="$t('sy01.111.subheader')"
                               :aria-label="`${$t('sy01.111.subheader')}. ${$t('sy01.111.body')}`"
                               tag="a" target="_blank">
          <h2>{{ $t('sy01.111.subheader') }}</h2>
          <p>{{ $t('sy01.111.body') }}</p>
        </analytics-tracked-tag>
      </li>
    </ul>
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';

export default {
  components: {
    AnalyticsTrackedTag,
  },
  data(app) {
    let symptomsCheckerUrl = app.$env.SYMPTOM_CHECKER_URL;
    if (this.$store.state.device.isNativeApp) {
      symptomsCheckerUrl += app.$env.SYMPTOM_CHECKER_NATIVE_QUERY_PARAMS;
    }
    return {
      symptomsCheckerUrl,
      conditionsCheckerUrl: app.$env.CONDITIONS_CHECKER_URL,
    };
  },
};
</script>

<style module lang="scss" scoped>
@import '../../style/buttons';
@import '../../style/listmenu';

.no-decoration {
  text-decoration: none;
}

.no-padding {
  margin-top: -0.5em;
  margin-left: -1em;
  margin-right: -1em;
}

</style>
