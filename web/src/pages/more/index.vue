<template xmlns:v-if="http://www.w3.org/1999/xhtml">
  <div v-if="showTemplate">
    <menu-item-list data-purpose="more-menu">
      <menu-item v-if="supportsLinkedProfiles && isProofLevel9"
                 id="linked-profiles-link"
                 header-tag="h2"
                 :href="moreLinkedProfilesPath"
                 :text="$t('more.linkedProfiles')"
                 :click-func="navigateToLinkedProfiles"/>
      <menu-item id="'account-and-settings'"
                 header-tag="h2"
                 :href="accountAndSettingsPath"
                 :text="$t('more.accountAndSettings')"
                 :aria-label="$t('more.accountAndSettings')"
                 :click-func="goToUrl"
                 :click-param="accountAndSettingsPath"/>
      <third-party-jump-off-button v-if="showGncrAccountAdmin"
                                   id="btn_gncr_admin"
                                   provider-id="gncr"
                                   :provider-configuration="thirdPartyProvider
                                     .gncr.admin" />
      <third-party-jump-off-button v-if="showSubstraktParticipation"
                                   id="btn_substrakt_participation"
                                   provider-id="substraktPatientPack"
                                   :provider-configuration="thirdPartyProvider
                                     .substraktPatientPack.patientParticipationGroups" />
      <menu-item :id="'help-and-support'"
                 :key="'help-and-support'"
                 header-tag="h2"
                 :target="'_blank'"
                 :href="nhsAppHelpAndSupportUrl"
                 :text="$t('more.helpAndSupport')"
                 :aria-label="$t('more.helpAndSupport')"/>
    </menu-item-list>

    <template v-if="$store.state.device.isNativeApp">
      <analytics-tracked-tag :text="$t('generic.logOut')"
                             data-purpose="button">
        <generic-button id="signout-button"
                        data-purpose="logout-button"
                        class="nhsuk-button nhsuk-button--secondary"
                        @click.prevent="signout">
          {{ $t('generic.logOut') }}
        </generic-button>
      </analytics-tracked-tag>
    </template>

    <p>
      {{ $t('generic.version') }} {{ $store.state.appVersion.webVersion }}
      <span v-show="$store.state.appVersion.nativeVersion">
        ({{ $store.state.appVersion.nativeVersion }})
      </span>
    </p>
    <p v-if="showCEMark">
      <ce-mark-icon/>
    </p>

  </div>
</template>

<script>
/* eslint-disable import/extensions */
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import CeMarkIcon from '@/components/icons/CeMarkIcon';
import MenuItem from '@/components/MenuItem';
import MenuItemList from '@/components/MenuItemList';
import sjrIf from '@/lib/sjrIf';
import ThirdPartyJumpOffButton from '@/components/ThirdPartyJumpOffButton';
import jumpOffProperties from '@/lib/third-party-providers/jump-off-configuration';
import GenericButton from '@/components/widgets/GenericButton';
import { MORE_ACCOUNTANDSETTINGS_PATH, MORE_LINKED_PROFILES_PATH } from '@/router/paths';
import { isTruthy } from '@/lib/utils';

export default {
  layout: 'nhsuk-layout',
  components: {
    CeMarkIcon,
    MenuItem,
    MenuItemList,
    GenericButton,
    AnalyticsTrackedTag,
    ThirdPartyJumpOffButton,
  },
  data() {
    return {
      accountAndSettingsPath: MORE_ACCOUNTANDSETTINGS_PATH,
      nhsAppHelpAndSupportUrl: this.$store.$env.BASE_NHS_APP_HELP_URL,
      moreLinkedProfilesPath: MORE_LINKED_PROFILES_PATH,
      isProxying: this.$store.getters['session/isProxying'],
      isProofLevel9: this.$store.getters['session/isProofLevel9'],
      thirdPartyProvider: jumpOffProperties.thirdPartyProvider,
      hasGncrAccountAdmin: sjrIf({
        $store: this.$store,
        journey: 'silverIntegration',
        context: {
          provider: 'gncr',
          serviceType: 'accountAdmin',
        },
      }),
      hasSubstraktParticipation: sjrIf({
        $store: this.$store,
        journey: 'silverIntegration',
        context: {
          provider: 'substraktPatientPack',
          serviceType: 'participation',
        },
      }),
    };
  },
  computed: {
    supportsLinkedProfiles() {
      return this.$store.state.serviceJourneyRules.rules.supportsLinkedProfiles;
    },
    showCEMark() {
      return isTruthy(this.$store.$env.CE_MARK_ENABLED);
    },
    showGncrAccountAdmin() {
      return this.hasGncrAccountAdmin && this.isProofLevel9;
    },
    showSubstraktParticipation() {
      return this.hasSubstraktParticipation && !this.isProxying && this.isProofLevel9;
    },
  },
  methods: {
    signout() {
      this.$store.dispatch('auth/logout');
    },
    navigateToLinkedProfiles() {
      this.$store.dispatch('navigation/setRouteCrumb', 'moreCrumb');
      this.goToUrl(this.moreLinkedProfilesPath);
    },
  },
};
</script>

<style module lang="scss" scoped>
@import "../../style/accessibility";
@import "../../style/listmenu";
@import "../../style/colours";
@import "../../style/webshared";
</style>
