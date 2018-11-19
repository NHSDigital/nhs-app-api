<template>
  <div>
    <header :class="[$style.slim]">
      <h1 :class="[$style.h1]"> {{ headerText }} </h1>
      <form :action="backLink" method="get">
        <input :value="true" type="hidden" name="reset">
        <input :value="this.$store.state.device.source" type="hidden" name="source">
        <button tabindex="0" type="submit">
          <back-icon/>
        </button>
      </form>
    </header>
    <div v-if="showTemplate" :class="[$style.webHeader, $style.throttlingContent, 'pull-content']">
      <p v-if="error"> {{ error }} </p>
      <p v-else-if="!showGpPractices"> {{ this.$t('th03.errors.noGpPracticesFound') }}</p>
      <ul v-else id="searchResults" :class="$style['list-menu']">
        <li v-for="gpPractice in gpPractices"
            :key="`gpPractice-${gpPractice.NACSCode}`">
          <analytics-tracked-tag :id="`btnGpPractice-${gpPractice.NACSCode}`"
                                 :class="$style['no-decoration']"
                                 :href="getHrefForGpPractice(gpPractice)"
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
  </div>
</template>

<script>
import BackIcon from '@/components/icons/BackIcon';
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import NHSSearchService from '@/services/nhs-search-service';
import { GP_FINDER, GP_FINDER_PARTICIPATION } from '@/lib/routes';

export default {
  layout: 'throttling',
  components: {
    BackIcon,
    AnalyticsTrackedTag,
  },
  data() {
    return {
      searchResults: {},
      error: undefined,
      searchError: false,
      apiKeyError: false,
      headerText: this.$t('th03.header'),
      backLink: GP_FINDER.path,
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
  asyncData({ route, store, redirect }) {
    const { searchQuery } = route.query;
    if (!store.app.$env.GP_LOOKUP_API_KEY) return { apiKeyError: true };
    if (!searchQuery || !searchQuery.trim()) return redirect(GP_FINDER.path);

    return NHSSearchService.searchGPPractices(store.app.$env, searchQuery)
      .then(response => ({ searchResults: response.data }))
      .catch(exception => ({ searchError: !!exception }));
  },
  created() {
    if (this.apiKeyError || this.searchError) {
      this.error = this.$t('th03.errors.unableToRetrieveResults');
    }
  },
  methods: {
    formatAddress(gpPractice) {
      return [gpPractice.Address1, gpPractice.Address2, gpPractice.Address3, gpPractice.City,
        gpPractice.County, gpPractice.Postcode].filter(Boolean).join(', ');
    },
    getHrefForGpPractice(gpPractice) {
      return `${GP_FINDER_PARTICIPATION.path}?odsCode=${gpPractice.NACSCode}` +
        `&practiceName=${encodeURIComponent(gpPractice.OrganisationName)}` +
        `&practiceAddress=${encodeURIComponent(this.formatAddress(gpPractice))}` +
        `&source=${this.$store.state.device.source}`;
    },
  },
};
</script>

<style module lang="scss" scoped>
@import '../../style/listmenu';
@import '../../style/headerslim';
@import '../../style/throttling/throttling';
@import '../../style/throttling/gpfinderresults';
</style>
