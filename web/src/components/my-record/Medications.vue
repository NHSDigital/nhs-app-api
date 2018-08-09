<template>
  <div :class="[$style.recordContent, getCollapseState]">
    <div v-if="hasError">
      <p>  {{ $t('my_record.genericErrorMessage') }} </p>
    </div>
    <div v-else>
      <div v-if="data != null && data.length > 0 ">
        <ul>
          <li v-for="(medication, medIndex) in orderedMedications" :key="`medication-${medIndex}`"
              :class="$style.medication">
            <label v-if="medication.date">{{ medication.date | datePart }}</label>
            <ul>
              <li v-for="(lineItem, lineItemIndex) in medication.lineItems"
                  :key="`line-${lineItemIndex}`" :class="$style.medicationLine">
                {{ lineItem.text }}
                <ul>
                  <li v-for="(innerLineItem, innerLineItemIndex) in lineItem.lineItems"
                      :key="`innerline-${innerLineItemIndex}`" :class="$style.medicationLine">
                    {{ innerLineItem }}
                  </li>
                </ul>
              </li>
            </ul>
          </li>
        </ul>
      </div>
      <div v-else>
        <p> {{ $t('my_record.genericNoDataMessage') }} </p>
      </div>
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

  .medication {
    border-bottom: 1px solid #e8edee;
    padding-bottom: 16px;
  }

  .medicationLine {
    @include small_text;
    padding-left: 16px;
    padding-right: 16px;
  }
</style>
