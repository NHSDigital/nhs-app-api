<template>
  <div v-if="showTemplate" id="mainDiv" :class="[$style.webHeader, 'pull-content']">
    <p v-if="error"> {{ error }} </p>
    <p v-if="!showGpPractices && !error"> {{ this.$t('th03.errors.noGpPracticesFound') }}</p>
    <ul id="searchResults" :class="$style['list-menu']">
      <li v-for="(gpPractice, gpPracticeIndex) in gpPractices"
          :key="`gpPractice-${gpPracticeIndex}`">
        <analytics-tracked-tag id="btnGpPractice"
                               :class="$style['no-decoration']"
                               :click-func="gpPracticeSelected"
                               :click-param="gpPractice.NACSCode"
                               href="#"
                               text=""
                               tag="a">
          <span :class="$style.fieldName">
            {{ gpPractice.OrganisationName }}
          </span>
          <p> {{ formatAddress(gpPractice) }} </p>
        </analytics-tracked-tag>
      </li>
    </ul>
  </div>
</template>

<script>
import HeaderSlim from '@/components/HeaderSlim';
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import NHSSearchService from '@/services/nhs-search-service';
import { GP_FINDER } from '@/lib/routes';

export default {
  layout: 'throttling',
  components: {
    HeaderSlim,
    AnalyticsTrackedTag,
  },
  data() {
    return {
      searchResults: {},
      error: undefined,
      apiKeyError: false,
    };
  },
  computed: {
    showGpPractices() {
      return this.searchResults && this.searchResults.value && this.searchResults.value.length > 0;
    },
    gpPractices() {
      return this.searchResults ? this.searchResults.value : undefined;
    },
  },
  asyncData(context) {
    const { searchQuery } = context.route.query;
    if (!context.store.app.$env.GP_LOOKUP_API_KEY) return { apiKeyError: true };
    if (!searchQuery || !searchQuery.trim()) return context.redirect(GP_FINDER.path);

    return NHSSearchService.searchGPPractices(context)
      .then(response => ({ searchResults: response.data }))
      .catch(exception => ({ error: exception }));
  },
  created() {
    if (this.apiKeyError) {
      this.error = this.$t('th03.errors.unableToRetrieveResults');
    }
  },
  methods: {
    formatAddress(gpPractice) {
      return [gpPractice.Address1, gpPractice.Address2, gpPractice.Address3, gpPractice.City,
        gpPractice.County, gpPractice.Postcode].filter(Boolean).join(', ');
    },
    gpPracticeSelected(nacsCode) {
      this.$store.$cookies.set('BetaCookie', { ODSCode: nacsCode, ParticipatingPractice: false });
    },
  },
};
</script>

<style module lang="scss" scoped>
@import '../../style/listmenu';
@import '../../style/throttling/gpfinderresults';
</style>
