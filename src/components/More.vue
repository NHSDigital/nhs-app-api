<template>
  <div>
    <a class="button" :href="organDonationUrl" id="btn_organdonation">
      {{ $t('more.organDonationButtonText') }}
    </a>
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
  @import '../style/fonts';
  @import '../style/textstyles';
  @import '../style/colours';
  @import '../style/buttons';
</style>

<style lang="scss" scoped>
  * {
    padding: 0;
    margin: 0;
  }

  div {
    margin: 15px 5px;
  }
</style>
