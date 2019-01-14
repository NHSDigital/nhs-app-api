<template>
  <div id="mainDiv" :class="[$style['no-padding'], 'pull-content']">
    <div :class="$style.info">
      <h2>{{ $t('organDonation.additionalDetails.subheader') }}</h2>
      <label :class="$style.label" for="ethnicity">
        {{ $t('organDonation.additionalDetails.ethnicity.label') }}
      </label>
      <select-dropdown :class="$style.select"
                       v-model="selectedEthnicity"
                       select-id="ethnicity"
                       select-name="ethnicity">
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
                       v-model="selectedReligion"
                       select-id="religion"
                       select-name="religion">
        <option v-for="option in religions"
                :key="option.id"
                :value="option.id"
                :disabled="option.value===''"
                :selected="option.value===''">
          {{ option.displayName }}
        </option>
      </select-dropdown>
      <p>{{ $t('organDonation.additionalDetails.description') }}</p>
    </div>

    <form id="back-form" :action="backAction" method="get">
      <generic-button id="back-button"
                      :class="[$style.button, $style.grey]"
                      @click.prevent="backClicked">
        {{ $t('organDonation.additionalDetails.backButton') }}
      </generic-button>
    </form>
  </div>
</template>

<script>
import get from 'lodash/fp/get';
import GenericButton from '@/components/widgets/GenericButton';
import SelectDropdown from '@/components/widgets/SelectDropdown';
import { ORGAN_DONATION } from '@/lib/routes';
import { DECISION_NOT_FOUND } from '@/store/modules/organDonation/mutation-types';

export default {
  components: {
    GenericButton,
    SelectDropdown,
  },
  data() {
    return {
      backAction: ORGAN_DONATION.path,
      selectedReligion: '',
      selectedEthnicity: '',
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
  asyncData({ redirect, store }) {
    if (store.state.organDonation.registration.decision === DECISION_NOT_FOUND) {
      return redirect(ORGAN_DONATION.path);
    }

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
