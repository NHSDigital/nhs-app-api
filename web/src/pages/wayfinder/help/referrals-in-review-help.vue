<template>
  <div>
    <h2 id="somethingIsMissingTitle">
      {{ $t('wayfinder.wayfinderHelp.referralsInReviewHelp.somethingIsMissingTitle') }}
    </h2>
    <p id="somethingIsMissingText1">
      {{ $t('wayfinder.wayfinderHelp.referralsInReviewHelp.somethingIsMissingText1') }}
    </p>
    <h2 id="cancellationNotShowingTitle">
      {{ $t('wayfinder.wayfinderHelp.referralsInReviewHelp.cancellationNotShowingTitle') }}
    </h2>
    <p id="cancellationNotShowingText">
      {{ $t('wayfinder.wayfinderHelp.referralsInReviewHelp.cancellationNotShowingText') }}
    </p>
    <desktop-generic-back-link v-if="!isNativeApp"
                               id="desktopBackLink"
                               data-purpose="back-to-wayfinder-button"
                               :path="wayfinderPath"
                               :button-text="'generic.back'"/>
  </div>
</template>

<script>
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import { WAYFINDER_PATH } from '@/router/paths';

export default {
  name: 'ReferralsInReviewHelp',
  components: {
    DesktopGenericBackLink,
  },
  data() {
    return {
      wayfinderPath: WAYFINDER_PATH,
    };
  },
  computed: {
    isNativeApp() {
      return this.$store.state.device.isNativeApp;
    },
  },
};
</script>
