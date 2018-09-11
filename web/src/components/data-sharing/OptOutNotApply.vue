<template>
  <div class="content">

    <p v-for="paragraph of $t('ds01.pages.p4.paragraphs')" :key="paragraph">
      {{ paragraph }}
    </p>
    <ul>
      <li v-for="listItem of $t('ds01.pages.p4.listItems')" :key="listItem">
        {{ listItem }}
      </li>
    </ul>

  </div>
</template>

<script>
export default {};
</script>

<style module scoped lang="scss">
@import '../../style/datasharingpage';
</style>
