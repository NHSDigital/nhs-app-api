<template>
  <div v-if="showTemplate">
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
        <alternative-appointment-actions/>
      </template>
    </error-page>

    <message-dialog-generic v-else-if="error.status === genericStatusCodes.INTERNAL_SERVER_ERROR
      || error.status === genericStatusCodes.BAD_GATEWAY" :id="errorId" override-style="plain">
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
    </message-dialog-generic>

    <message-dialog-generic v-else-if="error.status === genericStatusCodes.GATEWAY_TIMEOUT"
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

    <div v-else-if="error.status === appointmentStatusCodes.GP_SESSION_ERROR">
      <gp-appointment-gp-session-errors :error="error" />
    </div>

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
import ErrorButton from '@/components/errors/ErrorButton';
import ErrorContainer from '@/components/errors/ErrorContainer';
import ErrorLink from '@/components/errors/ErrorLink';
import ErrorPage from '@/components/errors/ErrorPage';
import ErrorPageMixin from '@/components/errors/ErrorPageMixin';
import ErrorParagraph from '@/components/errors/ErrorParagraph';
import ErrorTitle from '@/components/errors/ErrorTitle';
import GpAppointmentGpSessionErrors from '@/components/errors/pages/appointments/GpAppointmentGpSessionErrors';
import MessageDialogGeneric from '@/components/widgets/MessageDialogGeneric';
import ReportAProblem from '@/components/errors/ReportAProblem';

import {
  APPOINTMENTS_PATH,
  GP_APPOINTMENTS_PATH,
} from '@/router/paths';

import genericStatus from '@/components/errors/statusCodes/GenericStatusCodes';
import appointmentStatus from '@/components/errors/statusCodes/AppointmentCustomStatusCodes';

export default {
  name: 'GpAppointmentErrors',
  components: {
    AlternativeAppointmentActions,
    Contact111,
    ErrorButton,
    ErrorContainer,
    ErrorLink,
    ErrorPage,
    ErrorParagraph,
    ErrorTitle,
    GpAppointmentGpSessionErrors,
    MessageDialogGeneric,
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
    contactUsParam() {
      return {
        ErrCodeParam: 'errorcode',
        ErrCodeValue: this.serviceDeskReference,
      };
    },
    serviceDeskReference() {
      return get('$store.state.myAppointments.error.serviceDeskReference')(this) || '';
    },
  },
};
</script>
