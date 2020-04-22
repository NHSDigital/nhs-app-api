<template>
  <div>
    <template v-if="showTemplate && detailsLoaded">
      <ul :class="['nhsuk-u-margin-top-2',
                   'nhsuk-u-margin-bottom-4',
                   'nhsuk-u-padding-left-0']">
        <div v-for="message in messages" :key="message.key">
          <page-divider v-if="message.isFirstUnreadMessage"
                        id="receivedMessagesDivider"
                        :text="getUnreadText" />
          <sentMessage v-if="message.outboundMessage"
                       class="nhsuk-u-padding-bottom-4"
                       :message="message"
                       :sent-index="message.index"
                       :sent-prefix-identifier="message.prefixIdentifier"
                       :message-content="message.content"/>
          <receivedMessage v-else
                           class="nhsuk-u-padding-bottom-4"
                           :message="message"
                           :reply-index="message.index"
                           :reply-prefix-identifier="message.prefixIdentifier"
                           :message-content="message.content"/>
        </div>
      </ul>
      <menu-item-list id="messageDetailsOptionsList" class="nhsuk-u-margin-bottom-3">
        <menu-item v-if="deleteEnabled"
                   id="deleteMessage"
                   :text="$t('patient_practice_messaging.view_details.deleteMenuItemText')"
                   :click-func="deleteClicked"
                   header-tag="h2"
                   href="#"/>
      </menu-item-list>
    </template>
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
import ReceivedMessage from '@/components/patient-practice-messaging/ReceivedMessage';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import MenuItem from '@/components/MenuItem';
import MenuItemList from '@/components/MenuItemList';
import { PATIENT_PRACTICE_MESSAGING, PATIENT_PRACTICE_MESSAGING_DELETE } from '@/lib/routes';
import { redirectTo, isBlankString } from '@/lib/utils';
import srjIf from '@/lib/sjrIf';
import PageDivider from '@/components/widgets/PageDivider';

export default {
  layout: 'nhsuk-layout',
  components: {
    SentMessage,
    ReceivedMessage,
    DesktopGenericBackLink,
    MenuItem,
    MenuItemList,
    PageDivider,
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
    deleteEnabled() {
      return srjIf({ $store: this.$store, journey: 'deletePatientPracticeMessage' });
    },
    updateStatusEnabled() {
      return srjIf({ $store: this.$store, journey: 'updateStatusPatientPracticeMessage' });
    },
    messages() {
      return this.$store.state.patientPracticeMessaging.messages;
    },
    getUnreadText() {
      return this.$store.state.patientPracticeMessaging.unreadIndex > 0
        ? this.$t('patient_practice_messaging.view_details.unreadMessages')
        : this.$t('patient_practice_messaging.view_details.unreadMessage');
    },
  },
  async fetch({ store, redirect }) {
    if (isBlankString(store.state.patientPracticeMessaging.selectedMessageId)) {
      return redirect(PATIENT_PRACTICE_MESSAGING.path);
    }

    if (store.state.patientPracticeMessaging.selectedMessageId === '0'
      || store.state.patientPracticeMessaging.selectedMessageDetails !== undefined) {
      return undefined;
    }

    const selectedId = store.state.patientPracticeMessaging.selectedMessageId;

    return store.dispatch('patientPracticeMessaging/loadMessage', {
      id: selectedId,
      clearApiError: true,
    });
  },
  mounted() {
    if (this.$store.state.patientPracticeMessaging.loadedDetails
      && this.updateStatusEnabled
      && !isBlankString(this.$store.state.patientPracticeMessaging.selectedMessageId)
      && this.$store.state.patientPracticeMessaging.selectedMessageId !== '0') {
      this.$store.dispatch('patientPracticeMessaging/updateReadStatusAsRead');
    }
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

  .nhsuk-app-chat {
    list-style-type: none;
  }

</style>
