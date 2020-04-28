<template>
  <div v-if="showTemplate" class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full">
      <menu-item-list v-if="hasAnyAccess">
        <menu-item v-if="gpMessagingEnabled"
                   id="btn_im1_messaging"
                   data-sid="im1-messaging-list-item"
                   header-tag="h2"
                   :href="im1MessagingPath"
                   :text="$t('messagesHub.im1Messaging.subheader')"
                   :description="$t('messagesHub.im1Messaging.body')"
                   :aria-label="$t('messagesHub.im1Messaging.subheader') |
                     join($t('messagesHub.im1Messaging.body'), '. ')"
                   :click-func="navigate"/>
        <third-party-jump-off-button v-if="pkbEnabled"
                                     id="btn_pkb_messages_and_consultations"
                                     provider-id="pkb"
                                     :jump-off-type="thirdPartyProvider.pkb.messages.type"
                                     :redirect-path="thirdPartyProvider
                                       .pkb.messages.redirectPath" />
        <menu-item v-if="appMessagingEnabled"
                   id="btn_appMessaging"
                   header-tag="h2"
                   data-purpose="text_link"
                   :href="appMessagingPath"
                   :text="$t('messagesHub.appMessaging.subHeader')"
                   :description="$t('messagesHub.appMessaging.body')"
                   :click-func="navigate"
                   :aria-label="$t('messagesHub.appMessaging.subHeader') |
                     join($t('messagesHub.appMessaging.body') ,'. ')"/>
      </menu-item-list>
      <p v-else data-purpose="no-messages-available">
        {{ $t('messagesHub.noMessages') }}
      </p>
    </div>
  </div>
</template>

<script>
import ThirdPartyJumpOffButton from '@/components/ThirdPartyJumpOffButton';
import MenuItemList from '@/components/MenuItemList';
import sjrIf from '@/lib/sjrIf';
import jumpOffProperties from '@/lib/third-party-providers/jump-off-configuration';
import MenuItem from '@/components/MenuItem';
import { redirectTo } from '@/lib/utils';
import { PATIENT_PRACTICE_MESSAGING, HEALTH_INFORMATION_UPDATES } from '@/lib/routes';

export default {
  name: 'Messages',
  layout: 'nhsuk-layout',
  components: {
    MenuItem,
    MenuItemList,
    ThirdPartyJumpOffButton,
  },
  data() {
    return {
      im1MessagingPath: PATIENT_PRACTICE_MESSAGING.path,
      im1MessagingSjrEnabled: sjrIf({ $store: this.$store, journey: 'im1Messaging' }),
    };
  },
  computed: {
    hasAnyAccess() {
      return this.gpMessagingEnabled || this.appMessagingEnabled || this.pkbEnabled;
    },
    gpMessagingEnabled() {
      return this.im1MessagingSjrEnabled && this.$store.state.practiceSettings.im1MessagingEnabled;
    },
    appMessagingEnabled() {
      return sjrIf({ $store: this.$store, journey: 'messaging' });
    },
    appMessagingPath() {
      return HEALTH_INFORMATION_UPDATES.path;
    },
    thirdPartyProvider() {
      return jumpOffProperties.thirdPartyProvider;
    },
    hasPkbMessages() {
      return sjrIf({
        $store: this.$store,
        journey: 'silverIntegration',
        context: {
          provider: 'pkb',
          serviceType: 'messages',
        },
      });
    },
    isNativeApp() {
      return this.$store.state.device.isNativeApp;
    },
    isProxying() {
      return this.$store.getters['session/isProxying'];
    },
    pkbEnabled() {
      return this.hasPkbMessages && this.isNativeApp && !this.isProxying;
    },
  },
  methods: {
    navigate(event) {
      redirectTo(this, event.currentTarget.pathname);
      event.preventDefault();
    },
  },
};
</script>
