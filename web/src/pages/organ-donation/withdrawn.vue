<template>
  <div id="mainDiv" :class="[$style['no-padding'], 'pull-content']">
    <message-dialog :icon-text="$t('organDonation.withdrawn.dialogText')"
                    message-id="success-dialog" message-type="success">
      <message-text v-for="(item, index) in $t('organDonation.withdrawn.messageTextItems')"
                    :key="index">
        {{ item }}
      </message-text>
    </message-dialog>
    <div :class="$style.info">
      <h2>{{ $t('organDonation.withdrawn.whatNext.header') }}</h2>
      <p v-for="(item, index) in $t('organDonation.withdrawn.whatNext.bodyItems')" :key="index">
        {{ item }}
      </p>
    </div>
    <other-things-to-do :can-withdraw="false"/>
  </div>
</template>

<script>
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import OtherThingsToDo from '@/components/organ-donation/OtherThingsToDo';

export default {
  components: {
    MessageText,
    MessageDialog,
    OtherThingsToDo,
  },
  created() {
    this.$store.dispatch('organDonation/init');
  },
};
</script>

<style module lang="scss" scoped>
@import "../../style/info";
@import "../../style/spacings";
</style>
