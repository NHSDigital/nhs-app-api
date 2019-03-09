<template>
  <dcr-error-no-access v-if="showError"
                       :has-errored="problems.hasErrored"
                       :has-access="problems.hasAccess"
                       :class="[$style['record-content'], getCollapseState]"
                       :aria-hidden="isCollapsed"/>
  <div v-else :class="[$style['record-content'], getCollapseState]"
       :aria-hidden="isCollapsed">
    <div v-for="(problem, problemIndex) in orderedProblems"
         :key="`problem-${problemIndex}`" :class="$style['record-item']"
         data-purpose="record-item">
      <span v-if="problem.effectiveDate.value" :class="$style.fieldName">
        {{ problem.effectiveDate.value | datePart(problem.effectiveDate.datePart) }}
      </span>
      <p v-for="(lineItem, lineItemIndex) in problem.lineItems"
         :key="`line-${lineItemIndex}`">
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
      return orderBy([obj => obj.effectiveDate.value], ['desc'])(this.problems.data);
    },
    showError() {
      return this.problems.hasErrored ||
             this.problems.data.length === 0 ||
             !this.problems.hasAccess;
    },
  },
};

</script>

<style module lang="scss" scoped>
@import '../../../style/medrecordcontent';
@import '../../../style/medrecordtitle';

.fieldName {
  padding-left: 1.3em;
  padding-right: 1.3em;
  padding-bottom: 0.250rem;
  color: #425563;
  font-size: 0.813em;
  font-weight: 700;
}

</style>
