<template>
  <div v-if="showError" :class="[$style['record-content'], getCollapseState]">
    <p v-if="hasError">
      {{ $t('my_record.genericErrorMessage') }}
    </p>
    <p v-else>
      {{ $t('my_record.genericNoDataMessage') }}
    </p>
  </div>
  <div v-else :class="[$style['record-content'], getCollapseState]">
    <div v-for="(medication, medIndex) in orderedMedications"
         :key="`medication-${medIndex}`" :class="$style['record-item']"
         data-purpose="record-item">
      <label v-if="medication.date">{{ medication.date | datePart }}</label>
      <p v-for="(lineItem, lineItemIndex) in medication.lineItems"
         :key="`line-${lineItemIndex}`">
        {{ lineItem.text }}
        <ul>
          <li v-for="(innerLineItem, innerLineItemIndex) in lineItem.lineItems"
              :key="`innerline-${innerLineItemIndex}`">
            {{ innerLineItem }}
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
    data: {
      type: Array,
      default: () => [],
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
      return _.orderBy(this.data, [obj => obj.date], ['desc']);
    },
    showError() {
      return this.data.hasErrored ||
             (this.data && this.data.length === 0);
    },
  },
};

</script>

<style module lang="scss" scoped>
@import '../../style/medrecordcontent';

</style>
