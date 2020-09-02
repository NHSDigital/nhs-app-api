<template>
  <div v-if="showTemplate">
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <p class="nhsuk-caption-m nhsuk-caption--bottom">
          {{ $t('dataSharing.chooseIfDataFromYourHealthRecordsIsShared') }}
        </p>

        <contents/>

        <p>{{ $t('dataSharing.ifYouChooseToStopYourDataMightBeUsed') }}</p>

        <h2>{{ $t('dataSharing.whenRequiredByLaw') }}</h2>
        <p>{{ $t('dataSharing.ifThereIsALegalRequirement') }}</p>

        <h2>{{ $t('dataSharing.whenYouHaveGivenConsent') }}</h2>
        <p>{{ $t('dataSharing.ifYouHaveGivenYourConsent') }}</p>

        <h2>{{ $t('dataSharing.whenThereIsOverridingPublicInterest') }}</h2>
        <p>{{ $t('dataSharing.inAnEmergencyOrWhenSafetyOfOthersIsImportant') }}</p>

        <h2>{{ $t('dataSharing.whenInformationThatCanIdentifyIsRemoved') }}</h2>
        <p>{{ $t('dataSharing.informationMightBeUsedIfIdentifyingInformationIsRemoved') }}</p>

        <h2>{{ $t('dataSharing.whenThereIsASpecificExclusion') }}</h2>
        <p>{{ $t('dataSharing.yourConfidentalInformationCanStillBeUsed') }}</p>

        <pagination :previous-link="whereUsedPath"
                    previous-title="How confidential patient information is used"
                    :next-link="makeYourChoicePath"
                    next-title="Make your choice" />
      </div>
    </div>
  </div>
</template>

<script>
import Contents from '@/components/data-sharing/Contents';
import Pagination from '@/components/Pagination';
import {
  DATA_SHARING_MAKE_YOUR_CHOICE_PATH,
  DATA_SHARING_WHERE_USED_PATH,
} from '@/router/paths';

export default {
  name: 'DataSharingDoesNotApplyPage',
  components: {
    Contents,
    Pagination,
  },
  data() {
    return {
      makeYourChoicePath: DATA_SHARING_MAKE_YOUR_CHOICE_PATH,
      whereUsedPath: DATA_SHARING_WHERE_USED_PATH,
    };
  },
};
</script>
