<template>
  <menu-item v-if="path"
             :id="id"
             header-tag="h2"
             data-purpose="text_link"
             :href="path"
             :target="!isNativeApp? '_blank': undefined"
             :click-func="onClick"
             :click-param="path"
             :prevent-default="isNativeApp"
             :text="headerText()"
             :description="descriptionText()"
             :aria-label="headerText() |
               join(descriptionText() ,'. ')" />
</template>

<script>
import MenuItem from '@/components/MenuItem';
import { getThirdPartyLocaleText, removePatientIdPrefixFromPath, isTruthy } from '@/lib/utils';
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
      const matchedService = this.$store.getters['knownServices/matchOneById'](this.providerId);

      if (!matchedService) {
        return '';
      }
      const encodedUri = encodeURIComponent(matchedService.url + this.redirectPath);
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
    onClick(path) {
      if (isTruthy(this.$store.$env.THIRD_PARTY_JUMP_OFF_LOGGING_ENABLED)) {
        this.$store.dispatch('log/onInfo', this.createLogMessage());
      }

      if (this.isNativeApp) {
        this.goToUrl(path);
      }
    },
    createLogMessage() {
      const { provider, jumpOffId } = this.providerConfiguration;
      const path = removePatientIdPrefixFromPath(this.$route.path);

      return `Jump-off from ${path} to ${provider} - ${jumpOffId}`;
    },
  },
};
</script>
