<template>
  <div v-if="showTemplate" :class="[$style['pull-content'], $style.content,
                                    !$store.state.device.isNativeApp && $style.desktopWeb]">
    <div v-if="hasNoNominatedPharmacy">
      <no-nominated-pharmacy-warning/>
    </div>
    <div v-else
         :class="$style.info" data-purpose="info">
      <pharmacy-detail
        :pharmacy="nominatedPharmacy"
        :is-my-nominated-pharmacy="true"
        :previous-path="currentPage"
        :can-change-pharmacy="isCommunityPharmacy"
        :show-instruction="true"/>
    </div>
    <message-dialog v-if="!isCommunityPharmacy"
                    id="warning-dialog-dispensing-practice"
                    message-type="warning"
                    :icon-text="$t('messageIconText.important')">
      <message-text id="warning-text-1"
                    :class="$style.warningText">
        {{ $t('nominated_pharmacy.warning.changeDispensingPractice.line1') }}
      </message-text>
      <message-text id="warning-text-2"
                    :class="$style.warningText">
        {{ $t('nominated_pharmacy.warning.changeDispensingPractice.line2') }}
      </message-text>
    </message-dialog>
    <analytics-tracked-tag :text="$t('generic.backButton.text')"
                           :tabindex="-1">
      <desktopGenericBackLink v-if="!$store.state.device.isNativeApp"
                              :id="'back-button'"
                              :path="prescriptionsPath"
                              :button-text="'generic.backButton.text'"
                              @clickAndPrevent="backButtonClicked"/>
    </analytics-tracked-tag>
  </div>
</template>

<script>
/* eslint-disable global-require */
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import PharmacyDetail from '@/components/nominatedPharmacy/PharmacyDetail';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import NoNominatedPharmacyWarning from '@/components/nominatedPharmacy/NoNominatedPharmacyWarning';
import PharmacyType from '@/lib/pharmacy-detail/pharmacy-types';
import { PRESCRIPTIONS, NOMINATED_PHARMACY } from '@/lib/routes';
import { redirectTo } from '@/lib/utils';

export default {
  components: {
    AnalyticsTrackedTag,
    PharmacyDetail,
    DesktopGenericBackLink,
    NoNominatedPharmacyWarning,
    MessageDialog,
    MessageText,
  },
  data() {
    return {
      nominatedPharmacy: this.$store.state.nominatedPharmacy.pharmacy,
      hasNoNominatedPharmacy: this.$store.getters['nominatedPharmacy/hasNoNominatedPharmacy'],
      currentPage: NOMINATED_PHARMACY.path,
      prescriptionsPath: PRESCRIPTIONS.path,
    };
  },
  computed: {
    isCommunityPharmacy() {
      return (
        this.$store.state.nominatedPharmacy.pharmacy.pharmacyType === PharmacyType.P1);
    },
  },
  async asyncData({ store }) {
    if (store.state.nominatedPharmacy.hasLoaded === false) {
      await store.dispatch('nominatedPharmacy/clear');
      await store.dispatch('nominatedPharmacy/load');
    }
  },
  created() {
    if (!this.$store.getters['nominatedPharmacy/nominatedPharmacyEnabled']) {
      redirectTo(this, this.prescriptionsPath);
    }
  },
  mounted() {
    if (this.$store.state.nominatedPharmacy.hasLoaded) {
      this.$store.dispatch('flashMessage/show');
    }
  },
  methods: {
    backButtonClicked() {
      redirectTo(this, this.prescriptionsPath);
    },
  },
};
</script>

<style module lang="scss" scoped>
@import '../../style/listmenu';
@import "../../style/panels";
@import "../../style/colours";
@import "../../style/textstyles";
@import "../../style/home";

div {
  &.desktopWeb {
    max-width: 540px;

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
