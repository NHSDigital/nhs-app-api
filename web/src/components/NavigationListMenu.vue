<template>
  <div>
    <menu-item-list-header id="nav-popular-services-list-header"
                           header-tag="h2"
                           :widen-on-tablet="true"
                           :text="$t('navigation.popularServices')"/>
    <menu-item-list data-sid="navigation-list-menu" class="nhsuk-u-margin-top-0">
      <menu-item v-if="isProofLevel9"
                 id="menu-item-myRecord"
                 :header-tag="headerTag"
                 data-sid="your-health-menu-item"
                 :href="gpMedicalRecordPath"
                 :text="$t('navigation.viewYourGpHealthRecord')"
                 :aria-label="$t('navigation.viewYourGpHealthRecord')"
                 :click-func="goToUrl"
                 :click-param="gpMedicalRecordPath"/>
      <menu-item v-if="isProofLevel9"
                 id="menu-item-prescriptions"
                 :header-tag="headerTag"
                 data-sid="prescriptions-menu-item"
                 :href="prescriptionsPath"
                 :text="$t('navigation.orderARepeatPrescription')"
                 :aria-label="$t('navigation.orderARepeatPrescription')"
                 :click-func="goToUrl"
                 :click-param="prescriptionsPath"/>
      <menu-item v-if="gpMessagingAvailable"
                 id="btn_messages"
                 :header-tag="headerTag"
                 data-purpose="messages-menu-item"
                 :href="messagesPath"
                 :show-indicator="hasMessageIndicator"
                 :text="$t('navigation.viewYourMessages')"
                 :aria-label="messagesAriaLabel"
                 :click-func="navigateToMessages"/>
    </menu-item-list>
  </div>
</template>

<script>
import MenuItem from '@/components/MenuItem';
import MenuItemList from '@/components/MenuItemList';
import MenuItemListHeader from '@/components/MenuItemListHeader';
import {
  MESSAGES_PATH,
  GP_MEDICAL_RECORD_PATH,
  PRESCRIPTIONS_PATH,
} from '@/router/paths';
import { redirectTo } from '@/lib/utils';

export default {
  name: 'NavigationListMenu',
  components: {
    MenuItem,
    MenuItemList,
    MenuItemListHeader,
  },
  props: {
    headerTag: {
      type: String,
      default: 'h2',
    },
    hasMessageIndicator: {
      type: Boolean,
      default: false,
    },
  },
  data() {
    return {
      prescriptionsPath: PRESCRIPTIONS_PATH,
      gpMedicalRecordPath: GP_MEDICAL_RECORD_PATH,
      messagesPath: MESSAGES_PATH,
    };
  },
  computed: {
    isProofLevel9() {
      return this.$store.getters['session/isProofLevel9'];
    },
    gpMessagingAvailable() {
      return !this.$store.state.gpMessages.gpMessagingSessionUnavailable;
    },
    messagesAriaLabel() {
      return (this.hasMessageIndicator) ?
        `${this.$t('navigation.viewYourMessages')}
          ${this.$t('navigation.youHaveUnreadMessages')}` :
        this.$t('navigation.viewYourMessages');
    },
  },
  methods: {
    navigateToMessages() {
      redirectTo(this, this.messagesPath);
    },
  },
};
</script>

<style module lang="scss">
  @import '../style/colours';

  a:visited {
    color: $nhs_blue;
  }
</style>
