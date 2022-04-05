<template>
  <div v-if="showTemplate" class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full">
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
        <admin-help-menu-item v-if="hasCdssAdmin && !isProxying"/>
        <third-party-jump-off-button v-if="hasEngageAdmin && !isProxying"
                                     id="btn_engage_admin"
                                     provider-id="engage"
                                     :provider-configuration="thirdPartyProvider
                                       .engage.admin"/>
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

        <menu-item v-if="showWayfinder"
                   id="btn_wayfinderAppointments"
                   header-tag="h2"
                   data-purpose="text_link"
                   :href="wayfinderPath"
                   :click-func="redirectToWayfinder"
                   :text="$t('appointments.hub.wayfinder')"
                   :description="$t('appointments.hub.viewAndManageReferralsAndAppointments')"
                   :aria-label="ariaLabelCaption(
                     'appointments.hub.wayfinder',
                     'appointments.hub.viewAndManageReferralsAndAppointments')"/>
      </menu-item-list>
    </div>
  </div>
</template>

<script>
import AdminHelpMenuItem from '@/components/menuItems/AdminHelpMenuItem';
import MenuItem from '@/components/MenuItem';
import MenuItemList from '@/components/MenuItemList';
import ThirdPartyJumpOffButton from '@/components/ThirdPartyJumpOffButton';
import jumpOffProperties from '@/lib/third-party-providers/jump-off-configuration';
import {
  WAYFINDER_PATH,
  GP_APPOINTMENTS_PATH,
  HOSPITAL_APPOINTMENTS_PATH,
} from '@/router/paths';
import sjrIf from '@/lib/sjrIf';

export default {
  name: 'AppointmentsIndexPage',
  components: {
    AdminHelpMenuItem,
    MenuItem,
    MenuItemList,
    ThirdPartyJumpOffButton,
  },
  data() {
    return {
      isProxying: this.$store.getters['session/isProxying'],
      thirdPartyProvider: jumpOffProperties.thirdPartyProvider,
      test: this.$store.getters['serviceJourneyRules/silverIntegrationEnabled']({ serviceType: 'consultationsAdmin', provider: 'engage' }),
      hasCdssAdmin: sjrIf({ $store: this.$store, journey: 'cdssAdmin' }),
      hasEngageAdmin: sjrIf({
        $store: this.$store,
        journey: 'silverIntegration',
        context: {
          provider: 'engage',
          serviceType: 'consultationsAdmin',
        },
      }),
      hasWayfinder: sjrIf({ $store: this.$store, journey: 'wayfinder' }),
    };
  },
  computed: {
    gpAppointmentsPath() {
      return GP_APPOINTMENTS_PATH;
    },
    hospitalAppointmentsPath() {
      return HOSPITAL_APPOINTMENTS_PATH;
    },
    wayfinderPath() {
      return WAYFINDER_PATH;
    },
    showHospitalAppointments() {
      return !this.isProxying &&
        sjrIf({ $store: this.$store, journey: 'silverIntegrationAppointments' })
        && this.hasWayfinder === false;
    },
    showWayfinder() {
      return !this.isProxying && this.hasWayfinder;
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
    redirectToWayfinder() {
      this.$router.push(this.wayfinderPath);
    },
  },
};
</script>

<style module lang="scss" scoped>
@import "../../style/buttons";
</style>
