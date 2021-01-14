<template>
  <warning-banner id="corona-virus-banner" :should-be-floating="shouldBeFloating">
    <analytics-tracked-tag
      :class="$style['banner']"
      :text="$t('layout.coronavirus.coronavirusCovid19')"
      href="https://www.nhs.uk/conditions/coronavirus-covid-19/"
      tag="a"
      target="_blank">
      <p class="nhsuk-u-padding-bottom-2 nhsuk-u-padding-top-2
                nhsuk-u-margin-bottom-0 nhsuk-body-s">
        <strong>{{ $t('layout.coronavirus.coronavirusCovid19') }}</strong>
        {{ $t('layout.coronavirus.getInformationAboutCoronvirus') }}
      </p>
    </analytics-tracked-tag>
  </warning-banner>
</template>

<script>
import WarningBanner from '@/components/widgets/WarningBanner';
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';

export default {
  name: 'CoronaVirusBanner',
  components: {
    WarningBanner,
    AnalyticsTrackedTag,
  },
  props: {
    shouldBeFloating: {
      type: Boolean,
      default: false,
    },
  },
};
</script>
<style module lang="scss" scoped>
  @import "@/style/custom/coronavirus-banner";
</style>
