<template>
  <div :class="[$style.recordContent, getCollapseState]">
    <div v-if="data.hasErrored">
      <p> {{ $t('myRecord.genericErrorMessage') }} </p>
    </div>
    <div v-else>
      <div v-if="data.hasAccess">
        <div v-if="data.data.length > 0">
          <ul >
            <li v-for="(testResult, testIndex) in orderedTestResults"
                :key="`testResult-${testIndex}`" :class="$style.testResult">
              <label>{{ testResult.date | longDate }}</label>
              <p :class="$style.testTerm">{{ testResult.term }}</p>
              <ul>
                <li v-for="(lineItem, lineItemIndex) in testResult.lineItems"
                    :key="`line-${lineItemIndex}`" :class="$style.testResultLine">
                  {{ lineItem }}
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
    orderedTestResults() {
      return _.orderBy(this.data.data, [obj => obj.date], ['asc']);
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

  .testResult {
    border-bottom: 1px solid #e8edee;
    padding-bottom: 16px;
  }
  .testTerm {
    padding-bottom: 0px !important;
  }
  .testResultLine {
    @include small_text;
    padding-left: 32px;
  }
</style>
