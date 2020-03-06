<template>
  <div class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full">
      <div v-if="showTemplate" data-purpose="">
        <menu-item-list>
          <third-party-jump-off-button v-if="showManageYourReferral"
                                       id="btn_manage_your_referral"
                                       provider-id="ers"
                                       :jump-off-type="thirdPartyProvider.ers.
                                         manageYourReferral.type"
                                       :redirect-path="thirdPartyProvider.ers.
                                         manageYourReferral.redirectPath" />
        </menu-item-list>
      </div>
    </div>
  </div>
</template>

<script>

import MenuItemList from '@/components/MenuItemList';
import ThirdPartyJumpOffButton from '@/components/ThirdPartyJumpOffButton';
import sjrIf from '@/lib/sjrIf';
import jumpOffProperties from '@/lib/third-party-providers/jump-off-configuration';

export default {
  name: 'HospitalAppointments',
  layout: 'nhsuk-layout',
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
    hasSecondaryAppointments() {
      return sjrIf({
        $store: this.$store,
        journey: 'silverIntegration',
        context: {
          provider: 'ers',
          serviceType: 'secondaryAppointments',
        },
      });
    },
    showManageYourReferral() {
      return this.hasSecondaryAppointments && !this.isProxying;
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
