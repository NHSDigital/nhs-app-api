<template>
  <div data-purpose="olc-unavailable">
    <p data-purpose="info">
      {{ $t('onlineConsultations.unavailable.paragraph') }}
    </p>
    <h2 class="nhsuk-heading-m nhsuk-u-padding-bottom-0" data-purpose="coronavirus-heading">
      {{ $t('onlineConsultations.unavailable.coronavirusHeading') }}
    </h2>
    <p data-purpose="coronavirus-info">
      {{ $t('onlineConsultations.unavailable.coronavirusParagraph') }}
    </p>
    <p>
      <a href="https://111.nhs.uk/service/COVID-19/"
         data-purpose="coronavirus-link"
         target="_blank"
         rel="noopener noreferrer">
        {{ $t('onlineConsultations.unavailable.coronavirusLink') }}
      </a>
    </p>
    <desktopGenericBackLink v-if="!isNativeApp"
                            data-purpose="back-link"
                            :path="backLink"
                            :button-text="$t('onlineConsultations.orchestrator.backButton')"
                            @clickAndPrevent="backClicked"/>
  </div>
</template>

<script>
import last from 'lodash/fp/last';
import { redirectTo } from '@/lib/utils';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';

export default {
  name: 'OnlineConsultationsUnavailable',
  components: {
    DesktopGenericBackLink,
  },
  computed: {
    isNativeApp() {
      return this.$store.state.device.isNativeApp;
    },
    backLink() {
      return last(this.$router.history.router.previousPaths);
    },
  },
  methods: {
    backClicked() {
      this.$store.dispatch('pageLeaveWarning/shouldSkipDisplayingLeavingWarning', true);
      redirectTo(this, this.backLink);
    },
  },
};
</script>
