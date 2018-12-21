<template>
  <div class="content">

    <p>{{ $t('ds01.pages.p1.intro.paragraph') }}</p>

    <h2 :class="$style['title']">{{ $t('ds01.pages.p1.confidential.title') }}</h2>
    <p>{{ $t('ds01.pages.p1.confidential.paragraph1') }}</p>
    <ul>
      <li v-for="listItem of $t('ds01.pages.p1.confidential.listItems')" :key="listItem">
        {{ listItem }}
      </li>
    </ul>
    <p>{{ $t('ds01.pages.p1.confidential.paragraph2') }}</p>

    <h2 :class="$style['title']">
      {{ $t('ds01.pages.p1.patientInformation.title') }}
    </h2>
    <h3 :class="$style['title']">
      {{ $t('ds01.pages.p1.patientInformation.yourIndividualCareSubtitle') }}
    </h3>
    <p>
      {{ $t('ds01.pages.p1.patientInformation.yourIndividualCareParagraph') }}
    </p>

    <h3 :class="$style['title']">
      {{ $t('ds01.pages.p1.patientInformation.researchAndPlanningSubtitle') }}
    </h3>
    <p>{{ $t('ds01.pages.p1.patientInformation.researchAndPlanningParagraph') }}</p>
    <ul>
      <li v-for="listItem of $t('ds01.pages.p1.patientInformation.researchAndPlanningListItems')"
          :key="listItem">
        {{ listItem }}
      </li>
    </ul>

    <h2 :class="$style['title']">{{ $t('ds01.pages.p1.yourChoice.title') }}</h2>
    <p>{{ $t('ds01.pages.p1.yourChoice.paragraph') }}</p>

    <h2 :class="$style['title']">{{ $t('ds01.pages.p1.moreOptions.title') }}</h2>
    <p :aria-label="$t('ds01.pages.p1.moreOptions.paragraph.nhsWebsiteLink') +
    $t('ds01.pages.p1.moreOptions.paragraph.part2')">
      <analytics-tracked-tag :href="yourDataMattersUrl"
                             :class="$style['paragraph-link']"
                             :text="$t('ds01.pages.p1.moreOptions.paragraph.nhsWebsiteLink')"
                             tag="a" target="_blank">
        {{ $t('ds01.pages.p1.moreOptions.paragraph.nhsWebsiteLink') }}
      </analytics-tracked-tag>
      {{ $t('ds01.pages.p1.moreOptions.paragraph.part2') }}
    </p>

  </div>
</template>

<script>
/* eslint-disable import/extensions */
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';

export default {
  components: {
    AnalyticsTrackedTag,
  },
  data() {
    return {
      yourDataMattersUrl: this.$store.app.$env.YOUR_NHS_DATA_MATTERS_URL,
    };
  },
};
</script>

<style module scoped lang='scss'>
@import '../../style/datasharingpage';
</style>
