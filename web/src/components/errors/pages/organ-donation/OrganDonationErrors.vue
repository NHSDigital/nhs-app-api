<template>
  <div>
    <error-page v-if="hasRetried"
                id="organDonationSessionError"
                header-locale-ref="gpSessionErrors.organDonation.organDonationUnavailable"
                :update-header="false"
                :show-back-link="false">
      <template #content>
        <error-paragraph from="gpSessionErrors.organDonation.organDonationMessage"/>
        <h2>{{ $t('gpSessionErrors.organDonation.ifYouNeedInformationNow') }}</h2>
        <h3>{{ $t('gpSessionErrors.organDonation.ifYouNeedInformationNowEmail') }}</h3>
        <p><a :href="$t('gpSessionErrors.organDonation.emailLink')">
          {{ $t('gpSessionErrors.organDonation.emailText') }}</a></p>
      </template>
    </error-page>

    <shutter-container v-else id="shutter-dialog-599">
      <error-title title="gpSessionErrors.organDonation.tryAgainHeader"/>
      <error-paragraph from="gpSessionErrors.organDonation.youAreNotCurrentlyAble"/>
      <error-paragraph from="gpSessionErrors.temporaryProblem"/>
      <error-button from="generic.tryAgain" @click="tryAgain" />
    </shutter-container>

  </div>
</template>

<script>
import ErrorButton from '@/components/errors/ErrorButton';
import ShutterContainer from '@/components/shutters/ShutterContainer';
import ErrorPage from '@/components/errors/ErrorPage';
import ErrorParagraph from '@/components/errors/ErrorParagraph';
import ErrorTitle from '@/components/errors/ErrorTitle';
import { redirectTo, gpSessionErrorHasRetried } from '@/lib/utils';
import { ORGAN_DONATION_PATH } from '@/router/paths';

export default {
  name: 'OrganDonationErrors',
  components: {
    ErrorButton,
    ShutterContainer,
    ErrorPage,
    ErrorParagraph,
    ErrorTitle,
  },
  props: {
    error: {
      type: Object,
      default: undefined,
      required: true,
    },
    referenceCode: {
      type: String,
      default: undefined,
    },
    tryAgainRoute: {
      type: String,
      default: undefined,
      required: false,
    },
  },
  data() {
    return {
      hasRetried: gpSessionErrorHasRetried(),
    };
  },
  watch: {
    '$route.query.ts': function watchTimestamp() {
      this.hasRetried = gpSessionErrorHasRetried();
    },
  },
  methods: {
    tryAgain() {
      sessionStorage.setItem('hasRetried', true);
      redirectTo(this, ORGAN_DONATION_PATH, { hr: true }, true);
    },
  },
};
</script>
