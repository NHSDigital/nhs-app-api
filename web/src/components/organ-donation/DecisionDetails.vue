<template>
  <div>
    <h3 id="decision-details-header">
      {{ $t('organDonation.reviewYourDecision.decisionDetails.subheader') }}
    </h3>
    <p id="decision-details-text" :class="$style['mb-3']">
      {{ $t(decisionDetailsTextKey) }}
    </p>

    <div v-if="isSomeOrgans">
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
      <div v-if="hasNotStated" :class="$style.notStated">
        <h4>{{ $t('organDonation.reviewYourDecision.decisionDetails.notStatedHeader') }}</h4>
        <ul>
          <li v-for="choice in notStated" :key="choice">
            {{ $t(`organDonation.reviewYourDecision.decisionDetails.choices.${choice}`) }}
          </li>
        </ul>
      </div>
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
const pickNotStated = flow(pickBy(val => val === 'NotStated'), keys);

export default {
  name: 'DecisionDetails',
  props: {
    choices: {
      type: Object,
      default: () => ({}),
    },
    isSomeOrgans: {
      type: Boolean,
    },
  },
  data() {
    return {
      decisionDetailsTextKey: `organDonation.reviewYourDecision.decisionDetails.${this.isSomeOrgans ? 'someOrgansText' : 'allOrgansText'}`,
    };
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
    hasNotStated() {
      return !isEmpty(this.notStated);
    },
    notStated() {
      return pickNotStated(this.choices);
    },
  },
};
</script>

<style module lang="scss">
  @import "../../style/spacings";

  .chosen, .notChosen, .notStated {
    ul {
      margin-bottom: $three;
    }

    li {
      list-style-type: none;
    }
  }
</style>
