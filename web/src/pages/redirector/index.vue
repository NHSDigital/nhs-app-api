<template>
  <div v-if="showTemplate">
    <div v-if="shouldShowWarning">
      <h1>{{ featureName() }}</h1>
      <div :class="$style['app-interruption']" data-purpose="silver-integration-warning">
        <div :class="$style['app-interruption__content']">
          <h2>{{ $t('thirdPartyProviders.warningConjunctions.heading2') }}</h2>
          <p>{{ paragraphText() }}
            <strong>{{ providerName() }}</strong>.
          </p>
          <button class="nhsuk-button nhsuk-button--reverse"
                  @click.prevent="getAssertedLoginIdentityAndNavigate">
            {{ $t('thirdPartyProviders.warningConjunctions.button') }}</button>
          <p><a :href="linkHref()" target="_blank">{{ linkText() }}</a></p>
        </div>
      </div>
    </div>
    <div v-else :class="$style['spacer']" />
  </div>
</template>

<script>
import get from 'lodash/fp/get';
import { UriBuilder } from 'uribuilder';
import { REDIRECT_PARAMETER, INDEX, findByName, findByPath } from '@/lib/routes';
import { getThirdPartyLocaleText } from '@/lib/utils';

export default {
  layout: 'nhsuk-layout-chromeless',
  data() {
    return {
      showWarning: false,
      services: [],
      redirectPath: '',
      paragraphTextData: '',
      linkTextData: '',
      featureNameData: '',
    };
  },
  computed: {
    shouldShowWarning() {
      return this.showWarning;
    },
  },
  async mounted() {
    this.redirectPath = get(REDIRECT_PARAMETER)(this.$route.query);

    if (this.redirectPath === undefined || this.redirectPath === '') {
      this.$router.push(INDEX.path);
      return;
    }

    if (findByName(this.redirectPath)) {
      this.$router.push(findByName(this.redirectPath).path);
      return;
    }

    if (findByPath(this.redirectPath)) {
      this.$router.push(findByPath(this.redirectPath).path);
      return;
    }

    this.services = await this.$store.state.knownServices.knownServices
      .filter(service => this.redirectPath.includes(service.url));
    if (this.services.length > 0 &&
      (this.services[0].requiresAssertedLoginIdentity || {}) === true) {
      if (this.services[0].showThirdPartyWarning === false) {
        this.getAssertedLoginIdentityAndNavigate();
        return;
      }

      const featureName = this.getMessage('featureName');
      if (featureName !== '') {
        this.showWarning = true;
        this.$store.dispatch('pageTitle/updatePageTitle', featureName);
        return;
      }
    }
    this.$router.push(INDEX.path);
  },
  methods: {
    async getAssertedLoginIdentityAndNavigate() {
      await this.$store.app.$http
        .postV1PatientAssertedLoginIdentity({
          assertedLoginIdentityRequest: {
            IntendedRelyingPartyUrl: this.redirectPath,
          },
        })
        .then((response) => {
          window.location = this.appendAssertedLoginIdentity(this.redirectPath, response);
        })
        .catch(() => {
          this.$router.push(INDEX.path);
        });
    },
    appendAssertedLoginIdentity(uri, response) {
      const builder = UriBuilder.parse(uri);
      builder.query.assertedLoginIdentity = response.token;
      return builder.toString();
    },
    paragraphText() {
      if (this.paragraphTextData === '') {
        const paragraph = this.showWarning ? this.getWarningMessage('paragraph') : '';
        this.paragraphTextData = paragraph.replace('{{ servicePurchaser }}', this.servicePurchaser())
          .replace('{{ serviceType }}', this.serviceType());
      }
      return this.paragraphTextData;
    },
    linkText() {
      if (this.linkTextData === '') {
        const paragraph = this.showWarning ? this.getWarningMessage('linkText') : '';
        this.linkTextData = paragraph.replace('{{ serviceTypePlural }}', this.serviceTypePlural());
      }
      return this.linkTextData;
    },
    featureName() {
      if (this.featureNameData === '') {
        this.featureNameData = this.showWarning ? this.getMessage('featureName') : '';
      }
      return this.featureNameData;
    },
    servicePurchaser() {
      return this.showWarning ? this.getMessage('servicePurchaser') : '';
    },
    serviceType() {
      return this.showWarning ? this.getMessage('serviceType') : '';
    },
    serviceTypePlural() {
      return this.showWarning ? this.getMessage('serviceTypePlural') : '';
    },
    linkHref() {
      return this.showWarning ? this.getMessage('linkHref') : '';
    },
    providerName() {
      return this.showWarning ? this.getMessage('providerName') : '';
    },
    getWarningMessage(type) {
      if (this.services[0]) {
        return this.getText(`thirdPartyProviders.warningConjunctions.${type}`);
      }
      return '';
    },
    getMessage(property) {
      if (this.services[0]) {
        if (property === 'providerName') {
          return this.getText(`thirdPartyProviders.${this.services[0].id}.${property}`);
        }
        const thirdPartyContent = this.getText(`thirdPartyProviders.${this.services[0].id}`);
        const url = new URL(this.redirectPath);
        const pathAndQuery = url.pathname + url.search;

        return getThirdPartyLocaleText(thirdPartyContent, property, pathAndQuery, 'thirdPartyWarning');
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
@import '~nhsuk-frontend/packages/core/settings/colours';
@import '~nhsuk-frontend/packages/core/settings/globals';
@import '~nhsuk-frontend/packages/core/settings/spacing';
@import '~nhsuk-frontend/packages/core/tools/ifff';
@import '~nhsuk-frontend/packages/core/tools/sass-mq';
@import '~nhsuk-frontend/packages/core/tools/spacing';
@import "../../style/colours";

.spacer {
  min-height: 100vh;
}

.app-interruption {
  @include nhsuk-responsive-margin(4, "bottom");
  width: 100%;
  color: $color_nhsuk-white;
  background-color: $panel_blue;
  .app-interruption__content {
    @include nhsuk-responsive-padding(2);
    @include nhsuk-responsive-padding(4, "left");
    @include nhsuk-responsive-padding(4, "right");
    a {
      color: $color_nhsuk-white;
      text-decoration: underline;
      display: inline;
      &:visited {
        color: $color_nhsuk-white;
      }
      &:focus {
        color: $nhsuk-text-color;
      }
      &:hover {
        color: $nhsuk-text-color;
        text-decoration: none;
      }
    }
  }
}
</style>
