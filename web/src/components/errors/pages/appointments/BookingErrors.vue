<template>
  <div v-if="hasConnection">
    <error-page v-if="error.status === genericStatusCodes.FORBIDDEN"
                header-locale-ref="forbiddenErrors.appointments.gpAppointmentBookingUnavailable"
                :back-url="appointmentsPath">
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

    <error-container
      v-else-if="error.status === genericStatusCodes.INTERNAL_SERVER_ERROR
        || error.status === genericStatusCodes.BAD_GATEWAY"
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
      v-else-if="error.status === genericStatusCodes.GATEWAY_TIMEOUT"
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
  name: 'BookingErrors',
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
    hasConnection() {
      return !this.hasConnectionProblem();
    },
  },
};
</script>
