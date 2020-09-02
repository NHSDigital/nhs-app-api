<template>
  <div>
    <error-page v-if="hasRetried"
                id="presciptionsGpSessionError"
                :code="referenceCode"
                header-locale-ref="gpSessionErrors.prescriptions.youCanNotOrderOrViewPrescriptions"
                :back-url="backUrl"
                :update-header="false">
      <template v-slot:content>
        <p>{{ $t('gpSessionErrors.prescriptions.ifTheProblemContinues') }}
          <a href="https://111.nhs.uk" target="_blank" rel="noopener noreferrer"
             style="display:inline">
            {{ $t('gpSessionErrors.nhs111Link') }}</a>
          {{ $t('gpSessionErrors.orCall') }}
        </p>
      </template>
      <template v-slot:actions>
        <error-screen-alternative-actions
          alternative-actions-header="gpSessionErrors.prescriptions.otherThingsYouCanDo">
          <template v-slot:items>
            <emergency-prescription-menu-item />
          </template>
        </error-screen-alternative-actions>
      </template>
    </error-page>

    <error-container v-else id="error-dialog-599">
      <error-title title="gpSessionErrors.prescriptions.tryAgainHeader"/>
      <error-paragraph from="gpSessionErrors.prescriptions.youAreNotCurrentlyAble"/>
      <error-paragraph from="gpSessionErrors.temporaryProblem"/>
      <error-button from="generic.tryAgain" @click="tryAgain" />
      <error-link from="generic.back"
                  :action="backUrl"
                  :desktop-only="true"/>
    </error-container>


  </div>
</template>

<script>
import EmergencyPrescriptionMenuItem from '@/components/menuItems/EmergencyPrescriptionMenuItem';
import ErrorButton from '@/components/errors/ErrorButton';
import ErrorContainer from '@/components/errors/ErrorContainer';
import ErrorLink from '@/components/errors/ErrorLink';
import ErrorPage from '@/components/errors/ErrorPage';
import ErrorParagraph from '@/components/errors/ErrorParagraph';
import ErrorScreenAlternativeActions from '@/components/errors/ErrorScreenAlternativeActions';
import ErrorTitle from '@/components/errors/ErrorTitle';
import { redirectTo, gpSessionErrorHasRetried } from '@/lib/utils';
import {
  PRESCRIPTIONS_PATH,
} from '@/router/paths';

export default {
  name: 'PrescriptionErrors',
  components: {
    EmergencyPrescriptionMenuItem,
    ErrorButton,
    ErrorContainer,
    ErrorLink,
    ErrorPage,
    ErrorParagraph,
    ErrorScreenAlternativeActions,
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
      required: true,
    },
  },
  data() {
    return {
      backUrl: PRESCRIPTIONS_PATH,
    };
  },
  computed: {
    hasRetried() {
      return gpSessionErrorHasRetried(this.$store);
    },
  },
  methods: {
    tryAgain() {
      if (this.$store.state.device.isNativeApp) {
        sessionStorage.setItem('hasRetried', true);
      }
      this.$store.dispatch('session/setRetry', true);
      redirectTo(this, this.tryAgainRoute, { hr: true }, true);
    },
  },
};
</script>
