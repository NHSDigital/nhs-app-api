<template>
  <generic-button :class="[$style.button, $style.green]" data-id="login-button"
                  @click="loginClicked" >
    {{ $t('loginButton.login') }}
  </generic-button>
</template>

<script>

import GenericButton from '@/components/widgets/GenericButton';

export default {
  components: {
    GenericButton,
  },
  methods: {
    async loginClicked() {
      if (process.client) {
        this.$store.dispatch('analytics/satelliteTrack', 'login');
      }
    },
  },
};
</script>
<style module lang="scss" scoped>
  @import "../style/buttons";
</style>
