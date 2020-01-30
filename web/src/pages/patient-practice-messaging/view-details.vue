<template>
  <div>
    <div v-if="showTemplate && detailsLoaded">
      <sentMessage
        class="nhsuk-u-padding-bottom-4"/>
      <receivedMessages/>
    </div>
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <desktopGenericBackLink
          v-if="!$store.state.device.isNativeApp"
          :path="messagesPath"
          :button-text="'patient_practice_messaging.view_details.backButtonText.text'"
          @clickAndPrevent="backButtonClicked"/>
      </div>
    </div>
  </div>
</template>

<script>
import SentMessage from '@/components/patient-practice-messaging/SentMessage';
import ReceivedMessages from '@/components/patient-practice-messaging/ReceivedMessages';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import { INDEX, PATIENT_PRACTICE_MESSAGING } from '@/lib/routes';
import { redirectTo, isFalsy } from '@/lib/utils';

export default {
  layout: 'nhsuk-layout',
  components: {
    SentMessage,
    ReceivedMessages,
    DesktopGenericBackLink,
  },
  data() {
    return {
      messagesPath: PATIENT_PRACTICE_MESSAGING.path,
    };
  },
  computed: {
    detailsLoaded() {
      return this.$store.state.patientPracticeMessaging.loadedDetails;
    },
  },
  async fetch({ store, redirect }) {
    if (isFalsy(store.app.$env.PATIENT_PRACTICE_MESSAGING_ENABLED)) {
      return redirect(INDEX.path);
    }
    if (store.state.patientPracticeMessaging.selectedMessageId === undefined) {
      return redirect(PATIENT_PRACTICE_MESSAGING.path);
    }

    if (store.state.patientPracticeMessaging.selectedMessageId !== 0) {
      const selectedId = store.state.patientPracticeMessaging.selectedMessageId;
      return store.dispatch('patientPracticeMessaging/loadMessage', { id: selectedId, clearApiError: true });
    }
    return undefined;
  },
  mounted() {
    if (this.$store.state.patientPracticeMessaging.loadedDetails &&
      this.$store.state.patientPracticeMessaging.selectedMessageRecipient !== undefined) {
      this.$store.dispatch('header/updateHeaderText',
        `Message to ${this.$store.state.patientPracticeMessaging.selectedMessageRecipient.name}`);
      this.$store.dispatch('pageTitle/updatePageTitle',
        `Message to ${this.$store.state.patientPracticeMessaging.selectedMessageRecipient.name}`);

      if (this.$store.state.patientPracticeMessaging.selectedMessageId !== 0) {
        this.$store.dispatch('patientPracticeMessaging/updateReadStatusAsRead');
      }
    }
  },
  beforeDestroy() {
    this.$store.dispatch('patientPracticeMessaging/clear');
  },
  methods: {
    backButtonClicked() {
      redirectTo(this, this.messagesPath);
    },
  },
};
</script>

<style lang="scss">
  p.panel-content > a{
    display: inline;
    font-weight: normal;
    vertical-align: unset;
  }
</style>
