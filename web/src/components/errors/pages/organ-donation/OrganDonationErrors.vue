<template>
  <div>
    <error-page v-if="hasRetried"
                id="organDonationSessionError"
                header-locale-ref="gpSessionErrors.organDonation.organDonationUnavailable"
                :update-header="false"
                :show-back-link="false">
      <template v-slot:content>
        <error-paragraph from="gpSessionErrors.organDonation.organDonationMessage"/>
        <h2>{{ $t('gpSessionErrors.organDonation.ifYouNeedInformationNow') }}</h2>
        <h3>{{ $t('gpSessionErrors.organDonation.ifYouNeedInformationNowEmail') }}</h3>
        <p><a :href="$t('gpSessionErrors.organDonation.emailLink')">
          {{ $t('gpSessionErrors.organDonation.emailText') }}</a></p>
      </template>
    </error-page>

    <error-container v-else id="error-dialog-599">
      <error-title title="gpSessionErrors.organDonation.tryAgainHeader"/>
      <error-paragraph from="gpSessionErrors.organDonation.youAreNotCurrentlyAble"/>
      <error-paragraph from="gpSessionErrors.temporaryProblem"/>
      <error-button from="generic.tryAgain" @click="tryAgain" />
    </error-container>

  </div>
</template>

<script>
import ErrorButton from '@/components/errors/ErrorButton';
import ErrorContainer from '@/components/errors/ErrorContainer';
import ErrorPage from '@/components/errors/ErrorPage';
import ErrorParagraph from '@/components/errors/ErrorParagraph';
import ErrorTitle from '@/components/errors/ErrorTitle';
import { redirectTo, gpSessionErrorHasRetried } from '@/lib/utils';
import { ORGAN_DONATION_PATH } from '@/router/paths';

export default {
  name: 'OrganDonationErrors',
  components: {
    ErrorButton,
    ErrorContainer,
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
