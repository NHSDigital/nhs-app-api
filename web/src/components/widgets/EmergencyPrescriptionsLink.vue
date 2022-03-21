<template>
  <a :href="emergencyPrescriptionsUrl"
     target="_blank"
     rel="noopener noreferrer"
     style="display:inline"
     @click="nhs111LinkClicked">
    {{ $t('prescriptions.contactSurgery.emergencyPrescriptionLinkText') }}</a>
</template>

<script>
import { PRESCRIPTIONS_CONTACT_SURGERY_PATH } from '@/router/paths';

export default {
  name: 'EmergencyPrescriptionsLink',
  data() {
    return {
      emergencyPrescriptionsUrl: this.$store.$env.EMERGENCY_PRESCRIPTIONS_URL,
    };
  },
  methods: {
    nhs111LinkClicked() {
      this.$store.app.$analytics.trackButtonClick(
        PRESCRIPTIONS_CONTACT_SURGERY_PATH,
        true,
      );
    },
  },
};
</script>
