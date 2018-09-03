<template>
  <div v-if="showError" :class="[$style['record-content'], getCollapseState]">
    <p v-if="data.hasErrored">
      {{ $t('my_record.genericErrorMessage') }}
    </p>
    <p v-else>
      {{ $t('my_record.genericNoDataMessage') }}
    </p>
  </div>
  <div v-else :class="[$style['record-content'], getCollapseState]">
    <div v-for="(allergy, index) in orderedAllergies" :key="`allergy.name-${index}`"
         :class="$style['record-item']" data-purpose="record-item">
      <b v-if="allergy.date.value">
        {{ allergy.date.value | datePart(allergy.date.datePart) }}
      </b>
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

</style>
