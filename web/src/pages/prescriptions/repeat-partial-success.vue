<template>
  <div
    v-if="showTemplate"
    :class="[$style['pull-content'],
             !$store.state.device.isNativeApp && $style.desktopWeb]">
    <div :class="$style.info" data-purpose="info">
      <div v-if="unsuccessfulOrders" :class="$style.panel">
        <p>
          <b>{{ $t('prescriptions.partialSuccess.medicationNotOrdered') }}</b>
        </p>
        <p>{{ $t('prescriptions.partialSuccess.needMedicationNow') }}</p>
        <div class="nhsuk-do-dont-list" :class="$style['do-dont-list-spacing']">
          <ul class="nhsuk-list nhsuk-list--cross">
            <li v-for="(order) in unsuccessfulOrders" :key="order.name"
                :class="$style['list-item-spacing']">
              <Red-Cross />
              <b>{{ order.name }}</b>
            </li>
          </ul>
        </div>
      </div>

      <div v-if="successfulOrders" :class="$style.panel">
        <p>
          <b>{{ $t('prescriptions.partialSuccess.medicationOrdered') }}</b>
        </p>
        <p>{{ $t('prescriptions.partialSuccess.orderStatusUpdate') }}</p>
        <div class="nhsuk-do-dont-list" :class="$style['do-dont-list-spacing']">
          <ul class="nhsuk-list nhsuk-list--tick">
            <li v-for="(order) in successfulOrders" :key="order.name"
                :class="$style['list-item-spacing']">
              <Green-Tick />
              <b>{{ order.name }}</b>
            </li>
          </ul>
        </div>
      </div>

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
</template>

<script>
import GreenTick from '@/components/icons/GreenTick';
import RedCross from '@/components/icons/RedCross';
import GenericButton from '@/components/widgets/GenericButton';
import NoJsForm from '@/components/no-js/NoJsForm';
import { redirectTo } from '@/lib/utils';
import { PRESCRIPTIONS } from '@/lib/routes';

export default {
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
@import "../../style/info";
@import "../../style/panels";

.pull-content {
  &.desktopWeb {
    font-family: $frutiger-light;
    & > * {
      max-width: 540px;
    }
  }
  .panel {
    margin-bottom: 1em;
    b {
      padding-bottom: 0;
      padding-top: 0;
    }
  }
  .do-dont-list-spacing {
    margin-top: 1em;
    margin-bottom: 0;
    padding: 1em;
  }
  .list-item-spacing {
    margin-left: 1em;
    padding-top: 0;
  }
}
</style>
