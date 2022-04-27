<template>
  <div data-purpose="olc-unavailable">
    <p data-purpose="info">
      {{ $t('onlineConsultations.unavailable.thisServiceIsNormallyAvailable') }}
    </p>
    <h2 class="nhsuk-heading-m nhsuk-u-padding-bottom-0" data-purpose="coronavirus-heading">
      {{ $t('onlineConsultations.unavailable.coronavirusIfYouThinkYouMightHave') }}
    </h2>
    <p data-purpose="coronavirus-info">
      {{ $t('onlineConsultations.unavailable.coronavirusStayAtHome') }}
    </p>
    <contact-111
      :text="$t('onlineConsultations.unavailable.forUrgentMedicalAdvice.text')"
      :aria-label="$t('onlineConsultations.unavailable.forUrgentMedicalAdvice.label')"/>
    <p>
      <a :href="coronaConditionsUrl"
         data-purpose="coronavirus-link"
         target="_blank"
         rel="noopener noreferrer">
        {{ $t('onlineConsultations.unavailable.findOutWhatToDo') }}
      </a>
    </p>
    <desktopGenericBackLink v-if="!isNativeApp"
                            data-purpose="back-link"
                            :path="backLink"
                            :button-text="'onlineConsultations.orchestrator.backButton'"
                            @clickAndPrevent="backClicked"/>
  </div>
</template>

<script>
import last from 'lodash/fp/last';
import { redirectTo } from '@/lib/utils';
import Contact111 from '@/components/widgets/Contact111';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';

export default {
  name: 'OnlineConsultationsUnavailable',
  components: {
    Contact111,
    DesktopGenericBackLink,
  },
  data() {
    return {
      coronaConditionsUrl: this.$store.$env.CORONA_CONDITIONS_URL,
    };
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
