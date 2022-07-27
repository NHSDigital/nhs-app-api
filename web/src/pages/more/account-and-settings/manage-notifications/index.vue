<template>
  <div v-if="showTemplate">
    <p>{{ $t('notifications.notificationsTextHeading') }}
      <ul class="nhsuk-u-margin-top-3" >
        <li>{{ $t('notifications.notificationsTextListItem1') }}</li>
        <li>{{ $t('notifications.notificationsTextListItem2') }}</li>
        <li>{{ $t('notifications.notificationsTextListItem3') }}</li>
      </ul>
    </p>
    <labelled-toggle v-model="registered"
                     checkbox-id="allow_notifications"
                     :is-waiting="isWaiting"
                     :label="$t('notifications.notificationsSwitchLabel')"/>
    <div class="nhsuk-u-margin-top-4">
      <menu-item-list data-purpose="manage-notifications-menu">
        <menu-item
          id="btn_messagenotifications_example"
          data-purpose="messagenotifications-example-menu-item"
          header-tag="h2"
          :text="$t('notifications.exampleNotification')"
          :href="notificationsExamplePath"
          :click-func="navigateToExampleNotifications"
        />
        <menu-item
          id="btn_managenotification_morethanonedevice"
          data-purpose="managenotification-moredevice-menu-item"
          header-tag="h2"
          :text="$t('notifications.manageNotificationMoreThanOneDevice')"
          :href="notificationsMoreThanOneDevicePath"
          :click-func="navigateToNotificationsToMoreThanOneDevice"
        />
      </menu-item-list>
    </div>

    <p class="nhsuk-u-margin-top-4">
      <a id="app-more" href="#" @click.prevent.stop="openAppSettings">
        {{ $t('notifications.chooseHowNotificationsAreShown') }}
      </a>
    </p>
    <p>
      {{ $t('notifications.moreInformation')
      }}<analytics-tracked-tag
        :href="privacyUrl"
        :text="$t('notifications.notificationsLink')"
        class="inline"
        tag="a"
        target="_blank">{{
          $t('notifications.notificationsNhsAccountLinkText')
        }}</analytics-tracked-tag>.
    </p>
  </div>
</template>

<script>
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import CollapsibleDetails from '@/components/widgets/collapsible/CollapsibleDetails';
import LabelledToggle from '@/components/widgets/LabelledToggle';
import NativeApp from '@/services/native-app';
import MenuItem from '@/components/MenuItem';
import MenuItemList from '@/components/MenuItemList';
import {
  MORE_ACCOUNTANDSETTINGS_EXAMPLE_NOTIFICATIONS_PATH,
  MORE_ACCOUNTANDSETTINGS_MORETHAN_ONE_DEVICE_PATH,
} from '@/router/paths';
import { redirectTo } from '@/lib/utils';

export default {
  name: 'MoreAccountAndSettingsManageNotificationsPage',
  components: {
    AnalyticsTrackedTag,
    CollapsibleDetails,
    LabelledToggle,
    MenuItem,
    MenuItemList,
  },
  layout: 'nhsuk-layout',
  data() {
    return {
      privacyUrl: this.$store.$env.PRIVACY_POLICY_URL,
      notificationsExamplePath: MORE_ACCOUNTANDSETTINGS_EXAMPLE_NOTIFICATIONS_PATH,
      notificationsMoreThanOneDevicePath: MORE_ACCOUNTANDSETTINGS_MORETHAN_ONE_DEVICE_PATH,
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
        this.$store.dispatch('notifications/logAudit', {
          notificationsRegistered: !this.$store.state.notifications.registered,
          notificationsDecisionSource: 'Toggle',
        });
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
    navigateToExampleNotifications() {
      redirectTo(this, this.notificationsExamplePath);
    },
    navigateToNotificationsToMoreThanOneDevice() {
      redirectTo(this, this.notificationsMoreThanOneDevicePath);
    },
  },
};
</script>
