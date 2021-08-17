<template>
  <div :class="$style['home-header']">
    <header>
      <a id="help_icon" :class="[$style['anchor-icon'],$style['fixed-right']]"
         :href="helpAndSupportURL" target="_blank" rel="noopener noreferrer" tabindex="-1">
        <help-icon/>
      </a>
      <div :class="$style.spacer" />
      <nhs-logo/>
      <div :class="$style.spacer" />
      <div :class="$style.adviceBanner">
        <h2>{{ $t('login.howAreYouFeelingToday') }}</h2>
        <generic-button id="btn_home_symptoms"
                        :button-classes="['nhsuk-body', 'nhsuk-button',
                                          $store.state.device.isNativeApp
                                            ?'button':'', 'white']"
                        @click.prevent="checkAdviceButtonClicked()">
          {{ $t('login.adviceChecker') }}
        </generic-button>
      </div>
    </header>
  </div>
</template>

<script>
import GenericButton from '@/components/widgets/GenericButton';
import NhsLogo from '@/components/icons/NhsLogo';
import HelpIcon from '@/components/icons/HelpIcon';
import { GET_HEALTH_ADVICE_PATH } from '@/router/paths';
import { redirectTo } from '@/lib/utils';

export default {
  name: 'HomeHeader',
  components: {
    GenericButton,
    NhsLogo,
    HelpIcon,
  },
  data() {
    return {
      helpAndSupportURL: this.$store.$env.BASE_NHS_APP_HELP_URL,
    };
  },
  methods: {
    checkAdviceButtonClicked() {
      redirectTo(this, GET_HEALTH_ADVICE_PATH);
    },
  },
};
</script>

<style module lang="scss">
  @import "@/style/custom/home-header";
</style>
