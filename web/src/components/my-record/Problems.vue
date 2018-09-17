<template>
  <div v-if="showError" :class="[$style['record-content'], getCollapseState]"
       :aria-hidden="isCollapsed">
    <p v-if="data.hasErrored">
      {{ $t('my_record.genericErrorMessage') }}
    </p>
    <p v-else-if="!data.hasAccess">
      {{ $t('my_record.genericNoAccessMessage') }}
    </p>
    <p v-else>
      {{ $t('my_record.genericNoDataMessage') }}
    </p>
  </div>
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
      <hr>
    </div>
  </div>
</template>

<script>

import _ from 'lodash';

export default {
  props: {
    isCollapsed: {
      type: Boolean,
      default: true,
    },
    data: {
      type: Object,
      default: () => {},
    },
  },
  computed: {
    getCollapseState() {
      return this.isCollapsed ? this.$style.closed : this.$style.opened;
    },
    orderedProblems() {
      return _.orderBy(this.data.data, [obj => obj.effectiveDate.value], ['desc']);
    },
    showError() {
      return this.data.hasErrored ||
             this.data.data.length === 0 ||
             !this.data.hasAccess;
    },
  },
};

</script>

<style module lang="scss" scoped>
@import '../../style/medrecordcontent';

.fieldName {
  padding-left: 1.3em;
  padding-right: 1.3em;
  padding-bottom: 0.250rem;
  color: #425563;
  font-size: 0.813em;
  font-weight: 700;
}

</style>
