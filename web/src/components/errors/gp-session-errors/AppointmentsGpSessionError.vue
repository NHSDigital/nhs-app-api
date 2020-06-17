<template>
  <gp-session-error v-if="true" :code="code" area="appointments" :back-url="backUrl">
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
      <menu-item id="btn_corona"
                 header-tag="h2"
                 :href="coronaCheckerUrl"
                 :description="$t('sy01.corona.body')"
                 target="_blank"
                 :text="$t('sy01.corona.subheader')"
                 :aria-label="ariaLabelCaption(
                   'sy01.corona.subheader',
                   'sy01.corona.body')"/>
      <menu-item v-if="isCdssAdvice"
                 id="btn_gpAdvice"
                 data-purpose="text_link"
                 header-tag="h2"
                 :href="gpAdviceConditionsPath"
                 :text="$t('appointments.guidance.menuItem3.header')"
                 :description="$t('appointments.guidance.menuItem3.text')"
                 :click-func="navigate"
                 :click-param="gpAdviceConditionsPath"
                 :aria-label="ariaLabelCaption(
                   'appointments.guidance.menuItem3.header',
                   'appointments.guidance.menuItem3.text')"/>
      <menu-item v-if="isCdssAdmin"
                 id="btn_gpHelpNoAppointment"
                 header-tag="h2"
                 data-purpose="text_link"
                 :href="adminHelpPath"
                 :text="$t('appointments.guidance.menuItem2.header')"
                 :description="$t('appointments.guidance.menuItem2.text')"
                 :click-func="navigate"
                 :click-param="adminHelpPath"
                 :aria-label="ariaLabelCaption(
                   'appointments.guidance.menuItem2.header',
                   'appointments.guidance.menuItem2.text')"/>
      <menu-item id="btn_111"
                 header-tag="h2"
                 :href="symptomsCheckerUrl"
                 :description="$t('sy01.111.body')"
                 target="_blank"
                 :text="$t('sy01.111.subheader')"
                 :aria-label="ariaLabelCaption(
                   'sy01.111.subheaderAriaLabel',
                   'sy01.111.body')"/>
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
  SYMPTOM_CHECKER_URL,
  SYMPTOM_CHECKER_NATIVE_QUERY_PARAMS,
  CORONA_SERVICE_URL,
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
    let symptomsCheckerUrl = SYMPTOM_CHECKER_URL;
    if (this.$store.state.device.isNativeApp) {
      symptomsCheckerUrl += SYMPTOM_CHECKER_NATIVE_QUERY_PARAMS;
    }
    return {
      backUrl: APPOINTMENTS_PATH,
      symptomsCheckerUrl,
      coronaCheckerUrl: CORONA_SERVICE_URL,
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
  created() {
    this.$store.dispatch('header/updateHeaderText',
      this.$t('gpSessionErrors.appointments.header'));
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
