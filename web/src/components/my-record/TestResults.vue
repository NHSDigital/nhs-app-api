<template>
  <div :class="[$style.recordContent, getCollapseState]">
    <div v-if="data.hasErrored">
      <p> {{ $t('my_record.genericErrorMessage') }} </p>
    </div>
    <div v-else>
      <div v-if="data.hasAccess">
        <div v-if="data.data.length > 0">
          <ul >
            <li v-for="(testResult, testIndex) in orderedTestResults"
                :key="`testResult-${testIndex}`" :class="$style.testResult">
              <label v-if="testResult.date.value">
                {{ testResult.date.value | datePart(testResult.date.datePart) }}
              </label>
              <p v-if="supplier === 'TPP'">
                <nuxt-link :to="{
                  name: 'my-record-testresultdetail',
                  params: { testResultId: testResult.id }}">
                  {{ testResult.description }}</nuxt-link></p>
              <p v-if="supplier === 'EMIS'" :class="$style.testTerm">
                {{ testResult.description }}</p>
              <ul>
                <li v-for="(associatedText, associatedTextItemIndex) in testResult.associatedTexts"
                    :key="`associatedText-${associatedTextItemIndex}`"
                    :class="$style.testResultLine">
                  {{ associatedText }}
                </li>
              </ul>
              <ul>
                <li v-for="(lineItem, lineItemIndex) in testResult.testResultChildLineItems"
                    :key="`line-${lineItemIndex}`" :class="$style.testResultLine">
                  {{ lineItem.description }}
                  <ul>
                    <li v-for="(lineItemAssociatedText, lineItemAssociatedTextIndex)
                        in lineItem.associatedTexts"
                        :key="`lineAssociatedText-${lineItemAssociatedTextIndex}`"
                        :class="$style.testResultLineAssociatedText">
                      {{ lineItemAssociatedText }}
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
    supplier: {
      type: String,
      default: '',
    },
  },
  computed: {
    getCollapseState() {
      return this.isCollapsed ? this.$style.closed : this.$style.opened;
    },
    orderedTestResults() {
      return _.orderBy(this.data.data, [obj => obj.date.value], ['desc']);
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

     a:link, a:visited {
      color: $anchor_blue;
    }
  }

  .testTerm {
    padding-bottom: 0px !important;
  }

  .testResultLine {
    padding-left: 32px;
    list-style-type: disc;
    list-style-position: inside;
  }

  .testResultLineAssociatedText {
    padding-left: 32px;
    list-style-type:none;
  }

</style>
