<template>
  <div class="content">

    <h2 :class="$style['title']">{{ $t('ds01.pages.p2.research.title') }}</h2>
    <ul>
      <li v-for="listItem of $t('ds01.pages.p2.research.listItems')" :key="listItem">
        {{ listItem }}
      </li>
    </ul>
    <p v-for="paragraph of $t('ds01.pages.p2.research.paragraphs')" :key="paragraph">
      {{ paragraph }}
    </p>

    <h2 :class="$style['title']">{{ $t('ds01.pages.p2.planning.title') }}</h2>
    <ul>
      <li v-for="listItem of $t('ds01.pages.p2.planning.listItems')" :key="listItem">
        {{ listItem }}
      </li>
    </ul>
    <p v-for="paragraph of $t('ds01.pages.p2.planning.paragraphs')" :key="paragraph">
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
