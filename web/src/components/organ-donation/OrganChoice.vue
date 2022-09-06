<template>
  <nhs-uk-radio-group
    v-model="selectedValue"
    :name="organName"
    :items="choices"
    :error="showError"
    :error-text="inlineErrorMessage"
    :current-value="currentChoice"
    :heading="$t(title)"
    :legend-size="'xs'"
    @onselect="selected"
  />
</template>
<script>
import NhsUkRadioGroup from '@/components/nhsuk-frontend/NhsUkRadioGroup';
import { NO, YES } from '@/store/modules/organDonation/mutation-types';

export default {
  name: 'OrganChoice',
  components: {
    NhsUkRadioGroup,
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
    showError: {
      type: Boolean,
      required: true,
    },
  },
  data() {
    return {
      choices: [
        { label: this.$t('organDonation.someOrgans.yes'), value: YES },
        { label: this.$t('organDonation.someOrgans.no'), value: NO },
      ],
      inlineErrorMessage: this.$t('organDonation.someOrgans.chooseYesOrNoFor') + this.$t(this.title).toLowerCase(),
      selectedValue: this.currentChoice,
    };
  },
  computed: {
    currentChoice() {
      return this.$store.state.organDonation.registration.decisionDetails.choices[this.organName];
    },
  },
  methods: {
    selected(value) {
      this.$store.dispatch('organDonation/setSomeOrgans', { value, choice: this.organName });
    },
  },
};
</script>
