<template>
  <div>
    <error-container v-if="error.status === genericStatusCodes.BAD_REQUEST" :id="errorId">
      <error-title title="appointments.error.thereIsAProblemAppointments"
                   header="appointments.error.thereIsAProblem" />
      <contact-111
        :text="$t('appointments.error.tryAgainOrContactSurgeryOrOneOneOne.text')"
        :aria-label="$t('appointments.error.tryAgainOrContactSurgeryOrOneOneOne.label')"/>
      <error-link from="generic.back"
                  :action="appointmentsPath"
                  :desktop-only="true"/>
    </error-container>

    <error-page v-if="error.status === genericStatusCodes.FORBIDDEN"
                header-locale-ref="forbiddenErrors.appointments.gpAppointmentBookingUnavailable"
                :back-url="backUrl">
      <template v-slot:content>
        <p>{{ $t('forbiddenErrors.appointments.youCannotBookOnline') }}</p>
        <contact-111 :text="$t('forbiddenErrors.appointments.ifTheProblemContinues')"/>
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
      <contact-111
        :text="$t('appointments.error.ifTheProblemContinuesAndYouNeedToBook.text')"
        :aria-label="$t('appointments.error.ifTheProblemContinuesAndYouNeedToBook.label')"/>
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
      <contact-111
        :text="$t('appointments.error.ifTheProblemContinuesAndYouNeedToBookOrCancel.text')"
        :aria-label="$t('appointments.error.ifTheProblemContinuesAndYouNeedToBookOrCancel.label')"/>
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
        <contact-111 :text="$t('gpSessionErrors.appointments.ifTheProblemContinues')"/>
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
import Contact111 from '@/components/widgets/Contact111';
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
    Contact111,
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
