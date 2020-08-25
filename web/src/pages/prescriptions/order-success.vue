<template>
  <div v-if="showTemplate">
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <div v-if="isProxying">
          <p>
            {{ $t('prescriptions.orderSuccess.youHaveOrderedOnBehalfOf', { name, givenName }) }}
          </p>
          <switch-profile-button />
        </div>
        <div v-else>
          <p>
            {{ $t('prescriptions.orderSuccess.theOrderStatusWillBeUpdated') }}
          </p>
          <desktopGenericBackLink
            :path="backPath"
            :button-text="'prescriptions.orderSuccess.goToYourPrescriptionOrders'"
            @clickAndPrevent="backButtonClicked"/>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import SwitchProfileButton from '@/components/switch-profile/SwitchProfileButton';
import get from 'lodash/fp/get';
import { PRESCRIPTIONS_VIEW_ORDERS_PATH } from '@/router/paths';
import { redirectTo } from '@/lib/utils';

export default {
  name: 'PrescriptionOrderSuccess',
  components: {
    DesktopGenericBackLink,
    SwitchProfileButton,
  },
  data() {
    return {
      isProxying: this.$store.getters['session/isProxying'],
      name: get('$store.state.linkedAccounts.actingAsUser.fullName', this),
      givenName: get('$store.state.linkedAccounts.actingAsUser.givenName', this),
      backPath: PRESCRIPTIONS_VIEW_ORDERS_PATH,
    };
  },
  mounted() {
    this.$store.dispatch('device/unlockNavBar');
    this.$store.dispatch('repeatPrescriptionCourses/completeOrderJourney');
  },
  methods: {
    backButtonClicked() {
      redirectTo(this, this.backPath, null);
    },
  },
};
</script>
