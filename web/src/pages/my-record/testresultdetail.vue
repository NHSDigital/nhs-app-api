<template>
  <div v-if="showTemplate" :class="[$style.content, 'pull-content']">
    <div :class="$style['above-float-button']">
      <div :class="$style.info" data-purpose="info">
        <h2>{{ $t('my_record.testresultdetail.testResultTitle') }}</h2>
      </div>
      <div :class="$style['test-result-content']">
        <div v-if="!testResult.testResult">
          <p> {{ $t('my_record.testresultdetail.noTestResultData') }} </p>
        </div>
        <div v-else>
          <p>
            <span v-html="testResult.testResult"/>
          </p>
        </div>
      </div>
      <floating-button-bottom :button-classes="['grey']" @on-click="onBackButtonClicked">
        {{ $t('my_record.testresultdetail.backButton') }}
      </floating-button-bottom>
    </div>
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import FloatingButtonBottom from '@/components/widgets/FloatingButtonBottom';

export default {
  components: {
    FloatingButtonBottom,
  },
  beforeRouteEnter(to, from, next) {
    if (from.path === '/my-record') {
      next();
    } else {
      next('/my-record-warning');
    }
  },
  data() {
    return {
      testResult: '',
      hasLoaded: false,
    };
  },
  mounted() {
    const param = {
      testResultId: this.$route.params.testResultId,
    };
    this.$store.app.$http
      .getV1PatientTestResult(param)
      .then((data) => {
        this.testResult = data.response;
        this.hasLoaded = true;
      });
  },
  methods: {
    onBackButtonClicked() {
      this.$router.push('/my-record#testResultsHeader');
    },
  },
};

</script>

<style module lang="scss" scoped>
  @import '../../style/spacings';

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
