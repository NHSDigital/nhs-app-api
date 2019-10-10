<template>
  <scr-error-no-access v-if="showError"
                       :data="medications"
                       :class="[$style['record-content'],
                                getCollapseState]"
                       :aria-hidden="isCollapsed"
                       :has-undetermined-access="hasUndeterminedAccess"/>
  <div v-else-if="!isCollapsed" :class="[$style['record-content'], getCollapseState,
                                         !$store.state.device.isNativeApp && $style.desktopWeb]"
       :aria-hidden="isCollapsed">
    <div v-for="(medication, medIndex) in orderedMedications"
         :key="`medication-${medIndex}`" :class="$style['record-item']"
         data-purpose="record-item">
      <p v-if="medication.date" data-purpose="record-item-header"
         class="nhsuk-u-padding-0 nhsuk-u-margin-0 nhsuk-u-padding-left-3 nhsuk-u-padding-top-3
         nhsuk-body-s">{{ medication.date | datePart }}</p>
      <p v-else data-purpose="record-item-header"
         class="nhsuk-u-padding-0 nhsuk-u-margin-0 nhsuk-u-padding-left-3 nhsuk-u-padding-top-3
         nhsuk-body-s">{{ $t('my_record.noStartDate') }}</p>
      <p v-for="(lineItem, lineItemIndex) in medication.lineItems"
         :key="`line-${lineItemIndex}`" data-purpose="record-item-detail"
         class="nhsuk-u-padding-0 nhsuk-u-margin-0 nhsuk-u-padding-left-3">
        {{ lineItem.text }}
        <ul>
          <li v-for="(innerLineItem, innerLineItemIndex) in lineItem.lineItems"
              :key="`innerline-${innerLineItemIndex}`">
            {{ innerLineItem }}
          </li>
        </ul>
      </p>
      <hr aria-hidden="true">
    </div>
  </div>
</template>

<script>

import orderBy from 'lodash/fp/orderBy';
import ScrErrorNoAccess from '@/components/my-record/SharedComponents/SCRErrorNoAccess';

export default {
  name: 'Medications',
  components: {
    ScrErrorNoAccess,
  },
  props: {
    medications: {
      type: Array,
      default: () => [],
    },
    hasUndeterminedAccess: {
      type: Boolean,
      default: false,
    },
    isCollapsed: {
      type: Boolean,
      default: true,
    },
    hasError: {
      type: Boolean,
      default: false,
    },
  },
  computed: {
    getCollapseState() {
      return this.isCollapsed ? this.$style.closed : this.$style.opened;
    },
    orderedMedications() {
      return orderBy([obj => obj.date], ['desc'])(this.medications);
    },
    showError() {
      return this.medications.hasErrored
             || (this.medications && this.medications.length === 0);
    },
  },
};

</script>

<style module lang="scss" scoped>
@import '../../../style/medrecordcontent';
</style>
