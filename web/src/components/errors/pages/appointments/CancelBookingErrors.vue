<template>
  <div>
    <error-container v-if="error.status === genericStatusCodes.BAD_REQUEST" :id="generateErrorId()">
      <error-title title="appointments.error.thereIsAProblemAppointments"
                   header="appointments.error.thereIsAProblem" />
      <error-paragraph from="appointments.error.tryAgainOrContactSurgeryOrOneOneOne" />
      <error-link from="generic.back"
                  :action="appointmentsPath"
                  :desktop-only="true" />
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

    <error-container v-else-if="error.status === appointmentStatusCodes.APPOINTMENT_DOES_NOT_EXIST"
                     :id="generateErrorId()">
      <error-title title="appointments.error.youCannotCancelThisAppointment"/>
      <error-paragraph from="appointments.error.theAppointmentMayBeCancelledOrInThePast" />
      <error-link from="generic.back"
                  :action="appointmentsPath"
                  :desktop-only="true" />
    </error-container>

    <error-container
      v-else-if="error.status === appointmentStatusCodes.APPOINTMENT_TOO_LATE_TO_CANCEL"
      :id="generateErrorId()">
      <error-title title="appointments.error.contactYouSurgeryToCancel"/>
      <error-paragraph from="appointments.error.itIsTooLateToCancel" />
      <error-link from="generic.back"
                  :action="appointmentsPath"
                  :desktop-only="true" />
    </error-container>

    <error-container v-else-if="error.status === genericStatusCodes.INTERNAL_SERVER_ERROR
                       || error.status === genericStatusCodes.BAD_GATEWAY
                       || error.status === genericStatusCodes.GATEWAY_TIMEOUT"
                     :id="generateErrorId()">
      <error-title title="appointments.error.thereIsAProblemAppointments"
                   header="appointments.error.thereIsAProblem" />
      <error-paragraph from="appointments.error.tryAgainOrContactUs"
                       :variable="error.serviceDeskReference"/>
      <error-paragraph from="appointments.error.ifTheProblemContinuesAndYouNeedToBookOrCancel"/>
      <error-link from="generic.contactUs"
                  :action="contactUsUrl"
                  target="_blank"/>
      <error-link from="generic.back"
                  :action="appointmentsPath"
                  :desktop-only="true"/>
    </error-container>
  </div>
</template>

<script>
import CoronaVirusMenuItem from '@/components/menuItems/CoronaVirusMenuItem';
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

import genericStatus from '@/components/errors/statusCodes/GenericStatusCodes';
import appointmentStatus from '@/components/errors/statusCodes/AppointmentCustomStatusCodes';

import {
  APPOINTMENTS_PATH,
  GP_APPOINTMENTS_PATH,
} from '@/router/paths';
import sjrIf from '@/lib/sjrIf';

export default {
  name: 'CancelBookingErrors',
  components: {
    CoronaVirusMenuItem,
    ErrorContainer,
    ErrorLink,
    ErrorPage,
    ErrorParagraph,
    ErrorScreenAlternativeActions,
    ErrorTitle,
    GpAdviceMenuItem,
    GpAdminHelpMenuItem,
    OneOneOneServiceMenuItem,
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
    };
  },
  computed: {
    isCdssAdmin() {
      return sjrIf({ $store: this.$store, journey: 'cdssAdmin' });
    },
    isCdssAdvice() {
      return sjrIf({ $store: this.$store, journey: 'cdssAdvice' });
    },
  },
  methods: {
    generateErrorId() {
      return `error-dialog-${this.error.status}`;
    },
  },
};
</script>
