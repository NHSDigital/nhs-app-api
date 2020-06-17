<template>
  <div v-if="showTemplate" data-purpose="">
    <menu-item-list data-sid="navigation-list-menu">
      <menu-item id="btn_symptoms"
                 header-tag="h2"
                 :href="symptomsPath"
                 :text="$t('appointments.guidance.menuItem1.header')"
                 :description="$t('appointments.guidance.menuItem1.text')"
                 :click-func="goToSymptoms"
                 :aria-label="ariaLabelCaption(
                   'appointments.guidance.menuItem1.header',
                   'appointments.guidance.menuItem1.text')"/>

      <menu-item v-if="isCdssAdmin"
                 id="btn_gpHelpNoAppointment"
                 header-tag="h2"
                 data-purpose="text_link"
                 :href="adminHelpPath"
                 :text="$t('appointments.guidance.menuItem2.header')"
                 :description="$t('appointments.guidance.menuItem2.text')"
                 :click-func="goToAdminHelp"
                 :aria-label="ariaLabelCaption(
                   'appointments.guidance.menuItem2.header',
                   'appointments.guidance.menuItem2.text')"/>

      <menu-item v-if="isCdssAdvice"
                 id="btn_gpAdvice"
                 data-purpose="text_link"
                 header-tag="h2"
                 :href="gpAdviceConditionsPath"
                 :text="$t('appointments.guidance.menuItem3.header')"
                 :description="$t('appointments.guidance.menuItem3.text')"
                 :click-func="goToGpAdvice"
                 :aria-label="ariaLabelCaption(
                   'appointments.guidance.menuItem3.header',
                   'appointments.guidance.menuItem3.text')"/>
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
  SYMPTOMS_PATH,
} from '@/router/paths';

export default {
  name: 'AppointmentGuidanceMenu',
  components: {
    MenuItem,
    MenuItemList,
  },
  data() {
    return {
      symptomsPath: SYMPTOMS_PATH,
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
    goToSymptoms() {
      redirectTo(this, this.symptomsPath);
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
