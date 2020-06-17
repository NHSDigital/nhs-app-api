<template>
  <div v-if="showTemplate" class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full">
      <menu-item-list>
        <third-party-jump-off-button v-if="showManageYourReferral"
                                     id="btn_manage_your_referral"
                                     provider-id="ers"
                                     :provider-configuration="thirdPartyProvider.ers.
                                       manageYourReferral" />
        <third-party-jump-off-button v-if="showPkbAppointments"
                                     id="btn_pkb_appointments"
                                     provider-id="pkb"
                                     :provider-configuration="thirdPartyProvider.pkb.
                                       appointments" />
        <third-party-jump-off-button v-if="showPkbCieAppointments"
                                     id="btn_pkb_cie_appointments"
                                     provider-id="pkb"
                                     :provider-configuration="thirdPartyProvider.pkb.
                                       appointmentsCie" />
        <third-party-jump-off-button v-if="showGncrAppointments"
                                     id="btn_gncr_appointments"
                                     provider-id="gncr"
                                     :provider-configuration="thirdPartyProvider.gncr.
                                       appointments" />
      </menu-item-list>
    </div>
  </div>
</template>

<script>
import MenuItemList from '@/components/MenuItemList';
import ThirdPartyJumpOffButton from '@/components/ThirdPartyJumpOffButton';

import sjrIf from '@/lib/sjrIf';
import jumpOffProperties from '@/lib/third-party-providers/jump-off-configuration';

export default {
  name: 'HospitalAppointmentsPage',
  components: {
    MenuItemList,
    ThirdPartyJumpOffButton,
  },
  data() {
    return {
      isProxying: this.$store.getters['session/isProxying'],
      thirdPartyProvider: jumpOffProperties.thirdPartyProvider,
    };
  },
  computed: {
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
    hasGncrAppointments() {
      return sjrIf({
        $store: this.$store,
        journey: 'silverIntegration',
        context: {
          provider: 'gncr',
          serviceType: 'secondaryAppointments',
        },
      });
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
    hasPkbCieAppointments() {
      return sjrIf({
        $store: this.$store,
        journey: 'silverIntegration',
        context: {
          provider: 'pkbCie',
          serviceType: 'secondaryAppointments',
        },
      });
    },
    showGncrAppointments() {
      return this.hasGncrAppointments && !this.isProxying;
    },
    showManageYourReferral() {
      return this.hasErsAppointments && !this.isProxying;
    },
    showPkbAppointments() {
      return this.hasPkbAppointments && !this.isProxying;
    },
    showPkbCieAppointments() {
      return this.hasPkbCieAppointments && !this.isProxying;
    },
  },
  mounted() {
    this.$store.dispatch('device/unlockNavBar');
  },
};
</script>

<style module lang="scss" scoped>
@import "../../style/buttons";
</style>
