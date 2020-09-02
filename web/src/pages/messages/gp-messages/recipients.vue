<template>
  <div v-if="showTemplate" class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full">
      <p id="infoRecipients">{{ $t('messages.thisIsWhoYourSurgeryLetsYouMessage') }}</p>
      <menu-item-list id="recipientsMenuList" class="nhsuk-u-margin-bottom-3">
        <menu-item v-for="(recipient, index) in messageRecipients"
                   :id="`recipient-${recipient.recipientIdentifier}-${index}`"
                   :key="`recipient-${recipient.recipientIdentifier}-${index}`"
                   :text="recipient.name"
                   :click-func="recipientClicked"
                   :click-param="recipient"
                   header-tag="h2"
                   href="#"/>
      </menu-item-list>
      <desktop-generic-back-link v-if="!$store.state.device.isNativeApp"
                                 :path="urgencyPath"
                                 @clickAndPrevent="backLinkClicked"/>
    </div>
  </div>
</template>

<script>
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import MenuItem from '@/components/MenuItem';
import MenuItemList from '@/components/MenuItemList';
import { redirectTo, isEmptyArray } from '@/lib/utils';
import {
  GP_MESSAGES_URGENCY_PATH,
  GP_MESSAGES_PATH,
  GP_MESSAGES_CREATE_PATH,
} from '@/router/paths';

export default {
  name: 'GpMessagesRecipientsPage',
  components: {
    DesktopGenericBackLink,
    MenuItem,
    MenuItemList,
  },
  data() {
    return {
      urgencyPath: GP_MESSAGES_URGENCY_PATH,
      messageRecipients: this.$store.state.gpMessages.messageRecipients,
    };
  },
  created() {
    const { messageRecipients } = this.$store.state.gpMessages;
    if (!messageRecipients || isEmptyArray(messageRecipients)) {
      redirectTo(this, GP_MESSAGES_PATH);
    }
  },
  methods: {
    backLinkClicked() {
      redirectTo(this, this.urgencyPath);
    },
    recipientClicked(recipient) {
      this.$store.dispatch('gpMessages/setSelectedRecipient',
        { id: recipient.recipientIdentifier, name: recipient.name });
      this.$store.dispatch('gpMessages/setMessageSent', false);
      redirectTo(this, GP_MESSAGES_CREATE_PATH);
    },
  },
};
</script>
