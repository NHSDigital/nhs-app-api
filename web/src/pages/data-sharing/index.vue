<!--eslint-disable max-len-->
<!--needed to prevent blank spaces after links-->
<template>
  <div v-if="showTemplate">
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <p class="nhsuk-caption-m nhsuk-caption--bottom">
          {{ $t('dataSharing.chooseIfDataFromYourHealthRecordsIsShared') }}
        </p>

        <contents/>

        <p>{{ $t('dataSharing.yourHealthRecordsContainConfidentialInformation') }}</p>
        <p>{{ $t('dataSharing.yourChoiceWillOnlyApplyInEngland') }}</p>

        <inset-text>
          <p>{{ $t('dataSharing.youAreChoosingForHealthAndCareSystems') }}</p>
          <p>{{ $t('dataSharing.youAreNotChoosingForNhsApp') }}</p>
        </inset-text>

        <h2>{{ $t('dataSharing.whatIsConfidentialInformation') }}</h2>
        <p>{{ $t('dataSharing.confidentialInformationIsWhenTwoTypes') }}</p>
        <p>{{ $t('dataSharing.theTwoTypesOfInformationAre') }}</p>
        <ul>
          <li>{{ $t('dataSharing.somethingThatCanIdentifyYou') }}</li>
          <li>{{ $t('dataSharing.somethingAboutYourHealthCareOrTreatment') }}</li>
        </ul>
        <p>{{ $t('dataSharing.forExampleYourNameAndMedicine') }}</p>
        <p>{{ $t('dataSharing.identifiableInformationOnItsOnIsNotConfidential') }}</p>

        <h2>
          {{ $t('dataSharing.howWeUseConfidentialInformation') }}
        </h2>
        <h3>
          {{ $t('dataSharing.yourIndividualCare') }}
        </h3>
        <p>
          {{ $t('dataSharing.healthAndCareStaffMayUseInformationToHelpWithCare') }}
        </p>

        <h3>
          {{ $t('dataSharing.researchAndPlanning') }}
        </h3>
        <p>{{ $t('dataSharing.confidentialInformationMightBeUsedTo') }}</p>
        <ul>
          <li>{{ $t('dataSharing.planAndImporveHealthAndCareServices') }}</li>
          <li>{{ $t('dataSharing.researchAndDevelopCuresForSeriousIllnesses') }}</li>
        </ul>

        <h2>{{ $t('dataSharing.yourChoice') }}</h2>
        <p>{{ $t('dataSharing.youCanStopInformationUsedForResearchAndPlanning') }}</p>
        <p>{{ $t('dataSharing.ifYouAreHappyYouDoNotNeedToDoAnything') }}</p>
        <p>{{ $t('dataSharing.choiceWillNotImpactYourCare') }}</p>

        <h2>{{ $t('dataSharing.moreOptions') }}</h2>
        <p>
          <analytics-tracked-tag
            :href="yourDataMattersUrl"
            :text="$t('dataSharing.visitTheNhsWebsite')"
            class="inline"
            tag="a"
            target="_blank">{{ $t('dataSharing.visitTheNhsWebsite') }}</analytics-tracked-tag>{{ $t('dataSharing.forMoreInfomationOrToReadPrivacyNotice') }}
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
