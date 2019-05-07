<template>
  <span v-if="activeButton && !(showApiError || hasConnectionProblem)"
        :class="$style.headerCompanion">
    <no-js-form :action="activeButton.to" :value="activeButton.formData">
      <button id="header-companion-button"
              :class="$style.companionButton"
              @click.stop.prevent="onButtonClicked(activeButton.to)">
        {{ activeButton.text }}
      </button>
    </no-js-form>
  </span>
</template>
<script>
/* eslint-disable no-unused-vars */
import { APPOINTMENTS, APPOINTMENT_BOOKING_GUIDANCE, PRESCRIPTIONS, PRESCRIPTION_REPEAT_COURSES } from '@/lib/routes';
import NoJsForm from '@/components/no-js/NoJsForm';
import { redirectTo } from '@/lib/utils';

export default {
  name: 'HeaderCompanionButton',
  components: {
    NoJsForm,
  },
  data() {
    const buttonData = {};
    buttonData[APPOINTMENTS.name] = {
      to: APPOINTMENT_BOOKING_GUIDANCE.path,
      text: `${this.$t('appointments.index.bookButtonText')}`,
      formData: {},
    };
    buttonData[PRESCRIPTIONS.name] = {
      to: PRESCRIPTION_REPEAT_COURSES.path,
      text: `${this.$t('rp01.orderPrescriptionButton')}`,
      formData: {},
    };
    return {
      buttonData,
    };
  },
  computed: {
    activeButton() {
      const activeButtonData = this.buttonData[this.$route.name];
      if (activeButtonData && this.$route.name === APPOINTMENTS.name) {
        activeButtonData.formData.myAppointments = {
          disableCancellation: this.$store.state.myAppointments.disableCancellation,
        };
      }
      return activeButtonData;
    },
    hasConnectionProblem() {
      return this.$store.state.errors.hasConnectionProblem;
    },
    showApiError() {
      return this.$store.getters['errors/showApiError'];
    },
  },
  methods: {
    onButtonClicked(pathTo) {
      this.$store.app.$analytics.trackButtonClick(pathTo, true);
      redirectTo(this, pathTo, null);
    },
  },
};
</script>
<style module lang="scss" scoped>
 @import '../../style/colours';
 @import '../../style/screensizes';
 @import '../../style/textstyles';
 @import "../../style/fonts";
 @import '../../style/desktopcomponentsizes';

 .headerCompanion {
  @include main-container-width;
  display: block;
  margin: 0 auto;
  padding: 1em 16px 16px;

  .companionButton {
   @include button;
   box-sizing: border-box;
   background-color: $nhs_blue;
   border: none;
   border-radius: 0.125em;
   outline: none;
   transition: all ease 0.5s;
   cursor: pointer;
   width: auto;
   min-width: 16.875em;
   padding: 0.625em 2em;
   max-width: 960px;
   display: block;
  }

  .companionButton:focus {
    outline-color: $focus_highlight;
    box-shadow: 0 0 0 4px $focus_highlight;
  }
 }
</style>
