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
  INDEX,
  PATIENT_PRACTICE_MESSAGING_URGENCY,
  PATIENT_PRACTICE_MESSAGING_CREATE,
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
      urgencyPath: PATIENT_PRACTICE_MESSAGING_URGENCY.path,
      messageRecipients: this.$store.state.patientPracticeMessaging.messageRecipients,
    };
  },
  fetch({ store, redirect }) {
    const { messageRecipients } = store.state.patientPracticeMessaging;
    if (!messageRecipients || isEmptyArray(messageRecipients)) {
      redirect(INDEX.path);
    }
  },
  methods: {
    backLinkClicked() {
      redirectTo(this, this.urgencyPath);
    },
    recipientClicked(recipient) {
      this.$store.dispatch('patientPracticeMessaging/setSelectedRecipient',
        { id: recipient.recipientIdentifier, name: recipient.name });
      this.$store.dispatch('patientPracticeMessaging/setMessageSent', false);
      redirectTo(this, PATIENT_PRACTICE_MESSAGING_CREATE.path);
    },
  },
};
</script>
