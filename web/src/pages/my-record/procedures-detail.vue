<template>
  <div>
    <div v-if="showTemplate" :class="[$style.content, 'pull-content']">
      <div :class="$style['above-float-button']">
        <div :class="$style.info" data-purpose="info">
          <h2>{{ $t('my_record.procedureDetails.procedureTitle') }}</h2>
        </div>
        <div :class="$style['procedures-content']">
          <div v-if="!procedures">
            <p> {{ $t('my_record.procedureDetails.noProcedureData') }}</p>
          </div>
          <div v-else :class="$style['vision-procedures']">
            <p>
              <span v-html="procedures"/>
            </p>
          </div>
        </div>
        <form :action="myRecordReturnPath" method="get">
          <input :value="noJsWarningAcceptance" type="hidden" name="nojs">
          <floating-button-bottom :button-classes="['grey']" @click="onBackButtonClicked">
            {{ $t('my_record.procedureDetails.backButton') }}
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
      myRecordReturnPath: `${MYRECORD.path}#proceduresHeader`,
      noJsWarningAcceptance: JSON.stringify({ myRecord: { hasAcceptedTerms: true } }),
    };
  },
  computed: {
    procedures() {
      return this.$store.state.myRecord.record.procedures &&
        this.$store.state.myRecord.record.procedures.rawHtml ?
        this.$store.state.myRecord.record.procedures.rawHtml :
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

<style lang="scss" scoped>
@import '../../style/spacings';

.vision-procedures {
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

.procedures-content {
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
