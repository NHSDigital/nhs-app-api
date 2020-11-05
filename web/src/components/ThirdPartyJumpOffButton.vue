<template>
  <menu-item v-if="path"
             :id="id"
             header-tag="h2"
             data-purpose="text_link"
             :href="path"
             :target="!isNativeApp? '_blank': undefined"
             :click-func="isNativeApp? goToUrl : undefined"
             :click-param="isNativeApp? path : undefined"
             :text="headerText()"
             :description="descriptionText()"
             :aria-label="headerText() |
               join(descriptionText() ,'. ')" />
</template>

<script>
import MenuItem from '@/components/MenuItem';
import { getThirdPartyLocaleText } from '@/lib/utils';
import { REDIRECT_PARAMETER } from '@/router/names';
import { INTERSTITIAL_REDIRECTOR_PATH } from '@/router/paths';

export default {
  name: 'ThirdPartyJumpOffButton',
  components: {
    MenuItem,
  },
  props: {
    id: {
      type: String,
      required: true,
    },
    providerConfiguration: {
      type: Object,
      required: true,
    },
    providerId: {
      type: String,
      required: true,
    },
  },
  data() {
    return {
      redirectorPath: INTERSTITIAL_REDIRECTOR_PATH,
      headerTextData: '',
      descriptionTextData: '',
      isNativeApp: this.$store.state.device.isNativeApp,
      jumpOffId: this.providerConfiguration.jumpOffId,
      redirectPath: this.providerConfiguration.redirectPath,
    };
  },
  computed: {
    path() {
      const services = this.$store.state.knownServices.knownServices
        .filter(service => this.providerId.includes(service.id));
      if (services.length === 0) {
        return '';
      }
      const encodedUri = encodeURIComponent(services[0].url + this.redirectPath);
      return `/${this.redirectorPath}?${REDIRECT_PARAMETER}=${encodedUri}`;
    },
  },
  methods: {
    headerText() {
      return this.getMessage('headerText');
    },
    descriptionText() {
      return this.getMessage('descriptionText');
    },
    getMessage(property) {
      const thirdPartyLocales = this.getText(`thirdPartyProviders.${this.providerId}`);
      return getThirdPartyLocaleText(thirdPartyLocales, this.jumpOffId, 'jumpOffContent', property);
    },
    getText(key) {
      return this.$te(key) ? this.$t(key) : '';
    },
  },
};
</script>
