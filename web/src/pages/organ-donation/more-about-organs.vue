<template>
  <div id="mainDiv" class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full">
      <h2>{{ $t('organDonation.moreAboutOrgans.aboutOrgansAndTissue') }}</h2>
      <div v-for="(content, index) in $t('organDonation.moreAboutOrgans.contentItems')"
           :key="index">
        <h3>{{ content.subheader }}</h3>
        <p>{{ content.body }}</p>
      </div>
      <back-button v-if="!$store.state.device.isNativeApp"/>
    </div>
  </div>
</template>

<script>
import BackButton from '@/components/BackButton';
import NativeOnlyMixin from '@/components/NativeOnlyMixin';

export default {
  components: {
    BackButton,
  },
  mixins: [NativeOnlyMixin],
};
</script>
