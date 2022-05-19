<template>
  <div v-if="showTemplate">
    <silver-integration-panel
      v-if="shouldShowWarning"
      :known-service="matchedService"
      :redirect-path="redirectParameter"
      :jump-off-id="jumpOffId"
      :session-storage-name="sessionStorageName"
      :is-wayfinder-url="isWayfinderUrl"
      @click.stop.prevent="getAssertedLoginIdentityAndNavigate"/>
    <div v-else :class="$style['spacer']" />
  </div>
</template>

<script>
import NativeApp from '@/services/native-app';
import SilverIntegrationPanel from '@/components/redirector/SilverIntegrationPanel';
import agreedToThirdPartyWarning from '@/lib/sessionStorage';
import proofLevel from '@/lib/proofLevel';
import sjrIf from '@/lib/sjrIf';
import {
  REDIRECT_PAGE_PARAMETER,
  REDIRECT_PARAMETER,
  isNhsAppRouteName,
  findByRedirectEnum,
} from '@/router/names';
import {
  INDEX_PATH,
  UPLIFT_SILVER_INTEGRATION_PATH,
  SILVER_INTEGRATION_FEATURE_NOT_AVAILABLE_PATH,
} from '@/router/paths';
import { get, isEmpty } from 'lodash/fp';
import {
  getPathAndQuery,
  getThirdPartyJumpOff,
  isNhsAppHost,
  isNhsAppPath,
  pathWithPatientPrefixOrUndefined,
  redirectByName,
  redirectTo,
  removeNhsAppHost,
} from '@/lib/utils';
import { setWindowLocation } from '@/lib/window';
import {
  getJumpOffConfiguration,
  getWayfinderJumpOffConfiguration,
} from '@/lib/third-party-providers/jump-off-configuration';

