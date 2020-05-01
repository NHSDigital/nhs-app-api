<template>
  <div v-if="showTemplate" class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full">
      <p id="infoRecipients">{{ $t('im04.info') }}</p>
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
import {
  GP_MESSAGES,
  GP_MESSAGES_URGENCY,
  GP_MESSAGES_CREATE,
} from '@/lib/routes';
import { redirectTo, isEmptyArray } from '@/lib/utils';

export default {
  layout: 'nhsuk-layout',
  components: {
    DesktopGenericBackLink,
    MenuItem,
    MenuItemList,
  },
  data() {
    return {
      urgencyPath: GP_MESSAGES_URGENCY.path,
      messageRecipients: this.$store.state.gpMessages.messageRecipients,
    };
  },
  fetch({ store, redirect }) {
    const { messageRecipients } = store.state.gpMessages;
    if (!messageRecipients || isEmptyArray(messageRecipients)) {
      redirect(GP_MESSAGES.path);
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
      redirectTo(this, GP_MESSAGES_CREATE.path);
    },
  },
};
</script>
