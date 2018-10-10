<template>
  <div class="content">

    <p>{{ $t('ds01.pages.p1.intro.paragraph') }}</p>

    <h2 :class="$style['title']">{{ $t('ds01.pages.p1.confidential.title') }}</h2>
    <p v-for="paragraph of $t('ds01.pages.p1.confidential.paragraphs')" :key="paragraph">
      {{ paragraph }}
    </p>

    <h2 :class="$style['title']">{{ $t('ds01.pages.p1.yourChoice.title') }}</h2>
    <p>{{ $t('ds01.pages.p1.yourChoice.paragraph') }}</p>

    <a id="manage-choice-link" :title="$t('ds01.pages.p1.yourChoice.manageChoiceLink')"
       role="link" @click="goToManageChoices($event)">
      {{ $t('ds01.pages.p1.yourChoice.manageChoiceLink') }}
    </a>

    <h2 :class="$style['title']">{{ $t('ds01.pages.p1.moreOptions.title') }}</h2>
    <p :aria-label="$t('ds01.pages.p1.moreOptions.paragraph.part1') +
      $t('ds01.pages.p1.moreOptions.paragraph.nhsWebsiteLink') +
    $t('ds01.pages.p1.moreOptions.paragraph.part3')">
      {{ $t('ds01.pages.p1.moreOptions.paragraph.part1') }}
      <analytics-tracked-tag :href="yourDataMattersUrl"
                             :class="$style['paragraph-link']"
                             :text="$t('ds01.pages.p1.moreOptions.paragraph.nhsWebsiteLink')"
                             tag="a" target="_blank">
        {{ $t('ds01.pages.p1.moreOptions.paragraph.nhsWebsiteLink') }}
      </analytics-tracked-tag>
      {{ $t('ds01.pages.p1.moreOptions.paragraph.part3') }}
    </p>

  </div>
</template>

<script>
/* eslint-disable import/extensions */
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import { DATA_SHARING_PREFERENCES } from '@/lib/routes';

export default {
  components: {
    AnalyticsTrackedTag,
  },
  data() {
    return {
      yourDataMattersUrl: this.$store.app.$env.YOUR_NHS_DATA_MATTERS_URL,
    };
  },
  methods: {
    goToManageChoices(event) {
      event.preventDefault();
      this.$store.app.$analytics.trackButtonClick(`${DATA_SHARING_PREFERENCES.name}-overview:text_link:${DATA_SHARING_PREFERENCES.name}-manage-your-choice`);
      this.$emit('manage-choices');
    },
  },
};
</script>

<style module scoped lang='scss'>
@import '../../style/datasharingpage';
</style>
