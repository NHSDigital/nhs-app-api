<template>
  <div :class="dynamicStyle('symptomBanner')">
    <h2>{{ $t('symptomBanner.howAreYouFeeling') }}</h2>
    <form method="get" action="/check-your-symptoms">
      <generic-button id="btn_home_symptoms"
                      :class="[$style.white, ...dynamicStyle('button')]"
                      @click.prevent="checkSymptomsButtonClicked()">

        <svg :class="dynamicStyle('nhsuk-icon__arrow-right-circle')" xmlns="http://www.w3.org/2000/svg"
             viewBox="0 0 24 24" aria-hidden="true">
          <path d="M0 0h24v24H0z" fill="none"/>
          <path d="
          M12 2a10 10 0 0 0-9.95 9h11.64L9.74 7.05a1 1 0 0 1 1.41-1.41l5.66 5.65a1 1 0 0
           1 0 1.42l-5.66 5.65a1 1 0 0 1-1.41 0 1 1 0 0 1 0-1.41L13.69 13H2.05A10 10 0 1 0 12 2z"/>
        </svg>

        {{ $t('symptomBanner.checker') }}
      </generic-button>
    </form>
  </div>
</template>
<script>

import GenericButton from '@/components/widgets/GenericButton';
import { getDynamicStyle, exchangeStyle } from '@/lib/desktop-experience';

export default {
  name: 'SymptonBanner',
  components: {
    GenericButton,
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
    /*padding: 1rem 2rem;*/
    background: none;
    margin: 0;
    outline: none;

    h2 {
      color: $nhs_blue;
    }

    &:hover {
      background: #ffcd60;
      box-shadow: 0 0 0 4px #ffcd60;
      color: #212B32;
      text-decoration: none;
    }

    &:focus {
      box-shadow: 0 0 0 4px $focus_highlight;
    }

    &:visited {
      color: #330072;
    }
  }

  /** Links */
  .nhsuk-icon__arrow-right-circle-desktop {
    display: inline-block;
    vertical-align: middle;
    fill: #007f3b;
    height: 1.2em;
    left: -3px;
    top: -6px;
    width: 1.2em;
  }

  .nhsuk-icon, .nhsuk-icon__arrow-right-circle {
    display: none;
  }
</style>
