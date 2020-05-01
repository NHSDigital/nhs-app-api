<template>
  <div v-if="showTemplate" id="mainDiv">
    <menu-item-list>
      <menu-item id="btn_messages"
                 header-tag="h2"
                 data-purpose="text_link"
                 :href="messagesPath"
                 :text="$t('sc04.messages.subheader')"
                 :description="$t('sc04.messages.body')"
                 :click-func="navigate"
                 :aria-label="$t('sc04.messages.subheader') |
                   join($t('sc04.messages.body') ,'. ')"/>

      <menu-item v-if="adminHelpEnabled"
                 id="btn_gp_help"
                 header-tag="h2"
                 data-purpose="text_link"
                 :href="adminHelpPath"
                 :text="$t('sc04.requestGpHelp.subheader')"
                 :description="$t('sc04.requestGpHelp.body')"
                 :click-func="navigateToAdminHelp"
                 :aria-label="$t('sc04.requestGpHelp.subheader') |
                   join($t('sc04.requestGpHelp.body') ,'. ')"/>

      <organ-donation-link id="btn_organ_donation"
                           header-tag="h2"
                           :display-description="true"
                           :back-link-override="morePath"/>

      <third-party-jump-off-button
        v-if="showPkbSharedLinks"
        id="btn_pkb_shared_links"
        provider-id="pkb"
        :jump-off-type="thirdPartyProvider.pkb.sharedLinks.type"
        :redirect-path="thirdPartyProvider.pkb.sharedLinks.redirectPath" />

      <menu-item id="btn_data_sharing"
                 header-tag="h2"
                 data-purpose="text_link"
                 :href="dataSharingPath"
                 :text="$t('sc04.dataSharing.subheader')"
                 :description="$t('sc04.dataSharing.body')"
                 :click-func="navigateToDataSharing"
                 :aria-label="$t('sc04.dataSharing.subheader') |
                   join($t('sc04.dataSharing.body') ,'. ')"/>

    </menu-item-list>
  </div>
</template>

<script>
import MenuItem from '@/components/MenuItem';
import MenuItemList from '@/components/MenuItemList';
import OrganDonationLink from '@/components/organ-donation/OrganDonationLink';
import ThirdPartyJumpOffButton from '@/components/ThirdPartyJumpOffButton';
import jumpOffProperties from '@/lib/third-party-providers/jump-off-configuration';
import sjrIf from '@/lib/sjrIf';
import { APPOINTMENT_ADMIN_HELP, DATA_SHARING_OVERVIEW, MORE, MESSAGES } from '@/lib/routes';
import { createUri } from '@/lib/noJs';
import { redirectTo } from '@/lib/utils';

export default {
  layout: 'nhsuk-layout',
  components: {
    MenuItemList,
    MenuItem,
    OrganDonationLink,
    ThirdPartyJumpOffButton,
  },
  data() {
    return {
      adminHelpEnabled: sjrIf({ $store: this.$store, journey: 'cdssAdmin' }),
      adminHelpPath: createUri({
        path: APPOINTMENT_ADMIN_HELP.path,
        noJs: { onlineConsultations: { previousRoute: MORE.path } },
      }),
      isNativeApp: this.$store.state.device.isNativeApp,
      isProxying: this.$store.getters['session/isProxying'],
      morePath: MORE.path,
      thirdPartyProvider: jumpOffProperties.thirdPartyProvider,
      messagesPath: MESSAGES.path,
    };
  },
  computed: {
    dataSharingPath() {
      return this.$store.state.device.isNativeApp
        ? DATA_SHARING_OVERVIEW.path
        : this.$store.app.$env.YOUR_NHS_DATA_MATTERS_URL;
    },
    hasPkbSharedLinks() {
      return sjrIf({
        $store: this.$store,
        journey: 'silverIntegration',
        context: {
          provider: 'pkb',
          serviceType: 'libraries',
        },
      });
    },
    showPkbSharedLinks() {
      return this.hasPkbSharedLinks && this.isNativeApp && !this.isProxying;
    },
  },
  mounted() {
    this.$store.dispatch('device/unlockNavBar');
  },
  methods: {
    navigate(event) {
      redirectTo(this, event.currentTarget.pathname);
      event.preventDefault();
    },
    navigateToAdminHelp(event) {
      this.navigate(event);
      this.$store.dispatch('navigation/setNewMenuItem', 4);
      this.$store.dispatch('onlineConsultations/setPreviousRoute', this.morePath);
      this.$store.dispatch('navigation/setBackLinkOverride', this.morePath);
      this.$store.dispatch('navigation/setRouteCrumb', 'moreCrumb');
    },
    navigateToDataSharing(event) {
      if (this.$store.state.device.isNativeApp) {
        this.navigate(event);
      } else {
        window.open(this.dataSharingPath, '_blank');
      }
    },
  },
};
</script>
