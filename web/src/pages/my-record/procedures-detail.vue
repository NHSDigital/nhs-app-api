<!-- eslint-disable vue/no-v-html -->
<template>
  <div>
    <div v-if="showTemplate" :class="[$style.content,
                                      'pull-content',
                                      !$store.state.device.isNativeApp && $style.desktopWeb]">
      <div :class="$style['above-float-button']">
        <div :class="$style.info" data-purpose="info">
          <h2>{{ $t('my_record.procedureDetails.procedureTitle') }}</h2>
        </div>
        <div :class="$style['procedures-content']">
          <div v-if="!$store.state.myRecord.procedures.markup">
            <p> {{ $t('my_record.procedureDetails.noProcedureData') }}</p>
          </div>
          <div v-else :class="$style['vision-procedures']">
            <p>
              <span v-html="$store.state.myRecord.procedures.markup"/>
            </p>
          </div>
        </div>
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
import { MYRECORD } from '@/lib/routes';
import { redirectTo } from '@/lib/utils';
import DesktopGenericBackLink from '../../components/widgets/DesktopGenericBackLink';

export default {
  components: {
    DesktopGenericBackLink,
  },
  data() {
    const noJsData = JSON.stringify({ myRecord: { hasAcceptedTerms: true } });
    const location = '#proceduresHeader';
    return {
      myRecordReturnPath: MYRECORD.path + location,
      noJsWarningAcceptance: noJsData,
      noJsPath: `${MYRECORD.path}?nojs=${encodeURIComponent(noJsData) + location}`,
    };
  },
  async asyncData({ store }) {
    await store.dispatch('myRecord/loadProcedures');
  },
  methods: {
    onBackButtonClicked() {
      redirectTo(this, this.myRecordReturnPath);
    },
  },
};

</script>

<style module lang="scss" scoped>
@import '../../style/spacings';
@import '../../style/_textstyles';

.vision-procedures {
  min-width: 50em;
}

.content {
    @include space(padding, all, $three);
}

.above-float-button {
    margin-bottom: $marginBottomFullScreen;
}

.procedures-content {
    box-sizing: border-box;
    padding: 1em;
    padding-bottom: 0.5em;
    background-color: #ffffff;
    @include space(margin, bottom, $three);
    overflow-x: scroll;
    display: inline-block;
    margin-right: 2em;
    min-width: 100%;
}

div {
 &.desktopWeb {
  max-width: 540px;

  .procedures-content {
   max-width: 540px;
   overflow: auto;
   margin-right: 1em;
   width: 100%;
  }

  .vision-procedures {
   min-width: unset;
   max-width: 540px;
  }

  .vision-procedures > > p {
   max-width: 540px;
  }

  .content {
   padding-left: 0;
  }
 }
}
</style>
