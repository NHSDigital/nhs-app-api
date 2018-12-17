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
      <symptom-banner/>
    </header>
  </div>
</template>

<script>
import SymptomBanner from '@/components/SymptonBanner';
import NhsLogo from '@/components/icons/NhsLogo';
import HelpIcon from '../components/icons/HelpIcon';

export default {
  components: {
    SymptomBanner,
    NhsLogo,
    HelpIcon,
  },
  data() {
    return {
      helpAndSupportURL: this.$store.app.$env.HELP_AND_SUPPORT_URL,
    };
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
    top: 0;
    right: 0;
  }
</style>
