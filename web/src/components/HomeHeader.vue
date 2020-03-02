<template>
  <div :class="$style['home-header']">
    <header>
      <a id="help_icon" :class="[$style['anchor-icon'],$style['fixed-right']]"
         :href="helpAndSupportURL" target="_blank" tabindex="-1">
        <help-icon/>
      </a>
      <div :class="$style.spacer" />
      <nhs-logo/>
      <div :class="$style.spacer" />
      <div :class="$style.symptomBanner">
        <h2>{{ $t('symptomBanner.howAreYouFeeling') }}</h2>
        <generic-button id="btn_home_symptoms"
                        :button-classes="['nhsuk-body', 'nhsuk-button',
                                          $store.state.device.isNativeApp
                                            ?'button':'', 'white']"
                        @click.prevent="checkSymptomsButtonClicked()">
          {{ $t('symptomBanner.checker') }}
        </generic-button>
      </div>
    </header>
  </div>
</template>

<script>
import GenericButton from '@/components/widgets/GenericButton';
import NhsLogo from '@/components/icons/NhsLogo';
import HelpIcon from '../components/icons/HelpIcon';

export default {
  name: 'HomeHeader',
  components: {
    GenericButton,
    NhsLogo,
    HelpIcon,
  },
  data() {
    return {
      helpAndSupportURL: this.$store.app.$env.HELP_AND_SUPPORT_URL,
    };
  },
  computed: {
    symptomsUrl() {
      return '/check-your-symptoms';
    },
  },
  methods: {
    checkSymptomsButtonClicked() {
      this.$store.dispatch('device/goToCheckSymptoms');
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
