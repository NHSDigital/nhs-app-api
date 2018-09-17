<template>
  <div v-if="showError" :class="[$style['record-content'], getCollapseState]"
       :aria-hidden="isCollapsed">
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
  <div v-else :class="[$style['record-content'], getCollapseState]"
       :aria-hidden="isCollapsed">
    <div v-for="(testResult, testIndex) in orderedTestResults"
         :key="`testResult-${testIndex}`" :class="$style['record-item']"
         data-purpose="record-item">
      <span v-if="testResult.date.value" :class="$style.fieldName">
        {{ testResult.date.value | datePart(testResult.date.datePart) }}
      </span>
      <p v-if="supplier === 'TPP'">
        <nuxt-link :to="{
          name: 'my-record-testresultdetail',
          params: { testResultId: testResult.id }}" :class="$style.viewTestResult">
          {{ testResult.description }}</nuxt-link>
      </p>
      <p v-if="supplier === 'EMIS'" :class="$style.testTerm">
        {{ testResult.description }}</p>
      <ul :class="$style.testResultNoChild">
        <li v-for="(associatedText, associatedTextItemIndex) in testResult.associatedTexts"
            :key="`associatedText-${associatedTextItemIndex}`">
          {{ associatedText }}
        </li>
      </ul>
      <ul :class="$style.testResultLine">
        <li v-for="(lineItem, lineItemIndex) in testResult.testResultChildLineItems"
            :key="`line-${lineItemIndex}`">
          {{ lineItem.description }}
          <ul :class="$style.testResultChildAssociatedText">
            <li v-for="(lineItemAssociatedText, lineItemAssociatedTextIndex)
                in lineItem.associatedTexts"
                :key="`lineAssociatedText-${lineItemAssociatedTextIndex}`">
              {{ lineItemAssociatedText }}
            </li>
          </ul>
        </li>
      </ul>
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

  .fieldName {
    padding-left: 1.3em;
    padding-right: 1.3em;
    padding-bottom: 0.250rem;
    color: #425563;
    font-size: 0.813em;
    font-weight: 700;
  }

</style>
