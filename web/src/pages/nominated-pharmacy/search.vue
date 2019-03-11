<template>
  <div v-if="showTemplate" :class="[$style.content, 'pull-content']">
    <div :class="$style.info">
      <p> {{ $t('searchNominatedPharmacy.line1') }} </p>
      <h3> {{ $t('searchNominatedPharmacy.subHeader') }} </h3>
      <p> {{ $t('searchNominatedPharmacy.line2') }} </p>
      <error-message v-if="showError"
                     :id="$style['error-label']"
                     role="alert">
        {{ $t('searchNominatedPharmacy.emptySearchError') }}
      </error-message>
      <form @submit.prevent="searchFormSubmitted">
        <generic-text-input id="searchTextInput"
                            :class="$style.inputSpacing"
                            v-model="searchQuery"
                            type="text"
                            name="searchQuery"
                            maxlength="10"/>
        <generic-button :button-classes="['green', 'button']">
          {{ $t('searchNominatedPharmacy.searchButton') }}
        </generic-button>
      </form>
      <analytics-tracked-tag :href="allPharmaciesURL"
                             :text="$t('searchNominatedPharmacy.link1')"
                             tag="a">
        <abbreviations-arrow-right-icon />
        {{ $t('searchNominatedPharmacy.link1') }}
      </analytics-tracked-tag>
      <analytics-tracked-tag :href="allDispensingContractorsURL"
                             :text="$t('searchNominatedPharmacy.link2')"
                             tag="a">
        <abbreviations-arrow-right-icon />
        {{ $t('searchNominatedPharmacy.link2') }}
      </analytics-tracked-tag>
    </div>
  </div>
</template>

<script>
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import MedicationCourseStatus from '@/lib/medication-course-status';
import GenericButton from '@/components/widgets/GenericButton';
import GenericTextInput from '@/components/widgets/GenericTextInput';
import { getDynamicStyle } from '@/lib/desktop-experience';
import AbbreviationsArrowRightIcon from '@/components/icons/AbbreviationsArrowRightIcon';
import { NOMINATED_PHARMACY_SEARCH } from '@/lib/routes';
import ErrorMessage from '@/components/widgets/ErrorMessage';


export default {
  components: {
    GenericButton,
    GenericTextInput,
    AbbreviationsArrowRightIcon,
    AnalyticsTrackedTag,
    ErrorMessage,
  },
  data() {
    return {
      isButtonDisabled: false,
      searchQuery: '',
      allPharmaciesURL: NOMINATED_PHARMACY_SEARCH.path,
      allDispensingContractorsURL: NOMINATED_PHARMACY_SEARCH.path,
      submissionError: false,
    };
  },
  async searchNominatedPharmacy() {
    return {
      statusDisplayPriority: {
        [MedicationCourseStatus.Rejected]: 1,
        [MedicationCourseStatus.Requested]: 2,
        [MedicationCourseStatus.Approved]: 3,
      },
    };
  },

  computed: {
    hasLoaded() {
      return this.$store.state.searchNominatedPharmacy.hasLoaded;
    },
    showError() {
      return this.submissionError;
    },
  },
  created() {
  },
  methods: {
    dynamicStyle(...args) {
      return getDynamicStyle(this, args);
    },
    searchFormSubmitted() {
      this.submissionError = !this.validateSearchQuery(this.searchQuery);
    },
    validateSearchQuery(searchQuery) {
      let processedQuery;

      if (searchQuery) {
        processedQuery = searchQuery.trim();
      }
      if (processedQuery) {
        return processedQuery.length >= 1 && processedQuery.length <= 10;
      }
      return false;
    },
  },
};
</script>

<style module lang="scss" scoped>
@import "../../style/buttons";
@import "../../style/spacings";
@import '../../style/throttling/gpfindersearch';
@import '../../style/colours';

.above-float-button {
  margin-bottom: $marginBottomFullScreen;
}
.info {
  margin-bottom: 0em;
  padding: 0.5em 1em 0em 1em;
  :focus {
    outline-color: $focus_highlight;
  }
  a, analytics-tracked-tag {
    padding-bottom: 0.25em;
    padding-top: 0.5em;
    padding-left: 0em;
    text-align: left;
    text-decoration: none;
    margin-bottom: 0.25em;
    font-size: 1em;
    font-weight: bold;
  }
p {
    font-size: 1em;
    color: #212B32;
    margin-bottom: 0.25em;
  }
  h3 {
    margin-bottom: 0em;
    padding-bottom: 0em;
  }
}
.inputSpacing{
 margin-bottom: 1em;
}
</style>
