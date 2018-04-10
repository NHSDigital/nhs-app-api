<template>
  <div id="mainDiv">
    <main class="content">
    <a class="button" :href="organDonationUrl" id="btn_organdonation">
      {{ $t('more.organDonationButtonText') }}
    </a>
    </main>
  </div>
</template>

<script>
export default {
  data() {
    return {
      organDonationUrl: this.$config.ORGAN_DONATION_URL,
    };
  },
};
</script>

<style lang="scss">
  @import '../style/html';
  @import '../style/textstyles';
  @import '../style/elements';
  @import '../style/fonts';
  @import '../style/colours';
  @import '../style/buttons';
</style>
