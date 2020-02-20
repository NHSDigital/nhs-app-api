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
        </menu-item-list>
      </div>
    </div></div></template>

<script>

import { GP_APPOINTMENTS } from '@/lib/routes';
import MenuItem from '@/components/MenuItem';
import MenuItemList from '@/components/MenuItemList';

export default {
  name: 'Appointments',
  layout: 'nhsuk-layout',
  components: {
    MenuItem,
    MenuItemList,
  },
  computed: {
    gpAppointmentsPath() {
      return GP_APPOINTMENTS.path;
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
    preventDefault() {
      return true;
    },
  },
};
</script>

<style module lang="scss" scoped>
@import "../../style/buttons";
</style>
