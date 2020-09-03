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
      <div :class="$style.symptomBanner">
        <h2>{{ $t('login.howAreYouFeelingToday') }}</h2>
        <generic-button id="btn_home_symptoms"
                        :button-classes="['nhsuk-body', 'nhsuk-button',
                                          $store.state.device.isNativeApp
                                            ?'button':'', 'white']"
                        @click.prevent="checkSymptomsButtonClicked()">
          {{ $t('login.checkSymptoms') }}
        </generic-button>
      </div>
    </header>
  </div>
</template>

<script>
import GenericButton from '@/components/widgets/GenericButton';
import NhsLogo from '@/components/icons/NhsLogo';
import HelpIcon from '@/components/icons/HelpIcon';
import { CHECKYOURSYMPTOMS_PATH } from '@/router/paths';
import { redirectTo } from '@/lib/utils';
import {
  HELP_AND_SUPPORT_URL,
} from '@/router/externalLinks';

export default {
  name: 'HomeHeader',
  components: {
    GenericButton,
    NhsLogo,
    HelpIcon,
  },
  data() {
    return {
      helpAndSupportURL: HELP_AND_SUPPORT_URL,
    };
  },
  methods: {
    checkSymptomsButtonClicked() {
      redirectTo(this, CHECKYOURSYMPTOMS_PATH);
    },
  },
};
</script>

<style module lang="scss">
  @import "../style/homeheader";
  a.anchor-icon{
    color: $white;
    right: 0;
  }
  a.fixed-right {
    position: absolute;
    right: 0;
  }
  .symptomBanner {
    border-top:solid 1px #F0F4F5;
    width:100%;
    padding-right: 20px;
    padding-left:17px;

    h2 {
      color:$white;
    }
  }
</style>
