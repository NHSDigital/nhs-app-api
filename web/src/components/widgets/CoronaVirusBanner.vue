<template>
  <yellow-banner id="corona-virus-banner" :should-be-floating="shouldBeFloating">
    <analytics-tracked-tag
      :class="$style['banner']"
      :text="$t('coronaVirusBanner.header')"
      href="https://www.nhs.uk/conditions/coronavirus-covid-19/"
      tag="a"
      target="_blank">
      <p class="nhsuk-u-padding-bottom-2 nhsuk-u-padding-top-2
                nhsuk-u-margin-bottom-0 nhsuk-body-s">
        <strong>{{ $t('coronaVirusBanner.header') }}</strong>
        {{ $t('coronaVirusBanner.text') }}
      </p>
    </analytics-tracked-tag>
  </yellow-banner>
</template>

<script>
import YellowBanner from '@/components/widgets/YellowBanner';
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';

export default {
  name: 'CoronaVirusBanner',
  components: {
    YellowBanner,
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
  @import '../../style/colours';
  @import "../../style/arrow";
  @import '../../style/accessibility';
  @import '../../style/desktopWeb/accessibility';

  .banner {
    @include icon-arrow-left;
    color: $black;
    text-decoration: none;
    padding: 0 10px;
    margin: 0 -10px;
    background-position: right 10px center;

    &:hover {
      text-decoration: underline;
      box-shadow: none;
      cursor: pointer;
    }

    &:focus {
      @include focusStyleLightMenuItem;
      outline-color: $black;
      box-shadow: inset 0 0 0 4px $black;
      outline-offset: -5px;
    }
  }
</style>
