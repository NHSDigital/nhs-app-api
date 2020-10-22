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
                  :desktop-only="true" />
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
            <gp-advice-menu-item v-if="isCdssAdvice" route-crumb="appointmentsCrumb"/>
            <admin-help-menu-item v-if="isCdssAdmin"/>
            <one-one-one-service-menu-item />
          </template>
        </error-screen-alternative-actions>
      </template>
    </error-page>


    <error-container v-else-if="error.status === appointmentStatusCodes.APPOINTMENT_DOES_NOT_EXIST"
                     :id="errorId">
      <error-title title="appointments.confirmation.error.theAppointmentIsNoLongerAvailable"/>
      <error-paragraph from="appointments.confirmation.error.pleaseChooseADifferentAppointment" />
      <error-link from="generic.back"
                  :action="appointmentsPath"
                  :desktop-only="true"/>
    </error-container>

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
  </div>
</template>

<script>
import Contact111 from '@/components/widgets/Contact111';
import CoronaVirusMenuItem from '@/components/menuItems/CoronaVirusMenuItem';
import ErrorContainer from '@/components/errors/ErrorContainer';
import ErrorLink from '@/components/errors/ErrorLink';
import ErrorPage from '@/components/errors/ErrorPage';
import ErrorPageMixin from '@/components/errors/ErrorPageMixin';
import ErrorParagraph from '@/components/errors/ErrorParagraph';
import ErrorScreenAlternativeActions from '@/components/errors/ErrorScreenAlternativeActions';
import ErrorTitle from '@/components/errors/ErrorTitle';
import GpAdviceMenuItem from '@/components/menuItems/GpAdviceMenuItem';
import AdminHelpMenuItem from '@/components/menuItems/AdminHelpMenuItem';
import OneOneOneServiceMenuItem from '@/components/menuItems/OneOneOneServiceMenuItem';

import genericStatus from '@/components/errors/statusCodes/GenericStatusCodes';
import appointmentStatus from '@/components/errors/statusCodes/AppointmentCustomStatusCodes';

import {
  APPOINTMENTS_PATH,
  GP_APPOINTMENTS_PATH,
} from '@/router/paths';
import sjrIf from '@/lib/sjrIf';

export default {
  name: 'BookingConfirmationErrors',
  components: {
    Contact111,
    CoronaVirusMenuItem,
    ErrorContainer,
    ErrorLink,
    ErrorPage,
    ErrorParagraph,
    ErrorScreenAlternativeActions,
    ErrorTitle,
    GpAdviceMenuItem,
    AdminHelpMenuItem,
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
      errorId: `error-dialog-${this.error.status}`,
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
};
</script>
