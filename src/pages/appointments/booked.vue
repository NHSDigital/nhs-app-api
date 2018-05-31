<template>
  <main :class="$style.main">
    <success-dialog>
      <p>
        {{$t('appointments.booked.successText')}}
      </p>
    </success-dialog>
  </main>
</template>

<script>
import SuccessDialog from '@/components/SuccessDialog'; 
 
export default {
  middleware: ['auth', 'meta'],
  components: {
    SuccessDialog,
  },
};
</script>

<style module lang="scss">
  @import "../../style/html";
  @import "../../style/spacings";

  .main {
    @include space(padding, all, $three);
  }

</style>
