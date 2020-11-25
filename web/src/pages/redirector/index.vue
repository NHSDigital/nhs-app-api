<template>
  <div v-if="showTemplate">
    <silver-integration-panel
      v-if="shouldShowWarning"
      :known-service="matchedService"
      :redirect-path="redirectParameter"
      :jump-off-id="jumpOffId"
      :session-storage-name="sessionStorageName"
      @click.stop.prevent="getAssertedLoginIdentityAndNavigate"/>
    <div v-else :class="$style['spacer']" />
  </div>
</template>

<script>
import { get, isEmpty } from 'lodash/fp';
import agreedToThirdPartyWarning from '@/lib/sessionStorage';
import sjrIf from '@/lib/sjrIf';
import SilverIntegrationPanel from '@/components/redirector/SilverIntegrationPanel';
import {
  REDIRECT_PARAMETER,
  REDIRECT_PAGE_PARAMETER,
  isNhsAppRouteName,
  findByRedirectEnum,
} from '@/router/names';
import {
  INDEX_PATH,
  UPLIFT_SILVER_INTEGRATION_PATH,
  SILVER_INTEGRATION_FEATURE_NOT_AVAILABLE_PATH,
} from '@/router/paths';
import {
  getPathAndQuery,
  getThirdPartyJumpOff,
  redirectTo,
  redirectByName,
} from '@/lib/utils';
import { getJumpOffConfiguration } from '@/lib/third-party-providers/jump-off-configuration';

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
      redirectParameterAndQuery: '',
      buttonHref: '',
      buttonDisabled: false,
      sessionStorageName: '',
      thirdPartyServiceContent: '',
      jumpOffId: null,
    };
  },
  computed: {
    shouldShowWarning() {
      return this.showWarning && !agreedToThirdPartyWarning(this.sessionStorageName);
    },
  },
  mounted() {
    this.redirectParameter = get(REDIRECT_PARAMETER)(this.$route.query);
    const redirectEnum = get(REDIRECT_PAGE_PARAMETER)(this.$route.query);

    let path;

    if (redirectEnum) {
      const pathName = findByRedirectEnum(redirectEnum);

      if (pathName) {
        redirectByName(this, pathName);
        return;
      }
    }
    if (this.redirectParameter) {
      if (isNhsAppRouteName(this.redirectParameter)) {
        redirectByName(this, this.redirectParameter);
        return;
      }
      if (this.$store.app.isNhsAppPath(this.redirectParameter)) {
        path = this.redirectParameter;
      }
    } else {
      path = INDEX_PATH;
    }

    if (path) {
      redirectTo(this, path);
      return;
    }

    this.handleExternalRedirect();
  },
  methods: {
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
          this.$store.dispatch('log/onError', `Failed to get AssertedLoginIdentity: ${error.response.status}`);
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
      window.location = url;
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
    navigateToThirdParty() {
      this.sessionStorageName = `agreedThirdPartyWarning_${this.matchedService.id}`;

      if (this.matchedService.showThirdPartyWarning === false ||
          agreedToThirdPartyWarning(this.sessionStorageName)) {
        if (this.matchedService.requiresAssertedLoginIdentity || {} === true) {
          this.getAssertedLoginIdentityAndNavigate();
        } else {
          this.navigateToExternalPath(this.redirectParameter);
        }
        return;
      }
      this.showWarning = true;
    },
    checkIsProofLevel9({ featureJumpOffContent }, next) {
      if (this.$store.getters['session/isProofLevel9']) {
        next();
        return;
      }

      redirectTo(
        this,
        UPLIFT_SILVER_INTEGRATION_PATH,
        { featureName: featureJumpOffContent.jumpOffContent.headerText },
      );
    },
    checkCanAccessSilverIntegration({ jumpOffConfig, featureJumpOffContent }, next) {
      if (this.canAccessSilverIntegration(this.$store, jumpOffConfig)) {
        next();
        return;
      }

      redirectTo(
        this,
        SILVER_INTEGRATION_FEATURE_NOT_AVAILABLE_PATH,
        { featureName: featureJumpOffContent.jumpOffContent.headerText },
      );
    },
    getJumpOffConfig({ thirdPartyConfig }, next) {
      this.redirectParameterAndQuery = getPathAndQuery(this.redirectParameter);
      const jumpOffConfig = getThirdPartyJumpOff(
        { jumpOffs: Object.values(thirdPartyConfig) },
        this.redirectParameterAndQuery,
      );

      this.jumpOffId = jumpOffConfig.jumpOffId;
      if (this.jumpOffId === undefined) {
        redirectTo(this, INDEX_PATH);
        return;
      }
      this.thirdPartyServiceContent = this.getText(`thirdPartyProviders.${this.matchedService.id}`);

      const featureJumpOffContent = this.thirdPartyServiceContent.jumpOffs.find(
        item => item.id === this.jumpOffId,
      );

      next({ jumpOffConfig, featureJumpOffContent });
    },
    getThirdPartyConfig(_, next) {
      const thirdPartyConfig = getJumpOffConfiguration(this.matchedService.id);

      if (thirdPartyConfig) {
        next({ thirdPartyConfig });
        return;
      }

      if (this.matchedService.showThirdPartyWarning === false &&
          this.matchedService.requiresAssertedLoginIdentity === false) {
        this.navigateToExternalPath(this.redirectParameter);
        return;
      }

      redirectTo(this, INDEX_PATH);
    },
    handleExternalRedirect() {
      this.matchedService = this.$store.getters['knownServices/matchOneByUrl'](this.redirectParameter) || {};

      if (isEmpty(this.matchedService)) {
        redirectTo(this, INDEX_PATH);
        return;
      }

      let context = {};

      const steps = [
        this.getThirdPartyConfig,
        this.getJumpOffConfig,
        this.checkIsProofLevel9,
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
  },
};
</script>

<style module lang="scss">
.spacer {
  min-height: 100vh;
}
</style>
