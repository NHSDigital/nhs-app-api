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
import SentMessage from '@/components/gp-messages/SentMessage';
import ReceivedMessage from '@/components/gp-messages/ReceivedMessage';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import MenuItem from '@/components/MenuItem';
import MenuItemList from '@/components/MenuItemList';
import { GP_MESSAGES, GP_MESSAGES_DELETE } from '@/lib/routes';
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
      messagesPath: GP_MESSAGES.path,
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
  async fetch({ store, redirect }) {
    if (isBlankString(store.state.gpMessages.selectedMessageId)) {
      return redirect(GP_MESSAGES.path);
    }

    if (store.state.gpMessages.selectedMessageId === '0'
      || store.state.gpMessages.selectedMessageDetails !== undefined) {
      return undefined;
    }

    const selectedId = store.state.gpMessages.selectedMessageId;

    return store.dispatch('gpMessages/loadMessage', {
      id: selectedId,
      clearApiError: true,
    });
  },
  mounted() {
    if (this.$store.state.gpMessages.loadedDetails
      && this.updateStatusEnabled
      && !isBlankString(this.$store.state.gpMessages.selectedMessageId)
      && this.$store.state.gpMessages.selectedMessageId !== '0') {
      this.$store.dispatch('gpMessages/updateReadStatusAsRead');
    }
  },
  methods: {
    backButtonClicked() {
      redirectTo(this, this.messagesPath);
    },
    deleteClicked() {
      redirectTo(this, GP_MESSAGES_DELETE.path);
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
