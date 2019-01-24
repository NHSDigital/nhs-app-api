<template>
  <div>
    <div :class="$style.chosen">
      <h4>{{ $t('organDonation.reviewYourDecision.decisionDetails.chosenHeader') }}</h4>
      <ul>
        <li v-for="choice in chosen" :key="choice">
          {{ $t(`organDonation.reviewYourDecision.decisionDetails.choices.${choice}`) }}
        </li>
      </ul>
    </div>
    <div v-if="hasNotChosen" :class="$style.notChosen">
      <h4>{{ $t('organDonation.reviewYourDecision.decisionDetails.notChosenHeader') }}</h4>
      <ul>
        <li v-for="choice in notChosen" :key="choice">
          {{ $t(`organDonation.reviewYourDecision.decisionDetails.choices.${choice}`) }}
        </li>
      </ul>
    </div>
  </div>
</template>

<script>
import flow from 'lodash/fp/flow';
import isEmpty from 'lodash/fp/isEmpty';
import keys from 'lodash/fp/keys';
import pickBy from 'lodash/fp/pickBy';

const pickChosen = flow(pickBy(val => val === 'Yes'), keys);
const pickNotChosen = flow(pickBy(val => val === 'No'), keys);

export default {
  name: 'DecisionDetails',
  props: {
    choices: {
      type: Object,
      required: true,
    },
  },
  computed: {
    chosen() {
      return pickChosen(this.choices);
    },
    hasNotChosen() {
      return !isEmpty(this.notChosen);
    },
    notChosen() {
      return pickNotChosen(this.choices);
    },
  },
};
</script>

<style module lang="scss">
  @import "../../style/spacings";

  .chosen, .notChosen {
    ul {
      margin-bottom: $three;
    }

    li {
      list-style-type: none;
    }
  }
</style>
