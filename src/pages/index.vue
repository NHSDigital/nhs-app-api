<template>
  <div id="mainDiv">
    <main class="content">
      <div class="info">
        <h5>{{ greetingMessage }}</h5>
        <p>{{ $t('homeLoggedIn.description') }}</p>
      </div>
    </main>
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import PatientDetailsIcon from '@/components/icons/PatientDetailsIcon';

export default {
  middleware: ['auth', 'meta'],
  components: {
    PatientDetailsIcon,
  },
  computed: {
    greetingMessage() {
      const message = this.$t('homeLoggedIn.welcome');
      const { givenName, familyName } = this.$store.state.auth.user;
      return `${message}, ${givenName} ${familyName}`;
    },
  },
};
</script>

<style lang="scss">
@import "../style/html";
@import "../style/textstyles";
@import "../style/elements";

#mainDiv {
  padding: 0;
  @include default_text;
}
</style>
