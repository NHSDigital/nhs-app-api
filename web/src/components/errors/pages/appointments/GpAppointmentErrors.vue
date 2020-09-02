<template>
  <div>
    <error-container v-if="error.status === genericStatusCodes.BAD_REQUEST" :id="errorId">
      <error-title title="appointments.error.thereIsAProblemAppointments"
                   header="appointments.error.thereIsAProblem" />
      <error-paragraph from="appointments.error.tryAgainOrContactSurgeryOrOneOneOne" />
      <error-link from="generic.back"
                  :action="appointmentsPath"
                  :desktop-only="true"/>
    </error-container>

    <error-page v-if="error.status === genericStatusCodes.FORBIDDEN"
                header-locale-ref="forbiddenErrors.appointments.gpAppointmentBookingUnavailable"
                :back-url="backUrl">
      <template v-slot:content>
        <p>{{ $t('forbiddenErrors.appointments.youCannotBookOnline') }}</p>
        <p>{{ $t('forbiddenErrors.appointments.ifTheProblemContinues') }}
          <a href="https://111.nhs.uk" target="_blank" rel="noopener noreferrer"
             style="display:inline">
            {{ $t('forbiddenErrors.nhs111Link') }}</a>
          {{ $t('forbiddenErrors.orCall') }}
        </p>
      </template>
      <template v-slot:actions>
        <error-screen-alternative-actions
          alternative-actions-header="forbiddenErrors.appointments.whatYouCanDoNext">
          <template v-slot:items>
            <corona-virus-menu-item />
            <gp-advice-menu-item v-if="isCdssAdvice" :previous-route="appointmentsPath"/>
            <gp-admin-help-menu-item v-if="isCdssAdmin" :previous-route="appointmentsPath" />
            <one-one-one-service-menu-item />
          </template>
        </error-screen-alternative-actions>
      </template>
    </error-page>

    <error-container
      v-else-if="error.status === genericStatusCodes.INTERNAL_SERVER_ERROR
        || error.status === genericStatusCodes.BAD_GATEWAY"
      :id="errorId">
      <error-title title="appointments.error.thereIsAProblemLoading"/>
      <error-paragraph from="appointments.error.tryAgainNow"/>
      <error-paragraph from="appointments.error.ifTheProblemContinuesAndYouNeedToBook"/>
      <error-button from="generic.tryAgain" @click="$router.go()" />
      <report-a-problem :reference="error.serviceDeskReference"/>
      <error-link from="generic.back"
                  :action="backUrl"
                  :desktop-only="true"/>
    </error-container>

    <error-container v-else-if="error.status === genericStatusCodes.GATEWAY_TIMEOUT"
                     :id="errorId">
      <error-title title="appointments.error.thereIsAProblemLoading"/>
      <error-paragraph from="appointments.error.tryAgainNowOrContactUs"
                       :variable="error.serviceDeskReference"/>
      <error-paragraph from="appointments.error.ifTheProblemContinuesAndYouNeedToBookOrCancel"/>
      <error-button from="generic.tryAgain" @click="$router.go()" />
      <error-link from="generic.contactUs"
                  :action="contactUsUrl"
                  target="_blank"/>
    </error-container>

    <error-container
      v-else-if="error.status === appointmentStatusCodes.GP_SESSION_ERROR && !hasRetried"
      :id="errorId">
      <error-title title="gpSessionErrors.appointments.temporaryHeader"/>
      <error-paragraph from="gpSessionErrors.appointments.youCannotBookOnline"/>
      <error-paragraph from="gpSessionErrors.appointments.temporaryProblem"/>
      <error-button from="generic.tryAgain" @click="tryAgain" />
      <error-link from="generic.back"
                  :action="backUrl"
                  :desktop-only="true"/>
    </error-container>

    <error-page v-if="hasRetried && error.status === appointmentStatusCodes.GP_SESSION_ERROR"
                :code="error.serviceDeskReference"
                header-locale-ref="gpSessionErrors.appointments.gpAppointmentBookingUnavailable"
                :back-url="backUrl">
      <template v-slot:content>
        <p>{{ $t('gpSessionErrors.appointments.youCannotBookOnline') }}</p>
        <p>{{ $t('gpSessionErrors.appointments.ifTheProblemContinues') }}
          <a href="https://111.nhs.uk" target="_blank" rel="noopener noreferrer"
             style="display:inline">
            {{ $t('gpSessionErrors.nhs111Link') }}</a>
          {{ $t('gpSessionErrors.orCall') }}
        </p>
      </template>
      <template v-slot:actions>
        <error-screen-alternative-actions
          alternative-actions-header="gpSessionErrors.appointments.whatYouCanDoNext">
          <template v-slot:items>
            <corona-virus-menu-item />
            <gp-advice-menu-item v-if="isCdssAdvice" :previous-route="appointmentsPath"/>
            <gp-admin-help-menu-item v-if="isCdssAdmin" :previous-route="appointmentsPath" />
            <one-one-one-service-menu-item />
          </template>
        </error-screen-alternative-actions>
      </template>
    </error-page>

  </div>
