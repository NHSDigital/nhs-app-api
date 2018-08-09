<template>
  <div v-if="showTemplate" id="mainDiv">
    <main class="content">
      <error-warning-dialog error-or-warning="warning">
        <p>
          <b>{{ $t('my_record.noRecordAccess.warningHeader') }}</b>
          <br>
          {{ $t('my_record.noRecordAccess.warningBody') }}
        </p>
      </error-warning-dialog>
    </main>
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import ErrorWarningDialog from '@/components/errors/ErrorWarningDialog';

export default {
  components: {
    ErrorWarningDialog,
  },
};
</script>
<style lang="scss">
  @import '../../style/html';
  @import '../../style/textstyles';
  @import '../../style/elements';

  #mainDiv {
    padding: 0;
    @include default_text;
  }
</style>
