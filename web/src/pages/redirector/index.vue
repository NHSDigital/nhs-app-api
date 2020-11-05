<template>
  <div v-if="showTemplate">
    <silver-integration-panel
      v-if="shouldShowWarning"
      :known-service="services[0]"
      :redirect-path="redirectParameter"
      :jump-off-id="jumpOffId"
      :session-storage-name="sessionStorageName"
      @click.stop.prevent="getAssertedLoginIdentityAndNavigate"
    />
    <div v-else :class="$style['spacer']" />
  </div>
</template>

<script>
import get from 'lodash/fp/get';
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
import NativeApp from '@/services/native-app';
import jumpOffProperties from '@/lib/third-party-providers/jump-off-configuration';

export default {
  name: 'InterstitialRedirectorPage',
  components: {
    SilverIntegrationPanel,
  },
  data() {
    return {
      showWarning: false,
      services: [],
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

    this.showThirdPartyWarningOrNavigate();
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
      if (NativeApp.supportsNativeWebIntegration()) {
        NativeApp.openWebIntegration(url);
      } else {
        window.location = url;
      }
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
    showThirdPartyWarningOrNavigate() {
      this.services = this.$store.state.knownServices.knownServices
        .filter(service => this.redirectParameter.includes(service.url));

      if (this.services.length > 0) {
        const matchedService = this.services[0];
        this.sessionStorageName = `agreedThirdPartyWarning_${matchedService.id}`;
        this.redirectParameterAndQuery = getPathAndQuery(this.redirectParameter);
        this.thirdPartyServiceContent = this.getText(`thirdPartyProviders.${matchedService.id}`);

        const thirdPartyConfig = get(`thirdPartyProvider.${matchedService.id}`)(jumpOffProperties);

        if (!thirdPartyConfig) {
          redirectTo(this, INDEX_PATH);
          return;
        }
        const jumpOffConfig = getThirdPartyJumpOff(
          { jumpOffs: Object.values(thirdPartyConfig) },
          this.redirectParameterAndQuery,
        );

        this.jumpOffId = jumpOffConfig.jumpOffId;
        if (this.jumpOffId === undefined) {
          redirectTo(this, INDEX_PATH);
          return;
        }

        if (matchedService.id !== 'silver-third-party-api-test') {
          const featureJumpOffContent = this.thirdPartyServiceContent.jumpOffs.find(
            item => item.id === jumpOffConfig.jumpOffId,
          );

          if (!this.$store.getters['session/isProofLevel9']) {
            redirectTo(
              this,
              UPLIFT_SILVER_INTEGRATION_PATH,
              { featureName: featureJumpOffContent.jumpOffContent.headerText },
            );
            return;
          }

          if (!this.canAccessSilverIntegration(this.$store, jumpOffConfig)) {
            redirectTo(
              this,
              SILVER_INTEGRATION_FEATURE_NOT_AVAILABLE_PATH,
              { featureName: featureJumpOffContent.jumpOffContent.headerText },
            );
            return;
          }
        }

        if (matchedService.showThirdPartyWarning === false ||
            agreedToThirdPartyWarning(this.sessionStorageName)) {
          if (this.services[0].requiresAssertedLoginIdentity || {} === true) {
            this.getAssertedLoginIdentityAndNavigate();
          } else {
            this.navigateToExternalPath(this.redirectParameter);
          }
          return;
        }

        this.showWarning = true;
        return;
      }

      redirectTo(this, INDEX_PATH);
    },
  },
};
</script>

<style module lang="scss">
.spacer {
  min-height: 100vh;
}
</style>
