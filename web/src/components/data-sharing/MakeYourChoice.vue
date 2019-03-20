<template>
  <div :class="$style['content']">

    <p v-for="paragraph of $t('ds01.pages.p4.paragraphs')" :key="paragraph">
      {{ paragraph }}
    </p>

  </div>
</template>

<script>
export default {};
</script>

<style module scoped lang="scss">
@import '../../style/datasharingpage';
</style>
