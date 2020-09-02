<template>
  <div id="mainDiv" :class="[$style.form]" class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full">
      <h2>{{ $t('organDonation.additionalDetails.additionalDetails') }}</h2>
      <p>{{ $t('organDonation.additionalDetails.optionalInformationIsOnlyUsedBy') }}</p>
      <label for="ethnicity" class="nhsuk-label">
        {{ $t('organDonation.additionalDetails.ethnicityOptional') }}
      </label>
      <select-dropdown v-model="ethnicityId"
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
      <label for="religion" class="nhsuk-label">
        {{ $t('organDonation.additionalDetails.religionOptional') }}
      </label>
      <select-dropdown v-model="religionId"
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
      <generic-button id="continue-button"
                      :class="['nhsuk-button']"
                      @click.prevent="continueClicked">
        {{ $t('generic.continue') }}
      </generic-button>
      <back-button v-if="!$store.state.device.isNativeApp" />
    </div>
  </div>
</template>

<script>
import get from 'lodash/fp/get';
import BackButton from '@/components/BackButton';
import EnsureDecisionMixin from '@/components/organ-donation/EnsureDecisionMixin';
import GenericButton from '@/components/widgets/GenericButton';
import SelectDropdown from '@/components/widgets/SelectDropdown';
import { ORGAN_DONATION_REVIEW_YOUR_DECISION_PATH } from '@/router/paths';
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
        { id: '', displayName: this.$t('organDonation.additionalDetails.pleaseSelect') },
        ...get('$store.state.organDonation.referenceData.ethnicities')(this),
      ];
    },
    religions() {
      return [
        { id: '', displayName: this.$t('organDonation.additionalDetails.pleaseSelect') },
        ...get('$store.state.organDonation.referenceData.religions')(this),
      ];
    },
  },
  methods: {
    continueClicked() {
      this.$store.dispatch('organDonation/setAdditionalDetails', mapAdditionalDetails(this));
      redirectTo(this, ORGAN_DONATION_REVIEW_YOUR_DECISION_PATH);
    },
  },
};
</script>

<style module lang="scss" scoped>
 @import "../../style/forms";
</style>
