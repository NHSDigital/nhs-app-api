<template>
  <generic-button
    :class="[...dynamicStyle('button', 'login-button'), $style.green, buttonStateStyle]"
    :disabled="isButtonDisabled"
    data-id="login-button"
    @click="loginClicked">
    {{ $t('loginButton.login') }}
  </generic-button>
</template>

<script>

import GenericButton from '@/components/widgets/GenericButton';
import Sources from '@/lib/sources';
import { getDynamicStyle } from '@/lib/desktop-experience';

export default {
  components: {
    GenericButton,
  },
  data() {
    return {
      isButtonDisabled: false,
    };
  },
  computed: {
    buttonStateStyle() {
      return this.isButtonDisabled ? this.$style.disabled : '';
    },
  },
  methods: {
    async loginClicked() {
      if (process.client) {
        if (this.$store.state.device.source === Sources.Android) {
          // (Android only)
          // Disable login button on click.
          // Page should be refreshed onResume.
          this.isButtonDisabled = true;
        }
        this.$store.dispatch('analytics/satelliteTrack', 'login');
      }
    },
    dynamicStyle(...args) {
      return getDynamicStyle(this, args);
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import "../style/buttons";

  .account-menu-web {
    cursor: pointer
  }

  .login-button-desktop {
    padding-left: 2em !important;
    padding-right: 2em !important;
  }

</style>
