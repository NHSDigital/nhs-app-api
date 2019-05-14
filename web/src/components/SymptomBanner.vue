<template>
  <div :class="dynamicStyle('symptomBanner')">
    <h2>{{ $t('symptomBanner.howAreYouFeeling') }}</h2>
    <form method="get" action="/check-your-symptoms">
      <generic-button id="btn_home_symptoms"
                      :class="[$style.white, ...dynamicStyle('button')]"
                      @click.prevent="checkSymptomsButtonClicked()">

        <NHS-Arrow-Circle />

        {{ $t('symptomBanner.checker') }}
      </generic-button>
    </form>
  </div>
</template>
<script>

import GenericButton from '@/components/widgets/GenericButton';
import NHSArrowCircle from '@/components/icons/NHSArrowCircle';
import { getDynamicStyle, exchangeStyle } from '@/lib/desktop-experience';

export default {
  name: 'SymptomBanner',
  components: {
    GenericButton,
    NHSArrowCircle,
  },
  methods: {
    checkSymptomsButtonClicked() {
      this.$store.dispatch('device/goToCheckSymptoms');
      // this method will be refactored to the following:
      // const sourceValue = this.$store.state.device.source;
      // redirectTo('this, /check-your-symptoms', { source: sourceValue })
    },
    dynamicStyle(...args) {
      return getDynamicStyle(this, args, exchangeStyle({ button: 'btn_home_symptoms-desktop' }));
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import "../style/home";
  @import "../style/elements";
  @import '../style/buttons';

  .button-desktop {
    outline: none;
  }

  .btn_home_symptoms-desktop {
    @include default_text;
    color: $nhs_blue;
    font-size: 1em;
    line-height: 1em;
    font-weight: bold;
    text-decoration: none;
    vertical-align: middle;
    cursor: pointer;
    display: inline-block;
    border: none;
    background: none;
    margin: 0;
    outline: none;

    h2 {
      color: $nhs_blue;
    }
    &:focus {
      @include actionLinkHighlightStyle;
      background-color: $focus_highlight;
    }
    &:hover {
      @include linkHoverStyle;
    }
    &:visited {
      color: #330072;
    }
  }

</style>
