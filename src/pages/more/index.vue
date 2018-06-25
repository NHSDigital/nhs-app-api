<template>
  <div v-if="showTemplate" id="mainDiv">
    <main class="content">
      <div id="organdonantion" class="panel-title">
        <h2>{{ $t('more.organDonationLabel') }}</h2>
        <p>
          {{ $t('more.organDonationDesc') }}
        </p>
      </div>
      <a id="btn_organdonation" :href="organDonationUrl" class="button" target="_blank">
        {{ $t('more.organDonationButtonText') }}
      </a>
      <hr>
    </main>
  </div>
</template>

<script>
export default {
  middleware: ['auth', 'meta'],
  data() {
    return {
      organDonationUrl: process.env.ORGAN_DONATION_URL,
    };
  },
};

</script>

<style module lang="scss" scoped>
  @import "../../style/html";
  @import "../../style/fonts";
  @import "../../style/buttons";
</style>
