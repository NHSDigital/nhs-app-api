<template>
  <div :class="[$style.recordContent, getCollapseState]">
    <div v-if="data.hasErrored">
      <p> {{ $t('myRecord.genericErrorMessage') }} </p>
    </div>
    <div v-else>
      <div v-if="data.hasAccess">
        <div v-if="data.data.length > 0">
          <ul >
            <li v-for="(problem, problemIndex) in orderedProblems"
                :key="`problem-${problemIndex}`" :class="$style.problem">
              <label v-if="problem.effectiveDate.value">
                {{ problem.effectiveDate.value | datePart(problem.effectiveDate.datePart) }}
              </label>
              <ul>
                <li v-for="(lineItem, lineItemIndex) in problem.lineItems"
                    :key="`line-${lineItemIndex}`" :class="$style.problemLine">
                  {{ lineItem.text }}
                  <ul>
                    <li v-for="(childLineItem, childLineItemIndex) in lineItem.lineItems"
                        :key="`line-${childLineItemIndex}`" :class="$style.problemLine">
                      {{ childLineItem }}
                    </li>
                  </ul>
                </li>
              </ul>
            </li>
          </ul>
        </div>
        <div v-else>
          <p> {{ $t('myRecord.genericNoDataMessage') }} </p>
        </div>
      </div>
      <div v-else>
        <p> {{ $t('myRecord.genericNoAccessMessage') }} </p>
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
    orderedProblems() {
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

  .problem {
    border-bottom: 1px solid #e8edee;
    padding-bottom: 16px;
  }
  .problemDetail {
    padding-bottom: 0px !important;
  }
  .problemLine {
    @include small_text;
    padding-left: 16px;
  }
</style>
