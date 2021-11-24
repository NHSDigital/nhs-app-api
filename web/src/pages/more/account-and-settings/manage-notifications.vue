<template>
  <div v-if="showTemplate">
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <div>
          <p>{{ $t('notifications.weUseNotifications') }}</p>
          <p>{{ $t('notifications.theNhsAndConnected') }}</p>
          <labelled-toggle v-model="registered"
                           checkbox-id="allow_notifications"
                           :is-waiting="isWaiting"
                           :label="$t('notifications.turnOnNotificationsOnThisDevice')"
                           :hint-text="$t('notifications.whenOffYouMayNotBeTold')"/>
        </div>
        <collapsible-details>
          <template slot="header">
            {{ $t('notifications.aboutNotifications') }}
          </template>
          <p>{{ $t('notifications.ifYouWantToGetNotifications') }}</p>
          <p>{{ $t('notifications.ifYouShareThisDevice') }}</p>
        </collapsible-details>
        <p>
          {{ $t('notifications.moreInformation')
          }}<analytics-tracked-tag
            :href="privacyUrl"
            :text="$t('notifications.notificationsLink')"
            class="inline"
            tag="a"
            target="_blank">{{
              $t('notifications.notificationsLink')
            }}</analytics-tracked-tag>.
        </p>
        <p>
          <a id="app-more" href="#" @click.prevent.stop="openAppSettings">
            {{ $t('notifications.chooseHowNotificationsAreShown') }}
          </a>
        </p>
      </div>
    </div>
  </div>
</template>

<script>
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import CollapsibleDetails from '@/components/widgets/collapsible/CollapsibleDetails';
import LabelledToggle from '@/components/widgets/LabelledToggle';
import NativeApp from '@/services/native-app';

export default {
  name: 'MoreAccountAndSettingsManageNotificationsPage',
  components: {
    AnalyticsTrackedTag,
    CollapsibleDetails,
    LabelledToggle,
  },
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
  watch: {
    '$route.query.ts': async function watchTimestamp() {
      await this.$store.dispatch('notifications/load');
    },
  },
  created() {
    this.$store.dispatch('notifications/load');
  },
  methods: {
    openAppSettings() {
      NativeApp.openAppSettings();
    },
  },
};
</script>
