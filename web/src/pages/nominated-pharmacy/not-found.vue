<template>
  <div v-if="showTemplate"
       id="mainDiv"
       :class="[$style['pull-content'], !$store.state.device.isNativeApp && $style.desktopWeb]">
    <div :class="$style.info" data-purpose="info">
      <message-dialog message-type="warning" icon-text="Important">
        <message-text id="warning-text"
                      :class="$style.warningText">
          {{ $t('nominatedPharmacyNotFound.warningText') }}
        </message-text>
      </message-dialog>
      <p id="instruction">
        {{ $t('nominatedPharmacyNotFound.line') }}
      </p>
      <a id="link-to-add-pharmacy"
         :class="[$style.checkFeaturesLink, $style['link']]"
         href="#"
         tag="a"
         @click.prevent="goToAddNominatedPharmacy">
        {{ $t('nominatedPharmacyNotFound.nominatedPharmacyLink') }}
      </a>
    </div>

    <generic-button id="continue-button"
                    :button-classes="['green', 'button']"
                    @click.prevent="onContinueButtonClicked">
      {{ $t('nominatedPharmacyNotFound.continueButton') }}
    </generic-button>

    <analytics-tracked-tag :text="$t('th03.errors.backButton')">
      <generic-button id="back-button"
                      :button-classes="['grey', 'button']" :class="$style.back"
                      tabindex="0" @click.prevent="onBackButtonClicked">
        {{ $t('nominatedPharmacyNotFound.backButton') }}
      </generic-button>
    </analytics-tracked-tag>
  </div>
</template>

<script>
import GenericButton from '@/components/widgets/GenericButton';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import { PRESCRIPTIONS, PRESCRIPTION_REPEAT_COURSES, NOMINATED_PHARMACY_SEARCH } from '@/lib/routes';
import { redirectTo } from '@/lib/utils';

export default {
  components: {
    GenericButton,
    MessageDialog,
    MessageText,
  },
  methods: {
    onContinueButtonClicked() {
      redirectTo(this, PRESCRIPTION_REPEAT_COURSES.path, null);
    },
    onBackButtonClicked() {
      redirectTo(this, PRESCRIPTIONS.path, null);
    },
    goToAddNominatedPharmacy() {
      redirectTo(this, NOMINATED_PHARMACY_SEARCH.path, null);
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import '../../style/info';
  @import '../../style/fonts';
  @import '../../style/buttons';
  @import '../../style/textstyles';

  .link {
    margin-top: 0.5em;
    margin-bottom: 1.5em;
    cursor: pointer;
    text-decoration: underline;
    color: #005EB8;
    display: block;
    font-weight: bold;
  }
  div {
   &.desktopWeb {
    max-width: 540px;

    .warningText {
     font-family: $default_web;
     font-weight: normal;
    }

    li {
     font-family: $default_web;
     font-weight: normal;
    }

    p {
     font-family: $default_web;
     font-weight: normal;
    }
   }
  }
</style>
