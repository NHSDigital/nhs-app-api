<template>
  <div v-if="showTemplate" class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full">
      <div v-if="showTemplate" data-purpose="">
        <menu-item-list>
          <menu-item id="btn_choices"
                     header-tag="h2"
                     data-purpose="text_link"
                     :href="gpAppointmentsPath"
                     :click-func="redirectToGpAppointments"
                     :text="$t('appointments.hub.gpSurgeryAppointments')"
                     :description="$t('appointments.hub.viewAndManageAppointmentsAtYourSurgery')"
                     :aria-label="ariaLabelCaption(
                       'appointments.hub.gpSurgeryAppointments',
                       'appointments.hub.viewAndManageAppointmentsAtYourSurgery')"/>
          <menu-item v-if="showHospitalAppointments"
                     id="btn_hospital"
                     header-tag="h2"
                     data-purpose="text_link"
                     :href="hospitalAppointmentsPath"
                     :click-func="redirectToHospitalAppointments"
                     :text="$t('appointments.hub.hospitalAndOtherAppointments')"
                     :description="$t('appointments.hub.viewAndManageAppointmentsLikeReferrals')"
                     :aria-label="ariaLabelCaption(
                       'appointments.hub.hospitalAndOtherAppointments',
                       'appointments.hub.viewAndManageAppointmentsLikeReferrals')"/>
        </menu-item-list>
      </div>
    </div>
  </div>
</template>

<script>
import MenuItem from '@/components/MenuItem';
import MenuItemList from '@/components/MenuItemList';
import {
  GP_APPOINTMENTS_PATH,
  HOSPITAL_APPOINTMENTS_PATH,
} from '@/router/paths';
import sjrIf from '@/lib/sjrIf';

export default {
  name: 'AppointmentsIndexPage',
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
      return GP_APPOINTMENTS_PATH;
    },
    hospitalAppointmentsPath() {
      return HOSPITAL_APPOINTMENTS_PATH;
    },
    showHospitalAppointments() {
      return !this.isProxying &&
        sjrIf({ $store: this.$store, journey: 'silverIntegrationAppointments' });
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
  },
};
</script>

<style module lang="scss" scoped>
@import "../../style/buttons";
</style>
