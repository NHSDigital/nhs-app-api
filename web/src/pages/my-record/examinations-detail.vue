<!-- eslint-disable vue/no-v-html -->
<template>
  <div>
    <div v-if="showTemplate" :class="[$style.content,
                                      'pull-content',
                                      !$store.state.device.isNativeApp && $style.desktopWeb]">
      <div :class="$style['above-float-button']">
        <div :class="$style.info" data-purpose="info">
          <h2>{{ $t('my_record.examinationDetails.examinationTitle') }}</h2>
        </div>
        <div :class="$style['examination-content']">
          <div v-if="!$store.state.myRecord.examinations.markup">
            <p> {{ $t('my_record.examinationDetails.noExaminationData') }}</p>
          </div>
          <div v-else :class="$style['vision-examination']">
            <p>
              <span v-html="$store.state.myRecord.examinations.markup"/>
            </p>
          </div>
        </div>
        <form v-if="$store.state.device.isNativeApp" :action="myRecordReturnPath" method="get">
          <input :value="noJsWarningAcceptance" type="hidden" name="nojs">
          <floating-button-bottom :button-classes="['grey']" @click="onBackButtonClicked">
            {{ $t('my_record.examinationDetails.backButton') }}
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
import DesktopGenericBackLink from '../../components/widgets/DesktopGenericBackLink';

export default {
  components: {
    FloatingButtonBottom,
    DesktopGenericBackLink,
  },
  data() {
    return {
      myRecordReturnPath: `${MYRECORD.path}#examinationHeader`,
      noJsWarningAcceptance: this.$store.state.myRecord.nojsData,
    };
  },
  async asyncData({ store }) {
    await store.dispatch('myRecord/loadExaminations');
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
@import '../../style/spacings';
@import '../../style/_textstyles';

h3 {
  @include h4;
}

.vision-examination {
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

.examination-content {
    box-sizing: border-box;
    padding: 1em;
    padding-top: 0.5em;
    padding-bottom: 0.5em;
    margin-top: 0.5em;
    background-color: #ffffff;
    @include space(margin, bottom, $three);
    overflow-x: scroll;
    display: inline-block;
    margin-right: 2em;
    min-width: 100%;
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

  .examination-content {
   max-width: 540px;
   overflow: auto;
   margin-right: 1em;
   width: 100%;
  }

  .vision-examination {
   min-width: unset;
   max-width: 540px;
  }

  .vision-examination > > p {
   max-width: 540px;
  }

  .content {
   padding-left: 0;
  }
 }
}
</style>
