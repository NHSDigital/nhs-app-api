<template>
  <div class="content">
    <p v-for="paragraph in $t('ds01.pages.p3.paragraphs')" :key="paragraph">{{ paragraph }}</p>

    <ul :class="$style['choices-not-apply']">
      <li v-for="listItem of $t('ds01.pages.p3.listItems')" :key="listItem.title">
        <h2>{{ listItem.title }}</h2>
        <p>{{ listItem.text }}</p>
      </li>
    </ul>

  </div>
</template>

<script>
export default {
  name: 'WhereYourChoiceDoesNotApply',
};
</script>

<style module scoped lang='scss'>
@import '../../style/datasharingpage';

ul.choices-not-apply {
  list-style-type: none;
  li {
    margin-left: 0;
  }
}
</style>
