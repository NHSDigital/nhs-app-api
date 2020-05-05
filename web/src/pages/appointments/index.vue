<template>
  <div class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full">
      <div v-if="showTemplate" data-purpose="">
        <menu-item-list>
          <menu-item id="btn_choices"
                     header-tag="h2"
                     role="link"
                     data-purpose="text_link"
                     :href="gpAppointmentsPath"
                     :click-func="redirectToGpAppointments"
                     :description="$t('appointmentHubPage.gpSurgeryAppointments.body')"
                     :text="$t('appointmentHubPage.gpSurgeryAppointments.subheader')"
                     :aria-label="ariaLabelCaption(
                       'appointmentHubPage.gpSurgeryAppointments.subheader',
                       'appointmentHubPage.gpSurgeryAppointments.body')"
                     :prevent-default="preventDefault()"/>
          <menu-item v-if="showHospitalAppointments"
                     id="btn_hospital"
                     header-tag="h2"
                     role="link"
                     data-purpose="text_link"
                     :href="hospitalAppointmentsPath"
                     :click-func="redirectToHospitalAppointments"
                     :description="$t('appointmentHubPage.hospitalAppointments.body')"
                     :text="$t('appointmentHubPage.hospitalAppointments.subheader')"
                     :aria-label="ariaLabelCaption(
                       'appointmentHubPage.hospitalAppointments.subheader',
                       'appointmentHubPage.hospitalAppointments.body')"
                     :prevent-default="preventDefault()"/>
        </menu-item-list>
      </div>
    </div>
  </div>
</template>

<script>

import { GP_APPOINTMENTS, HOSPITAL_APPOINTMENTS } from '@/lib/routes';
import MenuItem from '@/components/MenuItem';
import MenuItemList from '@/components/MenuItemList';
import sjrIf from '@/lib/sjrIf';

export default {
  name: 'Appointments',
  layout: 'nhsuk-layout',
  components: {
    MenuItem,
    MenuItemList,
  },
  data() {
    return {
      isProxying: this.$store.getters['session/isProxying'],
    };
  },
  computed: {
    gpAppointmentsPath() {
      return GP_APPOINTMENTS.path;
    },
    hospitalAppointmentsPath() {
      return HOSPITAL_APPOINTMENTS.path;
    },
    hasPkbAppointments() {
      return sjrIf({
        $store: this.$store,
        journey: 'silverIntegration',
        context: {
          provider: 'pkb',
          serviceType: 'secondaryAppointments',
        },
      });
    },
    hasErsAppointments() {
      return sjrIf({
        $store: this.$store,
        journey: 'silverIntegration',
        context: {
          provider: 'ers',
          serviceType: 'secondaryAppointments',
        },
      });
    },
    isNativeApp() {
      return this.$store.state.device.isNativeApp;
    },
    showHospitalAppointments() {
      return !this.isProxying && (this.hasErsAppointments ||
        (this.hasPkbAppointments && this.isNativeApp));
    },
  },
  mounted() {
    this.$store.dispatch('device/unlockNavBar');
  },
  methods: {
    ariaLabelCaption(header, body) {
      return `${this.$t(header)}. ${this.$t(body)}`;
    },
    redirectToGpAppointments() {
      this.$router.push(this.gpAppointmentsPath);
    },
    redirectToHospitalAppointments() {
      this.$router.push(this.hospitalAppointmentsPath);
    },
    preventDefault() {
      return true;
    },
  },
};
</script>

<style module lang="scss" scoped>
@import "../../style/buttons";
</style>
