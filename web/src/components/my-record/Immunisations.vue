<template>
  <div v-if="showError" :class="[$style['record-content'], getCollapseState]">
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
  <div v-else :class="[$style['record-content'], getCollapseState]">
    <div v-for="(item, index) in orderedImmunisations" :key="`item-${index}`"
         :class="$style['record-item']" data-purpose="record-item">
      <b v-if="item.effectiveDate.value">
        {{ item.effectiveDate.value | datePart(item.effectiveDate.datePart) }}
      </b>
      <p>{{ item.term }}</p>
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
    orderedImmunisations() {
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

</style>
