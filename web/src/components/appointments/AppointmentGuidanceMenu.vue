<template>
  <div v-if="showTemplate" data-purpose="">
    <menu-item-list data-sid="navigation-list-menu">
      <menu-item
        id="btn_advice"
        header-tag="h2"
        :href="advicePath"
        :text="$t('appointments.guidance.healthAdvice.getHealthAdvice')"
        :description="$t('appointments.guidance.healthAdvice.findInformationAboutConditions')"
        :click-func="goToAdvice"
        :aria-label="ariaLabelCaption(
          'appointments.guidance.healthAdvice.getHealthAdvice',
          'appointments.guidance.healthAdvice.findInformationAboutConditions')"/>

      <menu-item
        v-if="isCdssAdmin"
        id="btn_gpHelpNoAppointment"
        header-tag="h2"
        data-purpose="text_link"
        :href="adminHelpPath"
        :text="$t('appointments.guidance.additionalGpServices.additionalGpServices')"
        :description="$t('appointments.guidance.additionalGpServices.getSickNotesAndLetters')"
        :click-func="goToAdminHelp"
        :aria-label="ariaLabelCaption(
          'appointments.guidance.additionalGpServices.additionalGpServices',
          'appointments.guidance.additionalGpServices.getSickNotesAndLetters')"/>

      <menu-item
        v-if="isCdssAdvice"
        id="btn_gpAdvice"
        data-purpose="text_link"
        header-tag="h2"
        :href="gpAdviceConditionsPath"
        :text="$t('appointments.guidance.askGp.forAdvice')"
        :description="$t('appointments.guidance.askGp.consultThroughOnlineForm')"
        :click-func="goToGpAdvice"
        :aria-label="ariaLabelCaption(
          'appointments.guidance.askGp.forAdvice',
          'appointments.guidance.askGp.consultThroughOnlineForm')"/>
    </menu-item-list>
  </div>
</template>

<script>
import MenuItem from '@/components/MenuItem';
import MenuItemList from '@/components/MenuItemList';
import { redirectTo } from '@/lib/utils';
import sjrIf from '@/lib/sjrIf';
import {
  APPOINTMENT_BOOKING_GUIDANCE_PATH,
  APPOINTMENT_ADMIN_HELP_PATH,
  APPOINTMENT_GP_ADVICE_PATH,
  ADVICE_PATH,
} from '@/router/paths';

export default {
  name: 'AppointmentGuidanceMenu',
  components: {
    MenuItem,
    MenuItemList,
  },
  data() {
    return {
      advicePath: ADVICE_PATH,
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
  mounted() {
    document.activeElement.blur();
  },
  methods: {
    setOlcNavigationContext() {
      this.$store.dispatch('onlineConsultations/setPreviousRoute', APPOINTMENT_BOOKING_GUIDANCE_PATH);
      this.$store.dispatch('navigation/setNewMenuItem', 1);
      this.$store.dispatch('navigation/setBackLinkOverride', APPOINTMENT_BOOKING_GUIDANCE_PATH);
    },
    goToAdvice() {
      redirectTo(this, this.advicePath);
    },
    goToAdminHelp() {
      this.setOlcNavigationContext();
      redirectTo(this, this.adminHelpPath);
    },
    goToGpAdvice() {
      this.setOlcNavigationContext();
      redirectTo(this, this.gpAdviceConditionsPath);
    },
    ariaLabelCaption(header, body) {
      return `${this.$t(header)}. ${this.$t(body)}`;
    },
  },
};
</script>
