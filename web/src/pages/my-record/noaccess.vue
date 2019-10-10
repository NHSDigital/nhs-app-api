<template>
  <div v-if="showTemplate" id="mainDiv" class="pull-content">
    <message-dialog message-type="warning">
      <message-text :is-header="true">
        {{ $t('myRecord.noRecordAccess.warningHeader') }}
      </message-text>
      <message-text>
        {{ $t('myRecord.noRecordAccess.warningBody') }}
      </message-text>
    </message-dialog>
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';

export default {
  components: {
    MessageDialog,
    MessageText,
  },
};
</script>
<style module lang="scss" scoped>

  #mainDiv {
    padding: 0;
  }
</style>
