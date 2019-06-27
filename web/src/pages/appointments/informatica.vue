<template>
  <div v-if="showTemplate"
       :class="[$style['pull-content'], !$store.state.device.isNativeApp && $style.desktopWeb]" >
    <div>
      <h2 id="guidance_sub_header">
        {{ $t('appointments.informatica.header') }}
      </h2>
      <div data-purpose="info" :class="$style.info">
        <p>{{ $t('appointments.informatica.description') }}</p>

        <p>
          {{ $t('appointments.informatica.link.prefix') }}
          <analytics-tracked-tag
            :href="informaticaUrl"
            :text="$t('appointments.informatica.link.text')"
            tag="a" target="_blank">
            {{
              $t('appointments.informatica.link.text')
            }}</analytics-tracked-tag>.
        </p>
      </div>
    </div>
  </div>
</template>

<script>
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';

export default {
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
@import "../../style/textstyles";
@import "../../style/nhsuklinks";

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
