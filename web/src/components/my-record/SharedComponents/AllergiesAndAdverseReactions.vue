<template>
  <scr-error-no-access v-if="showError"
                       :class="[$style['record-content'], getCollapseState]"
                       :aria-hidden="isCollapsed"
                       :has-errored="allergies.hasErrored"
                       :has-undetermined-access="allergies.hasUndeterminedAccess"/>

  <div v-else-if="!isCollapsed" :class="[$style['record-content'], getCollapseState,
                                         !$store.state.device.isNativeApp && $style.desktopWeb]"
       :aria-hidden="isCollapsed">
    <div v-for="(allergy, index) in orderedAllergies" :key="`allergy.name-${index}`"
         :class="$style['record-item']" data-purpose="record-item">
      <p v-if="allergy.date && allergy.date.value" data-purpose="record-item-header"
         class="nhsuk-u-padding-0 nhsuk-u-margin-0 nhsuk-u-padding-left-3 nhsuk-u-padding-top-3
         nhsuk-body-s">
        {{ allergy.date.value | datePart(allergy.date.datePart) }}
      </p>
      <p v-else data-purpose="record-item-header"
         class="nhsuk-u-padding-0 nhsuk-u-margin-0 nhsuk-u-padding-left-3 nhsuk-u-padding-top-3
         nhsuk-body-s">{{ $t('my_record.noStartDate') }}</p>

      <p data-purpose="record-item-detail"
         class="nhsuk-u-padding-0 nhsuk-u-margin-0 nhsuk-u-padding-left-3">{{ allergy.name }}</p>
      <p v-if="allergy.drug" data-purpose="record-item-detail"
         class="nhsuk-u-padding-0 nhsuk-u-margin-0 nhsuk-u-padding-left-3">
        {{ allergy.drug }}</p>
      <p v-if="allergy.reaction" data-purpose="record-item-detail"
         class="nhsuk-u-padding-0 nhsuk-u-margin-0 nhsuk-u-padding-left-3">
        {{ allergy.reaction }}</p>
      <hr aria-hidden="true">
    </div>
  </div>
</template>

<script>

import orderBy from 'lodash/fp/orderBy';
import ScrErrorNoAccess from '@/components/my-record/SharedComponents/SCRErrorNoAccess';

export default {
  name: 'AllergiesAndAdverseReactions',
  components: {
    ScrErrorNoAccess,
  },
  props: {
    allergies: {
      type: Object,
      default: () => {},
    },
    isCollapsed: {
      type: Boolean,
      default: true,
    },
  },
  computed: {
    getCollapseState() {
      return this.isCollapsed ? this.$style.closed : this.$style.opened;
    },
    orderedAllergies() {
      return orderBy([obj => this.getEffectiveDate(obj.date, '')], ['desc'])(this.allergies.data);
    },
    showError() {
      return this.allergies.hasErrored || this.allergies.data.length === 0;
    },
  },
  methods: {
    getEffectiveDate(effectiveDate, defaultValue) {
      return effectiveDate && effectiveDate.value ? effectiveDate.value : defaultValue;
    },
  },
};

</script>

<style module lang="scss" scoped>
  @import '../../../style/medrecordcontent';
</style>
