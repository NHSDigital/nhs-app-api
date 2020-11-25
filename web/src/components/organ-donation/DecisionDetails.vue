<template>
  <div>
    <h3 id="decision-details-header">
      {{ $t('organDonation.decisionDetails.decisionDetails') }}
    </h3>
    <p id="decision-details-text">
      {{ $t(decisionDetailsTextKey) }}
    </p>
    <div v-if="isSomeOrgans">
      <div id="chosen">
        <h4 class="nhsuk-heading-xs nhsuk-u-margin-0">
          {{ $t('organDonation.decisionDetails.youHaveChosenToDonate') }}
        </h4>
        <ul class="nhsuk-list nhsuk-u-margin-left-3">
          <li v-for="choice in chosen" :key="choice">
            {{ $t(`organDonation.organs.${choice}`) }}
          </li>
        </ul>
      </div>
      <div v-if="hasNotChosen" id="notChosen">
        <h4 class="nhsuk-heading-xs nhsuk-u-margin-0">
          {{ $t('organDonation.decisionDetails.youHaveChosenNotToDonate') }}
        </h4>
        <ul class="nhsuk-list nhsuk-u-margin-left-3">
          <li v-for="choice in notChosen" :key="choice">
            {{ $t(`organDonation.organs.${choice}`) }}
          </li>
        </ul>
      </div>
      <div v-if="hasNotStated" id="notStated">
        <h4 class="nhsuk-heading-xs nhsuk-u-margin-0">
          {{ $t('organDonation.decisionDetails.weDoNotHaveADecisionFor') }}
        </h4>
        <ul class="nhsuk-list nhsuk-u-margin-left-3">
          <li v-for="choice in notStated" :key="choice">
            {{ $t(`organDonation.organs.${choice}`) }}
          </li>
        </ul>
      </div>
    </div>
  </div>
</template>

<script>
import { flow, isEmpty, keys, pickBy } from 'lodash/fp';

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
      decisionDetailsTextKey: `organDonation.decisionDetails.${this.isSomeOrgans ? 'donateSomeOfMyOrgans' : 'donateAllOfMyOrgans'}`,
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
