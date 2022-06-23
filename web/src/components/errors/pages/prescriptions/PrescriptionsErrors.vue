<template>
  <div>
    <error-page v-if="hasRetried"
                id="presciptionsGpSessionError"
                :code="referenceCode"
                header-locale-ref="gpSessionErrors.prescriptions.youCanNotOrderOrViewPrescriptions"
                :back-url="backUrl"
                :update-header="false">
      <template #content>
        <p>{{ $t('gpSessionErrors.prescriptions.contactYourSurgery') }}</p>
        <contact-111 :text="$t('gpSessionErrors.prescriptions.forUrgentMedicalAdvice')"/>
      </template>
      <template #actions>
        <error-screen-alternative-actions
          alternative-actions-header="gpSessionErrors.prescriptions.otherThingsYouCanDo">
          <template #items>
            <emergency-prescription-menu-item />
          </template>
        </error-screen-alternative-actions>
      </template>
    </error-page>

    <shutter-container v-else id="shutter-dialog-599">
      <error-title title="gpSessionErrors.prescriptions.tryAgainHeader"/>
      <error-paragraph from="gpSessionErrors.prescriptions.youAreNotCurrentlyAble"/>
      <error-paragraph from="gpSessionErrors.temporaryProblem"/>
      <error-button from="generic.tryAgain" @click="tryAgain" />
      <error-link from="generic.back"
                  :action="backUrl"
                  :desktop-only="true"/>
    </shutter-container>


  </div>
</template>

<script>
import Contact111 from '@/components/widgets/Contact111';
import EmergencyPrescriptionMenuItem from '@/components/menuItems/EmergencyPrescriptionMenuItem';
import ErrorButton from '@/components/errors/ErrorButton';
import ShutterContainer from '@/components/shutters/ShutterContainer';
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
    Contact111,
    EmergencyPrescriptionMenuItem,
    ErrorButton,
    ShutterContainer,
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
      hasRetried: gpSessionErrorHasRetried(),
    };
  },
  watch: {
    '$route.query.ts': function watchTimestamp() {
      this.hasRetried = gpSessionErrorHasRetried();
    },
  },
  mounted() {
    this.$store.dispatch('navigation/setRouteCrumb', 'onDemandPrescriptionCrumb');
  },
  methods: {
    tryAgain() {
      sessionStorage.setItem('hasRetried', true);
      redirectTo(this, this.tryAgainRoute, { hr: true }, true);
    },
  },
};
</script>
