<template>
  <div v-if="showTemplate">
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <notifications-content/>
        <p>
          <a id="app-more" href="#" @click.prevent.stop="openAppMore">
            {{ $t('notifications.manageNotifications.manageHowNotificationsAreShown') }}
          </a>
        </p>
      </div>
    </div>
  </div>
</template>

<script>
import NotificationsContent from '@/components/Notifications/NotificationsContent';
import NativeApp from '@/services/native-app';

export default {
  name: 'MoreNotificationsPage',
  components: {
    NotificationsContent,
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
    openAppMore() {
      NativeApp.openAppMore();
    },
  },
};
</script>
