<template>
  <no-return-flow-layout>
    <div v-if="showTemplate">
      <div>
        <p>{{ $t('notifications.weUseNotifications') }}</p>
        <p>{{ $t('notifications.theNhsAndConnected') }}</p>
        <labelled-toggle v-model="registered"
                         checkbox-id="allow_notifications"
                         :is-waiting="isWaiting"
                         :label="$t('notifications.turnOnNotifications')"
                         :hint-text="$t('notifications.tellMeAbout')"/>
        <p>{{ $t('notifications.turnOnNotificationsForEachDevice') }}</p>
        <p>{{ $t('notifications.ifYouShareThisDevice') }}</p>
        <p>
          {{ $t('notifications.moreInformation')
          }}<analytics-tracked-tag
            :href="privacyUrl"
            :text="$t('notifications.notificationsLink')"
            class="inline"
            tag="a"
            target="_blank">{{
              $t('notifications.notificationsLink')
            }}</analytics-tracked-tag>
        </p>
      </div>
      <primary-button id="btn_continue" @click="onContinue">
        {{ $t('generic.continue') }}
      </primary-button>
    </div>
  </no-return-flow-layout>
</template>

<script>
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import LabelledToggle from '@/components/widgets/LabelledToggle';
import NativeApp from '@/services/native-app';
import NoReturnFlowLayout from '@/layouts/no-return-flow-layout';
import PrimaryButton from '@/components/PrimaryButton';
import RedirectMixin from '@/components/RedirectMixin';

export default {
  name: 'Index',
  components: {
    AnalyticsTrackedTag,
    LabelledToggle,
    NoReturnFlowLayout,
    PrimaryButton,
  },
  mixins: [RedirectMixin],
  data() {
    return {
      privacyUrl: this.$store.$env.PRIVACY_POLICY_URL,
    };
  },
  computed: {
    isWaiting() {
      return this.$store.state.notifications.isWaiting;
    },
    registered: {
      get() {
        return this.$store.state.notifications.registered;
      },
      set() {
        this.$store.dispatch('spinner/prevent', true);
        this.$store.dispatch('notifications/toggle');
      },
    },
  },
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
    NativeApp.showHeaderSlim();
    NativeApp.hideWhiteScreen();
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
