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
      <span v-if="problem.effectiveDate && problem.effectiveDate.value" :class="$style.fieldName">
        {{ problem.effectiveDate.value | datePart(problem.effectiveDate.datePart) }}
      </span>
      <span v-else :class="$style.fieldName">{{ $t('my_record.noStartDate') }}</span>
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
@import '../../../style/medrecordtitle';

.fieldName {
  padding-left: 1.3em;
  padding-right: 1.3em;
  padding-bottom: 0.250rem;
  color: #425563;
  font-size: 0.813em;
  font-weight: 700;
}

div {
 &.desktopWeb {
  max-width: 540px;
  cursor: default;

  span {
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
