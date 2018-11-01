<template>
  <div class="content">
    <generic-button id="signout-button"
                    :class="[$style.button, $style.grey]" @click="signoutClicked">
      {{ $t('signOutButton.signOut') }}
    </generic-button>
  </div>
</template>

<script>

import GenericButton from '@/components/widgets/GenericButton';

function signoutClicked(event) {
  event.preventDefault();
  this.$store.dispatch('auth/logout');
}

export default {
  components: {
    GenericButton,
  },
  methods: {
    signoutClicked,
  },
};
</script>

<style module lang="scss" scoped>
  @import '../style/buttons';
</style>
