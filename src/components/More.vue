<template>
  <div id="mainDiv">
    <main class="content">
    <a class="button" :href="organDonationUrl" target="_blank" id="btn_organdonation">
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
  @import '../style/elements';
  @import '../style/buttons';
</style>
