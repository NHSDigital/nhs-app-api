<template>
  <div id="mainDiv" :class="[$style['no-padding'], 'pull-content']">
    <div :class="$style.info">
      <h2>{{ $t('organDonation.additionalDetails.subheader') }}</h2>
      <label :class="$style.label" for="ethnicity">
        {{ $t('organDonation.additionalDetails.ethnicity.label') }}
      </label>
      <select-dropdown v-model="ethnicityId"
                       :class="$style.select"
                       :required="false"
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
      <select-dropdown v-model="religionId"
                       :class="$style.select"
                       :required="false"
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

      <back-button />
    </div>
  </div>
</template>

<script>
import get from 'lodash/fp/get';
import BackButton from '@/components/BackButton';
import EnsureDecisionMixin from '@/components/organ-donation/EnsureDecisionMixin';
import GenericButton from '@/components/widgets/GenericButton';
import SelectDropdown from '@/components/widgets/SelectDropdown';
import { ORGAN_DONATION_REVIEW_YOUR_DECISION } from '@/lib/routes';
import { redirectTo } from '@/lib/utils';

const mapAdditionalDetails = self => ({
  ethnicityId: self.ethnicityId,
  religionId: self.religionId,
});

export default {
  components: {
    BackButton,
    GenericButton,
    SelectDropdown,
  },
  mixins: [EnsureDecisionMixin],
  data() {
    return {
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
  },
  methods: {
    continueClicked() {
      this.$store.dispatch('organDonation/setAdditionalDetails', mapAdditionalDetails(this));
      redirectTo(this, ORGAN_DONATION_REVIEW_YOUR_DECISION.path, null);
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
