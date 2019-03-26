<template>
  <div id="mainDiv" :class="[$style['no-padding'], 'pull-content']">
    <h2>{{ $t('organDonation.moreAboutOrgans.header') }}</h2>
    <div v-for="(content, index) in $t('organDonation.moreAboutOrgans.contentItems')"
         :key="index"
         :class="$style.info">
      <h3>{{ content.subheader }}</h3>
      <p>{{ content.body }}</p>
    </div>
    <back-button :class="$style['mt-3']"/>
  </div>
</template>

<script>
import BackButton from '@/components/BackButton';

export default {
  components: {
    BackButton,
  },
};
</script>

<style module lang="scss" scoped>
  @import "../../style/info";
  @import "../../style/spacings";
</style>
