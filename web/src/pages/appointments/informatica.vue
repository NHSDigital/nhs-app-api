<template>
  <div v-if="showTemplate" >
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <h2 id="guidance_sub_header">
          {{ $t('appointments.informatica.header') }}
        </h2>
        <div :class="$style.info" data-purpose="info">
          <p>{{ $t('appointments.informatica.description') }}</p>

          <p>
            {{ $t('appointments.informatica.link.prefix') }}
            <analytics-tracked-tag
              :href="informaticaUrl"
              :text="$t('appointments.informatica.link.text')"
              tag="a" target="_blank">
              {{ $t('appointments.informatica.link.text') }}
            </analytics-tracked-tag>.
          </p>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';

export default {
  layout: 'nhsuk-layout',

  components: {
    AnalyticsTrackedTag,
  },
  data() {
    return {
      informaticaUrl: this.$store.state.serviceJourneyRules.rules.appointments.informaticaUrl,
    };
  },
};
</script>

<style module lang="scss" scoped>
@import "../../style/info";
@import "../../style/nhsuklinks";
@import "../../style/textstyles";

a {
  text-decoration: underline;
}
div {
  .info {
    a {
      display: inline;
      vertical-align: unset;
      font-weight: normal;
      padding: 0;
    }
  }
  .desktopWeb {
    max-width: 540px;
    .info {
      p {
        font-family: $default-web;
        font-weight: lighter;
      }
    }
  }
}
</style>
