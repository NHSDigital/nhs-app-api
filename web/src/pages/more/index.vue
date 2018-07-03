<template>
  <div v-if="showTemplate" id="mainDiv">
    <main :class="$style.content">
      <div id="organdonantion" :class="$style['panel-title']">
        <h2>{{ $t('more.organDonationLabel') }}</h2>
        <p>
          {{ $t('more.organDonationDesc') }}
        </p>
      </div>
      <a id="btn_organdonation" :href="organDonationUrl" :class="$style.button" target="_blank">
        {{ $t('more.organDonationButtonText') }}
      </a>
      <hr>
      <br>
      <div id="dataSharing" :class="$style['panel-title']">
        <h2>{{ $t('more.dataSharingHeaderText') }}</h2>
        <p>
          {{ $t('more.dataSharingInfoText') }}
        </p>
      </div>
      <a id="btn_datasharing" :href="dataSharingUrl" :class="$style.button" target="_blank">
        {{ $t('more.dataSharingButtonText') }}
      </a>
    </main>
  </div>
</template>

<script>
export default {
  middleware: ['auth', 'meta'],
  data() {
    return {
      organDonationUrl: process.env.ORGAN_DONATION_URL,
      dataSharingUrl: process.env.DATA_SHARING_URL,
    };
  },
};

</script>

<style module lang="scss">
  @import "../../style/html";
  @import "../../style/fonts";
  @import "../../style/buttons";
  @import "../../style/elements";
</style>
