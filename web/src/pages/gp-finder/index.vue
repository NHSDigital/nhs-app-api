<template>
  <div v-if="versionCheckComplete && throttlingEnabled" :class="$style.flexContainer">
    <analytics-tracked-tag id="help_icon"
                           :class="$style['help-icon']"
                           :href="helpAndSupportURL"
                           tag="a"
                           text="help"
                           target="_blank"
                           tabindex="-1">
      <help-icon/>
    </analytics-tracked-tag>

    <div :class="[$style.throttlingHeader, $style['header-container']]">
      <header>
        <div :class="$style.spacer" />
        <nhs-logo :class="$style.nhsoLogo"/>
        <div :class="$style.spacer" />
      </header>
      <throttling-banner />
    </div>

    <div :class="$style.throttlingContent">
      <h3>{{ $t('th02.heading1') }}</h3>
      <h4>{{ $t('th02.heading2') }}</h4>

      <p id="search-label">{{ $t('th02.hintText') }}</p>
      <error-message v-if="showError"
                     :id="$style['error-label']"
                     role="alert">
        {{ $t('th02.emptySearchError') }}
      </error-message>
      <form @submit.prevent="searchFormSubmitted">
        <generic-text-input id="searchTextInput"
                            v-model="searchQuery"
                            type="text"
                            a-labelled-by="search-label"
                            name="searchQuery"
                            maxlength="150"/>
        <analytics-tracked-tag :text="$t('th02.callToAction')">
          <generic-button :button-classes="['green', 'button']"
                          @click="ctaClicked">
            {{ $t('th02.callToAction') }}
          </generic-button>
        </analytics-tracked-tag>
      </form>

      <analytics-tracked-tag :click-func="hasAnAccountLinkClicked"
                             :text="$t('th02.hasAnAccountLink')"
                             tag="a"
                             tabindex="0">
        {{ $t('th02.hasAnAccountLink') }}
      </analytics-tracked-tag>
    </div>
  </div>
</template>

<script>
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import ErrorMessage from '@/components/widgets/ErrorMessage';
import GenericButton from '@/components/widgets/GenericButton';
import GenericTextInput from '@/components/widgets/GenericTextInput';
import HelpIcon from '@/components/icons/HelpIcon';
import NhsLogo from '@/components/icons/NhsLogo';
import ThrottlingBanner from '@/components/ThrottlingBanner';
import { setCookie } from '@/lib/cookie-manager';
import { GP_FINDER_RESULTS, LOGIN } from '@/lib/routes';
import NativeCallbacks from '@/services/native-app';
import moment from 'moment';

export default {
  layout: 'throttling',
  components: {
    NhsLogo,
    HelpIcon,
    ThrottlingBanner,
    ErrorMessage,
    GenericTextInput,
    GenericButton,
    AnalyticsTrackedTag,
  },
  data() {
    return {
      searchQuery: '',
      continueClicked: false,
      helpAndSupportURL: this.$store.app.$env.HELP_AND_SUPPORT_URL,
      submissionError: false,
      versionCheckComplete: false,
    };
  },
  computed: {
    showError() {
      return this.submissionError;
    },
    throttlingEnabled() {
      return this.$store.app.$env.THROTTLING_ENABLED;
    },
  },
  beforeCreate() {
    if (process.client) {
      NativeCallbacks.hideHeader();
    }
  },
  mounted() {
    if (!this.throttlingEnabled) {
      this.goToUrl(LOGIN.path);
      return;
    }
    this.$nextTick(() => {
      if (process.client) {
        const self = this;

        setTimeout(() => {
          if (self.$store.state.device.isNativeApp) {
            const nativeAppVersion = this.$store.state.appVersion.nativeVersion;

            if (!nativeAppVersion || (nativeAppVersion && nativeAppVersion.toString().startsWith('0'))) {
              setCookie({
                cookies: self.$store.app.$cookies,
                key: 'BetaCookie',
                value: {
                  Skipped: true,
                  NeverShowCheckFeatureLink: true,
                },
                options: {
                  maxAge: moment.duration(1, 'y').asSeconds(),
                  secure: this.$store.app.$env.SECURE_COOKIES,
                },
              });

              if (process.client) {
                NativeCallbacks.storeBetaCookie();
              }

              window.location.reload(true);
            } else {
              self.versionCheckComplete = true;
            }
          } else {
            self.versionCheckComplete = true;
          }
        }, 100);
      }
    });
  },
  methods: {
    searchFormSubmitted() {
      this.ctaClicked();
    },
    async ctaClicked() {
      if (this.continueClicked) return;
      this.continueClicked = true;

      const processedQuery = this.processQuery(this.searchQuery);

      if (!processedQuery) {
        this.submissionError = true;
        this.continueClicked = false;
        return;
      }

      const gpSearchResponse = await this.searchForGpPractices(processedQuery);

      if (gpSearchResponse.submissionError) {
        this.submissionError = true;
        this.continueClicked = false;
        return;
      }

      this.$store.dispatch('throttling/setSearchQuery', processedQuery);
      this.$store.dispatch('throttling/setSearchResults', gpSearchResponse);
      this.goToUrl(GP_FINDER_RESULTS.path);

      this.continueClicked = false;
    },
    processQuery(searchQuery) {
      let processedQuery;

      if (searchQuery) {
        processedQuery = searchQuery.trim();
      }

      if (processedQuery) {
        processedQuery = this.sanitiseSearchQuery(processedQuery);
      }

      return processedQuery;
    },
    async searchForGpPractices(searchQuery) {
      const gpSearchResult = {
        technicalError: false,
        noResultsFound: false,
        tooManyResults: false,
        organisations: undefined,
        submissionError: false,
      };

      const gpSearchRequest = {
        SearchTerm: searchQuery,
      };

      await this.$store.app.$http.postV1Gpsearch({ gpSearchRequest })
        .then((response) => {
          gpSearchResult.organisations = response.organisations;
          if (response.organisationQueryCount > this.$store.app.$env.GP_LOOKUP_API_RESULTS_LIMIT) {
            gpSearchResult.tooManyResults = true;
            return;
          }
          gpSearchResult.noResultsFound = !response.organisationQueryCount;
        })
        .catch((error) => {
          if (!error.response || !error.response.status) {
            gpSearchResult.technicalError = true;
            return;
          }

          if (error.response.status === 400) {
            gpSearchResult.submissionError = true;
            return;
          }

          gpSearchResult.technicalError = true;
        });

      return gpSearchResult;
    },
    sanitiseSearchQuery(searchQuery) {
      return searchQuery
        ? searchQuery.replace(/[-]/g, ' ').replace(/\s\s+/g, ' ').replace(/[/\\^$*+&?,.()|[\]{}"~:!<>£;@%^'`]/g, '')
        : searchQuery;
    },
    hasAnAccountLinkClicked() {
      setCookie({
        cookies: this.$store.app.$cookies,
        key: 'BetaCookie',
        value: {
          Skipped: true,
        },
        options: {
          maxAge: moment.duration(1, 'y').asSeconds(),
        },
      });

      if (process.client) {
        NativeCallbacks.storeBetaCookie();
      }

      this.goToUrl(LOGIN.path);
    },
  },
};
</script>

<style module lang="scss" scoped>
@import '../../style/buttons';
@import '../../style/throttling/throttling';
@import '../../style/throttling/gpfindersearch';
</style>
