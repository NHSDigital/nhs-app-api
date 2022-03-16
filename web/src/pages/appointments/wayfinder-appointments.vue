<template>
  <div v-if="showTemplate" class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full">
      <div v-if="!hasReferrals" class="nhsuk-u-padding-top-4">
        <p>
          {{ $t('appointments.wayfinder.youMayHaveOtherReferrals') }}
        </p>

        <p>
          {{ $t('appointments.wayfinder.contactTheOrganisation') }}
        </p>

        <p>
          {{ $t('appointments.wayfinder.contactTheHealthcareProvider') }}
        </p>

        <h2>
          {{ $t('appointments.wayfinder.otherReferralsAppointmentsAndServices') }}
        </h2>

        <menu-item-list>
          <third-party-jump-off-button v-if="showManageYourReferral"
                                       id="btn_manage_your_referral"
                                       provider-id="ers"
                                       :provider-configuration="thirdPartyProvider.ers.
                                         manageYourReferralWayfinder" />

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

          <third-party-jump-off-button v-if="showPkbSecondaryCareAppointments"
                                       id="btn_pkb_secondary_care_appointments"
                                       provider-id="pkb"
                                       :provider-configuration="thirdPartyProvider.pkb.
                                         appointmentsPkbSecondaryCare" />

          <third-party-jump-off-button v-if="showPkbMyCareViewAppointments"
                                       id="btn_pkb_my_care_view_appointments"
                                       provider-id="pkb"
                                       :provider-configuration="thirdPartyProvider.pkb.
                                         appointmentsPkbMyCareView" />

          <third-party-jump-off-button v-if="showGncrAppointments"
                                       id="btn_gncr_appointments"
                                       provider-id="gncr"
                                       :provider-configuration="thirdPartyProvider.gncr.
                                         appointments" />
        </menu-item-list>
      </div>
    </div>
  </div>
</template>

<script>
import { EventBus, UPDATE_HEADER, UPDATE_TITLE } from '@/services/event-bus';
import jumpOffProperties from '@/lib/third-party-providers/jump-off-configuration';
import MenuItemList from '@/components/MenuItemList';
import ThirdPartyJumpOffButton from '@/components/ThirdPartyJumpOffButton';
import sjrIf from '@/lib/sjrIf';
import { isEmptyArray } from '@/lib/utils';

const loadData = async (store) => {
  await store.dispatch('wayfinderAppointments/load');
};

export default {
  name: 'WayfinderAppointmentsPage',
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
    hasReferrals() {
      const { referrals, upcomingAppointments, pastAppointments }
        = this.$store.state.wayfinderAppointments;

      return !isEmptyArray(referrals) &&
        !isEmptyArray(upcomingAppointments) &&
        !isEmptyArray(pastAppointments);
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
    hasPkbSecondaryCareAppointments() {
      return sjrIf({
        $store: this.$store,
        journey: 'silverIntegration',
        context: {
          provider: 'pkbSecondaryCare',
          serviceType: 'secondaryAppointments',
        },
      });
    },
    hasPkbMyCareViewAppointments() {
      return sjrIf({
        $store: this.$store,
        journey: 'silverIntegration',
        context: {
          provider: 'pkbMyCareView',
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
    showPkbSecondaryCareAppointments() {
      return this.hasPkbSecondaryCareAppointments && !this.isProxying;
    },
    showPkbMyCareViewAppointments() {
      return this.hasPkbMyCareViewAppointments && !this.isProxying;
    },
  },
  async mounted() {
    await loadData(this.$store);

    const { referrals, upcomingAppointments, pastAppointments }
      = this.$store.state.wayfinderAppointments;

    if (isEmptyArray(referrals) &&
      isEmptyArray(upcomingAppointments) &&
      isEmptyArray(pastAppointments)) {
      EventBus.$emit(UPDATE_HEADER, {
        headerKey: 'appointments.wayfinder.noReferralsOrAppointments',
      });
      EventBus.$emit(UPDATE_TITLE, 'appointments.wayfinder.noReferralsOrAppointments');
    }
  },
};
</script>
