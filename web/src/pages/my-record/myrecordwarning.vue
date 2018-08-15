<template>
  <div v-if="showTemplate" id="mainDiv" class="pull-content">
    <message-dialog message-type="warning" icon-text="Important">
      <message-text>
        {{ $t('my_record.myRecordWarning.warningText') }}
      </message-text>
    </message-dialog>
    <div :class="$style.info" data-purpose="info">
      <h4>{{ $t('my_record.myRecordWarning.title') }}</h4>
      <ul>
        <li>{{ $t('my_record.myRecordWarning.bulletPoints.bp1') }}</li>
        <li>{{ $t('my_record.myRecordWarning.bulletPoints.bp2') }}</li>
      </ul>
      <h4>{{ $t('my_record.myRecordWarning.extraTitle') }}</h4>
      <ul>
        <li>{{ $t('my_record.myRecordWarning.extraBulletPoints.bp1') }}</li>
        <li>{{ $t('my_record.myRecordWarning.extraBulletPoints.bp2') }}</li>
      </ul>
      <p>
        {{ $t('my_record.myRecordWarning.agreementText') }}
      </p>
    </div>
    <generic-button :button-classes="['green']" @on-click="onContinueButtonClicked()">
      {{ $t('my_record.myRecordWarning.agreeButtonText') }}
    </generic-button>
    <generic-button :button-classes="['grey']" @on-click="onBackButtonClicked()">
      {{ $t('my_record.myRecordWarning.backButtonText') }}
    </generic-button>
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import FloatingButtonBottom from '@/components/widgets/FloatingButtonBottom';
import GenericButton from '@/components/widgets/GenericButton';

export default {
  components: {
    MessageDialog,
    MessageText,
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

<style module lang="scss" scoped>
  @import '../../style/info';

</style>
