<template>
  <div>
    <div v-if="showTemplate && detailsLoaded">
      <ul :class="['nhsuk-u-margin-top-2',
                   'nhsuk-u-margin-bottom-4',
                   'nhsuk-u-padding-left-0']">
        <sentMessage v-if="isOutbound"
                     class="nhsuk-u-padding-bottom-4"
                     :message="selectedMessage"
                     :sent-index="0"
                     :sent-prefix-identifier="'initial'"
                     :message-content="selectedMessage.content"/>
        <receivedMessage v-else class="nhsuk-u-padding-bottom-4"
                         :message="selectedMessage"
                         :reply-index="0"
                         :reply-prefix-identifier="'initial'"
                         :message-content="selectedMessage.content"/>
        <div v-for="(reply, index) in readMessages"
             :key="`readMessageReply`+ index">
          <div v-if="reply.outboundMessage">
            <sentMessage class="nhsuk-u-padding-bottom-4"
                         :message="reply"
                         :sent-index="index"
                         :sent-prefix-identifier="'readReply'"
                         :message-content="reply.replyContent"/>
          </div>
          <div v-else>
            <receivedMessage class="nhsuk-u-padding-bottom-4"
                             :message="reply"
                             :reply-index="index"
                             :reply-prefix-identifier="'read'"
                             :message-content="reply.replyContent"/>
          </div>
        </div>
        <page-divider v-if="hasUnreadMessages" id="receivedMessagesDivider" :text="unreadText" />
        <div v-for="(reply, index) in unreadMessages"
             :key="`unreadMessageReply`+ index">
          <div v-if="reply.outboundMessage">
            <sentMessage class="nhsuk-u-padding-bottom-4"
                         :message="reply"
                         :sent-index="index"
                         :sent-prefix-identifier="'unreadReply'"
                         :message-content="reply.replyContent"/>
          </div>
          <div v-else>
            <receivedMessage class="nhsuk-u-padding-bottom-4"
                             :message="reply"
                             :reply-index="index"
                             :reply-prefix-identifier="'unread'"
                             :message-content="reply.replyContent"/>
          </div>
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
import ReceivedMessage from '@/components/patient-practice-messaging/ReceivedMessage';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import MenuItem from '@/components/MenuItem';
import MenuItemList from '@/components/MenuItemList';
import { PATIENT_PRACTICE_MESSAGING, PATIENT_PRACTICE_MESSAGING_DELETE } from '@/lib/routes';
import { redirectTo } from '@/lib/utils';
import srjIf from '@/lib/sjrIf';
import takeWhile from 'lodash/fp/takeWhile';
import dropWhile from 'lodash/fp/dropWhile';
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
    messageID() {
      return this.$store.state.patientPracticeMessaging.selectedMessageId;
    },
    deleteEnabled() {
      return srjIf({ $store: this.$store, journey: 'deletePatientPracticeMessage' });
    },
    updateStatusEnabled() {
      return srjIf({ $store: this.$store, journey: 'updateStatusPatientPracticeMessage' });
    },
    selectedMessage() {
      return this.$store.state.patientPracticeMessaging.selectedMessageDetails.messageDetails;
    },
    isOutbound() {
      return this.selectedMessage.outboundMessage;
    },
    selectedMessageReplies() {
      return this.selectedMessage.messageReplies;
    },
    readMessages() {
      return takeWhile(m => !m.isUnread)(this.selectedMessageReplies);
    },
    unreadMessages() {
      return dropWhile(m => !m.isUnread)(this.selectedMessageReplies);
    },
    hasUnreadMessages() {
      return this.unreadMessages.length > 0;
    },
    unreadText() {
      if (this.unreadMessages.length > 1) {
        return this.$t('patient_practice_messaging.view_details.unreadMessages');
      }
      return this.$t('patient_practice_messaging.view_details.unreadMessage');
    },
  },
  async fetch({ store, redirect }) {
    if (store.state.patientPracticeMessaging.selectedMessageId === undefined) {
      return redirect(PATIENT_PRACTICE_MESSAGING.path);
    }

    if (store.state.patientPracticeMessaging.selectedMessageId !== 0 &&
        store.state.patientPracticeMessaging.selectedMessageDetails === undefined) {
      const selectedId = store.state.patientPracticeMessaging.selectedMessageId;
      return store.dispatch('patientPracticeMessaging/loadMessage', { id: selectedId, clearApiError: true });
    }
    return undefined;
  },
  mounted() {
    if (this.$store.state.patientPracticeMessaging.loadedDetails &&
      this.updateStatusEnabled) {
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

  .nhsuk-app-chat {
    list-style-type: none;
  }

</style>
