<template>
  <menu-item id="emergency_prescription"
             header-tag="h2"
             :href="emergencyPrescriptionsUrl"
             :description="$t('gpSessionErrors.prescriptions.emergencyDescription')"
             target="_blank"
             :text="$t('gpSessionErrors.prescriptions.emergencyHeader')"
             :aria-label="ariaLabelCaption(
               'gpSessionErrors.prescriptions.emergencyHeader',
               'gpSessionErrors.prescriptions.emergencyDescription')"/>
</template>
<script>
import MenuItem from '@/components/MenuItem';

export default {
  name: 'EmergencyPrescriptionMenuItem',
  components: {
    MenuItem,
  },
  props: {
    code: {
      type: String,
      default: undefined,
    },
  },
  data() {
    return {
      emergencyPrescriptionsUrl: this.$store.$env.EMERGENCY_PRESCRIPTIONS_URL,
    };
  },
  methods: {
    ariaLabelCaption(header, body) {
      return `${this.$t(header)}. ${this.$t(body)}`;
    },
  },
};
</script>
