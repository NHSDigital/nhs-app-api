<template>
  <div v-if="showTemplate">
    <silver-integration-panel
      v-if="shouldShowWarning"
      :known-service="services[0]"
      :redirect-path="redirectParameter"
      :session-storage-name="sessionStorageName"
      @click.stop.prevent="getAssertedLoginIdentityAndNavigate"
    />
    <div v-else :class="$style['spacer']" />
  </div>
</template>

<script>
import get from 'lodash/fp/get';
import agreedToThirdPartyWarning from '@/lib/sessionStorage';
import SilverIntegrationPanel from '@/components/redirector/SilverIntegrationPanel';
import {
  REDIRECT_PARAMETER,
  REDIRECT_PAGE_PARAMETER,
  isNhsAppRouteName,
  findByRedirectEnum,
} from '@/router/names';
import {
  INDEX_PATH,
} from '@/router/paths';
import {
  getPathAndQuery,
  getThirdPartyJumpOff,
  redirectTo,
  redirectByName,
} from '@/lib/utils';
import NativeApp from '@/services/native-app';

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

    if (redirectEnum && findByRedirectEnum(redirectEnum)) {
      redirectByName(this, redirectEnum);
      return;
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

    this.services = this.$store.state.knownServices.knownServices
      .filter(service => this.redirectParameter.includes(service.url));

    if (this.services.length > 0) {
      const matchedService = this.services[0];
      this.sessionStorageName = `agreedThirdPartyWarning_${matchedService.id}`;
      this.redirectParameterAndQuery = getPathAndQuery(this.redirectParameter);
      this.thirdPartyServiceContent = this.getText(`thirdPartyProviders.${matchedService.id}`);
      this.JumpOffId =
        getThirdPartyJumpOff(this.thirdPartyServiceContent, this.redirectParameterAndQuery).id;

      if (this.JumpOffId === undefined) {
        redirectTo(this, INDEX_PATH);
        return;
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
  methods: {
    async getAssertedLoginIdentityAndNavigate() {
      return this.$store.app.$http
        .postV1PatientAssertedLoginIdentity({
          assertedLoginIdentityRequest: {
            IntendedRelyingPartyUrl: this.redirectParameter,
            ProviderId: this.thirdPartyServiceContent.serviceId,
            ProviderName: this.thirdPartyServiceContent.providerName,
            JumpOffId: this.JumpOffId,
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
  },
};
</script>

<style module lang="scss">
.spacer {
  min-height: 100vh;
}
</style>
