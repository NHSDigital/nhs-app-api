<template>
  <div :class="[$style.recordContent, getCollapseState]">
    <ul :class="$style.allergyAndAdverseReactions">
      <li v-for="allergy in orderedAllergies" :key="allergy.name">
        <label>{{ allergy.date | longDate }}</label>
        <p>{{ allergy.symptom }}</p>
      </li>
    </ul>
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
  },
  computed: {
    getCollapseState() {
      return this.isCollapsed ? this.$style.closed : this.$style.opened;
    },
    orderedAllergies() {
      return _.orderBy(this.data, [obj => obj.date], ['asc']);
    },
  },
};

</script>

<style module lang="scss">
  @import '../../style/html';
  @import '../../style/fonts';
  @import '../../style/spacings';
  @import '../../style/colours';
  @import '../../style/elements';

  .recordContent { @include record-content };

  .allergyAndAdverseReactions li{
    border-bottom: 1px solid #e8edee;
  }
</style>
