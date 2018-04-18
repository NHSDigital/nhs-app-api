<template>
  <div id="mainDiv">
    <main class="content">
      <div class="info">
        <h5>{{ $t('homeLoggedIn.welcome') }}</h5>
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
