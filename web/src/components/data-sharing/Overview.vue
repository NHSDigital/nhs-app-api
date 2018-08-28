<template>
  <div class="content">
    <p>{{ $t('ds01.pages.p1.intro.paragraph') }}</p>
    <h3 :class="$style['title']">{{ $t('ds01.pages.p1.confidential.title') }}</h3>
    <p v-for="paragraph of $t('ds01.pages.p1.confidential.paragraphs')" :key="paragraph">
      {{ paragraph }}
    </p>
    <p>{{ $t('ds01.pages.p1.confidential.paragraphs[1]') }}</p>
    <h3 :class="$style['title']">{{ $t('ds01.pages.p1.yourChoice.title') }}</h3>
    <p>{{ $t('ds01.pages.p1.yourChoice.paragraph') }}</p>
    <a :title="$t('ds01.pages.p1.yourChoice.manageChoiceLink')" @click="goToManageChoices($event)">
      {{ $t('ds01.pages.p1.yourChoice.manageChoiceLink') }}
    </a>
    <h3 :class="$style['title']">{{ $t('ds01.pages.p1.moreOptions.title') }}</h3>
    <p>{{ $t('ds01.pages.p1.moreOptions.paragraph.part1') }}
      <a :href="yourDataMattersUrl" :class="$style['paragraph-link']" target="_blank">
        {{ $t('ds01.pages.p1.moreOptions.paragraph.nhsWebsiteLink') }}</a>
      {{ $t('ds01.pages.p1.moreOptions.paragraph.part3') }}
    </p>
  </div>
</template>

<script>
export default {
  data() {
    return {
      yourDataMattersUrl: process.env.YOUR_NHS_DATA_MATTERS_URL,
    };
  },
  methods: {
    goToManageChoices(event) {
      event.preventDefault();
      this.$emit('manage-choices');
    },
  },
};
</script>

<style module scoped lang='scss'>
@import '../../style/datasharingpage';
</style>
