<template>
  <div v-if="showTemplate"
       :class="[$style['pull-content'], $style.content,
                !$store.state.device.isNativeApp && $style.desktopWeb]">
    <pharmacy-detail id="pharmacy-detail"
                     :pharmacy="nominatedPharmacy"
                     :is-my-nominated-pharmacy="false" />
    <generic-button id="confirm-button"
                    :button-classes="['nhsuk-button']"
                    @click.stop.prevent="submitNominatedPharmacy">
      {{ $t('nominated_pharmacy.confirm.confirmButton') }}
    </generic-button>
    <analytics-tracked-tag :text="$t('generic.backButton.text')"
                           :tabindex="-1">
      <generic-button v-if="$store.state.device.isNativeApp" id="back-button"
                      :button-classes="['nhsuk-button', 'nhsuk-button--secondary']"
                      tabindex="0" @click.prevent="cancelButtonClicked">
        {{ $t('generic.backButton.text') }}
      </generic-button>
      <desktopGenericBackLink v-else
                              id="back-link"
                              :path="nominatedPharmacySearchResultsPath"
                              :button-text="'generic.backButton.text'"
                              @clickAndPrevent="cancelButtonClicked"/>
    </analytics-tracked-tag>
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import GenericButton from '@/components/widgets/GenericButton';
import PharmacyDetail from '@/components/nominatedPharmacy/PharmacyDetail';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import { redirectTo } from '@/lib/utils';
import { NOMINATED_PHARMACY_SEARCH_RESULTS, NOMINATED_PHARMACY } from '@/lib/routes';

export default {
  components: {
    GenericButton,
    AnalyticsTrackedTag,
    PharmacyDetail,
    DesktopGenericBackLink,
  },
  data() {
    return {
      nominatedPharmacy: this.$store.state.nominatedPharmacy.selectedNominatedPharmacy,
      nominatedPharmacySearchResultsPath: NOMINATED_PHARMACY_SEARCH_RESULTS.path,
    };
  },
  created() {
    if (this.nominatedPharmacy === null) {
      redirectTo(this, NOMINATED_PHARMACY.path);
    }
  },
  methods: {
    async submitNominatedPharmacy() {
      try {
        let successMessage = this.$t('nominated_pharmacy.confirm.pharmacyChanged');
        if (this.$store.state.nominatedPharmacy.pharmacy.pharmacyName === undefined) {
          successMessage = this.$t('nominated_pharmacy.confirm.pharmacyChosen');
        }
        await this.$store.dispatch('nominatedPharmacy/update', this.nominatedPharmacy.odsCode);
        this.$store.dispatch('flashMessage/addSuccess', successMessage);
        this.$store.dispatch('nominatedPharmacy/clearSelectedNominatedPharmacy');
        redirectTo(this, NOMINATED_PHARMACY.path);
      } catch (error) {
        /*
        empty catch block as the
        ApiError.vue (component) handles and
        surfaces appropriate error content based on the http status code returned from the API
        */
      }
    },
    cancelButtonClicked() {
      redirectTo(this, this.nominatedPharmacySearchResultsPath);
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import "../../style/spacings";
  @import "../../style/buttons";
  @import "../../style/info";

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
