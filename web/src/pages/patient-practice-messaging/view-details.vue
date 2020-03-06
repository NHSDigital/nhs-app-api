<template>
  <div>
    <div v-if="showTemplate && detailsLoaded">
      <sentMessage
        class="nhsuk-u-padding-bottom-4"/>
      <receivedMessages/>
      <menu-item-list id="messageDetailsOptionsList" class="nhsuk-u-margin-bottom-3">
        <menu-item id="deleteMessage"
                   :text="$t('patient_practice_messaging.view_details.deleteMenuItemText')"
                   :click-func="deleteClicked"
                   header-tag="h2"
                   href="#"/>
      </menu-item-list>
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
import MenuItem from '@/components/MenuItem';
import MenuItemList from '@/components/MenuItemList';
import { PATIENT_PRACTICE_MESSAGING, PATIENT_PRACTICE_MESSAGING_DELETE } from '@/lib/routes';
import { redirectTo } from '@/lib/utils';

export default {
  layout: 'nhsuk-layout',
  components: {
    SentMessage,
    ReceivedMessages,
    DesktopGenericBackLink,
    MenuItem,
    MenuItemList,
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
    messageID() {
      return this.$store.state.patientPracticeMessaging.selectedMessageId;
    },
  },
  async fetch({ store, redirect }) {
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
    if (this.$store.state.patientPracticeMessaging.loadedDetails) {
      if (this.$store.state.patientPracticeMessaging.selectedMessageId !== 0) {
        this.$store.dispatch('patientPracticeMessaging/updateReadStatusAsRead');
      }
    }
  },
  beforeDestroy() {
    this.$store.dispatch('patientPracticeMessaging/clearSelectedRetainingId');
  },
  methods: {
    backButtonClicked() {
      redirectTo(this, this.messagesPath);
    },
    deleteClicked() {
      redirectTo(this, PATIENT_PRACTICE_MESSAGING_DELETE.path);
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
