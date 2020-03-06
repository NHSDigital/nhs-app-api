<template>
  <menu-item id="btn_jumpoff"
             header-tag="h2"
             data-purpose="text_link"
             :href="redirectUrl()"
             :text="headerText()"
             :target="target"
             :description="descriptionText()"
             :aria-label="headerText |
               join(descriptionText ,'. ')"/>
</template>

<script>
import MenuItem from '@/components/MenuItem';
import { redirectTo, getThirdPartyLocaleText } from '@/lib/utils';
import {
  INTERSTITIAL_REDIRECTOR,
} from '@/lib/routes';

export default {
  name: 'ThirdPartyJumpOffButton',
  components: {
    MenuItem,
  },
  props: {
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
      services: [],
      headerTextData: '',
      descriptionTextData: '',
      target: !this.$store.state.device.isNativeApp ? '_blank' : '',
    };
  },
  methods: {
    navigate(event) {
      redirectTo(this, this.redirectUrl());
      event.preventDefault();
    },
    redirectUrl() {
      this.services = this.$store.state.knownServices.knownServices
        .filter(service => this.providerId.includes(service.id));
      const encodedUri = encodeURIComponent(this.services[0].url);
      return `${this.redirectorPath}?redirect_to=${encodedUri}${this.redirectPath}`;
    },
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
