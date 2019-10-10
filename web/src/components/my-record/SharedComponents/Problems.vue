<template>
  <dcr-error-no-access v-if="showError"
                       :has-errored="problems.hasErrored"
                       :has-access="problems.hasAccess"
                       :class="[$style['record-content'],
                                getCollapseState]"
                       :aria-hidden="isCollapsed"
                       :has-undetermined-access="problems.hasUndeterminedAccess"/>
  <div v-else-if="!isCollapsed" :class="[$style['record-content'], getCollapseState,
                                         !$store.state.device.isNativeApp && $style.desktopWeb]"
       :aria-hidden="isCollapsed">
    <div v-for="(problem, problemIndex) in orderedProblems"
         :key="`problem-${problemIndex}`" :class="$style['record-item']"
         data-purpose="record-item">
      <p v-if="problem.effectiveDate && problem.effectiveDate.value"
         data-purpose="record-item-header"
         class="nhsuk-u-padding-0 nhsuk-u-margin-0 nhsuk-u-padding-left-3 nhsuk-u-padding-top-3
         nhsuk-body-s">
        {{ problem.effectiveDate.value | datePart(problem.effectiveDate.datePart) }}
      </p>
      <p v-else data-purpose="record-item-header">
        {{ $t('my_record.noStartDate') }}</p>
      <p v-for="(lineItem, lineItemIndex) in problem.lineItems"
         :key="`line-${lineItemIndex}`"
         data-purpose="record-item-detail"
         class="nhsuk-u-padding-0 nhsuk-u-margin-0 nhsuk-u-padding-left-3">
        {{ lineItem.text }}
        <ul>
          <li v-for="(childLineItem, childLineItemIndex) in lineItem.lineItems"
              :key="`line-${childLineItemIndex}`">
            {{ childLineItem }}
          </li>
        </ul>
      </p>
      <hr aria-hidden="true">
    </div>
  </div>
</template>

<script>

import orderBy from 'lodash/fp/orderBy';
import DcrErrorNoAccess from '@/components/my-record/SharedComponents/DCRErrorNoAccess';

export default {
  name: 'Problems',
  components: {
    DcrErrorNoAccess,
  },
  props: {
    isCollapsed: {
      type: Boolean,
      default: true,
    },
    problems: {
      type: Object,
      default: () => {},
    },
  },
  computed: {
    getCollapseState() {
      return this.isCollapsed ? this.$style.closed : this.$style.opened;
    },
    orderedProblems() {
      return orderBy([problem => this.getEffectiveDate(problem.effectiveDate, '')], ['desc'])(this.problems.data);
    },
    showError() {
      return this.problems.hasErrored ||
             this.problems.data.length === 0 ||
             !this.problems.hasAccess;
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
