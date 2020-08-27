<template>
  <gp-session-error :code="code"
                    area="appointments"
                    :back-url="backUrl">
    <template v-slot:content>
      <p>{{ $t('gpSessionErrors.appointments.youCannotBookOnline') }}</p>
      <p>{{ $t('gpSessionErrors.appointments.ifTheProblemContinues') }}
        <a href="https://111.nhs.uk" target="_blank" rel="noopener noreferrer"
           style="display:inline">
          {{ $t('gpSessionErrors.nhs111Link') }}</a>
        {{ $t('gpSessionErrors.orCall') }}
      </p>
    </template>
    <template v-slot:items>
      <menu-item
        id="btn_corona"
        header-tag="h2"
        :href="coronaCheckerUrl"
        :description="$t('sy01.corona.body')"
        target="_blank"
        :text="$t('sy01.corona.subheader')"
        :aria-label="ariaLabelCaption(
          'sy01.corona.subheader',
          'sy01.corona.body')"/>
      <menu-item
        v-if="isCdssAdvice"
        id="btn_gpAdvice"
        data-purpose="text_link"
        header-tag="h2"
        :href="gpAdviceConditionsPath"
        :text="$t('appointments.guidance.askGp.forAdvice')"
        :description="$t('appointments.guidance.askGp.consultThroughOnlineForm')"
        :click-func="navigate"
        :click-param="gpAdviceConditionsPath"
        :aria-label="ariaLabelCaption(
          'appointments.guidance.askGp.forAdvice',
          'appointments.guidance.askGp.consultThroughOnlineForm')"/>
      <menu-item
        v-if="isCdssAdmin"
        id="btn_gpHelpNoAppointment"
        header-tag="h2"
        data-purpose="text_link"
        :href="adminHelpPath"
        :text="$t('appointments.guidance.additionalGpServices.additionalGpServices')"
        :description="$t('appointments.guidance.additionalGpServices.getSickNotesAndLetters')"
        :click-func="navigate"
        :click-param="adminHelpPath"
        :aria-label="ariaLabelCaption(
          'appointments.guidance.additionalGpServices.header',
          'appointments.guidance.additionalGpServices.text')"/>
      <menu-item
        id="btn_111"
        header-tag="h2"
        :href="symptomsCheckerUrl"
        target="_blank"
        :text="$t('appointments.guidance.OneOneOne.use111Online')"
        :description="$t('appointments.guidance.OneOneOne.checkIfYouNeedUrgentHelp')"
        :aria-label="ariaLabelCaption(
          'appointments.guidance.OneOneOne.useOneOneOneOnline',
          'appointments.guidance.OneOneOne.checkIfYouNeedUrgentHelp')"/>
    </template>
  </gp-session-error>
</template>

<script>
import MenuItem from '@/components/MenuItem';
import GpSessionError from '@/components/errors/GPSessionError';
import sjrIf from '@/lib/sjrIf';
import {
  APPOINTMENTS_PATH,
  APPOINTMENT_BOOKING_GUIDANCE_PATH,
  APPOINTMENT_ADMIN_HELP_PATH,
  APPOINTMENT_GP_ADVICE_PATH,
} from '@/router/paths';
import { redirectTo } from '@/lib/utils';
import {
  SYMPTOM_CHECKER_NATIVE_QUERY_PARAMS,
} from '@/router/externalLinks';

export default {
  name: 'AppointmentsGpSessionError',
  components: {
    MenuItem,
    GpSessionError,
  },
  props: {
    code: {
      type: String,
      default: undefined,
    },
  },
  data() {
    let symptomsCheckerUrl = this.$store.$env.SYMPTOM_CHECKER_URL;
    if (this.$store.state.device.isNativeApp) {
      symptomsCheckerUrl += SYMPTOM_CHECKER_NATIVE_QUERY_PARAMS;
    }
    return {
      backUrl: APPOINTMENTS_PATH,
      symptomsCheckerUrl,
      coronaCheckerUrl: this.$store.$env.CORONA_SERVICE_URL,
      adminHelpPath: APPOINTMENT_ADMIN_HELP_PATH,
      gpAdviceConditionsPath: APPOINTMENT_GP_ADVICE_PATH,
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
    ariaLabelCaption(header, body) {
      return `${this.$t(header)}. ${this.$t(body)}`;
    },
    navigate(path) {
      this.$store.dispatch('onlineConsultations/setPreviousRoute', APPOINTMENT_BOOKING_GUIDANCE_PATH);
      redirectTo(this, path);
    },
  },
};
</script>
