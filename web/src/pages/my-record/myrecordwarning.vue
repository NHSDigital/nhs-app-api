<template>
  <div v-if="showTemplate" id="mainDiv">
    <main class="content">
      <error-warning-dialog error-or-warning="warning">
        <p>
          {{ $t('myRecord.myRecordWarning.warningText') }}
        </p>
      </error-warning-dialog>

      <div class="info">
        <h4>{{ $t('myRecord.myRecordWarning.title') }}</h4>
        <ul>
          <li>{{ $t('myRecord.myRecordWarning.bulletPoints.bp1') }}</li>
          <li>{{ $t('myRecord.myRecordWarning.bulletPoints.bp2') }}</li>
          <li>{{ $t('myRecord.myRecordWarning.bulletPoints.bp3') }}</li>
          <li>{{ $t('myRecord.myRecordWarning.bulletPoints.bp4') }}</li>
        </ul>
        <br>
        <p>
          {{ $t('myRecord.myRecordWarning.agreementText') }}
        </p>
        <generic-button :button-classes="['green']" @on-click="onContinueButtonClicked()">
          {{ $t('myRecord.myRecordWarning.agreeButtonText') }}
        </generic-button>
        <generic-button :button-classes="['grey']" @on-click="onBackButtonClicked()">
          {{ $t('myRecord.myRecordWarning.backButtonText') }}
        </generic-button>
      </div>
    </main>
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import ErrorWarningDialog from '@/components/errors/ErrorWarningDialog';
import FloatingButtonBottom from '@/components/widgets/FloatingButtonBottom';
import GenericButton from '@/components/widgets/GenericButton';

export default {
  components: {
    ErrorWarningDialog,
    FloatingButtonBottom,
    GenericButton,
  },
  methods: {
    onContinueButtonClicked() {
      this.$router.push('../my-record');
    },
    onBackButtonClicked() {
      this.$router.push('/');
    },
  },
};
</script>

<style lang="scss" scoped>
  li {
    list-style-type: disc;
  }
</style>

<style lang="scss">
  @import '../../style/html';
  @import '../../style/textstyles';
  @import '../../style/elements';
  @import '../../style/fonts';

  #mainDiv {
    padding: 0;
    @include default_text;
  }

  .info .h4 {
    color: $nhs_blue;
  }
</style>
