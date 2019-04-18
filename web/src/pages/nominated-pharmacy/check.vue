<template>
  <div v-if="showTemplate"
       id="mainDiv"
       :class="[$style['pull-content'], !$store.state.device.isNativeApp && $style.desktopWeb]">
    <div v-if="hasNoNominatedPharmacy"
         :class="$style.info" data-purpose="info">
      <message-dialog message-type="warning" icon-text="Important">
        <message-text id="warning-text"
                      :class="$style.warningText">
          {{ $t('nominatedPharmacyNotFound.warningText') }}
        </message-text>
      </message-dialog>
      <p id="instruction">
        {{ $t('nominatedPharmacyNotFound.line') }}
      </p>
      <a id="link-to-nominate-pharmacy"
         :class="[$style.checkFeaturesLink, $style['link']]"
         href="#"
         tag="a"
         @click.prevent="goToAddOrChangeNominatedPharmacy">
        {{ $t('nominatedPharmacyNotFound.nominatedPharmacyLink') }}
      </a>
    </div>
    <div v-else
         :class="$style.info" data-purpose="info">
      <pharmacy-detail id="pharmacy-details"
                       :pharmacy="pharmacy"
                       :is-my-nominated-pharmacy="true" />
    </div>

    <generic-button id="continue-button-found"
                    :button-classes="['green', 'button']"
                    @click.prevent="onContinueButtonClicked">
      {{ getContinueButtonText }}
    </generic-button>

    <generic-button id="back-button"
                    :button-classes="['grey', 'button']" :class="$style.back"
                    tabindex="0" @click.prevent="onBackButtonClicked">
      {{ $t('nominatedPharmacyNotFound.backButton') }}
    </generic-button>
  </div>
</template>

<script>
import GenericButton from '@/components/widgets/GenericButton';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import PharmacyDetail from '@/components/nominatedPharmacy/PharmacyDetail';
import { PRESCRIPTIONS, PRESCRIPTION_REPEAT_COURSES, NOMINATED_PHARMACY_SEARCH } from '@/lib/routes';
import { redirectTo } from '@/lib/utils';

export default {
  components: {
    GenericButton,
    MessageDialog,
    MessageText,
    PharmacyDetail,
  },
  data() {
    return {
      pharmacy: this.$store.state.nominatedPharmacy.pharmacy,
      hasNoNominatedPharmacy: this.$store.getters['nominatedPharmacy/hasNoNominatedPharmacy'],
    };
  },
  computed: {
    getContinueButtonText() {
      return this.hasNoNominatedPharmacy ?
        this.$t('nominatedPharmacyNotFound.continueButton') :
        this.$t('nominatedPharmacy.continueButton');
    },
  },
  created() {
    if (this.$store.state.nominatedPharmacy.hasLoaded === false) {
      redirectTo(this, PRESCRIPTIONS.path, null);
    }
  },
  methods: {
    onContinueButtonClicked() {
      redirectTo(this, PRESCRIPTION_REPEAT_COURSES.path, null);
    },
    onBackButtonClicked() {
      redirectTo(this, PRESCRIPTIONS.path, null);
    },
    goToAddOrChangeNominatedPharmacy() {
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
  @import "../../style/panels";
  @import '../../style/listmenu';
  @import "../../style/home";


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
