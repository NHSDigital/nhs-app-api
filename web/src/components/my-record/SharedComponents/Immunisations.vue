<template>
  <dcr-error-no-access v-if="showError"
                       :has-access="immunisations.hasAccess"
                       :has-errored="immunisations.hasErrored"
                       :class="[$style['record-content'], getCollapseState]"
                       :aria-hidden="isCollapsed"
                       :has-undetermined-access="immunisations.hasUndeterminedAccess"/>
  <div v-else-if="!isCollapsed" :class="[$style['record-content'],
                                         getCollapseState,
                                         !$store.state.device.isNativeApp && $style.desktopWeb]"
       :aria-hidden="isCollapsed">
    <div v-for="(item, index) in orderedImmunisations" :key="`item-${index}`"
         :class="$style['record-item']" data-purpose="record-item">
      <p v-if="item.effectiveDate && item.effectiveDate.value"
         data-purpose="record-item-header"
         class="nhsuk-u-padding-0 nhsuk-u-margin-0 nhsuk-u-padding-left-3 nhsuk-u-padding-top-3
         nhsuk-body-s">
        {{ item.effectiveDate.value | datePart(item.effectiveDate.datePart) }}
      </p>
      <p v-else data-purpose="record-item-header"
         class="nhsuk-u-padding-0 nhsuk-u-margin-0 nhsuk-u-padding-left-3 nhsuk-u-padding-top-3
         nhsuk-body-s">{{ $t('my_record.noStartDate') }}</p>
      <p data-purpose="record-item-detail"
         class="nhsuk-u-padding-0 nhsuk-u-margin-0 nhsuk-u-padding-left-3">{{ item.term }}</p>
      <p v-if="item.nextDate != null" data-purpose="record-item-detail"
         class="nhsuk-u-padding-0 nhsuk-u-margin-0 nhsuk-u-padding-left-3">
        {{ $t('my_record.immunisations.nextDate') }}{{ getNextDateFormatted(item.nextDate) }}
      </p>
      <p v-if="item.status != null" data-purpose="record-item-detail"
         class="nhsuk-u-padding-0 nhsuk-u-margin-0 nhsuk-u-padding-left-3">
        {{ $t('my_record.immunisations.status') }}{{ item.status }}
      </p>
      <hr aria-hidden="true">
    </div>
  </div>
</template>


<script>

import orderBy from 'lodash/fp/orderBy';
import DcrErrorNoAccess from '@/components/my-record/SharedComponents/DCRErrorNoAccess';

export default {
  name: 'Immunisations',
  components: {
    DcrErrorNoAccess,
  },
  props: {
    isCollapsed: {
      type: Boolean,
      default: true,
    },
    immunisations: {
      type: Object,
      default: () => {},
    },
  },
  computed: {
    getCollapseState() {
      return this.isCollapsed ? this.$style.closed : this.$style.opened;
    },
    orderedImmunisations() {
      return orderBy([obj => obj.effectiveDate.value], ['desc'])(this.immunisations.data);
    },
    showError() {
      return this.immunisations.hasErrored ||
             this.immunisations.data.length === 0 ||
             !this.immunisations.hasAccess;
    },
  },
  methods: {
    getNextDateFormatted(nextDate) {
      return nextDate.rawValue != null ?
        nextDate.rawValue : this.$options.filters.datePart(nextDate.value, nextDate.datePart);
    },
  },
};

</script>

<style module lang="scss" scoped>
@import '../../../style/medrecordcontent';
</style>
