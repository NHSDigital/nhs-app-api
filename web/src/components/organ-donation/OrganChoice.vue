<template>
  <radio-group :key="organName"
               :name="organName"
               :radios="choices"
               :inline="true"
               :header="$t(title)"
               :current-value="currentChoice"
               :show-error="showError"
               :error-message="$t('organDonation.someOrgans.inlineErrorMessage')"
               @select="selected"/>
</template>
<script>
import RadioGroup from '@/components/RadioGroup';
import { NO, NOT_STATED, YES } from '@/store/modules/organDonation/mutation-types';

export default {
  name: 'OrganChoice',
  components: {
    RadioGroup,
  },
  props: {
    title: {
      type: String,
      required: true,
    },
    organName: {
      type: String,
      required: true,
    },
    showErrors: {
      type: Boolean,
      required: true,
    },
  },
  data() {
    return {
      choices: [
        { label: this.$t('organDonation.someOrgans.choices.yes'), value: YES },
        { label: this.$t('organDonation.someOrgans.choices.no'), value: NO },
      ],
    };
  },
  computed: {
    currentChoice() {
      return this.$store.state.organDonation.registration.decisionDetails.choices[this.organName];
    },
    showError() {
      return this.showErrors && this.currentChoice === NOT_STATED;
    },
  },
  methods: {
    selected(value) {
      this.$store.dispatch('organDonation/setSomeOrgans', { value, choice: this.organName });
    },
  },
};
</script>
<style module lang="scss" scoped>
</style>
