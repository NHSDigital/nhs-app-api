<template>
  <a :href="$t('prescriptions.contactSurgery.emergencyPrescriptionLink')"
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
