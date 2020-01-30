<template>
  <div v-if="showTemplate" class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full">
      <p id="infoRecipients">{{ $t('im04.info') }}</p>
      <menu-item-list id="recipientsMenuList" class="nhsuk-u-margin-bottom-3">
        <menu-item v-for="recipient in recipients"
                   :id="`recipient-${recipient.recipientGuid}`"
                   :key="`recipient-${recipient.recipientGuid}`"
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
import { isFalsy, redirectTo } from '@/lib/utils';

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
      recipients: this.$store.state.patientPracticeMessaging.messageRecipients,
    };
  },
  async fetch({ store, redirect }) {
    if (isFalsy(store.app.$env.PATIENT_PRACTICE_MESSAGING_ENABLED)) {
      return redirect(INDEX.path);
    }
    return store.dispatch('patientPracticeMessaging/loadRecipients');
  },
  methods: {
    backLinkClicked() {
      redirectTo(this, this.urgencyPath);
    },
    recipientClicked(recipient) {
      this.$store.dispatch('patientPracticeMessaging/setSelectedRecipient',
        { id: recipient.recipientGuid, name: recipient.name });
      redirectTo(this, PATIENT_PRACTICE_MESSAGING_CREATE.path);
    },
  },
};
</script>
