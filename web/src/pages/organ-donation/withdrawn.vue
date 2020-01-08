<template>
  <div id="mainDiv" class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full">
      <message-dialog :icon-text="$t('organDonation.withdrawn.dialogText')"
                      message-id="success-dialog" message-type="success">
        <message-text v-for="(item, index) in $t('organDonation.withdrawn.messageTextItems')"
                      :key="index">
          {{ item }}
        </message-text>
      </message-dialog>
      <div>
        <h2>{{ $t('organDonation.withdrawn.whatNext.header') }}</h2>
        <p v-for="(item, index) in $t('organDonation.withdrawn.whatNext.bodyItems')" :key="index">
          {{ item }}
        </p>
      </div>
      <other-things-to-do :can-withdraw="false"/>
    </div>
  </div>
</template>

<script>
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import NativeOnlyMixin from '@/components/NativeOnlyMixin';
import OtherThingsToDo from '@/components/organ-donation/OtherThingsToDo';

export default {
  layout: 'nhsuk-layout',
  components: {
    MessageText,
    MessageDialog,
    OtherThingsToDo,
  },
  mixins: [NativeOnlyMixin],
  created() {
    this.$store.dispatch('organDonation/init');
  },
};
</script>
