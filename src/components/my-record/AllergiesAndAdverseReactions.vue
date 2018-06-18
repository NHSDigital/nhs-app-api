<template>
  <div :class="[$style.recordContent, getCollapseState]">
    <div v-if="data.hasErrored">
      <p>  {{ $t('myRecord.genericErrorMessage') }} </p>
    </div>
    <div v-else>
      <div v-if="data.data.length > 0">
        <ul :class="$style.allergyAndAdverseReactions">
          <li v-for="(allergy, index) in orderedAllergies" :key="`allergy.name-${index}`">
            <label>{{ allergy.date.value | datePart(allergy.date.datePart) }}</label>
            <p>{{ allergy.name }}</p>
          </li>
        </ul>
      </div>
      <div v-else>
        <p> {{ $t('myRecord.genericNoDataMessage') }} </p>
      </div>
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
