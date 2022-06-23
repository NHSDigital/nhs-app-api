<template>
  <div v-if="showTemplate">
    <message-dialog-generic
      v-if="error.status === genericStatusCodes.BAD_REQUEST"
      :id="errorId" override-style="plain">
      <error-title title="appointments.error.cannotAccessGpAppointments"
                   header="appointments.error.cannotAccessGpAppointments" />
      <error-link v-if="isNativeApp"
                  from="appointments.error.goBackAndTryAgain"
                  :action="appointmentsBookingPath"/>
      <error-paragraph from="appointments.error.contactSurgeryToBookCancelOrPrescription" />
      <contact-111
        :text="$t('appointments.error.forurgentMedicalAdviceGoTo.text')"
        :aria-label="$t('appointments.error.forurgentMedicalAdviceGoTo.label')"/>
      <error-link from="generic.back"
                  :action="appointmentsPath"
                  :desktop-only="true"/>
    </message-dialog-generic>

    <error-page v-else-if="error.status === genericStatusCodes.FORBIDDEN"
                header-locale-ref="forbiddenErrors.appointments.gpAppointmentBookingUnavailable"
                :back-url="backUrl">
      <template #content>
        <p>{{ $t('forbiddenErrors.appointments.youCannotBookOnline') }}</p>
        <contact-111 :text="$t('forbiddenErrors.appointments.ifTheProblemContinues')"/>
      </template>
      <template #actions>
        <alternative-appointment-actions/>
      </template>
    </error-page>

    <message-dialog-generic
      v-else-if="error.status === appointmentStatusCodes.APPOINTMENT_DOES_NOT_EXIST"
      :id="errorId" override-style="plain">
      <error-title title="appointments.confirmation.error.theAppointmentIsNoLongerAvailable"/>
      <error-link v-if="isNativeApp"
                  from="appointments.confirmation.error.pleaseChooseADifferentAppointment"
                  :action="appointmentsBookingPath"/>
      <error-link from="generic.back"
                  :action="appointmentsPath"
                  :desktop-only="true"/>
    </message-dialog-generic>

    <error-container
      v-else-if="error.status === appointmentStatusCodes.MAX_PENDING_APPOINTMENTS_BOOKED"
      :id="errorId"
      override-style="plain">
      <error-title title="appointments.confirmation.error.youHaveReachedYourAppoinmentLimit"/>
      <error-paragraph from="appointments.confirmation.error.youCannotBookAnyMore" />
      <error-paragraph from="appointments.confirmation.error.contactYourSurgeryIfYouNeedToBook" />
      <error-paragraph from="appointments.confirmation.error.youCanGoBack" />
      <contact-111
        :text="$t('appointments.confirmation.error.forUrgentMedicalAdvice.text')"
        :aria-label="$t('appointments.confirmation.error.forUrgentMedicalAdvice.label')"/>
      <error-link from="generic.back"
                  :action="appointmentsPath"
                  :desktop-only="true" />
    </error-container>

    <message-dialog-generic v-else-if="error.status === genericStatusCodes.INTERNAL_SERVER_ERROR
                              || error.status === genericStatusCodes.BAD_GATEWAY
                              || error.status === genericStatusCodes.GATEWAY_TIMEOUT"
                            :id="errorId" override-style="plain">
      <error-title title="appointments.error.cannotAccessGpAppointments"
                   header="appointments.error.cannotAccessGpAppointments" />
      <error-link v-if="isNativeApp"
                  from="appointments.error.goBackAndTryAgain"
                  :action="appointmentsBookingPath"/>
      <error-paragraph from="appointments.error.contactSurgeryToBookCancelOrPrescription" />
      <contact-111
        :text="$t('appointments.error.forurgentMedicalAdviceGoTo.text')"
        :aria-label="$t('appointments.error.forurgentMedicalAdviceGoTo.label')"/>
      <error-link from="appointments.error.contactWithErrorCode"
                  :action="contactUsUrl"
                  target="_blank"
                  :query-param="contactUsParam"
                  :params="{errorCode: error.serviceDeskReference}"/>
      <error-link from="generic.back"
                  :action="appointmentsPath"
                  :desktop-only="true"/>
    </message-dialog-generic>

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
import get from 'lodash/fp/get';
import AlternativeAppointmentActions from '@/components/appointments/AlternativeAppointmentActions';
import Contact111 from '@/components/widgets/Contact111';
import ErrorContainer from '@/components/errors/ErrorContainer';
import ErrorLink from '@/components/errors/ErrorLink';
import ErrorPage from '@/components/errors/ErrorPage';
import ErrorPageMixin from '@/components/errors/ErrorPageMixin';
import ErrorParagraph from '@/components/errors/ErrorParagraph';
import ErrorTitle from '@/components/errors/ErrorTitle';
import MessageDialogGeneric from '@/components/widgets/MessageDialogGeneric';

import genericStatus from '@/components/errors/statusCodes/GenericStatusCodes';
import appointmentStatus from '@/components/errors/statusCodes/AppointmentCustomStatusCodes';

import {
  APPOINTMENTS_PATH,
  GP_APPOINTMENTS_PATH,
  APPOINTMENT_BOOKING_PATH,
} from '@/router/paths';

export default {
  name: 'BookingConfirmationErrors',
  components: {
    AlternativeAppointmentActions,
    Contact111,
    ErrorContainer,
    ErrorLink,
    ErrorPage,
    ErrorParagraph,
    ErrorTitle,
    MessageDialogGeneric,
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
      appointmentsBookingPath: APPOINTMENT_BOOKING_PATH,
      contactUsUrl: this.$store.$env.CONTACT_US_URL,
      genericStatusCodes: genericStatus,
      appointmentStatusCodes: appointmentStatus,
      errorId: `error-dialog-${this.error.status}`,
    };
  },
  computed: {
    isNativeApp() {
      return this.$store.state.device.isNativeApp;
    },
    contactUsParam() {
      return {
        ErrCodeParam: 'errorcode',
        ErrCodeValue: this.serviceDeskReference,
      };
    },
    serviceDeskReference() {
      return get('$store.state.availableAppointments.error.serviceDeskReference')(this) || '';
    },
  },
};
</script>
