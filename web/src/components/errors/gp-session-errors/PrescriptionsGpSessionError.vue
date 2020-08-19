<template>
  <gp-session-error :code="code" :back-url="backUrl" area="prescriptions" :update-header="false">
    <template v-slot:content>
      <p>{{ $t('gpSessionErrors.prescriptions.ifTheProblemContinues') }}
        <a href="https://111.nhs.uk" target="_blank" rel="noopener noreferrer"
           style="display:inline">
          {{ $t('gpSessionErrors.nhs111Link') }}</a>
        {{ $t('gpSessionErrors.orCall') }}
      </p>
    </template>
    <template v-slot:items>
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
  </gp-session-error>
</template>
<script>
import GpSessionError from '@/components/errors/GPSessionError';
import MenuItem from '@/components/MenuItem';
import { PRESCRIPTIONS_PATH } from '@/router/paths';

export default {
  name: 'PrescriptionsGpSessionError',
  components: {
    GpSessionError,
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
      backUrl: PRESCRIPTIONS_PATH,
    };
  },
  methods: {
    ariaLabelCaption(header, body) {
      return `${this.$t(header)}. ${this.$t(body)}`;
    },
  },
};
</script>
