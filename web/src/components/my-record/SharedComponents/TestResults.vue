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
        <span v-if="testResult.date.value" :class="$style.fieldName">
          {{ testResult.date.value | datePart(testResult.date.datePart) }}
        </span>
        <span v-else :class="$style.fieldName">
          {{ $t('my_record.noStartDate') }}
        </span>
        <p v-if="supplier === 'TPP'">
          <a :href="getTestResultPath(testResult.id)"
             :class="$style.viewTestResult"
             tabindex="0"
             @click="activateTestResult(testResult.id, $event)"
             @keypress="onKeyDown(testResult.id, $event)">{{ testResult.description }}
          </a>
        </p>
        <p v-if="supplier === 'EMIS'" :class="$style.testTerm">
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
        <span v-if="testResult.date.value" :class="$style.fieldName">
          {{ testResult.date.value | datePart(testResult.date.datePart) }}
        </span>
        <span v-else :class="$style.fieldName">
          {{ $t('my_record.noStartDate') }}
        </span>
        <p v-for="(associatedText, associatedTextItemIndex) in testResult.associatedTexts"
           :key="`associatedText-${associatedTextItemIndex}`">
          {{ associatedText }}
        </p>
        <hr aria-hidden="true">
      </div>
    </div>
    <div v-else-if="supplier === 'VISION'">
      <a :class="$style.viewTestResult"
         :href="testResultsPath + nojsQuery"
         tabindex="0"
         @click.prevent="viewVisionTestResults"
         @keypress="onKeyDownVision($event)">
        {{ $t('my_record.testResults.visionDetailsLink') }}
      </a>
    </div>
  </div>
</template>

<script>
import orderBy from 'lodash/fp/orderBy';
import DcrErrorNoAccess from '@/components/my-record/SharedComponents/DCRErrorNoAccess';
import { MY_RECORD_VISION_TEST_RESULTS_DETAIL } from '@/lib/routes';
import { redirectTo } from '@/lib/utils';

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
    activateTestResult(testResultId, event) {
      event.preventDefault();
      redirectTo(this, this.getTestResultPath(testResultId));
    },
    viewVisionTestResults() {
      redirectTo(this, this.testResultsPath, null);
    },
    getTestResultPath(testResultId) {
      return `/my-record/testresultdetail/${testResultId}`;
    },
    onKeyDownVision(e) {
      if (e.keyCode === 13) {
        this.viewVisionTestResults();
      }
    },
    onKeyDown(testResultId, e) {
      if (e.keyCode === 13) {
        this.activateTestResult(testResultId, e);
      }
    },
    getEffectiveDate(effectiveDate, defaultValue) {
      return effectiveDate && effectiveDate.value ? effectiveDate.value : defaultValue;
    },
  },
};

</script>

<style module lang="scss" scoped>
  @import '../../../style/medrecordcontent';
  @import '../../../style/medrecordtitle';
  @import '../../../style/desktopWeb/accessibility';

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

  div {
   &.desktopWeb {
    a {
     cursor: pointer;
     &:focus {
      @include outlineStyle
     }
    }
    span {
     font-family: $default_web;
     font-weight: normal;
    }
    p {
     font-family: $default_web;
     font-weight: normal;
    }
   }
  }

</style>
