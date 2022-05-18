<template>
  <div v-if="anyAvailableServices">
    <h2 id="other-services-header">
      {{ $t('wayfinder.otherServices') }}
    </h2>

    <menu-item-list>
      <third-party-jump-off-button
        v-if="showManageYourReferral"
        id="btn_manage_your_referral"
        provider-id="ers"
        :provider-configuration="thirdPartyProvider.ers.
          manageYourReferralWayfinder" />

      <third-party-jump-off-button
        v-if="showPkbAppointments"
        id="btn_pkb_appointments"
        provider-id="pkb"
        :provider-configuration="thirdPartyProvider.pkb.
          appointments" />

      <third-party-jump-off-button
        v-if="showGncrAppointments"
        id="btn_gncr_appointments"
        provider-id="gncr"
        :provider-configuration="thirdPartyProvider.gncr.
          appointmentsWayfinder" />
    </menu-item-list>

  </div>
</template>

<script>
import jumpOffProperties from '@/lib/third-party-providers/jump-off-configuration';
import MenuItemList from '@/components/MenuItemList';
import ThirdPartyJumpOffButton from '@/components/ThirdPartyJumpOffButton';
import sjrIf from '@/lib/sjrIf';

export default {
  name: 'OtherAvailableServicesMenuItems',
  components: {
    MenuItemList,
    ThirdPartyJumpOffButton,
  },
  props: {
    showErs: {
      type: Boolean,
      default: () => true,
    },
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
    showGncrAppointments() {
      return this.hasGncrAppointments && !this.isProxying;
    },
    showManageYourReferral() {
      return this.hasErsAppointments && this.showErs && !this.isProxying;
    },
    showPkbAppointments() {
      return this.hasPkbAppointments && !this.isProxying;
    },
    anyAvailableServices() {
      return this.showGncrAppointments
        || this.showManageYourReferral
        || this.showPkbAppointments;
    },
  },
};
</script>
