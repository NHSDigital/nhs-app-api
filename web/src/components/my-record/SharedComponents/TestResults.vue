<template>
  <dcr-error-no-access v-if="showError"
                       :has-access="results.hasAccess"
                       :has-errored="results.hasErrored"
                       :class="[$style['record-content'], getCollapseState]"
                       :aria-hidden="isCollapsed"/>
  <div v-else :class="[$style['record-content'], getCollapseState]"
       :aria-hidden="isCollapsed">
    <div v-if="supplier === 'TPP' || supplier === 'EMIS'">
      <div v-for="(testResult, testIndex) in orderedTestResults"
           :key="`testResult-${testIndex}`" :class="$style['record-item']"
           data-purpose="record-item">
        <span v-if="testResult.date.value" :class="$style.fieldName">
          {{ testResult.date.value | datePart(testResult.date.datePart) }}
        </span>
        <p v-if="supplier === 'TPP'">
          <a
            :href="getTestResultPath(testResult.id)"
            :class="$style.viewTestResult"
            @click="activateTestResult(testResult.id, $event)">{{ testResult.description }}
          </a>
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
        <hr aria-hidden="true">
      </div>
    </div>
    <div v-else-if="supplier === 'VISION'">
      <a :class="$style.viewTestResult" @click="viewVisionTestResults($event)">
        {{ $t('my_record.testResults.visionDetailsLink') }}
      </a>
    </div>
  </div>
</template>

<script>
import orderBy from 'lodash/fp/orderBy';
import DcrErrorNoAccess from '@/components/my-record/SharedComponents/DCRErrorNoAccess';
import { MY_RECORD_VISION_TEST_RESULTS_DETAIL } from '@/lib/routes';

export default {
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
  computed: {
    getCollapseState() {
      return this.isCollapsed ? this.$style.closed : this.$style.opened;
    },
    resultsData() {
      return this.results;
    },
    orderedTestResults() {
      return orderBy([obj => obj.date.value], ['desc'])(this.results.data);
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
    activateTestResult(testResultId, event) {
      event.preventDefault();
      this.$router.push(this.getTestResultPath(testResultId));
    },
    viewVisionTestResults(event) {
      event.preventDefault();
      this.$router.push(MY_RECORD_VISION_TEST_RESULTS_DETAIL.path);
    },
    getTestResultPath(testResultId) {
      return `/my-record/testresultdetail/${testResultId}`;
    },
  },
};

</script>

<style module lang="scss" scoped>
  @import '../../../style/medrecordcontent';
  @import '../../../style/medrecordtitle';

  .viewTestResult {
    padding: 1em;
    font-size: 0.875em;
  }

  .fieldName {
    padding-left: 1.3em;
    padding-right: 1.3em;
    padding-bottom: 0.250rem;
    color: #425563;
    font-size: 0.813em;
    font-weight: 700;
  }

</style>
