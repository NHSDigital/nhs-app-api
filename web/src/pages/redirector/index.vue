<template>
  <div v-if="showTemplate">
    <silver-integration-panel
      v-if="shouldShowWarning"
      :known-service="services[0]"
      :redirect-path="redirectPath"
      :session-storage-name="sessionStorageName"
      @click.stop.prevent="getAssertedLoginIdentityAndNavigate"
    />
    <div v-else :class="$style['spacer']" />
  </div>
</template>

<script>
import consola from 'consola';
import get from 'lodash/fp/get';
import agreedToThirdPartyWarning from '@/lib/sessionStorage';
import SilverIntegrationPanel from '@/components/redirector/SilverIntegrationPanel';
import NativeApp from '@/services/native-app';
import { REDIRECT_PAGE_PARAMETER, REDIRECT_PARAMETER, INDEX, findByName, findByPage, findByPath } from '@/lib/routes';
import { getPathAndQuery, getThirdPartyJumpOff } from '@/lib/utils';

export default {
  components: {
    SilverIntegrationPanel,
  },
  layout(context) {
    const redirectPath = get(REDIRECT_PARAMETER)(context.route.query);

    if (redirectPath) {
      const services = context.store.state.knownServices.knownServices
        .filter(service => redirectPath.includes(service.url));

      if (services.length > 0 && services[0].showThirdPartyWarning === true) {
        return 'nhsuk-layout';
      }
    }
    return 'nhsuk-layout-chromeless';
  },
  data() {
    return {
      showWarning: false,
      services: [],
      redirectPath: '',
      redirectPathAndQuery: '',
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
  async mounted() {
    this.redirectPath = get(REDIRECT_PARAMETER)(this.$route.query);
    const redirectPage = get(REDIRECT_PAGE_PARAMETER)(this.$route.query);

    let route;

    if (this.redirectPath) {
      route = findByName(this.redirectPath) || findByPath(this.redirectPath);
    } else if (redirectPage) {
      route = findByPage(redirectPage) || INDEX;
    } else {
      route = INDEX;
    }

    if (route) {
      this.$router.push(route.path);
      return;
    }

    this.services = await this.$store.state.knownServices.knownServices
      .filter(service => this.redirectPath.includes(service.url));

    if (this.services.length > 0) {
      const matchedService = this.services[0];
      this.sessionStorageName = `agreedThirdPartyWarning_${matchedService.id}`;
      this.redirectPathAndQuery = getPathAndQuery(this.redirectPath);
      this.thirdPartyServiceContent = this.getText(`thirdPartyProviders.${matchedService.id}`);
      this.JumpOffId =
        getThirdPartyJumpOff(this.thirdPartyServiceContent, this.redirectPathAndQuery).id;

      if (this.JumpOffId === undefined) {
        this.$router.push(INDEX.path);
        return;
      }

      if (this.services[0].showThirdPartyWarning === false ||
          agreedToThirdPartyWarning(this.sessionStorageName)) {
        if (this.services[0].requiresAssertedLoginIdentity || {} === true) {
          this.getAssertedLoginIdentityAndNavigate();
        } else {
          this.navigateToExternalPath(this.redirectPath);
        }
        return;
      }

      this.showWarning = true;
      return;
    }
    this.$router.push(INDEX.path);
  },
  methods: {
    async getAssertedLoginIdentityAndNavigate() {
      return this.$store.app.$http
        .postV1PatientAssertedLoginIdentity({
          assertedLoginIdentityRequest: {
            IntendedRelyingPartyUrl: this.redirectPath,
            ProviderId: this.thirdPartyServiceContent.serviceId,
            ProviderName: this.thirdPartyServiceContent.providerName,
            JumpOffId: this.JumpOffId,
          },
          ignoreError: true,
        })
        .then((response) => {
          this.navigateToExternalPath(
            this.appendAssertedLoginIdentity(this.redirectPath, response),
          );
        })
        .catch((error) => {
          consola.error(new Error(`Failed to get AssertedLoginIdentity: ${error.response.status}`));
          this.$router.push(INDEX.path);
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
    navigateToExternalPath(path) {
      if (NativeApp.supportsNativeNavigation()) {
        NativeApp.navigateToThirdParty(path);
      } else {
        window.location = path;
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
