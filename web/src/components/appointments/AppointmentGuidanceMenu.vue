<template>
  <div v-if="showTemplate" :class="$style['no-padding']" data-purpose="">
    <ul :class="[$style['list-menu'], !$store.state.device.isNativeApp && $style.desktopWeb]"
        role="list">
      <li role="link">
        <analytics-tracked-tag
          id="btn_symptoms"
          :tabindex="-1"
          :text="$t('appointments.guidance.menuItem1.header')"
          :aria-label="`${$t('appointments.guidance.menuItem1.header')}.
            ${$t('appointments.guidance.menuItem1.text')}`"
          data-purpose="text_link">
          <a id="btn_symptoms_link"
             :href="symptomsPath"
             :class="$style['no-decoration']"
             @click="navigate($event)">
            <h2>{{ $t('appointments.guidance.menuItem1.header') }}</h2>
            <p :class="!$store.state.device.isNativeApp && $style.desktopWeb">
              {{ $t('appointments.guidance.menuItem1.text') }}</p>
          </a>
        </analytics-tracked-tag>
      </li>
      <sjr-if journey="cdssAdmin" tag="li" role="link">
        <analytics-tracked-tag
          id="btn_gpHelpNoAppointment"
          :tabindex="-1"
          :text="$t('appointments.guidance.menuItem2.header')"
          :aria-label="`${$t('appointments.guidance.menuItem2.header')}.
            ${$t('appointments.guidance.menuItem2.text')}`"
          data-purpose="text_link">
          <a id="btn_gp_advice"
             :href="adminHelpPath"
             @click="navigate($event)">
            <h2>{{ $t('appointments.guidance.menuItem2.header') }}</h2>
            <p :class="!$store.state.device.isNativeApp && $style.desktopWeb">
              {{ $t('appointments.guidance.menuItem2.text') }}</p>
          </a>
        </analytics-tracked-tag>
      </sjr-if>
      <sjr-if journey="cdssAdvice" tag="li" role="link">
        <analytics-tracked-tag
          id="btn_gpAdvice"
          :tabindex="-1"
          :text="$t('appointments.guidance.menuItem3.header')"
          :aria-label="`${$t('appointments.guidance.menuItem3.header')}.
            ${$t('appointments.guidance.menuItem3.text')}`"
          data-purpose="text_link">
          <a id="btn_gp_help"
             :href="gpAdvicePath"
             :class="$style['no-decoration']"
             @click="navigate($event)">
            <h2>{{ $t('appointments.guidance.menuItem3.header') }}</h2>
            <p :class="!$store.state.device.isNativeApp && $style.desktopWeb">
              {{ $t('appointments.guidance.menuItem3.text') }}</p>
          </a>
        </analytics-tracked-tag>
      </sjr-if>
    </ul>
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import { SYMPTOMS, APPOINTMENT_ADMIN_HELP, APPOINTMENT_BOOKING_GUIDANCE, APPOINTMENT_GP_ADVICE } from '@/lib/routes';
import { redirectTo } from '@/lib/utils';
import { createUri } from '@/lib/noJs';
import SjrIf from '@/components/SjrIf';

export default {
  name: 'AppointmentGuidanceMenu',
  components: {
    AnalyticsTrackedTag,
    SjrIf,
  },
  computed: {
    symptomsPath() {
      return SYMPTOMS.path;
    },
    adminHelpPath() {
      const noJsData = {
        onlineConsultations: {
          previousRoute: APPOINTMENT_BOOKING_GUIDANCE.path,
        },
      };
      return createUri({ path: APPOINTMENT_ADMIN_HELP.path, noJs: noJsData });
    },
    gpAdvicePath() {
      return APPOINTMENT_GP_ADVICE.path;
    },
  },
  mounted() {
    document.activeElement.blur();
  },
  methods: {
    navigate(event) {
      let header;
      let title;
      redirectTo(this, event.currentTarget.pathname, null);
      event.preventDefault();

      if (event.currentTarget.pathname === APPOINTMENT_ADMIN_HELP.path) {
        if (event.currentTarget.id === 'btn_gp_help') {
          header = this.$t('pageHeaders.appointmentAdminHelp');
          title = this.$t('pageTitles.appointmentAdminHelp');
          this.$store.dispatch('onlineConsultations/setPreviousRoute', APPOINTMENT_BOOKING_GUIDANCE.path);
        } else {
          header = this.$t('pageHeaders.appointmentGpAdvice');
          title = this.$t('pageTitles.appointmentGpAdvice');
        }
        this.$store.dispatch('header/updateHeaderText', header);
        this.$store.dispatch('pageTitle/updatePageTitle', title);
        this.$store.dispatch('navigation/setNewMenuItem', 1);
      }
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import '../../style/buttons';
  @import '../../style/listmenu';
  @import "../../style/desktopWeb/accessibility";

  .list-menu a {
    outline: 0;

    &:focus {
      @include outlineStyle;
    }

    &:hover {
      @include outlineStyleLight;
    }
  }

  .no-decoration {
    text-decoration: none;
  }

  .no-padding {
    margin-top: -0.5em;
    margin-left: -1em;
    margin-right: -1em;
  }

</style>
