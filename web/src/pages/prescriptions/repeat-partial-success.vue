<template>
  <div
    v-if="showTemplate">
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <div data-purpose="info">
          <div v-if="unsuccessfulOrders" :class="$style.panel">
            <h2 class="nhsuk-u-padding-bottom-2 nhsuk-u-margin-bottom-0">
              {{ $t('prescriptions.partialSuccess.medicationNotOrdered') }}
            </h2>
            <p class="nhsuk-u-padding-top-0
            nhsuk-u-margin-bottom-3
            nhsuk-u-padding-bottom-0">{{ $t('prescriptions.partialSuccess.needMedicationNow') }}</p>
            <div class="nhsuk-do-dont-list
                        nhsuk-u-margin-top-3
                        nhsuk-u-margin-bottom-3
                        nhsuk-u-padding-left-0
                        nhsuk-u-padding-bottom-0">
              <ul class="nhsuk-list nhsuk-list--cross">
                <li v-for="(order) in unsuccessfulOrders" :key="order.name"
                    class="nhsuk-u-padding-top-0
                nhsuk-u-padding-bottom-0">
                  <Red-Cross />
                  {{ order.name }}
                </li>
              </ul>
            </div>
          </div>

          <div v-if="successfulOrders" :class="$style.panel">
            <h2 class="nhsuk-u-padding-bottom-2 nhsuk-u-margin-bottom-0">
              {{ $t('prescriptions.partialSuccess.medicationOrdered') }}
            </h2>
            <p class="nhsuk-u-padding-top-0
            nhsuk-u-margin-bottom-3
            nhsuk-u-padding-bottom-0">
              {{ $t('prescriptions.partialSuccess.orderStatusUpdate') }}
            </p>
            <div class="nhsuk-do-dont-list
                        nhsuk-u-margin-top-3
                        nhsuk-u-margin-bottom-3
                        nhsuk-u-padding-left-0
                        nhsuk-u-padding-bottom-0">
              <ul class="nhsuk-list nhsuk-list--tick">
                <li v-for="(order) in successfulOrders" :key="order.name"
                    class="nhsuk-u-padding-top-0
                nhsuk-u-padding-bottom-0">
                  <Green-Tick />
                  {{ order.name }}
                </li>
              </ul>
            </div>
          </div>
          <div class="nhsuk-u-margin-top-3">
            <no-js-form :action="prescriptionsHomeUrl" :value="{}" method="get">
              <generic-button
                id="btn_back_to_prescriptions"
                :button-classes="['nhsuk-button', 'nhs-uk-button--secondary']"
                :class="$style.back"
                tabindex="0"
                @click="backToPrescriptionsClicked">
                {{ $t('prescriptions.partialSuccess.backButton') }}
              </generic-button>
            </no-js-form>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import GreenTick from '@/components/icons/GreenTick';
import RedCross from '@/components/icons/RedCross';
import GenericButton from '@/components/widgets/GenericButton';
import NoJsForm from '@/components/no-js/NoJsForm';
import { redirectTo } from '@/lib/utils';
import { PRESCRIPTIONS } from '@/lib/routes';

export default {
  layout: 'nhsuk-layout',
  components: {
    GreenTick,
    RedCross,
    GenericButton,
    NoJsForm,
  },
  data() {
    return {
      successfulOrders: this.$store.state.repeatPrescriptionCourses
        .partialOrderResult.successfulOrders,
      unsuccessfulOrders: this.$store.state.repeatPrescriptionCourses
        .partialOrderResult.unsuccessfulOrders,
      prescriptionsHomeUrl: PRESCRIPTIONS.path,
    };
  },
  mounted() {
    this.$store.dispatch('repeatPrescriptionCourses/completeOrderJourney');
  },
  created() {
    if (!this.$store.state.repeatPrescriptionCourses.partialOrderResult) {
      redirectTo(this, PRESCRIPTIONS.path);
    }
  },
  methods: {
    backToPrescriptionsClicked() {
      redirectTo(this, PRESCRIPTIONS.path);
    },
  },
};
</script>

<style module lang="scss" scoped>
@import "../../style/forms";
@import "../../style/panels";
</style>
