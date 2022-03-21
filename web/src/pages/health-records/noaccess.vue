<template>
  <div v-if="showTemplate" id="mainDiv" class="pull-content">
    <message-dialog-generic message-type="warning">
      <message-text :is-header="true">
        {{ $t('myRecord.youDoNotHaveAccessToYourRecord') }}
      </message-text>
      <message-text>
        {{ $t('myRecord.contactSurgeryForMoreInformation') }}
      </message-text>
    </message-dialog-generic>
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import MessageDialogGeneric from '@/components/widgets/MessageDialogGeneric';
import MessageText from '@/components/widgets/MessageText';

export default {
  components: {
    MessageDialogGeneric,
    MessageText,
  },
};
</script>

<style module lang="scss" scoped>
  @import "@/style/custom/no-access";
</style>
