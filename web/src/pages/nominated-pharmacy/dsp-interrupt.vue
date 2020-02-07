<template>
  <div v-if="showTemplate">
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <generic-button id="continue-button"
                        :button-classes="['nhsuk-button', 'nhsuk-button--primary']"
                        @click.stop.prevent="continueButtonClicked">
          {{ $t('nominated_pharmacy.dspInterrupt.continueButton') }}
        </generic-button>
      </div>
    </div>
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <analytics-tracked-tag v-if="!$store.state.device.isNativeApp"
                               :text="$t('generic.backButton.text')"
                               :tabindex="-1">
          <desktopGenericBackLink id="back-link"
                                  :button-text="'generic.backButton.text'"
                                  @clickAndPrevent="backButtonClicked"/>
        </analytics-tracked-tag>
      </div>
    </div>
  </div>
</template>

<script>
import GenericButton from '@/components/widgets/GenericButton';
import { redirectTo } from '@/lib/utils';
import { NOMINATED_PHARMACY_CHOOSE_TYPE, NOMINATED_PHARMACY_ONLINE_ONLY_CHOICES } from '@/lib/routes';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';

export default {
  layout: 'nhsuk-layout',
  components: {
    GenericButton,
    DesktopGenericBackLink,
    AnalyticsTrackedTag,
  },
  methods: {
    continueButtonClicked() {
      redirectTo(this, NOMINATED_PHARMACY_ONLINE_ONLY_CHOICES.path);
    },
    backButtonClicked() {
      redirectTo(this, NOMINATED_PHARMACY_CHOOSE_TYPE.path);
    },
  },
};
</script>

<style scoped>

</style>
