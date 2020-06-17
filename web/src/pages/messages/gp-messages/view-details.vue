<template>
  <div>
    <template v-if="showTemplate && detailsLoaded">
      <ul :class="['nhsuk-u-margin-top-2',
                   'nhsuk-u-margin-bottom-4',
                   'nhsuk-u-padding-left-0']">
        <div v-for="message in messages" :key="message.key">
          <template v-if="message.isFirstUnreadMessage">
            <scroll-to-anchor id="unreadMessages" />
            <page-divider id="receivedMessagesDivider" :text="getUnreadText" />
          </template>
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
      <div aria-hidden="true" class="nhsuk-u-margin-bottom-3 nhsuk-reply-info-divider"/>
      <div class="nhsuk-u-padding-left-3 nhsuk-u-margin-bottom-3">
        <span class="nhsuk-heading-xs nhsuk-u-margin-top-0 nhsuk-u-margin-bottom-0">
          {{ $t('gp_messages.view_details.replyInformation.header') }}
        </span>
        <span class="nhsuk-label nhsuk-u-margin-top-0
              nhsuk-u-padding-bottom-0">
          {{ $t('gp_messages.view_details.replyInformation.subText') }}
        </span>
      </div>
      <menu-item-list id="messageDetailsOptionsList" class="nhsuk-u-margin-bottom-3">
        <menu-item id="newMessage"
                   :text="$t('gp_messages.view_details.sendMessageMenuItemText')"
                   :click-func="sendNewMessageClicked"
                   header-tag="h2"
                   href="#"/>
        <menu-item v-if="deleteEnabled"
                   id="deleteMessage"
                   :text="$t('gp_messages.view_details.deleteMenuItemText')"
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
          :button-text="'gp_messages.view_details.backButtonText.text'"
          @clickAndPrevent="backButtonClicked"/>
      </div>
    </div>
  </div>
</template>

<script>
import get from 'lodash/fp/get';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import MenuItem from '@/components/MenuItem';
import MenuItemList from '@/components/MenuItemList';
import PageDivider from '@/components/widgets/PageDivider';
import ReceivedMessage from '@/components/gp-messages/ReceivedMessage';
import ScrollToAnchor from '@/components/widgets/ScrollToAnchor';
import SentMessage from '@/components/gp-messages/SentMessage';
import { redirectTo, isBlankString } from '@/lib/utils';
import srjIf from '@/lib/sjrIf';
import {
  GP_MESSAGES_PATH,
  GP_MESSAGES_DELETE_PATH,
  GP_MESSAGES_VIEW_MESSAGE_PATH,
  GP_MESSAGES_URGENCY_PATH,
} from '@/router/paths';

export default {
  name: 'GpMessagesViewDetailsPage',
  components: {
    DesktopGenericBackLink,
    MenuItem,
    MenuItemList,
    PageDivider,
    ReceivedMessage,
    ScrollToAnchor,
    SentMessage,
  },
  data() {
    return {
      recipient: get('$store.state.gpMessages.selectedMessageRecipient.name')(this),
      messagesPath: GP_MESSAGES_PATH,
    };
  },
  computed: {
    detailsLoaded() {
      return this.$store.state.gpMessages.loadedDetails;
    },
    deleteEnabled() {
      return srjIf({ $store: this.$store, journey: 'deleteGpMessages' });
    },
    updateStatusEnabled() {
      return srjIf({ $store: this.$store, journey: 'updateStatusGpMessages' });
    },
    messages() {
      return this.$store.state.gpMessages.messages;
    },
    getUnreadText() {
      return this.$store.state.gpMessages.unreadIndex > 0
        ? this.$t('gp_messages.view_details.unreadMessages')
        : this.$t('gp_messages.view_details.unreadMessage');
    },
  },
  async created() {
    const { selectedMessageId, selectedMessageDetails } = this.$store.state.gpMessages;

    if (isBlankString(this.$store.state.gpMessages.selectedMessageId)) {
      redirectTo(this, GP_MESSAGES_PATH);
      return;
    }

    if (selectedMessageId !== '0') {
      if (selectedMessageDetails === undefined) {
        await this.$store.dispatch('gpMessages/loadMessage', {
          id: this.$store.state.gpMessages.selectedMessageId,
          clearApiError: true,
        });
      }

      if (this.$store.state.gpMessages.loadedDetails && this.updateStatusEnabled) {
        this.$store.dispatch('gpMessages/updateReadStatusAsRead');
      }
    }

    this.$store.dispatch('navigation/clearBackLinkOverride');
  },
  methods: {
    backButtonClicked() {
      redirectTo(this, this.messagesPath);
    },
    deleteClicked() {
      redirectTo(this, GP_MESSAGES_DELETE_PATH);
    },
    sendNewMessageClicked() {
      this.$store.dispatch('navigation/setBackLinkOverride', GP_MESSAGES_VIEW_MESSAGE_PATH);
      this.$store.dispatch('gpMessages/setUrgencyChoice', undefined);
      redirectTo(this, GP_MESSAGES_URGENCY_PATH);
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

  .nhsuk-reply-info-divider {
    border-top: 1px solid #d8dde0;
  }
</style>
