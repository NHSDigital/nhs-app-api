<template>
  <div id="mainDiv">
    <main class="content">
      <error-warning-dialog errorOrWarning="error">
        <p class="header">
          {{$t('appointments.noConnection.dialogHeader')}}
        </p>
        <p>
          {{$t('appointments.noConnection.dialogText')}}
        </p>
      </error-warning-dialog>
      <button class="button" @click="$emit('retry')">
        {{$t('appointments.noConnection.retryText')}}
      </button>
      <p>
        {{$t('appointments.noConnection.info')}}
      </p>
    </main>
  </div>
</template>
<script>
import ErrorWarningDialog from '@/components/ErrorWarningDialog';

export default {
  components: {
    ErrorWarningDialog,
  },
};
</script>

<style lang="scss">
  @import '../../style/html';
  @import '../../style/elements';
  @import '../../style/buttons';

</style>
