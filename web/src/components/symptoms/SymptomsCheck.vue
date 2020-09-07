<template>
  <div v-if="showTemplate" data-purpose="">
    <menu-item-list>

      <menu-item id="btn_corona"
                 header-tag="h2"
                 :click-func="navigateToWebIntegration"
                 :click-param="coronaCheckerUrl"
                 :href="coronaCheckerUrl"
                 target="_blank"
                 :text="$t('symptomsChecker.getAdviceAboutCoronavirus')"
                 :description="$t('symptomsChecker.findOutWhatToDoIfYouHaveCoronavirus')"
                 :aria-label="ariaLabelCaption(
                   'symptomsChecker.getAdviceAboutCoronavirus',
                   'symptomsChecker.findOutWhatToDoIfYouHaveCoronavirus')"/>

      <menu-item id="btn_choices"
                 header-tag="h2"
                 :click-func="navigateToWebIntegration"
                 :click-param="conditionsCheckerUrl"
                 data-purpose="text_link"
                 target="_blank"
                 :href="conditionsCheckerUrl"
                 :text="$t('symptomsChecker.searchConditionsAndTreatments')"
                 :description="$t('symptomsChecker.findTrustedNhsInformation')"
                 :aria-label="ariaLabelCaption(
                   'symptomsChecker.searchConditionsAndTreatments',
                   'symptomsChecker.findTrustedNhsInformation')"/>

      <menu-item id="btn_111"
                 header-tag="h2"
                 :click-func="navigateToWebIntegration"
                 :click-param="symptomsCheckerUrl"
                 :href="symptomsCheckerUrl"
                 target="_blank"
                 :text="$t('symptomsChecker.useNhs111Online')"
                 :description="$t('symptomsChecker.checkIfYouNeedUrgentHelp')"
                 :aria-label="ariaLabelCaption(
                   'symptomsChecker.useNhsOneOneOneOnline',
                   'symptomsChecker.checkIfYouNeedUrgentHelp')"/>

      <menu-item v-if="loggedIn && isCdssAdvice && isProofLevel9"
                 id="btn_gpAdvice"
                 header-tag="h2"
                 data-purpose="text_link"
                 :click-func="navigate"
                 :href="gpAdviceConditionsPath"
                 :text="$t('symptomsChecker.askYourGpForAdvice')"
                 :description="$t('symptomsChecker.consultThroughOnlineForm')"
                 :aria-label="ariaLabelCaption(
                   'symptomsChecker.askYourGpForAdvice',
                   'symptomsChecker.consultThroughOnlineForm')"/>
    </menu-item-list>
  </div>
</template>

<script>
import MenuItem from '@/components/MenuItem';
import MenuItemList from '@/components/MenuItemList';
import NativeApp from '@/services/native-app';
import sjrIf from '@/lib/sjrIf';
import { SYMPTOMS_PATH, APPOINTMENT_GP_ADVICE_PATH } from '@/router/paths';
import { APPOINTMENT_GP_ADVICE_NAME } from '@/router/names';
import { redirectTo } from '@/lib/utils';
import {
  SYMPTOM_CHECKER_NATIVE_QUERY_PARAMS,
} from '@/router/externalLinks';

export default {
  name: 'SymptomsCheck',
  components: {
    MenuItemList,
    MenuItem,
  },
  data() {
    let symptomsCheckerUrl = this.$store.$env.SYMPTOM_CHECKER_URL;
    if (this.$store.state.device.isNativeApp) {
      symptomsCheckerUrl += SYMPTOM_CHECKER_NATIVE_QUERY_PARAMS;
    }
    return {
      symptomsCheckerUrl,
      conditionsCheckerUrl: this.$store.$env.CONDITIONS_CHECKER_URL,
      symptomsPath: SYMPTOMS_PATH,
      coronaCheckerUrl: this.$store.$env.CORONA_SERVICE_URL,
      gpAdviceConditionsPath: APPOINTMENT_GP_ADVICE_NAME,
    };
  },
  computed: {
    loggedIn() {
      return this.$store.getters['session/isLoggedIn'];
    },
    isCdssAdvice() {
      return sjrIf({ $store: this.$store, journey: 'cdssAdvice' });
    },
    isProofLevel9() {
      return this.$store.getters['session/isProofLevel9'];
    },
  },
  methods: {
    navigate(event) {
      if (event.currentTarget.pathname === this.gpAdviceConditionsPath) {
        this.$store.dispatch('onlineConsultations/setPreviousRoute', this.symptomsPath);
        this.$store.dispatch('navigation/setBackLinkOverride', this.symptomsPath);
        this.$store.dispatch('navigation/setRouteCrumb', 'symptomsCrumb');
        this.$store.dispatch('navigation/setNewMenuItem', 0);
      }
      redirectTo(this, APPOINTMENT_GP_ADVICE_PATH);
      event.preventDefault();
    },
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
