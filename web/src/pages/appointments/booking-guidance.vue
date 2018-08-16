<template>

  <div v-if="showTemplate" class="pull-content">
    <h2>{{ $t('appointments.guidance.header') }}</h2>
    <div :class="$style.info" data-purpose="info">
      <p>{{ $t('appointments.guidance.text') }}</p>

      <strong>1. {{ $t('appointments.guidance.li1.header') }}</strong>
      <p>{{ $t('appointments.guidance.li1.text') }}</p>

      <strong>2. {{ $t('appointments.guidance.li2.header') }}</strong>
      <p>{{ $t('appointments.guidance.li2.text') }}</p>

      <strong>3. {{ $t('appointments.guidance.li3.header') }}</strong>
      <p>{{ $t('appointments.guidance.li3.text') }}</p>
    </div>

    <button id="btn_check_symptoms" :class="$style.button" @click="onCheckSymptomClicked">
      {{ $t('appointments.guidance.symptomButttonText') }}
    </button>
    <button id="btn_appointment" :class="[$style.button, $style.green]"
            @click="onBookButtonClicked">
      {{ $t('appointments.guidance.bookButtonText') }}
    </button>
  </div>

</template>

<script>
/* eslint-disable import/extensions */
import Routes from '@/Routes';

export default {
  data() {
    let symptomsCheckerUrl = process.env.SYMPTOM_CHECKER_URL;
    if (this.$store.state.device.isNativeApp) {
      symptomsCheckerUrl += process.env.SYMPTOM_CHECKER_NATIVE_QUERY_PARAMS;
    }
    return {
      symptomsCheckerUrl,
    };
  },
  methods: {
    onCheckSymptomClicked() {
      window.open(this.symptomsCheckerUrl, '_blank');
    },
    onBookButtonClicked() {
      this.$router.push(Routes.APPOINTMENT_BOOKING.path);
    },
  },
};

</script>

<style module lang="scss" scoped>
@import "../../style/buttons";
@import "../../style/info";
</style>
