<template>
  <div v-if="showTemplate" data-purpose="">
    <menu-item-list data-sid="navigation-list-menu">
      <menu-item id="btn_symptoms"
                 header-tag="h2"
                 :href="symptomsPath"
                 :text="$t('appointments.guidance.menuItem1.header')"
                 :description="$t('appointments.guidance.menuItem1.text')"
                 :click-func="navigate"
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
                 :click-func="navigate"
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
                 :click-func="navigate"
                 :aria-label="ariaLabelCaption(
                   'appointments.guidance.menuItem3.header',
                   'appointments.guidance.menuItem3.text')"/>
    </menu-item-list>
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import MenuItem from '@/components/MenuItem';
import MenuItemList from '@/components/MenuItemList';
import {
  APPOINTMENT_ADMIN_HELP,
  APPOINTMENT_BOOKING_GUIDANCE,
  APPOINTMENT_GP_ADVICE,
  SYMPTOMS,
} from '@/lib/routes';
import { redirectTo } from '@/lib/utils';
import { createUri } from '@/lib/noJs';
import srjIf from '@/lib/sjrIf';

export default {
  name: 'AppointmentGuidanceMenu',
  components: {
    MenuItem,
    MenuItemList,
  },
  computed: {
    symptomsPath() {
      return SYMPTOMS.path;
    },
    adminHelpPath() {
      return createUri({
        path: APPOINTMENT_ADMIN_HELP.path,
        noJs: { onlineConsultations: { previousRoute: APPOINTMENT_BOOKING_GUIDANCE.path } },
      });
    },
    gpAdviceConditionsPath() {
      return createUri({
        path: APPOINTMENT_GP_ADVICE.path,
        noJs: { onlineConsultations: { previousRoute: APPOINTMENT_BOOKING_GUIDANCE.path } },
      });
    },
    isCdssAdmin() {
      return srjIf({ $store: this.$store, journey: 'cdssAdmin' });
    },
    isCdssAdvice() {
      return srjIf({ $store: this.$store, journey: 'cdssAdvice' });
    },
  },
  mounted() {
    document.activeElement.blur();
  },
  methods: {
    navigate(event) {
      redirectTo(this, event.currentTarget.pathname, null);
      event.preventDefault();

      if (event.currentTarget.id !== 'btn_symptoms_link') {
        this.$store.dispatch('onlineConsultations/setPreviousRoute', APPOINTMENT_BOOKING_GUIDANCE.path);
        this.$store.dispatch('navigation/setNewMenuItem', 1);
      }
    },
    ariaLabelCaption(header, body) {
      return `${this.$t(header)}. ${this.$t(body)}`;
    },
  },
};
</script>
