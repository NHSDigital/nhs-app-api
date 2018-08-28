<template>
  <div class="content">
    <p v-for="paragraph in $t('ds01.pages.p2.paragraphs')" :key="paragraph">{{ paragraph }}</p>
  </div>
</template>

<script>
export default {};
</script>

<style module scoped lang='scss'>
@import '../../style/datasharingpage';
</style>
