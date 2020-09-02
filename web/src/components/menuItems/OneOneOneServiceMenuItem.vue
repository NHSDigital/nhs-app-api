<template>
  <menu-item
    id="btn_111"
    header-tag="h2"
    :href="symptomsCheckerUrl"
    target="_blank"
    :text="$t('appointments.guidance.OneOneOne.use111Online')"
    :description="$t('appointments.guidance.OneOneOne.checkIfYouNeedUrgentHelp')"
    :aria-label="ariaLabelCaption(
      'appointments.guidance.OneOneOne.useOneOneOneOnline',
      'appointments.guidance.OneOneOne.checkIfYouNeedUrgentHelp')"/>
</template>

<script>
import MenuItem from '@/components/MenuItem';
import {
  SYMPTOM_CHECKER_NATIVE_QUERY_PARAMS,
} from '@/router/externalLinks';

export default {
  name: 'OneOneOneServiceMenuItem',
  components: {
    MenuItem,
  },
  data() {
    let symptomsCheckerUrl = this.$store.$env.SYMPTOM_CHECKER_URL;
    if (this.$store.state.device.isNativeApp) {
      symptomsCheckerUrl += SYMPTOM_CHECKER_NATIVE_QUERY_PARAMS;
    }
    return {
      symptomsCheckerUrl,
    };
  },
  methods: {
    ariaLabelCaption(header, body) {
      return `${this.$t(header)}. ${this.$t(body)}`;
    },
  },
};
</script>
