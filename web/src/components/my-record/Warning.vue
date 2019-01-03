<template>
  <div v-if="showTemplate" id="mainDiv" class="pull-content">
    <message-dialog message-type="warning" icon-text="Important">
      <message-text>
        {{ $t('my_record.warning.warningText') }}
      </message-text>
    </message-dialog>
    <div :class="$style.info" data-purpose="info">
      <h2 :class="[$style.h2]">{{ $t('my_record.warning.title') }}</h2>
      <ul>
        <li>{{ $t('my_record.warning.bulletPoints.bp1') }}</li>
        <li>{{ $t('my_record.warning.bulletPoints.bp2') }}</li>
      </ul>
      <h2 :class="[$style.h2]">{{ $t('my_record.warning.extraTitle') }}</h2>
      <ul>
        <li>{{ $t('my_record.warning.extraBulletPoints.bp1') }}</li>
        <li>{{ $t('my_record.warning.extraBulletPoints.bp2') }}</li>
      </ul>
      <p>
        {{ $t('my_record.warning.agreementText') }}
      </p>
    </div>
    <form :action="myRecordPath" method="get">
      <input :value="JSON.stringify({ myRecord: { hasAcceptedTerms: true }})"
             type="hidden"
             name="nojs">

      <generic-button :class="[$style.button, $style.green]" @click="onContinueButtonClicked">
        {{ $t('my_record.warning.agreeButtonText') }}
      </generic-button>
    </form>
    <form :action="indexPath" method="get">
      <generic-button :class="[$style.button, $style.grey]" @click="onBackButtonClicked">
        {{ $t('my_record.warning.backButtonText') }}
      </generic-button>
    </form>
  </div>
</template>

<script>
import GenericButton from '@/components/widgets/GenericButton';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import { INDEX, MYRECORD } from '@/lib/routes';

export default {
  name: 'Warning',
  components: {
    GenericButton,
    MessageDialog,
    MessageText,
  },
  data() {
    return {
      indexPath: INDEX.path,
      myRecordPath: MYRECORD.path,
    };
  },
  methods: {
    onContinueButtonClicked(event) {
      event.preventDefault();
      this.$store.dispatch('myRecord/acceptTerms');
    },
    onBackButtonClicked(event) {
      event.preventDefault();
      this.$router.push(this.indexPath);
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import '../../style/info';
  @import '../../style/fonts';
  @import '../../style/buttons';
  .h2 {
    display: block;
    font-family: $frutiger-bold;
    font-weight: 700;
    font-size: 1.125em;
    line-height: 1.125em;
    color: #425563;
    letter-spacing: -0.063em;
    padding-bottom: 0.5em;
    padding-top: 0.5em;
  }
</style>
