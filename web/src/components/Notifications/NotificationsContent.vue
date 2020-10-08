<template>
  <div>
    <p>{{ $t('notifications.manageNotifications.notificationsMayInclude') }}</p>
    <p>{{ $t('notifications.manageNotifications.ifYouShareThisDevice') }}</p>
    <p>
      {{ $t('notifications.manageNotifications.moreInformation')
      }}<analytics-tracked-tag
        :href="privacyURl"
        :text="$t('notifications.manageNotifications.notificationsLink')"
        class="inline"
        tag="a"
        target="_blank">{{
          $t('notifications.manageNotifications.notificationsLink')
        }}</analytics-tracked-tag>.
    </p>
    <labelled-toggle v-model="registered"
                     checkbox-id="allow_notifications"
                     :is-waiting="isWaiting"
                     :label="$t('notifications.manageNotifications.allowNotifications')"
                     :hint-text="$t('notifications.manageNotifications.iAcceptNotifications')"/>
  </div>
</template>

<script>
import LabelledToggle from '@/components/widgets/LabelledToggle';
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import {
  PRIVACY_POLICY_URL,
} from '@/router/externalLinks';

export default {
  name: 'NotificationsContent',
  components: {
    LabelledToggle,
    AnalyticsTrackedTag,
  },
  data() {
    return {
      privacyURl: PRIVACY_POLICY_URL,
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
};
</script>

<style scoped>

</style>
