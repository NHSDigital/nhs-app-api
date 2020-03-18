<template>
  <menu-item :id="id"
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
import {
  INTERSTITIAL_REDIRECTOR,
  REDIRECT_PARAMETER,
} from '@/lib/routes';

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
    jumpOffType: {
      type: String,
      required: true,
    },
    providerId: {
      type: String,
      required: true,
    },
    redirectPath: {
      type: String,
      required: true,
    },
  },
  data() {
    return {
      redirectorPath: INTERSTITIAL_REDIRECTOR.path,
      headerTextData: '',
      descriptionTextData: '',
      isNativeApp: this.$store.state.device.isNativeApp,
    };
  },
  computed: {
    path() {
      const services = this.$store.state.knownServices.knownServices
        .filter(service => this.providerId.includes(service.id));
      const encodedUri = encodeURIComponent(services[0].url + this.redirectPath);
      return `${this.redirectorPath}?${REDIRECT_PARAMETER}=${encodedUri}`;
    },
  },
  methods: {
    headerText() {
      return this.getMessage('headerText');
    },
    descriptionText() {
      return this.getMessage('descriptionText');
    },
    getMessage(type) {
      const thirdPartyLocales = this.getText(`thirdPartyProviders.${this.providerId}`);
      return getThirdPartyLocaleText(thirdPartyLocales, type, this.redirectPath, 'jumpOffContent');
    },
    getText(key) {
      return this.$te(key) ? this.$t(key) : '';
    },
  },
};
</script>
