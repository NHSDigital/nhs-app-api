<template>
  <div v-if="showTemplate">
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
        <p>
          <a :href="coronaConditionsUrl"
             rel="noopener noreferrer"
             :aria-label="$t('appointments.book.findOutWhatToDo')">
            {{ $t('appointments.book.findOutWhatToDo') }}</a>
        </p>
        <alternative-appointment-actions :show-coronavirus-item="false"/>
      </template>
    </error-page>

    <message-dialog-generic
      v-else-if="error && (error.status === genericStatusCodes.INTERNAL_SERVER_ERROR
        || error.status === genericStatusCodes.BAD_GATEWAY)"
      :id="errorId" override-style="plain">
      <error-title title="appointments.error.cannotShowAppointments"
                   header="appointments.error.cannotShowAppointments" />
      <error-paragraph from="appointments.error.goBackAndTryAgain" />
      <error-paragraph from="appointments.error.ifYouNeedToBook" />
      <contact-111
        :text="$t('appointments.error.forUrgentMedicalAdvice.text')"
        :aria-label="$t('appointments.error.forUrgentMedicalAdvice.label')"/>
      <error-link from="appointments.error.contactWithErrorCode"
                  :action="contactUsUrl"
                  target="_blank"
                  :query-param="contactUsParam"
                  :params="{errorCode: error.serviceDeskReference}"/>
      <error-link from="generic.back"
                  :action="appointmentsPath"
                  :desktop-only="true"/>
    </message-dialog-generic>

    <message-dialog-generic
      v-else-if="error && error.status === genericStatusCodes.GATEWAY_TIMEOUT"
      :id="errorId" override-style="plain">
      <error-title title="appointments.error.cannotShowGpAppointments"/>
      <error-paragraph from="appointments.error.tryLoadingAppointmentsAgain"/>
      <error-button from="generic.tryAgain" @click="$router.go()" />
      <error-paragraph from="appointments.error.contactYourGPSurgeryDirectly"/>
      <contact-111
        :text="$t('appointments.book.forUrgentMedicalAdvice.text')"
        :aria-label="$t('appointments.book.forUrgentMedicalAdvice.label')"/>
      <error-link from="appointments.error.contactWithErrorCode"
                  :action="contactUsUrl"
                  target="_blank"
                  :query-param="contactUsParam"
                  :params="{errorCode: error.serviceDeskReference}"/>
    </message-dialog-generic>

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
import get from 'lodash/fp/get';
import AlternativeAppointmentActions from '@/components/appointments/AlternativeAppointmentActions';
import Contact111 from '@/components/widgets/Contact111';
import ErrorButton from '@/components/errors/ErrorButton';
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
    MessageDialogGeneric,
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
      coronaConditionsUrl: this.$store.$env.CORONA_CONDITIONS_URL,
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
