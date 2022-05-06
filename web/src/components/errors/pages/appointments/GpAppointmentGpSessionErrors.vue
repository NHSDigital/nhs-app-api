<template>
  <div>
    <message-dialog-generic v-if="!hasRetried" :id="errorId" override-style="plain">
      <error-title title="gpSessionErrors.appointments.temporaryHeader"/>
      <error-paragraph from="gpSessionErrors.appointments.youCannotBookOnline"/>
      <error-paragraph from="gpSessionErrors.appointments.temporaryProblem"/>
      <error-button from="generic.tryAgain" @click="tryAgain" />
      <error-link from="generic.back"
                  :action="backUrl"
                  :desktop-only="true"/>
    </message-dialog-generic>

    <error-page v-else
                id="alternative_actions"
                :code="error.serviceDeskReference"
                header-locale-ref="gpSessionErrors.appointments.gpAppointmentBookingUnavailable"
                :back-url="backUrl">
      <template v-slot:content>
        <p>{{ $t('gpSessionErrors.appointments.youCannotBookOnline') }}</p>
        <p>{{ $t('gpSessionErrors.appointments.contactYourSurgery') }}</p>
        <contact-111 :text="$t('gpSessionErrors.appointments.forUrgentMedicalAdvice')"/>
      </template>
      <template v-slot:actions>
        <alternative-appointment-actions/>
      </template>
    </error-page>
  </div>
</template>

<script>
import AlternativeAppointmentActions from '@/components/appointments/AlternativeAppointmentActions';
import Contact111 from '@/components/widgets/Contact111';
import ErrorButton from '@/components/errors/ErrorButton';
import ErrorLink from '@/components/errors/ErrorLink';
import ErrorPage from '@/components/errors/ErrorPage';
import ErrorPageMixin from '@/components/errors/ErrorPageMixin';
import ErrorParagraph from '@/components/errors/ErrorParagraph';
import ErrorTitle from '@/components/errors/ErrorTitle';
import MessageDialogGeneric from '@/components/widgets/MessageDialogGeneric';

import {
  APPOINTMENTS_PATH,
  GP_APPOINTMENTS_PATH,
} from '@/router/paths';
import { redirectTo, gpSessionErrorHasRetried } from '@/lib/utils';

export default {
  name: 'GpAppointmentGpSessionErrors',
  components: {
    AlternativeAppointmentActions,
    Contact111,
    ErrorButton,
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
      contactUsUrl: this.$store.$env.CONTACT_US_URL,
      errorId: `error-dialog-${this.error.status}`,
    };
  },
  computed: {
    hasRetried() {
      return gpSessionErrorHasRetried();
    },
  },
  mounted() {
    this.$store.dispatch('navigation/setRouteCrumb', 'onDemandAppointmentCrumb');
  },
  methods: {
    tryAgain() {
      sessionStorage.setItem('hasRetried', true);

      redirectTo(this, GP_APPOINTMENTS_PATH, { hr: true }, true);
    },
  },
};
</script>
