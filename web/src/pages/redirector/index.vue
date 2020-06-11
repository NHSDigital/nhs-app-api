<template>
  <div v-if="showTemplate">
    <warning-content-panel v-if="shouldShowWarning" data-purpose="silver-integration-warning">
      <template slot="header">
        {{ $t('thirdPartyProviders.warningConjunctions.heading2') }}
      </template>
      <template>
        <p>{{ paragraphText() }}
          <strong>{{ providerName() }}</strong>.
        </p>

        <a :href="buttonHref"
           :class="['nhsuk-button', 'nhsuk-button--reverse',
                    buttonDisabled ? 'nhsuk-button--disabled': '']"
           @click="buttonClick">
          {{ $t('thirdPartyProviders.warningConjunctions.button') }}
        </a>

        <p>
          <a class="inline-link" :href="linkHref()"
             target="_blank" rel="noopener noreferrer">
            {{ linkText() }}
          </a>
        </p>
      </template>
    </warning-content-panel>
    <div v-else :class="$style['spacer']" />
  </div>
</template>

<script>
import get from 'lodash/fp/get';
import agreedToThirdPartyWarning from '@/lib/sessionStorage';
import WarningContentPanel from '@/components/widgets/WarningContentPanel';
import { REDIRECT_PAGE_PARAMETER, REDIRECT_PARAMETER, INDEX, findByName, findByPage, findByPath } from '@/lib/routes';
import { getPathAndQuery, getThirdPartyJumpOff, getThirdPartyLocaleText } from '@/lib/utils';

export default {
  components: {
    WarningContentPanel,
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
      this.sessionStorageName = `agreedThirdPartyWarning_${this.services[0].id}`;
      this.redirectPathAndQuery = getPathAndQuery(this.redirectPath);
      this.thirdPartyServiceContent = this.getText(`thirdPartyProviders.${this.services[0].id}`);
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
          window.location = this.redirectPath;
        }
        return;
      }

      const featureName = this.getWarningMessage('featureName');
      this.setButtonHref();
      this.showWarning = true;
      this.$store.dispatch('pageTitle/updatePageTitle', featureName);
      this.$store.dispatch('header/updateHeaderText', featureName);
      return;
    }
    this.$router.push(INDEX.path);
  },
  methods: {
    async postPatientAssertedLoginIdentity() {
      return this.$store.app.$http
        .postV1PatientAssertedLoginIdentity({
          assertedLoginIdentityRequest: {
            IntendedRelyingPartyUrl: this.redirectPath,
            ProviderId: this.thirdPartyServiceContent.serviceId,
            ProviderName: this.thirdPartyServiceContent.providerName,
            JumpOffId: this.JumpOffId,
          },
        })
        .then(response => this.appendAssertedLoginIdentity(this.redirectPath, response));
    },
    async setButtonHref() {
      if (this.services[0].requiresAssertedLoginIdentity || {} === true) {
        await this.postPatientAssertedLoginIdentity()
          .then((response) => {
            this.buttonHref = response;
          });
      } else {
        this.buttonHref = this.redirectPath;
      }
    },
    async getAssertedLoginIdentityAndNavigate() {
      await this.postPatientAssertedLoginIdentity()
        .then((response) => {
          window.location = response;
        })
        .catch(() => {
          this.$router.push(INDEX.path);
        });
    },
    buttonClick(event) {
      if (this.buttonDisabled) {
        event.preventDefault();
      } else {
        this.buttonDisabled = true;
        sessionStorage.setItem(this.sessionStorageName, true);
      }
    },
    appendAssertedLoginIdentity(uri, response) {
      if (uri.indexOf('?') !== -1) {
        return `${uri}&assertedLoginIdentity=${response.token}`;
      }
      return `${uri}?assertedLoginIdentity=${response.token}`;
    },
    paragraphText() {
      if (this.showWarning) {
        const paragraph = this.getWarningConjunction('paragraph');
        return paragraph
          .replace('{{ servicePurchaser }}', this.servicePurchaser())
          .replace('{{ serviceType }}', this.serviceType());
      }
      return '';
    },
    linkText() {
      if (this.showWarning) {
        const paragraph = this.getWarningConjunction('linkText');
        return paragraph.replace('{{ serviceTypePlural }}', this.serviceTypePlural());
      }
      return '';
    },
    servicePurchaser() {
      return this.showWarning ? this.getWarningMessage('servicePurchaser') : '';
    },
    serviceType() {
      return this.showWarning ? this.getWarningMessage('serviceType') : '';
    },
    serviceTypePlural() {
      return this.showWarning ? this.getWarningMessage('serviceTypePlural') : '';
    },
    linkHref() {
      return this.showWarning ? this.getWarningMessage('linkHref') : '';
    },
    providerName() {
      if (this.showWarning && this.services[0]) {
        if (this.getWarningMessage('brandName') !== undefined) {
          return this.getWarningMessage('brandName');
        }
        return this.getText(`thirdPartyProviders.${this.services[0].id}.providerName`);
      }
      return '';
    },
    getWarningConjunction(type) {
      if (this.services[0]) {
        return this.getText(`thirdPartyProviders.warningConjunctions.${type}`);
      }
      return '';
    },
    getWarningMessage(property) {
      if (this.services[0]) {
        return getThirdPartyLocaleText(this.thirdPartyServiceContent, this.redirectPathAndQuery, 'thirdPartyWarning', property);
      }
      return '';
    },
    getText(key) {
      return this.$te(key) ? this.$t(key) : '';
    },
  },
};
</script>

<style module lang="scss">
.spacer {
  min-height: 100vh;
}
</style>
