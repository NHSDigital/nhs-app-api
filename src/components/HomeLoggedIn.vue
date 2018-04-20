<template>
  <div id="mainDiv">
    <main class="content">
      <div class="info">
        <h5>{{greetingMessage}}</h5>
        <p>{{ $t('homeLoggedIn.description') }}</p>
      </div>
    </main>
  </div>
</template>

<script>
import PatientDetailsIcon from '@/components/icons/PatientDetailsIcon';

export default {
  components: {
    PatientDetailsIcon,
  },
  computed: {
    greetingMessage() {
      const message = this.$t('homeLoggedIn.welcome');
      const givenName = this.$store.state.auth.user.givenName;
      const familyName = this.$store.state.auth.user.familyName;
      return `${message}, ${givenName} ${familyName}`;
    },
  },
};
</script>

<style lang="scss">
  @import '../style/html';
  @import '../style/textstyles';
  @import '../style/elements';
  @import '../style/fonts';
  @import '../style/colours';

  #mainDiv {
    padding: 0;
    @include default_text;
  }
</style>
