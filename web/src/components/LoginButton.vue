<template>
  <generic-button
    :class="[...dynamicStyle('button', 'login-button'), $style['nhs-login'], buttonStateStyle]"
    type="submit"
    data-id="login-button">
    {{ $t('loginButton.login') }}
  </generic-button>
</template>

<script>

import GenericButton from '@/components/widgets/GenericButton';
import { getDynamicStyle } from '@/lib/desktop-experience';

export default {
  name: 'LoginButton',
  components: {
    GenericButton,
  },
  computed: {
    buttonStateStyle() {
      return this.isButtonDisabled ? this.$style.disabled : '';
    },
  },
  methods: {
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
