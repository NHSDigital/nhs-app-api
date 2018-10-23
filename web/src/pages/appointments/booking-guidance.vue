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

    <generic-button id="btn_check_symptoms" :class="$style.button" @click="onCheckSymptomClicked">
      {{ $t('appointments.guidance.symptomButttonText') }}
    </generic-button>
    <generic-button id="btn_appointment" :class="[$style.button, $style.green]"
                    @click="onBookButtonClicked">
      {{ $t('appointments.guidance.bookButtonText') }}
    </generic-button>
  </div>

</template>

<script>
/* eslint-disable import/extensions */
import { APPOINTMENT_BOOKING, SYMPTOMS } from '@/lib/routes';
import GenericButton from '@/components/widgets/GenericButton';

export default {
  components: {
    GenericButton,
  },
  methods: {
    onCheckSymptomClicked() {
      this.$router.push(SYMPTOMS.path);
    },
    onBookButtonClicked() {
      this.$router.push(APPOINTMENT_BOOKING.path);
    },
  },
};

</script>

<style module lang="scss" scoped>
@import "../../style/buttons";
@import "../../style/info";
</style>
