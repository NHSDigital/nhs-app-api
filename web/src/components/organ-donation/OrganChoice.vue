<template>
  <div :class="$style['new-choice']">
    <h3>{{ $t(title) }}</h3>
    <div :class="$style['horizontal-radio']">
      <generic-radio-button
        :class="$style['choice-radio-button']"
        :name="organName"
        :label="$t('organDonation.someOrgans.choiceYes')"
        :value="'Yes'"
        :model="currentChoice"
        @select="isSelected"/>
    </div>
    <div :class="$style['horizontal-radio']">
      <generic-radio-button
        :class="$style['choice-radio-button']"
        :name="organName"
        :label="$t('organDonation.someOrgans.choiceNo')"
        :value="'No'"
        :model="currentChoice"
        @select="isSelected"/>
    </div>
  </div>
</template>
<script>
import GenericRadioButton from '@/components/widgets/GenericRadioButton';

export default {
  name: 'OrganChoice',
  components: {
    GenericRadioButton,
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
  },
  computed: {
    currentChoice() {
      return this.$store.state.organDonation.registration.decisionDetails.choices[this.organName];
    },
  },
  methods: {
    isSelected(value) {
      this.$store.dispatch('organDonation/setSomeOrgans', { value, choice: this.organName });
    },
  },
};
</script>
<style module lang="scss" scoped>
@import "../../style/info";
@import "../../style/buttons";

.horizontal-radio {
  float: left;
  margin-right: 10px;
}

.new-choice {
    clear: left;
}

.choice-radio-button {
  margin-top: 0.5em;
  margin-bottom: 0.5em;
  &:last-of-type {
    margin-bottom: 1.5em;
  }
}
</style>
