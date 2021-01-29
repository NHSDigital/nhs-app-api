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
import NativeCallbacks from '@/services/native-app';
import RedirectMixin from '@/components/RedirectMixin';

export default {
  name: 'Index',
  components: {
    NotificationsContent,
    NoReturnFlowLayout,
    PrimaryButton,
  },
  mixins: [RedirectMixin],
  created() {
    const { notificationCookieExists, registered } = this.$store.state.notifications;
    if (!notificationCookieExists && registered) {
      this.$store.dispatch('notifications/addNotificationCookie');
    }

    if (notificationCookieExists || registered) {
      this.$store.dispatch('notifications/logMetrics', {
        screenShown: false,
        notificationsRegistered: true,
        didErrorAttemptingToUpdateStatus: false,
      });
      this.conditionalRedirect();
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
