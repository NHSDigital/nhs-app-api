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
        :can-change-pharmacy="true"
        :show-instruction="true"/>
    </div>
    <analytics-tracked-tag :text="$t('generic.backButton.text')"
                           :tabindex="-1">
      <generic-button v-if="$store.state.device.isNativeApp"
                      :id="'back-button'"
                      :button-classes="['grey', 'button']"
                      :class="$style.back"
                      @click.prevent="backButtonClicked">
        {{ $t('generic.backButton.text') }}
      </generic-button>
      <desktopGenericBackLink v-else
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
import GenericButton from '@/components/widgets/GenericButton';
import PharmacyDetail from '@/components/nominatedPharmacy/PharmacyDetail';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import NoNominatedPharmacyWarning from '@/components/nominatedPharmacy/NoNominatedPharmacyWarning';
import { PRESCRIPTIONS, NOMINATED_PHARMACY } from '@/lib/routes';
import { redirectTo } from '@/lib/utils';

export default {
  components: {
    AnalyticsTrackedTag,
    GenericButton,
    PharmacyDetail,
    DesktopGenericBackLink,
    NoNominatedPharmacyWarning,
  },
  data() {
    return {
      nominatedPharmacy: this.$store.state.nominatedPharmacy.pharmacy,
      hasNoNominatedPharmacy: this.$store.getters['nominatedPharmacy/hasNoNominatedPharmacy'],
      currentPage: NOMINATED_PHARMACY.path,
      prescriptionsPath: PRESCRIPTIONS.path,
    };
  },
  async asyncData({ store }) {
    if (store.state.nominatedPharmacy.hasLoaded === false) {
      await store.dispatch('nominatedPharmacy/clear');
      await store.dispatch('nominatedPharmacy/load');
    }
  },
  created() {
    if (!this.$store.getters['nominatedPharmacy/nominatedPharmacyEnabled']) {
      redirectTo(this, this.prescriptionsPath, null);
    }
  },
  methods: {
    backButtonClicked() {
      redirectTo(this, this.prescriptionsPath, null);
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
