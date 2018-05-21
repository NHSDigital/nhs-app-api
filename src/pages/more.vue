<template>
  <div id="mainDiv">
    <main class="content">
      <a id="btn_organdonation" :href="organDonationUrl" class="button" target="_blank">
        {{ $t('more.organDonationButtonText') }}
      </a>
    </main>
  </div>
</template>

<script>
export default {
  data() {
    return {
      organDonationUrl: process.env.ORGAN_DONATION_URL,
    };
  },
};
</script>

<style lang="scss">
@import "../style/html";
@import "../style/elements";
@import "../style/buttons";
</style>
