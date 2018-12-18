<template>
  <div>
    <div v-if="showTemplate" :class="[$style.content, 'pull-content']">
      <div :class="$style['above-float-button']">
        <div :class="$style.info" data-purpose="info">
          <h2>{{ $t('my_record.testresultdetails.testResultTitle') }}</h2>
        </div>
        <div :class="$style['test-result-content']">
          <div v-if="!testResults">
            <p> {{ $t('my_record.testresultdetails.noTestResultData') }} </p>
          </div>
          <div v-else :class="$style['vision-test-results']">
            <p>
              <span v-html="testResults"/>
            </p>
          </div>
        </div>
        <form :action="myRecordReturnPath" method="get">
          <input :value="noJsWarningAcceptance" type="hidden" name="nojs">
          <floating-button-bottom :button-classes="['grey']" @click="onBackButtonClicked">
            {{ $t('my_record.testresultdetails.backButton') }}
          </floating-button-bottom>
        </form>
      </div>
    </div>
  </div>
</template>

<script>
import FloatingButtonBottom from '@/components/widgets/FloatingButtonBottom';
import { MYRECORD } from '@/lib/routes';

export default {
  components: {
    FloatingButtonBottom,
  },
  data() {
    return {
      myRecordReturnPath: `${MYRECORD.path}#testResultsHeader`,
      noJsWarningAcceptance: JSON.stringify({ myRecord: { hasAcceptedTerms: true } }),
    };
  },
  computed: {
    testResults() {
      return this.$store.state.myRecord.record.testResults &&
        this.$store.state.myRecord.record.testResults.rawHtml ?
        this.$store.state.myRecord.record.testResults.rawHtml :
        undefined;
    },
  },
  methods: {
    onBackButtonClicked(event) {
      event.preventDefault();
      this.$router.push(this.myRecordReturnPath);
    },
  },
};

</script>

<style module lang="scss" scoped>
  @import '../../style/spacings';

  .vision-test-results {
    min-width: 50em;

    p {
      padding-right: 1em;
    }
  }

  .content {
    @include space(padding, all, $three);
  }

  .above-float-button {
    margin-bottom: $marginBottomFullScreen;
  }

  .test-result-content {
    box-sizing: border-box;
    padding: 1em;
    padding-top: 0.5em;
    padding-bottom: 0.5em;
    margin-top: 0.5em;
    background-color: #ffffff;
    @include space(margin, bottom, $three);
    overflow-x: scroll;
  }

  .info h2 {
    color: #005EB8;
    padding-bottom: 0.5em;
    padding-top: 0.5em;
    font-weight: 700;
    font-size: 1.375em;
    line-height: 1.375em;
  }
</style>
