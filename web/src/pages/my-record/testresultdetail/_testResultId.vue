<!-- eslint-disable vue/no-v-html -->
<template>
  <div>
    <div v-if="showTemplate" :class="[$style.content,
                                      'pull-content',
                                      !$store.state.device.isNativeApp && $style.desktopWeb]">
      <div :class="$style['above-float-button']">
        <div :class="$style.info" data-purpose="info">
          <h2>{{ $t('my_record.testresultdetail.testResultTitle') }}</h2>
          <div :class="$style['test-result-content']">
            <div v-if="!$store.state.myRecord.detailedTestResult.data" id="noTestResult">
              <p> {{ $t('my_record.testresultdetail.noTestResultData') }} </p>
            </div>
            <div v-else id="resultDetails">
              <p>
                <span v-html="$store.state.myRecord.detailedTestResult.data.testResult"/>
              </p>
            </div>
          </div>
        </div>
        <form v-if="$store.state.device.isNativeApp" :action="myRecordReturnPath" method="get">
          <input :value="noJsWarningAcceptance" type="hidden" name="nojs">
          <floating-button-bottom :button-classes="['grey']" @click.prevent="onBackButtonClicked">
            {{ $t('my_record.testresultdetail.backButton') }}
          </floating-button-bottom>
        </form>
        <desktopGenericBackLink
          v-if="!$store.state.device.isNativeApp"
          :path="noJsPath"
          :button-text="'my_record.diagnosisDetails.backButton'"
          @clickAndPrevent="onBackButtonClicked"/>
      </div>
    </div>
  </div>
</template>

<script>
import FloatingButtonBottom from '@/components/widgets/FloatingButtonBottom';
import { MYRECORD } from '@/lib/routes';
import { redirectTo } from '@/lib/utils';
import DesktopGenericBackLink from '../../../components/widgets/DesktopGenericBackLink';

export default {
  components: {
    FloatingButtonBottom,
    DesktopGenericBackLink,
  },
  data() {
    const noJsData = JSON.stringify({ myRecord: { hasAcceptedTerms: true } });
    const location = '#testResultsHeader';
    return {
      myRecordReturnPath: MYRECORD.path + location,
      noJsWarningAcceptance: noJsData,
      noJsPath: `${MYRECORD.path}?nojs=${encodeURIComponent(noJsData) + location}`,
    };
  },
  async asyncData({ route, store }) {
    await store.dispatch('myRecord/loadDetailedTestResult', route.params.testResultId);
  },
  methods: {
    onBackButtonClicked() {
      redirectTo(this, this.myRecordReturnPath);
    },
  },
};

</script>

<style module lang="scss" scoped>
  @import '../../../style/spacings';
  @import '../../../style/textstyles';

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

  div {
   &.desktopWeb {
    max-width: 540px;

    .test-result-content {
     max-width: 540px;
     overflow: auto;
     margin-right: 1em;
     width: 100%;
    }

    .content {
     padding-left: 0;
    }
   }
  }
</style>
