<template xmlns:v-if="http://www.w3.org/1999/xhtml">
  <div v-if="showTemplate">
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <p v-for="(paragraph, index) of $t('account.notifications.paragraphs')" :key="index">
          {{ paragraph }}
        </p>
        <labelled-toggle v-model="registered"
                         checkbox-id="allow_notifications"
                         :is-waiting="isWaiting"
                         :label="$t('account.notifications.toggleLabel')"/>
        <nhs-arrow-banner :banner-text="$t('account.notifications.settingsLinkText')"
                          :open-new-window="false"
                          :click-action="openAppSettings"/>
      </div>
    </div>
  </div>
</template>

<script>
import LabelledToggle from '@/components/widgets/LabelledToggle';
import NativeApp from '@/services/native-app';
import NhsArrowBanner from '@/components/widgets/NhsArrowBanner';

export default {
  layout: 'nhsuk-layout',
  components: {
    LabelledToggle,
    NhsArrowBanner,
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
    if (process.client) {
      this.$store.dispatch('notifications/load');
    }
  },
  methods: {
    openAppSettings() {
      NativeApp.openAppSettings();
    },
  },
};
</script>
