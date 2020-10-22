<template>
  <div v-if="showTemplate">
    <menu-item-list>
      <menu-item id="btn_corona"
                 header-tag="h2"
                 :click-func="navigateToWebIntegration"
                 :click-param="coronaCheckerUrl"
                 :href="coronaCheckerUrl"
                 target="_blank"
                 :text="$t('adviceCheck.getAdviceAboutCoronavirus')"
                 :description="$t('adviceCheck.findOutWhatToDoIfYouHaveCoronavirus')"
                 :aria-label="ariaLabelCaption(
                   'adviceCheck.getAdviceAboutCoronavirus',
                   'adviceCheck.findOutWhatToDoIfYouHaveCoronavirus')"/>

      <menu-item id="btn_choices"
                 header-tag="h2"
                 :click-func="navigateToWebIntegration"
                 :click-param="conditionsCheckerUrl"
                 data-purpose="text_link"
                 target="_blank"
                 :href="conditionsCheckerUrl"
                 :text="$t('adviceCheck.searchConditionsAndTreatments')"
                 :description="$t('adviceCheck.findTrustedNhsInformation')"
                 :aria-label="ariaLabelCaption(
                   'adviceCheck.searchConditionsAndTreatments',
                   'adviceCheck.findTrustedNhsInformation')"/>

      <menu-item id="btn_111"
                 header-tag="h2"
                 :click-func="navigateToWebIntegration"
                 :click-param="symptomsCheckerUrl"
                 :href="symptomsCheckerUrl"
                 target="_blank"
                 :text="$t('adviceCheck.useNhs111Online')"
                 :description="$t('adviceCheck.checkIfYouNeedUrgentHelp')"
                 :aria-label="ariaLabelCaption(
                   'adviceCheck.useNhsOneOneOneOnline',
                   'adviceCheck.checkIfYouNeedUrgentHelp')"/>

      <gp-advice-menu-item v-if="isLoggedIn && isCdssAdvice && isProofLevel9"/>

      <third-party-jump-off-button v-if="isLoggedIn && showEngageMedicalAdvice && isProofLevel9"
                                   id="btn_engage_medical_advice"
                                   provider-id="engage"
                                   :provider-configuration="thirdPartyProvider.engage.medical"/>
    </menu-item-list>
  </div>
</template>

<script>
import GpAdviceMenuItem from '@/components/menuItems/GpAdviceMenuItem';
import MenuItem from '@/components/MenuItem';
import MenuItemList from '@/components/MenuItemList';
import NativeApp from '@/services/native-app';
import sjrIf from '@/lib/sjrIf';
import { SYMPTOM_CHECKER_NATIVE_QUERY_PARAMS } from '@/router/externalLinks';
import ThirdPartyJumpOffButton from '@/components/ThirdPartyJumpOffButton';
import jumpOffProperties from '@/lib/third-party-providers/jump-off-configuration';

export default {
  name: 'AdviceCheck',
  components: {
    GpAdviceMenuItem,
    MenuItemList,
    MenuItem,
    ThirdPartyJumpOffButton,
  },
  data() {
    let symptomsCheckerUrl = this.$store.$env.SYMPTOM_CHECKER_URL;
    if (this.$store.state.device.isNativeApp) {
      symptomsCheckerUrl += SYMPTOM_CHECKER_NATIVE_QUERY_PARAMS;
    }
    return {
      symptomsCheckerUrl,
      conditionsCheckerUrl: this.$store.$env.CONDITIONS_CHECKER_URL,
      coronaCheckerUrl: this.$store.$env.CORONA_SERVICE_URL,
      isLoggedIn: this.$store.getters['session/isLoggedIn'],
      isCdssAdvice: sjrIf({ $store: this.$store, journey: 'cdssAdvice' }),
      isProofLevel9: this.$store.getters['session/isProofLevel9'],
      showEngageMedicalAdvice: sjrIf({
        $store: this.$store,
        journey: 'silverIntegration',
        context: {
          provider: 'engage',
          serviceType: 'consultations',
        },
      }),
      thirdPartyProvider: jumpOffProperties.thirdPartyProvider,
    };
  },
  methods: {
    ariaLabelCaption(header, body) {
      return `${this.$t(header)}. ${this.$t(body)}`;
    },
    navigateToWebIntegration(url) {
      if (NativeApp.supportsNativeWebIntegration()) {
        NativeApp.openWebIntegration(url);
      } else {
        window.open(url, '_blank', 'noopener,noreferrer');
      }
    },
  },
};
</script>

<style module lang="scss" scoped>
@import "../../style/buttons";
</style>
