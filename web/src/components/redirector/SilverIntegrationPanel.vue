<template>
  <warning-content-panel data-purpose="silver-integration-warning">
    <template slot="header">
      {{ $t('thirdPartyProviders.warningConjunctions.heading2') }}
    </template>
    <template>
      <p class="nhsuk-body-m">{{ paragraphText() }}
        <strong>{{ providerName() }}</strong>.
      </p>

      <a :href="redirectPath"
         :class="['nhsuk-button', 'nhsuk-button--reverse',
                  buttonDisabled ? 'nhsuk-button--disabled': '']"
         @click.stop.prevent="buttonClick">
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
</template>

<script>
import WarningContentPanel from '@/components/widgets/WarningContentPanel';
import { getPathAndQuery, getThirdPartyLocaleText } from '@/lib/utils';
import { UPDATE_HEADER, UPDATE_TITLE, EventBus } from '@/services/event-bus';

export default {
  name: 'SilverIntegrationPanel',
  components: { WarningContentPanel },
  props: {
    knownService: {
      type: Object,
      required: true,
    },
    redirectPath: {
      type: String,
      required: true,
    },
    sessionStorageName: {
      type: String,
      required: true,
    },
  },
  data() {
    return {
      buttonDisabled: false,
      thirdPartyServiceContent: this.getText(`thirdPartyProviders.${this.knownService.id}`),
      redirectPathAndQuery: getPathAndQuery(this.redirectPath),
    };
  },
  async mounted() {
    const featureName = this.getWarningMessage('featureName');

    EventBus.$emit(UPDATE_HEADER, featureName, true);
    EventBus.$emit(UPDATE_TITLE, featureName, true);
  },
  methods: {
    buttonClick(event) {
      if (!this.buttonDisabled) {
        this.buttonDisabled = true;
        sessionStorage.setItem(this.sessionStorageName, true);
        this.$emit('click', event);
      }
    },
    getText(key) {
      return this.$te(key) ? this.$t(key) : '';
    },
    getWarningConjunction(type) {
      if (this.knownService) {
        return this.getText(`thirdPartyProviders.warningConjunctions.${type}`);
      }
      return '';
    },
    getWarningMessage(property) {
      if (this.knownService) {
        return getThirdPartyLocaleText(this.thirdPartyServiceContent, this.redirectPathAndQuery, 'thirdPartyWarning', property);
      }
      return '';
    },
    linkHref() {
      return this.getWarningMessage('linkHref');
    },
    linkText() {
      const paragraph = this.getWarningConjunction('linkText');
      return paragraph.replace('{{ serviceTypePlural }}', this.serviceTypePlural());
    },
    paragraphText() {
      const paragraph = this.getWarningConjunction('paragraph');
      return paragraph
        .replace('{{ servicePurchaser }}', this.servicePurchaser())
        .replace('{{ serviceType }}', this.serviceType());
    },
    providerName() {
      if (this.knownService) {
        if (this.getWarningMessage('brandName') !== undefined) {
          return this.getWarningMessage('brandName');
        }
        return this.getText(`thirdPartyProviders.${this.knownService.id}.providerName`);
      }
      return '';
    },
    servicePurchaser() {
      return this.getWarningMessage('servicePurchaser');
    },
    serviceType() {
      return this.getWarningMessage('serviceType');
    },
    serviceTypePlural() {
      return this.getWarningMessage('serviceTypePlural');
    },
  },
};
</script>

