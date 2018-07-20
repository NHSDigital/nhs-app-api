<template>
  <main v-if="showTemplate" :class="$style.content">
    <div :class="$style['above-float-button']">
      <div :class="$style.info">
        <h2>{{ $t('myRecord.testResultDetail.testResultTitle') }}</h2>
      </div>
      <div :class="$style['test-result-content']">
        <div v-if="testResult.hasErrored">
          <p> {{ $t('myRecord.genericErrorMessage') }} </p>
        </div>
        <div v-else>
          <p>
            <span v-html="testResult.testResult"/>
          </p>
        </div>
      </div>
      <floating-button-bottom @on-click="onBackButtonClicked">
        {{ $t('myRecord.testResultDetail.backButton') }}
      </floating-button-bottom>
    </div>
  </main>
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
      next('/my-record/myrecordwarning');
    }
  },
  data() {
    return {
      testResult: '',
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
      });
  },
  methods: {
    onBackButtonClicked() {
      this.$router.push('/my-record');
    },
  },
};

</script>

<style module lang="scss">
  @import '../../style/html';
  @import '../../style/fonts';
  @import '../../style/spacings';
  @import "../../style/buttons";
  @import '../../style/elements';
  @import "../../style/html";
  @import "../../style/textstyles";
  @import "../../style/colours";

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
