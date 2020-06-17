<template xmlns:v-if="http://www.w3.org/1999/xhtml">
  <div v-if="showTemplate">
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <p>{{ $t('account.notifications.youCanChoose') }}</p>
        <p>{{ $t('account.notifications.ifYouShare.prefix') }}<analytics-tracked-tag
          :href="privacyPolicyURL"
          :text="$t('account.notifications.ifYouShare.linkText')"
          class="inline"
          tag="a"
          target="_blank">{{
            $t('account.notifications.ifYouShare.linkText')
          }}</analytics-tracked-tag>.
        </p>
        <labelled-toggle v-model="registered"
                         checkbox-id="allow_notifications"
                         :is-waiting="isWaiting"
                         :label="$t('account.notifications.toggleLabel')"
                         :hint-text="$t('account.notifications.toggleHint')"/>
        <p>
          <a id="app-settings" href="#" @click.prevent.stop="openAppSettings">
            {{ $t('account.notifications.settingsLinkText') }}
          </a>
        </p>
      </div>
    </div>
  </div>
</template>

<script>
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import LabelledToggle from '@/components/widgets/LabelledToggle';
import NativeApp from '@/services/native-app';
import {
  PRIVACY_POLICY_URL,
} from '@/router/externalLinks';

export default {
  name: 'AccountNotificationsPage',
  components: {
    AnalyticsTrackedTag,
    LabelledToggle,
  },
  data() {
    return {
      privacyPolicyURL: PRIVACY_POLICY_URL,
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