export default {
  name: 'InterstitialRedirectorPage',
  components: {
    SilverIntegrationPanel,
  },
  data() {
    return {
      showWarning: false,
      matchedService: {},
      redirectParameter: '',
      buttonHref: '',
      buttonDisabled: false,
      sessionStorageName: '',
      thirdPartyServiceContent: '',
      jumpOffId: null,
      isWayfinderUrl: false,
    };
  },
  computed: {
    shouldShowWarning() {
      return this.showWarning && !agreedToThirdPartyWarning(this.sessionStorageName);
    },
  },
  watch: {
    '$route.query': function watchQueryString() {
      this.startRedirect();
    },
  },
  mounted() {
    this.$store.dispatch('device/pageLoadComplete');
    this.startRedirect();
  },
  methods: {
    getDecodedRedirectParam() {
      const redirectParam = get(REDIRECT_PARAMETER)(this.$route.query);
      if (redirectParam) {
        // If the redirect parameter does not contain a colon (i.e. "https://"), it must be encoded (deep link scenario), so needs decoding.
        return redirectParam.includes(':') ? redirectParam : decodeURIComponent(redirectParam);
      }
      return undefined;
    },
    startRedirect() {
      this.redirectParameter = this.getDecodedRedirectParam();
      const redirectEnum = get(REDIRECT_PAGE_PARAMETER)(this.$route.query);

      if (redirectEnum) {
        const pathName = findByRedirectEnum(redirectEnum);

        if (pathName) {
          redirectByName(this, pathName);
          return;
        }
      }

      if (this.redirectParameter) {
        this.handleRedirectParameter(this.redirectParameter);
        return;
      }

      redirectTo(this, INDEX_PATH);
    },
    async getAssertedLoginIdentityAndNavigate() {
      return this.$store.app.$http
        .postV1PatientAssertedLoginIdentity({
          assertedLoginIdentityRequest: {
            IntendedRelyingPartyUrl: this.redirectParameter,
            ProviderId: this.thirdPartyServiceContent.serviceId,
            ProviderName: this.thirdPartyServiceContent.providerName,
            JumpOffId: this.jumpOffId,
            Action: 'SilverIntegrationJumpOff',
          },
          ignoreError: true,
        })
        .then((response) => {
          this.navigateToExternalPath(
            this.appendAssertedLoginIdentity(this.redirectParameter, response),
          );
        })
        .catch((error) => {
          this.$store.dispatch('log/onError', `Failed to get AssertedLoginIdentity: ${JSON.stringify(error)}`);
          redirectTo(this, INDEX_PATH);
        });
    },
    appendAssertedLoginIdentity(uri, response) {
      if (uri.indexOf('?') !== -1) {
        return `${uri}&assertedLoginIdentity=${response.token}`;
      }
      return `${uri}?assertedLoginIdentity=${response.token}`;
    },
    getText(key) {
      return this.$te(key) ? this.$t(key) : '';
    },
    navigateToExternalPath(url) {
      if (NativeApp.supportsNativeWebIntegration()) {
        NativeApp.openWebIntegration(
          url,
          !isEmpty(this.matchedService.domains) ? this.matchedService.domains : [],
          this.currentHelpUrl,
        );
      } else {
        setWindowLocation(url);
      }
    },
    logSilverIntegrationJumpOffBlockedMetrics(reason) {
      this.$store.app.$http.postV1ApiUsersMeSilverIntegrationBlockedMetrics({
        silverIntegrationJumpOffBlockedData: {
          ProviderId: this.thirdPartyServiceContent.serviceId,
          ProviderName: this.thirdPartyServiceContent.providerName,
          JumpOffId: this.jumpOffId,
          Reason: reason,
        },
        ignoreError: true,
      });
    },
    canAccessSilverIntegration(store, { provider, serviceType }) {
      return sjrIf({
        $store: store,
        journey: 'silverIntegration',
        context: {
          provider,
          serviceType,
        },
      });
    },
    hasWayfinder() {
      return sjrIf({ $store: this.$store, journey: 'wayfinder' });
    },
    navigateToThirdParty({ url }) {
      this.sessionStorageName = `agreedThirdPartyWarning_${this.matchedService.id}`;

      if (this.matchedService.showThirdPartyWarning === false ||
          agreedToThirdPartyWarning(this.sessionStorageName)) {
        if (this.matchedService.requiresAssertedLoginIdentity || {} === true) {
          this.getAssertedLoginIdentityAndNavigate();
        } else {
          this.navigateToExternalPath(url);
        }
        return;
      }
      this.showWarning = true;
    },
    checkProofLevel({ jumpOffConfig, featureJumpOffContent }, next) {
      if (this.$store.getters['session/isProofLevel9'] || jumpOffConfig.proofLevel === proofLevel.P5) {
        next();
        return;
      }
      this.logSilverIntegrationJumpOffBlockedMetrics('Proof');
      redirectTo(
        this,
        UPLIFT_SILVER_INTEGRATION_PATH,
        { featureName: featureJumpOffContent.jumpOffContent.headerText },
      );
    },
    checkCanAccessSilverIntegration({ jumpOffConfig, featureJumpOffContent }, next) {
      if (this.isWayfinderUrl) {
        next();
        return;
      }
      if (this.canAccessSilverIntegration(this.$store, jumpOffConfig)) {
        next();
        return;
      }
      this.logSilverIntegrationJumpOffBlockedMetrics('SJR');
      redirectTo(
        this,
        SILVER_INTEGRATION_FEATURE_NOT_AVAILABLE_PATH,
        { featureName: featureJumpOffContent.jumpOffContent.headerText },
      );
    },
    updateTitle({ featureJumpOffContent }, next) {
      if (!isEmpty(featureJumpOffContent)) {
        if (this.isWayfinderUrl) {
          next();
          return;
        }

        const { jumpOffContent, thirdPartyWarning } = featureJumpOffContent;
        if (!isEmpty(thirdPartyWarning)) {
          this.$route.meta.titleKey = featureJumpOffContent.thirdPartyWarning.featureName;
        } else if (!isEmpty(jumpOffContent)) {
          this.$route.meta.titleKey = featureJumpOffContent.jumpOffContent.headerText;
        }
      }

      next();
    },
    getJumpOffConfig({ url, thirdPartyConfig }, next) {
      const redirectParameterAndQuery = getPathAndQuery(url);
      let jumpOffConfig;

      if (this.isWayfinderUrl) {
        jumpOffConfig = thirdPartyConfig;
      } else {
        jumpOffConfig = getThirdPartyJumpOff(
          { jumpOffs: Object.values(thirdPartyConfig) },
          redirectParameterAndQuery,
        );
      }

      this.jumpOffId = jumpOffConfig.jumpOffId;
      if (this.jumpOffId === undefined) {
        redirectTo(this, INDEX_PATH);
        return;
      }

      let featureJumpOffContent = {};
      if (this.isWayfinderUrl) {
        this.thirdPartyServiceContent = this.getText(`thirdPartyProviders.wayfinder.wayfinderJumpOffs.${this.matchedService.id}`);
        featureJumpOffContent = this.thirdPartyServiceContent;
      } else {
        this.thirdPartyServiceContent = this.getText(`thirdPartyProviders.${this.matchedService.id}`);

        featureJumpOffContent = this.thirdPartyServiceContent.jumpOffs.find(
          item => item.id === this.jumpOffId,
        );
      }
      next({ jumpOffConfig, featureJumpOffContent });
    },
    getThirdPartyConfig({ url }, next) {
      let thirdPartyConfig;
      if (this.isWayfinderUrl) {
        thirdPartyConfig = getWayfinderJumpOffConfiguration(this.matchedService.id);
      } else {
        thirdPartyConfig = getJumpOffConfiguration(this.matchedService.id);
      }

      if (thirdPartyConfig) {
        next({ url, thirdPartyConfig });
        return;
      }

      if (this.matchedService.showThirdPartyWarning === false &&
          this.matchedService.requiresAssertedLoginIdentity === false) {
        this.navigateToExternalPath(url);
        return;
      }

      redirectTo(this, INDEX_PATH);
    },
    handleInternalRedirect(url) {
      if (isNhsAppPath(url, this.$router)) {
        redirectTo(this, url);
        return;
      }

      redirectTo(this, INDEX_PATH);
    },
    getWayfinderProvider(url) {
      const pathAndQuery = getPathAndQuery(decodeURIComponent(url));
      const wayfinderConfig = getJumpOffConfiguration('wayfinder');
      const wayfinderProvider = wayfinderConfig[this.matchedService.id];

      if (!wayfinderProvider) {
        return null;
      }

      const pathRegex = wayfinderProvider.acceptablePathsRegex;
      if (pathRegex && pathAndQuery.match(new RegExp(pathRegex))) {
        return wayfinderProvider;
      }
      return null;
    },
    handleExternalRedirect(url) {
      this.matchedService = this.$store.getters['knownServices/matchOneByUrl'](url) || {};
      if (isEmpty(this.matchedService)) {
        redirectTo(this, INDEX_PATH);
        return;
      }

      let context = { url };

      if (this.getWayfinderProvider(url) && this.hasWayfinder()) {
        this.isWayfinderUrl = true;
      }

      const steps = [
        this.getThirdPartyConfig,
        this.getJumpOffConfig,
        this.updateTitle,
        this.checkProofLevel,
        this.checkCanAccessSilverIntegration,
        this.navigateToThirdParty,
      ];

      const executeStep = (index) => {
        if (index < steps.length) {
          steps[index](context, (values) => {
            context = { ...context, ...values };
            executeStep(index + 1);
          });
        }
      };

      executeStep(0);
    },
    handleRedirectParameter(redirectParameter) {
      if (isNhsAppRouteName(redirectParameter)) {
        redirectByName(this, redirectParameter);
        return;
      }

      const path = this.resolvePatientUrl(redirectParameter);
      if (isNhsAppHost(redirectParameter) || isNhsAppPath(path, this.$router)) {
        this.handleInternalRedirect(path);
        return;
      }

      this.handleExternalRedirect(redirectParameter);
    },
    resolvePatientUrl(url) {
      const trimmedPath = removeNhsAppHost(url);
      return pathWithPatientPrefixOrUndefined({
        store: this.$store,
        path: trimmedPath,
        router: this.$router,
      })
      || trimmedPath;
    },
  },
};
</script>

<style module lang="scss">
  @import "@/style/custom/redirector";
</style>