</template>

<script>

import CoronaVirusMenuItem from '@/components/menuItems/CoronaVirusMenuItem';
import ErrorButton from '@/components/errors/ErrorButton';
import ErrorContainer from '@/components/errors/ErrorContainer';
import ErrorLink from '@/components/errors/ErrorLink';
import ErrorPage from '@/components/errors/ErrorPage';
import ErrorPageMixin from '@/components/errors/ErrorPageMixin';
import ErrorParagraph from '@/components/errors/ErrorParagraph';
import ErrorScreenAlternativeActions from '@/components/errors/ErrorScreenAlternativeActions';
import ErrorTitle from '@/components/errors/ErrorTitle';
import GpAdviceMenuItem from '@/components/menuItems/GpAdviceMenuItem';
import GpAdminHelpMenuItem from '@/components/menuItems/GpAdminHelpMenuItem';
import OneOneOneServiceMenuItem from '@/components/menuItems/OneOneOneServiceMenuItem';
import ReportAProblem from '@/components/errors/ReportAProblem';

import {
  APPOINTMENTS_PATH,
  GP_APPOINTMENTS_PATH,
} from '@/router/paths';
import { redirectTo, gpSessionErrorHasRetried } from '@/lib/utils';
import sjrIf from '@/lib/sjrIf';

import genericStatus from '@/components/errors/statusCodes/GenericStatusCodes';
import appointmentStatus from '@/components/errors/statusCodes/AppointmentCustomStatusCodes';

export default {
  name: 'GpAppointmentErrors',
  components: {
    CoronaVirusMenuItem,
    ErrorButton,
    ErrorContainer,
    ErrorLink,
    ErrorPage,
    ErrorParagraph,
    ErrorScreenAlternativeActions,
    ErrorTitle,
    GpAdminHelpMenuItem,
    GpAdviceMenuItem,
    OneOneOneServiceMenuItem,
    ReportAProblem,
  },
  mixins: [ErrorPageMixin],
  props: {
    error: {
      type: Object,
      default: undefined,
      required: true,
    },
  },
  data() {
    return {
      backUrl: APPOINTMENTS_PATH,
      appointmentsPath: GP_APPOINTMENTS_PATH,
      contactUsUrl: this.$store.$env.CONTACT_US_URL,
      genericStatusCodes: genericStatus,
      appointmentStatusCodes: appointmentStatus,
      errorId: `error-dialog-${this.error.status}`,
    };
  },
  computed: {
    hasRetried() {
      return gpSessionErrorHasRetried(this.$store);
    },
    isCdssAdmin() {
      return sjrIf({ $store: this.$store, journey: 'cdssAdmin' });
    },
    isCdssAdvice() {
      return sjrIf({ $store: this.$store, journey: 'cdssAdvice' });
    },
  },
  methods: {
    tryAgain() {
      if (this.$store.state.device.isNativeApp) {
        sessionStorage.setItem('hasRetried', true);
      }
      this.$store.dispatch('session/setRetry', true);
      redirectTo(this, GP_APPOINTMENTS_PATH, { hr: true }, true);
    },
  },
};
</script>
