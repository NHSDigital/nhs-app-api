<template>
  <no-return-flow-layout>
    <div v-if="showTemplate">
      <notifications-content/>
      <primary-button id="btn_continue" @click="onContinue">
        {{ $t('generic.continue') }}
      </primary-button>
    </div>
  </no-return-flow-layout>
</template>

<script>
import PrimaryButton from '@/components/PrimaryButton';
import NoReturnFlowLayout from '@/layouts/no-return-flow-layout';
import NotificationsContent from '@/components/Notifications/NotificationsContent';
import { redirectTo } from '@/lib/utils';
import { INDEX_PATH } from '@/router/paths';
import NativeCallbacks from '@/services/native-app';
import TermsConditionsMixin from '@/components/TermsConditionsMixin';

export default {
  name: 'Index',
  components: {
    NotificationsContent,
    NoReturnFlowLayout,
    PrimaryButton,
  },
  mixins: [TermsConditionsMixin],
  async beforeCreate() {
    if (!this.$store.state.device.isNativeApp) {
      redirectTo(this, INDEX_PATH);
    }
  },
  created() {
    const { notificationCookieExists, registered } = this.$store.state.notifications;
    if (!notificationCookieExists
      && registered) {
      this.$store.dispatch('notifications/addNotificationCookie');
    }

    if (notificationCookieExists
      || registered) {
      this.$store.dispatch('notifications/logMetrics', {
        screenShown: false,
        notificationsRegistered: true,
        didErrorAttemptingToUpdateStatus: false,
      });
      redirectTo(this, INDEX_PATH);
    }
  },
  mounted() {
    NativeCallbacks.showHeaderSlim();
    NativeCallbacks.hideWhiteScreen();
  },
  methods: {
    onContinue() {
      this.$store.dispatch('notifications/addNotificationCookie');

      const { toggleUpdated } = this.$store.state.notifications;
      if (!toggleUpdated) {
        this.$store.dispatch('notifications/logMetrics', {
          screenShown: true,
          notificationsRegistered: false,
          didErrorAttemptingToUpdateStatus: false,
        });
      }

      this.conditionalRedirect();
    },
  },
};
</script>
