<template>
  <div>
    <success-icon/>
    <div :class="[$style.msg, $style.success]">
      <slot/>
    </div>
  </div>
</template>

<script> 
import SuccessIcon from '@/components/icons/SuccessIcon'; 
 
export default { 
  components: { 
    SuccessIcon, 
  }, 
  data() { 
    return { 
      msgClass: 'msg', 
    }; 
  }, 
}; 
</script> 

<style module lang="scss">
  @import "../style/messages";

</style>
