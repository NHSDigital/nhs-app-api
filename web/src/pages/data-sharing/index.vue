<!--eslint-disable max-len-->
<!--needed to prevent blank spaces after links-->
<template>
  <div v-if="showTemplate">
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <p class="nhsuk-caption-m nhsuk-caption--bottom">
          {{ $t('ds01.mainHeader') }}
        </p>

        <contents/>

        <p>{{ $t('ds01.pages.p1.intro.paragraph1') }}</p>
        <p>{{ $t('ds01.pages.p1.intro.paragraph2') }}</p>

        <inset-text :paragraphs="$t('ds01.pages.ndop.paragraphs')"/>

        <h2>{{ $t('ds01.pages.p1.confidential.title') }}</h2>
        <p>{{ $t('ds01.pages.p1.confidential.paragraph1') }}</p>
        <p>{{ $t('ds01.pages.p1.confidential.paragraph2') }}</p>
        <ul>
          <li v-for="listItem of $t('ds01.pages.p1.confidential.listItems')" :key="listItem">
            {{ listItem }}
          </li>
        </ul>
        <p>{{ $t('ds01.pages.p1.confidential.paragraph3') }}</p>
        <p>{{ $t('ds01.pages.p1.confidential.paragraph4') }}</p>

        <h2>
          {{ $t('ds01.pages.p1.patientInformation.title') }}
        </h2>
        <h3>
          {{ $t('ds01.pages.p1.patientInformation.yourIndividualCareSubtitle') }}
        </h3>
        <p>
          {{ $t('ds01.pages.p1.patientInformation.yourIndividualCareParagraph') }}
        </p>

        <h3>
          {{ $t('ds01.pages.p1.patientInformation.researchAndPlanningSubtitle') }}
        </h3>
        <p>{{ $t('ds01.pages.p1.patientInformation.researchAndPlanningParagraph') }}</p>
        <ul>
          <li v-for="listItem of
            $t('ds01.pages.p1.patientInformation.researchAndPlanningListItems')" :key="listItem">
            {{ listItem }}
          </li>
        </ul>

        <h2>{{ $t('ds01.pages.p1.yourChoice.title') }}</h2>
        <p>{{ $t('ds01.pages.p1.yourChoice.paragraph1') }}</p>
        <p>{{ $t('ds01.pages.p1.yourChoice.paragraph2') }}</p>
        <p>{{ $t('ds01.pages.p1.yourChoice.paragraph3') }}</p>

        <h2>{{ $t('ds01.pages.p1.moreOptions.title') }}</h2>
        <p>
          <analytics-tracked-tag
            :href="yourDataMattersUrl"
            :text="$t('ds01.pages.p1.moreOptions.paragraph.nhsWebsiteLink')"
            class="inline"
            tag="a"
            target="_blank">{{ $t('ds01.pages.p1.moreOptions.paragraph.nhsWebsiteLink') }}</analytics-tracked-tag>{{ $t('ds01.pages.p1.moreOptions.paragraph.part2') }}
        </p>

        <pagination :next-link="whereUsedPath"
                    next-title="How confidential patient information is used" />
      </div>
    </div>
  </div>
</template>

<script>
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import Contents from '@/components/data-sharing/Contents';
import InsetText from '@/components/InsetText';
import Pagination from '@/components/Pagination';
import { DATA_SHARING_WHERE_USED_PATH } from '@/router/paths';
import {
  YOUR_NHS_DATA_MATTERS_URL,
} from '@/router/externalLinks';

export default {
  name: 'DataSharingOverviewPage',
  components: {
    AnalyticsTrackedTag,
    Contents,
    InsetText,
    Pagination,
  },
  data() {
    return {
      whereUsedPath: DATA_SHARING_WHERE_USED_PATH,
      yourDataMattersUrl: YOUR_NHS_DATA_MATTERS_URL,
    };
  },
};
</script>
