<template>
  <div v-if="activeButton && !(showApiError || hasConnectionProblem)">
    <no-js-form :action="path" :value="activeButton.formData">
      <button id="header-companion-button"
              class="nhsuk-button"
              @click.stop.prevent="onButtonClicked(path)">
        {{ activeButton.text }}
      </button>
    </no-js-form>
  </div>
</template>
<script>
import GetNavigationPathFromPrescriptions from '@/lib/prescriptions/navigation';
import NoJsForm from '@/components/no-js/NoJsForm';
import { redirectTo } from '@/lib/utils';
import { APPOINTMENT_BOOKING_GUIDANCE_PATH } from '@/router/paths';
import { APPOINTMENTS_NAME, PRESCRIPTIONS_NAME } from '@/router/names';

export default {
  name: 'HeaderCompanionButton',
  components: {
    NoJsForm,
  },
  data() {
    const buttonData = {};
    buttonData[APPOINTMENTS_NAME] = {
      text: `${this.$t('appointments.index.bookButtonText')}`,
      formData: {},
    };
    buttonData[PRESCRIPTIONS_NAME] = {
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
      if (activeButtonData && this.$route.name === APPOINTMENTS_NAME) {
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
    path() {
      if (this.$route.name === APPOINTMENTS_NAME) {
        return APPOINTMENT_BOOKING_GUIDANCE_PATH;
      }
      if (this.$route.name === PRESCRIPTIONS_NAME) {
        return GetNavigationPathFromPrescriptions(this.$store);
      }
      return '';
    },
  },
  methods: {
    onButtonClicked(pathTo) {
      this.$store.app.$analytics.trackButtonClick(pathTo, true);
      redirectTo(this, pathTo);
    },
  },
};
</script>

