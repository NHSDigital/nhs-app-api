<template>
  <div id="mainDiv">
    <main class="content">
      <div class="info">
        <h5>{{ $t('home.loggedIn.welcome') }}</h5>
        <p>{{ $t('home.loggedIn.description') }}</p>
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
  mounted() {
    this.$store.dispatch('header/setTitle', this.$t('home.loggedIn.header.title'));
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
