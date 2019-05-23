<template>
  <div :class="$style['content']">

    <p v-for="paragraph of $t('ds01.pages.p4.paragraphs')" :key="paragraph">
      {{ paragraph }}
    </p>

    <inset-content :paragraphs="$t('ds01.pages.ndop.paragraphs')"/>

  </div>
</template>

<script>
import InsetContent from '@/components/data-sharing/InsetContent';

export default {
  name: 'MakeYourChoice',
  components: {
    InsetContent,
  },
};
</script>

<style module scoped lang="scss">
@import '../../style/datasharingpage';
</style>
