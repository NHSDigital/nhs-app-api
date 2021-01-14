<template>
  <div v-if="showTemplate" class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full">
      <h2 id="guidance_sub_header">
        {{ $t('appointments.informatica.bookingIsNotCurrentlyAvailable') }}
      </h2>
      <div :class="$style.info" data-purpose="info">
        <p>{{ $t('appointments.informatica.yourSurgeryUsesAppointmentsOnline') }}</p>
        <p>
          {{ $t('appointments.informatica.ifYouHaveAUsernameAndPassword') }}
          <analytics-tracked-tag
            :href="informaticaUrl"
            :text="$t('appointments.informatica.logInToAppointmentsOnline')"
            tag="a" target="_blank">
            {{ $t('appointments.informatica.logInToAppointmentsOnline') }}
          </analytics-tracked-tag>.
        </p>
      </div>
    </div>
  </div>
</template>

<script>
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';

export default {
  name: 'InformaticaPage',
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
  @import "@/style/custom/informatica";
</style>
