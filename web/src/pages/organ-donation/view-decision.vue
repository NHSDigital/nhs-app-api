<template>
  <div id="mainDiv" :class="[$style['no-padding'], 'pull-content']">
    <message-dialog message-id="success-dialog" message-type="success">
      <message-text>We have updated your decision</message-text>
    </message-dialog>
    <your-decision :decision="$store.state.organDonation.registration.decision"/>
    <other-things-to-do/>
  </div>
</template>

<script>
import EnsureDecisionMixin from '@/components/organ-donation/EnsureDecisionMixin';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import OtherThingsToDo from '@/components/organ-donation/OtherThingsToDo';
import YourDecision from '@/components/organ-donation/YourDecision';

export default {
  components: {
    MessageText,
    MessageDialog,
    OtherThingsToDo,
    YourDecision,
  },
  mixins: [EnsureDecisionMixin],
};
</script>

<style module lang="scss" scoped>
  @import "../../style/info";

</style>
