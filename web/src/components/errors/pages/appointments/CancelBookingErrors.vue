<template>
  <div v-if="hasConnection">
    <error-container v-if="error.status === genericStatusCodes.BAD_REQUEST" :id="errorId">
      <error-title title="appointments.error.thereIsAProblemAppointments"
                   header="appointments.error.thereIsAProblem" />
      <contact-111
        :text="$t('appointments.error.tryAgainOrContactSurgeryOrOneOneOne.text')"
        :aria-label="$t('appointments.error.tryAgainOrContactSurgeryOrOneOneOne.label')"/>
      <error-link from="generic.back"
                  :action="appointmentsPath"
                  :desktop-only="true" />
    </error-container>

    <error-page v-else-if="error.status === genericStatusCodes.FORBIDDEN"
                header-locale-ref="forbiddenErrors.appointments.gpAppointmentBookingUnavailable"
                :back-url="backUrl">
      <template v-slot:content>
        <p>{{ $t('forbiddenErrors.appointments.youCannotBookOnline') }}</p>
        <contact-111 :text="$t('forbiddenErrors.appointments.ifTheProblemContinues')"/>
      </template>
      <template v-slot:actions>
        <alternative-appointment-actions/>
      </template>
    </error-page>

    <error-container v-else-if="error.status === appointmentStatusCodes.APPOINTMENT_DOES_NOT_EXIST"
                     :id="errorId">
      <error-title title="appointments.error.youCannotCancelThisAppointment"/>
      <error-paragraph from="appointments.error.theAppointmentMayBeCancelledOrInThePast" />
      <error-link from="generic.back"
                  :action="appointmentsPath"
                  :desktop-only="true" />
    </error-container>

    <error-container
      v-else-if="error.status === appointmentStatusCodes.APPOINTMENT_TOO_LATE_TO_CANCEL"
      :id="errorId">
      <error-title title="appointments.error.contactYouSurgeryToCancel"/>
      <error-paragraph from="appointments.error.itIsTooLateToCancel" />
      <error-link from="generic.back"
                  :action="appointmentsPath"
                  :desktop-only="true" />
    </error-container>

    <error-container v-else-if="error.status === genericStatusCodes.INTERNAL_SERVER_ERROR
                       || error.status === genericStatusCodes.BAD_GATEWAY
                       || error.status === genericStatusCodes.GATEWAY_TIMEOUT"
                     :id="errorId">
      <error-title title="appointments.error.thereIsAProblemAppointments"
                   header="appointments.error.thereIsAProblem" />
      <error-paragraph from="appointments.error.tryAgainOrContactUs"
                       :variable="error.serviceDeskReference"/>
      <contact-111
        :text="$t('appointments.error.ifTheProblemContinuesAndYouNeedToBookOrCancel.text')"
        :aria-label="$t('appointments.error.ifTheProblemContinuesAndYouNeedToBookOrCancel.label')"/>
      <error-link from="generic.contactUs"
                  :action="contactUsUrl"
                  target="_blank"/>
      <error-link from="generic.back"
                  :action="appointmentsPath"
                  :desktop-only="true"/>
    </error-container>

    <error-container v-else id="error-dialog-unknown">
      <error-title title="apiErrors.pageHeader"
                   header="apiErrors.pageHeader" />
      <error-paragraph from="apiErrors.header" />
      <contact-111
        :text="$t('appointments.error.ifTheProblemContinuesAndYouNeedToBookOrCancel.text')"
        :aria-label="$t('appointments.error.ifTheProblemContinuesAndYouNeedToBookOrCancel.label')"/>
    </error-container>
  </div>
</template>

<script>
import AlternativeAppointmentActions from '@/components/appointments/AlternativeAppointmentActions';
import Contact111 from '@/components/widgets/Contact111';
import ErrorContainer from '@/components/errors/ErrorContainer';
import ErrorLink from '@/components/errors/ErrorLink';
import ErrorPage from '@/components/errors/ErrorPage';
import ErrorPageMixin from '@/components/errors/ErrorPageMixin';
import ErrorParagraph from '@/components/errors/ErrorParagraph';
import ErrorTitle from '@/components/errors/ErrorTitle';

import genericStatus from '@/components/errors/statusCodes/GenericStatusCodes';
import appointmentStatus from '@/components/errors/statusCodes/AppointmentCustomStatusCodes';

import {
  APPOINTMENTS_PATH,
  GP_APPOINTMENTS_PATH,
} from '@/router/paths';

export default {
  name: 'CancelBookingErrors',
  components: {
    AlternativeAppointmentActions,
    Contact111,
    ErrorContainer,
    ErrorLink,
    ErrorPage,
    ErrorParagraph,
    ErrorTitle,
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
    hasConnection() {
      return !this.hasConnectionProblem();
    },
  },
};
</script>
