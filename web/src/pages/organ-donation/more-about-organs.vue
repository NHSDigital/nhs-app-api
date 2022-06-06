<template>
  <div id="mainDiv" class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full">
      <h2>{{ $t('organDonation.moreAboutOrgans.aboutOrgansAndTissue') }}</h2>
      <div v-for="(content, index) in $t('organDonation.moreAboutOrgans.contentItems')"
           :key="index">
        <h3>{{ content.subheader }}</h3>
        <p>{{ content.body }}</p>
      </div>
      <desktop-generic-back-link v-if="!$store.state.device.isNativeApp"
                                 :path="backLink"
                                 :button-text="'generic.back'"
                                 @clickAndPrevent="backClicked"/>
    </div>
  </div>
</template>

<script>
import NativeOnlyMixin from '@/components/NativeOnlyMixin';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import DynamicBackLinkMixin from '@/components/organ-donation/DynamicBackLinkMixin';

export default {
  components: {
    DesktopGenericBackLink,
  },
  mixins: [NativeOnlyMixin, DynamicBackLinkMixin],
};
</script>
