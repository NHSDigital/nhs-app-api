<template>
  <div v-if="showError" :class="[$style['record-content'], getCollapseState]"
       :aria-hidden="isCollapsed">
    <p v-if="data.hasErrored">
      {{ $t('my_record.genericErrorMessage') }}
    </p>
    <p v-else>
      {{ $t('my_record.genericNoDataMessage') }}
    </p>
  </div>
  <div v-else :class="[$style['record-content'], getCollapseState]"
       :aria-hidden="isCollapsed">
    <div v-for="(allergy, index) in orderedAllergies" :key="`allergy.name-${index}`"
         :class="$style['record-item']" data-purpose="record-item">
      <span v-if="allergy.date.value" :class="$style.fieldName">
        {{ allergy.date.value | datePart(allergy.date.datePart) }}
      </span>
      <p>{{ allergy.name }}</p>
      <hr>
    </div>
  </div>
</template>

<script>

import _ from 'lodash';

export default {
  props: {
    data: {
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
      return _.orderBy(this.data.data, [obj => obj.date.value], ['desc']);
    },
    showError() {
      return this.data.hasErrored || this.data.data.length === 0;
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
