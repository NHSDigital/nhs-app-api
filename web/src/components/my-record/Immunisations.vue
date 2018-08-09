<template>
  <div :class="[$style.recordContent, getCollapseState]">
    <div v-if="data.hasErrored">
      <p>  {{ $t('my_record.genericErrorMessage') }} </p>
    </div>
    <div v-else>
      <div v-if="data.hasAccess">
        <div v-if="data.data.length > 0">
          <ul :class="$style.immunisations">
            <li v-for="(item, index) in orderedImmunisations" :key="`item-${index}`">
              <label v-if="item.effectiveDate.value">
                {{ item.effectiveDate.value | datePart(item.effectiveDate.datePart) }}
              </label>
              <p class="immunisationTerm">{{ item.term }}</p>
            </li>
          </ul>
        </div>
        <div v-else>
          <p> {{ $t('my_record.genericNoDataMessage') }} </p>
        </div>
      </div>
      <div v-else>
        <p> {{ $t('my_record.genericNoAccessMessage') }} </p>
      </div>
    </div>
    <hr>
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
  },
};

</script>

<style lang="scss" module>
  @import '../../style/html';
  @import '../../style/fonts';
  @import '../../style/spacings';
  @import '../../style/colours';
  @import '../../style/elements';

  .recordContent { @include record-content };

  .immunisations li{
    border-bottom: 1px solid #e8edee;
  }
</style>
