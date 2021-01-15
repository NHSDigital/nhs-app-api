<template>
  <div v-if="hasConnection">
    <error-page v-if="error && error.status === genericStatusCodes.FORBIDDEN"
                header-locale-ref="forbiddenErrors.appointments.gpAppointmentBookingUnavailable"
                :back-url="appointmentsPath">
      <template v-slot:content>
        <p>{{ $t('forbiddenErrors.appointments.youCannotBookOnline') }}</p>
        <contact-111 :text="$t('forbiddenErrors.appointments.ifTheProblemContinues')"/>
      </template>
      <template v-slot:actions>
        <alternative-appointment-actions/>
      </template>
    </error-page>

    <error-page v-else-if="!availableAppointments && !error"
                id="no-appoinments-available"
                header-locale-ref="appointments.book.noAppointmentsAvailable"
                :back-url="appointmentsPath">
      <template v-slot:content>
        <p>{{ $t('appointments.book.youWillNeedToContactGpSurgery') }}</p>
        <contact-111
          :text="$t('appointments.book.forUrgentMedicalAdvice.text')"
          :aria-label="$t('appointments.book.forUrgentMedicalAdvice.label')"/>
      </template>
      <template v-slot:actions>
        <h2>{{ $t('appointments.book.ifYouThinkYouMightHaveCoronavirus') }}</h2>
        <p>{{ $t('appointments.book.stayAtHome') }}</p>
        <p>
          <a href="https://111.nhs.uk/COVID-19"
             rel="noopener noreferrer"
             :aria-label="$t('appointments.book.useThe111CoronavirusService.label')">
            {{ $t('appointments.book.useThe111CoronavirusService.text') }}</a>
        </p>
        <alternative-appointment-actions :show-coronavirus-item="false"/>
      </template>
    </error-page>

    <error-container
      v-else-if="error && (error.status === genericStatusCodes.INTERNAL_SERVER_ERROR
        || error.status === genericStatusCodes.BAD_GATEWAY)"
      :id="errorId">
      <error-title title="appointments.error.thereIsAProblemLoading"/>
      <error-paragraph from="appointments.error.tryAgainOrContactUs"
                       :variable="error.serviceDeskReference"/>
      <contact-111
        :text="$t('appointments.error.ifTheProblemContinuesAndYouNeedToBook.text')"
        :aria-label="$t('appointments.error.ifTheProblemContinuesAndYouNeedToBook.label')"/>
      <error-link from="generic.contactUs"
                  :action="contactUsUrl"
                  target="_blank"/>
      <error-link from="generic.back"
                  :action="appointmentsPath"
                  :desktop-only="true"/>
    </error-container>

    <error-container
      v-else-if="error && error.status === genericStatusCodes.GATEWAY_TIMEOUT"
      :id="errorId">
      <error-title title="appointments.error.thereIsAProblemLoading"/>
      <error-paragraph from="appointments.error.tryAgainNowOrContactUs"
                       :variable="error.serviceDeskReference"/>
      <contact-111
        :text="$t('appointments.error.ifTheProblemContinuesAndYouNeedToBook.text')"
        :aria-label="$t('appointments.error.ifTheProblemContinuesAndYouNeedToBook.label')"/>
      <error-button from="generic.tryAgain" @click="$router.go()" />
      <error-link from="generic.contactUs"
                  :action="contactUsUrl"
                  target="_blank" />
      <error-link from="generic.back"
                  :action="appointmentsPath"
                  :desktop-only="true" />
    </error-container>
    <error-container v-else id="error-dialog-unknown">
      <error-title title="apiErrors.pageHeader"
                   header="apiErrors.pageHeader" />
      <error-paragraph from="apiErrors.header" />
      <contact-111
        :text="$t('appointments.error.ifTheProblemContinuesAndYouNeedToBook.text')"
        :aria-label="$t('appointments.error.ifTheProblemContinuesAndYouNeedToBook.label')"/>
    </error-container>
  </div>
</template>

<script>
import AlternativeAppointmentActions from '@/components/appointments/AlternativeAppointmentActions';
import Contact111 from '@/components/widgets/Contact111';
import ErrorButton from '@/components/errors/ErrorButton';
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
  name: 'BookingErrors',
  components: {
    AlternativeAppointmentActions,
    Contact111,
    ErrorButton,
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
    },
    availableAppointments: {
      type: Boolean,
      default: false,
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
    errorId() {
      return this.error ? `error-dialog-${this.error.status}` : 'unknown-error';
    },
    hasConnection() {
      return !this.hasConnectionProblem();
    },
  },
};
</script>
