<template>
  <generic-button :class="[$style.button, $style.green, isButtonDisabled ? $style.disabled : '']"
                  :disabled="isButtonDisabled"
                  data-id="login-button"
                  @click="loginClicked">
    {{ $t('loginButton.login') }}
  </generic-button>
</template>

<script>

import GenericButton from '@/components/widgets/GenericButton';
import Sources from '@/lib/sources';

export default {
  components: {
    GenericButton,
  },
  data() {
    return {
      isButtonDisabled: false,
    };
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
  },
};
</script>

<style module lang="scss" scoped>
  @import "../style/buttons";
</style>
