<template>
  <div>
    <h2 id="title1">
      {{ $t('wayfinder.wayfinderHelp.referralsInReviewHelp.title1') }}
    </h2>
    <p id="text1">
      {{ $t('wayfinder.wayfinderHelp.referralsInReviewHelp.text1') }}
    </p>
    <h2 id="title2">
      {{ $t('wayfinder.wayfinderHelp.referralsInReviewHelp.title2') }}
    </h2>
    <p id="text2">
      {{ $t('wayfinder.wayfinderHelp.referralsInReviewHelp.text2') }}
    </p>
    <p id="text3">
      {{ $t('wayfinder.wayfinderHelp.referralsInReviewHelp.text3') }}
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
