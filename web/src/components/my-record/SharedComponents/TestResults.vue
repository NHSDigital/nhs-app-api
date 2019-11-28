<template>
  <dcr-error-no-access v-if="showError"
                       :has-access="results.hasAccess"
                       :has-errored="results.hasErrored"
                       :class="[$style['record-content'],
                                getCollapseState]"
                       :aria-hidden="isCollapsed"
                       :has-undetermined-access="results.hasUndeterminedAccess"/>
  <div v-else-if="!isCollapsed"
       :class="[$style['record-content'], getCollapseState,
                !$store.state.device.isNativeApp && $style.desktopWeb]"
       :aria-hidden="isCollapsed">
    <div v-if="supplier === 'TPP' || supplier === 'EMIS'">
      <div v-for="(testResult, testIndex) in orderedTestResults"
           :key="`testResult-${testIndex}`" :class="$style['record-item']"
           data-purpose="record-item">
        <p v-if="testResult.date && testResult.date.value"
           data-purpose="record-item-header"
           class="nhsuk-u-padding-0 nhsuk-u-margin-0 nhsuk-u-padding-left-3 nhsuk-u-padding-top-3
           nhsuk-body-s">
          {{ testResult.date.value | datePart(testResult.date.datePart) }}
        </p>
        <p v-else data-purpose="record-item-header"
           class="nhsuk-u-padding-0 nhsuk-u-margin-0 nhsuk-u-padding-left-3 nhsuk-u-padding-top-3
           nhsuk-body-s">
          {{ $t('my_record.noStartDate') }}
        </p>
        <p v-if="supplier === 'TPP'"
           class="nhsuk-u-padding-0 nhsuk-u-margin-0 nhsuk-u-padding-left-3">
          <a :href="getTestResultPath(testResult.id)"
             style="display:inline-block;"
             tabindex="0"
             @keypress.enter.prevent="activateTestResult(testResult.id)"
             @click.prevent="activateTestResult(testResult.id)">{{ testResult.description }}
          </a>
        </p>
        <p v-if="supplier === 'EMIS'" data-purpose="record-item-detail"
           class="nhsuk-u-padding-0 nhsuk-u-margin-0 nhsuk-u-padding-left-3">
          {{ testResult.description }}
        </p>
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
        <hr aria-hidden="true">
      </div>
    </div>
    <div v-else-if="supplier === 'MICROTEST'">
      <div v-for="(testResult, testIndex) in results.data"
           :key="`testResult-${testIndex}`" :class="$style['record-item']"
           data-purpose="record-item">
        <p v-if="testResult.date.value"
           data-purpose="record-item-header"
           class="nhsuk-u-padding-0 nhsuk-u-margin-0 nhsuk-u-padding-left-3 nhsuk-u-padding-top-3
           nhsuk-body-s">
          {{ testResult.date.value | datePart(testResult.date.datePart) }}
        </p>
        <p v-else data-purpose="record-item-header"
           class="nhsuk-u-padding-0 nhsuk-u-margin-0 nhsuk-u-padding-left-3 nhsuk-u-padding-top-3
           nhsuk-body-s">
          {{ $t('my_record.noStartDate') }}
        </p>
        <p v-for="(associatedText, associatedTextItemIndex) in testResult.associatedTexts"
           :key="`associatedText-${associatedTextItemIndex}`"
           data-purpose="record-item-detail">
          {{ associatedText }}
        </p>
        <hr aria-hidden="true">
      </div>
    </div>
    <div v-else-if="supplier === 'VISION'">
      <a :href="testResultsPath + nojsQuery"
         tabindex="0"
         @click.prevent="viewVisionTestResults"
         @keypress.enter="viewVisionTestResults">
        {{ $t('my_record.testResults.visionDetailsLink') }}
      </a>
    </div>
  </div>
</template>

<script>
import orderBy from 'lodash/fp/orderBy';
import DcrErrorNoAccess from '@/components/my-record/SharedComponents/DCRErrorNoAccess';
import { redirectTo } from '@/lib/utils';
import { MY_RECORD_VISION_TEST_RESULTS_DETAIL } from '@/lib/routes';

export default {
  name: 'TestResults',
  components: {
    DcrErrorNoAccess,
  },
  props: {
    isCollapsed: {
      type: Boolean,
      default: true,
    },
    results: {
      type: Object,
      default: () => {},
    },
    supplier: {
      type: String,
      default: '',
    },
  },
  data() {
    return {
      testResultsPath: MY_RECORD_VISION_TEST_RESULTS_DETAIL.path,
      nojsQuery: `?nojs=${encodeURIComponent(this.$store.state.myRecord.nojsData)}`,
    };
  },
  computed: {
    getCollapseState() {
      return this.isCollapsed ? this.$style.closed : this.$style.opened;
    },
    resultsData() {
      return this.results;
    },
    orderedTestResults() {
      return orderBy([result => this.getEffectiveDate(result.date, '')], ['desc'])(this.results.data);
    },
    showError() {
      if (this.supplier === 'VISION') {
        return (this.results.rawHtml === null);
      }
      return this.results.hasErrored ||
              this.results.data.length === 0 ||
              !this.results.hasAccess;
    },
  },
  methods: {
    activateTestResult(testResultId) {
      redirectTo(this, this.getTestResultPath(testResultId));
    },
    viewVisionTestResults() {
      redirectTo(this, this.testResultsPath);
    },
    getTestResultPath(testResultId) {
      return `/my-record/testresultdetail/${testResultId}`;
    },
    getEffectiveDate(effectiveDate, defaultValue) {
      return effectiveDate && effectiveDate.value ? effectiveDate.value : defaultValue;
    },
  },
};

</script>

<style module lang="scss" scoped>
  @import '../../../style/medrecordcontent';
</style>
