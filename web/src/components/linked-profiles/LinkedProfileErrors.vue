<template>
  <div v-if="error.status === gpSessionErrorStatus">
    <error-container v-if="!hasRetried" id="linked-profiles-599-temporary-error">
      <error-title title="gpSessionErrors.linkedProfiles.temporaryHeader"/>
      <error-paragraph from="gpSessionErrors.linkedProfiles.youAreNotCurrentlyAble"/>
      <error-paragraph from="gpSessionErrors.temporaryProblem"/>
      <error-button from="generic.tryAgain" @click="tryAgain" />
      <error-link from="generic.back"
                  :action="backUrl"
                  :desktop-only="true"/>
    </error-container>

    <error-page v-else id="linked-profiles-599-error"
                :code="error.serviceDeskReference"
                header-locale-ref="gpSessionErrors.linkedProfiles.sorryLinkedProfiles"
                :back-url="backUrl">
      <template v-slot:content>
        <p>{{ $t('gpSessionErrors.linkedProfiles.ifYouNeedAccessToHealthServices') }}</p>
        <contact-111 :text="$t('gpSessionErrors.linkedProfiles.forUrgentMedicalAdvice')"/>
        <p><a :href="findOutMoreUrl" target="_blank" rel="noopener noreferrer">
          {{ $t('profiles.findOutMore') }}</a></p>
        <p>{{ $t('gpSessionErrors.linkedProfiles.ifYouHaveAccessedLinkedProfiles') }}
          <strong>{{ $t('apiErrors.reportAProblem') }}</strong>.
        </p>
      </template>
    </error-page>
  </div>

  <error-container v-else id="error-dialog-unknown">
    <error-title title="apiErrors.pageHeader"
                 header="apiErrors.pageHeader" />
    <error-paragraph from="apiErrors.header" />
    <contact-111
      :text="$t('appointments.error.ifTheProblemContinuesAndYouNeedToBookOrCancel.text')"
      :aria-label="$t('appointments.error.ifTheProblemContinuesAndYouNeedToBookOrCancel.label')"/>
  </error-container>
</template>

<script>
import Contact111 from '@/components/widgets/Contact111';
import ErrorButton from '@/components/errors/ErrorButton';
import ErrorContainer from '@/components/errors/ErrorContainer';
import ErrorLink from '@/components/errors/ErrorLink';
import ErrorPage from '@/components/errors/ErrorPage';
import ErrorPageMixin from '@/components/errors/ErrorPageMixin';
import ErrorParagraph from '@/components/errors/ErrorParagraph';
import ErrorTitle from '@/components/errors/ErrorTitle';

import {
  LINKED_PROFILES_PATH,
  MORE_PATH,
} from '@/router/paths';
import { redirectTo, gpSessionErrorHasRetried, GP_SESSION_ERROR_STATUS } from '@/lib/utils';
import { LINKED_PROFILES_FIND_OUT_MORE_URL } from '@/router/externalLinks';

export default {
  name: 'LinkedProfileErrors',
  components: {
    Contact111,
    ErrorButton,
    ErrorContainer,
    ErrorLink,
    ErrorPage,
    ErrorParagraph,
    ErrorTitle,
  },
  mixins: [ErrorPageMixin],
  data() {
    return {
      backUrl: this.$store.state.navigation.backLinkOverride || MORE_PATH,
      contactUsUrl: this.$store.$env.CONTACT_US_URL,
      findOutMoreUrl: LINKED_PROFILES_FIND_OUT_MORE_URL,
      gpSessionErrorStatus: GP_SESSION_ERROR_STATUS,
    };
  },
  computed: {
    hasRetried() {
      return gpSessionErrorHasRetried(this.$store);
    },
    error() {
      return this.$store.state.linkedAccounts.error;
    },
  },
  methods: {
    tryAgain() {
      if (this.$store.state.device.isNativeApp) {
        sessionStorage.setItem('hasRetried', true);
      }

      this.$store.dispatch('session/setRetry', true);
      redirectTo(this, LINKED_PROFILES_PATH, { hr: true }, true);
    },
  },
};
</script>
