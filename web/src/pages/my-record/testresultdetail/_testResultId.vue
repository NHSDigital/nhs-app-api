<!-- eslint-disable vue/no-v-html -->
<template>
  <div>
    <div v-if="showTemplate" :class="[$style.content,
                                      'pull-content',
                                      !$store.state.device.isNativeApp && $style.desktopWeb]">
      <div :class="$style['above-float-button']">
        <div :class="$style.info" data-purpose="info">
          <h2>{{ $t('my_record.testresultdetail.testResultTitle') }}</h2>
        </div>
        <div :class="$style['test-result-content']">
          <div v-if="!$store.state.myRecord.detailedTestResult.data">
            <p> {{ $t('my_record.testresultdetail.noTestResultData') }} </p>
          </div>
          <div v-else>
            <p>
              <span v-html="$store.state.myRecord.detailedTestResult.data"/>
            </p>
          </div>
        </div>
        <form v-if="$store.state.device.isNativeApp" :action="myRecordReturnPath" method="get">
          <input :value="noJsWarningAcceptance" type="hidden" name="nojs">
          <floating-button-bottom :button-classes="['grey']" @click="onBackButtonClicked">
            {{ $t('my_record.testresultdetail.backButton') }}
          </floating-button-bottom>
        </form>
        <desktopGenericBackLink
          v-if="!$store.state.device.isNativeApp"
          :path="myRecordReturnPath"
          :button-text="'my_record.diagnosisDetails.backButton'"
          :state-transfer-required="true"/>
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
    return {
      myRecordReturnPath: `${MYRECORD.path}#testResultsHeader`,
      noJsWarningAcceptance: JSON.stringify({ myRecord: { hasAcceptedTerms: true } }),
    };
  },
  async asyncData({ route, store }) {
    await store.dispatch('myRecord/loadDetailedTestResult', route.params.testResultId);
  },
  methods: {
    onBackButtonClicked(event) {
      event.preventDefault();
      redirectTo(this, this.myRecordReturnPath, null);
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

  .info h2 {
    color: #005EB8;
    padding-bottom: 0.5em;
    padding-top: 0.5em;
    font-weight: 700;
    font-size: 1.375em;
    line-height: 1.375em;
  }

  div {
   &.desktopWeb {
    max-width: 540px;

    .info h2 {
     font-family: $default-web;
     color: black;
    }

    p {
     font-family: $default-web;
     font-weight: normal;
    }

    .test-result-content {
     max-width: 540px;
     overflow: auto;
     margin-right: 1em;
     width: 100%;
    }

    .content {
     padding-left: 0;
    }

    .backButton {
     font-family: $default-web;
     color: $nhs_blue;
     font-size: 1.125em;
     line-height: 1.125em;
     font-weight: normal;
     vertical-align: middle;
     cursor: pointer;
     display: inline-block;
     border: none;
     background: none;
     outline: none;
     text-decoration: underline;
     margin-top: 1em;
     margin-bottom: 2em;
    }

    .backButton:focus {
     box-sizing: content-box;
     outline-color: $focus_highlight;
     box-shadow: 0 0 0 4px $focus_highlight;
     outline-width: 2em;
    }

    .backButton:hover {
     background: #ffcd60;
     outline: none;
     box-sizing: border-box;
     text-decoration: underline;
     background-clip: content-box;
    }
   }
  }
</style>
