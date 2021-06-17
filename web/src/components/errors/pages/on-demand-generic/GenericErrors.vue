<template>
  <div>
    <error-page v-if="hasRetried"
                id="genericGpSessionError"
                header-locale-ref="gpSessionErrors.genericOnDemand.serviceUnavailable"
                :back-url="backUrl"
                :back-text="backText"
                :show-back-link="isDesktop"
                :update-header="false">

      <template v-slot:content>
        <error-title title="gpSessionErrors.genericOnDemand.serviceUnavailable"/>
        <h2>{{ $t('gpSessionErrors.genericOnDemand.ifYouNeedInformationNow') }}</h2>
        <error-paragraph from="gpSessionErrors.genericOnDemand.contactGpSurgery"/>
        <contact-111 :text="$t('gpSessionErrors.genericOnDemand.forUrgentMedicalAdvice')"/>
      </template>
    </error-page>

    <error-container v-else id="error-dialog-599">
      <error-title title="gpSessionErrors.genericOnDemand.tryAgainHeader"/>
      <error-paragraph from="gpSessionErrors.genericOnDemand.temporaryProblem"/>
      <error-button from="generic.tryAgain" @click="tryAgain" />
    </error-container>
  </div>
</template>

<script>
import Contact111 from '@/components/widgets/Contact111';
import ErrorButton from '@/components/errors/ErrorButton';
import ErrorContainer from '@/components/errors/ErrorContainer';
import ErrorPage from '@/components/errors/ErrorPage';
import ErrorParagraph from '@/components/errors/ErrorParagraph';
import ErrorTitle from '@/components/errors/ErrorTitle';
import { redirectTo, gpSessionErrorHasRetried } from '@/lib/utils';
import { INDEX_PATH } from '@/router/paths';

export default {
  name: 'GenericErrors',
  components: {
    Contact111,
    ErrorButton,
    ErrorContainer,
    ErrorPage,
    ErrorParagraph,
    ErrorTitle,
  },
  props: {
    tryAgainRoute: {
      type: String,
      default: undefined,
      required: true,
    },
  },
  data() {
    return {
      backUrl: INDEX_PATH,
      backText: 'gpSessionErrors.genericOnDemand.backToHome',
      isDesktop: !this.$store.state.device.isNativeApp,
    };
  },
  computed: {
    hasRetried() {
      return gpSessionErrorHasRetried();
    },
  },
  methods: {
    tryAgain() {
      sessionStorage.setItem('hasRetried', true);

      redirectTo(this, this.tryAgainRoute, { hr: true }, true);
    },
  },
};
</script>
