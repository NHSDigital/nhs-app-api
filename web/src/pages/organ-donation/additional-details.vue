<template>
  <div id="mainDiv" :class="[$style['no-padding'], 'pull-content']">
    <div :class="$style.info">
      <h2>{{ $t('organDonation.additionalDetails.subheader') }}</h2>
      <form id="continue-form" :action="continueAction" method="post">
        <input :value="JSON.stringify($store.state.organDonation)"
               type="hidden"
               name="nojs.organDonation">
        <label :class="$style.label" for="ethnicity">
          {{ $t('organDonation.additionalDetails.ethnicity.label') }}
        </label>
        <select-dropdown :class="$style.select"
                         :required="false"
                         v-model="ethnicityId"
                         select-id="ethnicity"
                         select-name="nojs.organDonation.additionalDetails.ethnicityId">
          <option v-for="option in ethnicities"
                  :key="option.id"
                  :value="option.id"
                  :disabled="option.value===''"
                  :selected="option.value===''">
            {{ option.displayName }}
          </option>
        </select-dropdown>

        <label :class="$style.label" for="religion">
          {{ $t('organDonation.additionalDetails.religion.label') }}
        </label>
        <select-dropdown :class="$style.select"
                         :required="false"
                         v-model="religionId"
                         select-id="religion"
                         select-name="nojs.organDonation.additionalDetails.religionId">
          <option v-for="option in religions"
                  :key="option.id"
                  :value="option.id"
                  :disabled="option.value===''"
                  :selected="option.value===''">
            {{ option.displayName }}
          </option>
        </select-dropdown>
        <p>{{ $t('organDonation.additionalDetails.description') }}</p>

        <generic-button id="continue-button"
                        :class="[$style.button, $style.green]"
                        @click.prevent="continueClicked">
          {{ $t('organDonation.additionalDetails.continueButton') }}
        </generic-button>
      </form>

      <form id="back-form" :action="backAction" method="get">
        <generic-button id="back-button"
                        :class="[$style.button, $style.grey]"
                        @click.prevent="backClicked">
          {{ $t('organDonation.additionalDetails.backButton') }}
        </generic-button>
      </form>
    </div>
  </div>
</template>

<script>
import get from 'lodash/fp/get';
import EnsureDecisionMixin from '@/components/organ-donation/EnsureDecisionMixin';
import GenericButton from '@/components/widgets/GenericButton';
import SelectDropdown from '@/components/widgets/SelectDropdown';
import NoJsForm from '@/components/no-js/NoJsForm';
import { ORGAN_DONATION, ORGAN_DONATION_REVIEW_YOUR_DECISION } from '@/lib/routes';
import { DECISION_NOT_FOUND } from '@/store/modules/organDonation/mutation-types';

const mapAdditionalDetails = self => ({
  ethnicityId: self.ethnicityId,
  religionId: self.religionId,
});

export default {
  components: {
    GenericButton,
    NoJsForm,
    SelectDropdown,
  },
  mixins: [EnsureDecisionMixin],
  data() {
    return {
      backAction: ORGAN_DONATION.path,
      continueAction: ORGAN_DONATION_REVIEW_YOUR_DECISION.path,
      ethnicityId: get('$store.state.organDonation.additionalDetails.ethnicityId')(this),
      religionId: get('$store.state.organDonation.additionalDetails.religionId')(this),
    };
  },
  computed: {
    ethnicities() {
      return [
        { id: '', displayName: this.$t('organDonation.additionalDetails.ethnicity.placeholder') },
        ...get('$store.state.organDonation.referenceData.ethnicities')(this),
      ];
    },
    religions() {
      return [
        { id: '', displayName: this.$t('organDonation.additionalDetails.religion.placeholder') },
        ...get('$store.state.organDonation.referenceData.religions')(this),
      ];
    },
    nojs() {
      return JSON.stringify({
        organDonation: {
          registration: mapAdditionalDetails(this),
        },
      });
    },
  },
  asyncData({ store }) {
    return store.dispatch('organDonation/getReferenceData');
  },
  mounted() {
    if (this.$store.state.organDonation.registration.decision === DECISION_NOT_FOUND) {
      this.$router.push(ORGAN_DONATION.path);
    }
  },
  methods: {
    backClicked() {
      this.$router.push(ORGAN_DONATION.path);
    },
    continueClicked() {
      this.$store.dispatch('organDonation/setAdditionalDetails', mapAdditionalDetails(this));
      this.$router.push(ORGAN_DONATION_REVIEW_YOUR_DECISION.path);
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import "../../style/spacings";
  @import "../../style/buttons";
  @import "../../style/info";
  @import "../../style/accessibility";

  .label {
    margin-top: $one;
  }

  .select {
    margin-bottom: $three;
  }
</style>
