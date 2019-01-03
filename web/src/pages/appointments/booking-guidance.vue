<template>

  <div v-if="showTemplate" class="pull-content">
    <h2 id="guidance_sub_header" :class="[isDesktopWeb ? $style.desktopWeb : $style.web]">
      {{ $t('appointments.guidance.header') }}</h2>
    <div :class="[$style.info, isDesktopWeb ? $style.desktopWeb : $style.web]"
         data-purpose="info">
      <p :class="[isDesktopWeb ? $style.desktopWeb : $style.web]">
        {{ $t('appointments.guidance.text') }}</p>

      <strong :class="[isDesktopWeb ? $style.desktopWeb : $style.web]">
        1. {{ $t('appointments.guidance.li1.header') }}</strong>
      <p :class="[isDesktopWeb ? $style.desktopWeb : $style.web]">
        {{ $t('appointments.guidance.li1.text') }}</p>

      <strong :class="[isDesktopWeb ? $style.desktopWeb : $style.web]">
        2. {{ $t('appointments.guidance.li2.header') }}</strong>
      <p :class="[isDesktopWeb ? $style.desktopWeb : $style.web]">
        {{ $t('appointments.guidance.li2.text') }}</p>

      <strong :class="[isDesktopWeb ? $style.desktopWeb : $style.web]">
        3. {{ $t('appointments.guidance.li3.header') }}</strong>
      <p :class="[isDesktopWeb ? $style.desktopWeb : $style.web]">
        {{ $t('appointments.guidance.li3.text') }}</p>
    </div>

    <generic-button id="btn_check_symptoms"
                    :class="[$style.button, isDesktopWeb ? $style.desktopWeb : $style.web]"
                    tabindex="0"
                    @click="onCheckSymptomClicked">
      {{ $t('appointments.guidance.symptomButttonText') }}
    </generic-button>
    <no-js-form :action="appointmentBookingPath" :value="formData">
      <generic-button
        id="btn_appointment"
        :class="[$style.button, $style.green,
                 isDesktopWeb ? $style.desktopWeb : $style.web]"
        tabindex="0"
        @click.stop.prevent="onBookButtonClicked">
        {{ $t('appointments.guidance.bookButtonText') }}
      </generic-button>
    </no-js-form>
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import { APPOINTMENT_BOOKING, SYMPTOMS } from '@/lib/routes';
import GenericButton from '@/components/widgets/GenericButton';
import NoJsForm from '@/components/no-js/NoJsForm';

export default {
  components: {
    GenericButton,
    NoJsForm,
  },
  data() {
    return {
      isDesktopWeb: (this.$store.state.device.source !== 'android'
        && this.$store.state.device.source !== 'ios'),
    };
  },
  computed: {
    appointmentBookingPath() {
      return APPOINTMENT_BOOKING.path;
    },
    formData() {
      return {
        myAppointments: {
          disableCancellation: this.$store.state.myAppointments.disableCancellation,
        },
      };
    },
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

.button:focus{
 outline-color: $focus_highlight;
 box-shadow: inset 0 0 0 4px $focus_highlight;
}

.button.green:focus{
 outline-color: $focus_highlight;
 box-shadow: inset 0 0 0 4px $focus_highlight;
}
</style>
