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
            <p class="nhsuk-u-padding-top-0 nhsuk-u-margin-bottom-3 nhsuk-u-padding-bottom-0">
              {{ $t('prescriptions.partialSuccess.ifYouNeedToOrderNow') }}</p>
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
              {{ $t('prescriptions.partialSuccess.theOrderStatusWillBeUpdated') }}
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
            <p>
              <a id="back_to_prescriptions_link" href="#" @click="backToPrescriptionsClicked">
                {{ $t('prescriptions.partialSuccess.goToYourPrescriptionOrders') }}
              </a>
            </p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import GreenTick from '@/components/icons/GreenTick';
import RedCross from '@/components/icons/RedCross';
import { redirectTo } from '@/lib/utils';
import { PRESCRIPTIONS_PATH, PRESCRIPTIONS_VIEW_ORDERS_PATH } from '@/router/paths';

export default {
  layout: 'nhsuk-layout',
  components: {
    GreenTick,
    RedCross,
  },
  data() {
    return {
      successfulOrders: this.$store.state.repeatPrescriptionCourses
        .partialOrderResult.successfulOrders,
      unsuccessfulOrders: this.$store.state.repeatPrescriptionCourses
        .partialOrderResult.unsuccessfulOrders,
    };
  },
  mounted() {
    this.$store.dispatch('repeatPrescriptionCourses/completeOrderJourney');
  },
  created() {
    if (!this.$store.state.repeatPrescriptionCourses.partialOrderResult) {
      redirectTo(this, PRESCRIPTIONS_PATH);
    }
  },
  methods: {
    backToPrescriptionsClicked() {
      redirectTo(this, PRESCRIPTIONS_VIEW_ORDERS_PATH);
    },
  },
};
</script>

<style module lang="scss" scoped>
@import "../../style/forms";
@import "../../style/panels";
</style>
