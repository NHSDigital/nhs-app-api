<template>
  <div :class="$style['new-choice']">
    <h3>{{ $t(title) }}</h3>
    <div :class="$style['horizontal-radio']">
      <generic-radio-button
        :class="$style['choice-radio-button']"
        :name="organName"
        :label="$t('organDonation.someOrgans.choiceYes')"
        :value="'Yes'"
        :checked="currentChoice === 'Yes'"
        @select="selected"/>
    </div>
    <div :class="$style['horizontal-radio']">
      <generic-radio-button
        :class="$style['choice-radio-button']"
        :name="organName"
        :label="$t('organDonation.someOrgans.choiceNo')"
        :value="'No'"
        :checked="currentChoice === 'No'"
        @select="selected"/>
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
    selected(value) {
      this.$store.dispatch('organDonation/setSomeOrgans', { value, choice: this.organName });
    },
  },
};
</script>
<style module lang="scss" scoped>
@import "../../style/info";
@import "../../style/buttons";
@import "../../style/spacings";

.horizontal-radio {
  float: left;
  margin-right: $four;
}

.new-choice {
    clear: left;
}

.choice-radio-button {
  margin-top: $one;
  margin-bottom: $two;
  &:last-of-type {
    margin-bottom: 1.5em;
  }
}
</style>
