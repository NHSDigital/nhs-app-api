<template>
  <div v-if="showTemplate">
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <p class="nhsuk-caption-m nhsuk-caption--bottom">
          {{ $t('dataSharing.chooseIfDataFromYourHealthRecordsIsShared') }}
        </p>

        <contents/>

        <p>{{ $t('dataSharing.theNhsCollectsInformationFrom') }}</p>
        <ul>
          <li>{{ $t('dataSharing.allNhsOrgsTrustsAndLocalAuthorities') }}</li>
          <li>{{ $t('dataSharing.privateOrganisations') }}</li>
        </ul>

        <p>{{ $t('dataSharing.researchBodiesAndOrgsCanRequestIncluding') }}</p>
        <ul>
          <li>{{ $t('dataSharing.universityResearchers') }}</li>
          <li>{{ $t('dataSharing.hospitalResearchers') }}</li>
          <li>{{ $t('dataSharing.medicalRoyalColleges') }}</li>
          <li>{{ $t('dataSharing.pharmaceuticalCompanies') }}</li>
        </ul>

        <h2>{{ $t('dataSharing.whoCannotUseConfidentialInformation') }}</h2>
        <p>{{ $t('dataSharing.accessWillNotBeGivenFor') }}</p>
        <ul>
          <li>{{ $t('dataSharing.marketingPurposes') }}</li>
          <li>{{ $t('dataSharing.insurancePurposes') }}</li>
        </ul>
        <p>{{ $t('dataSharing.unlessYouRequestThis') }}</p>

        <h2>{{ $t('dataSharing.howConfidentialInformationIsProtected') }}</h2>
        <p>{{ $t('dataSharing.yourInformationIsLookedAfter') }}</p>
        <p>{{ $t('dataSharing.everyOrganisationWillTakeEveryStepTo') }}</p>
        <ul>
          <li>{{ $t('dataSharing.keeyDataSecure') }}</li>
          <li>{{ $t('dataSharing.useDataThatCannotIdentifyYouWhenPossible') }}</li>
          <li>{{ $t('dataSharing.useDataToBenefitHealthAndCare') }}</li>
          <li>{{ $t('dataSharing.notUseDataForMarketingOrInsurance') }}</li>
          <li>{{ $t('dataSharing.makeItClearWhyAndHowDataIsBeingUsed') }}</li>
        </ul>
        <p>{{ $t('dataSharing.allNhsOrganisationMustProvideInformation') }}</p>

        <pagination :previous-link="overviewPath"
                    previous-title="Overview"
                    :next-link="doesNotApplyPath"
                    next-title="When your choice does not apply" />
      </div>
    </div>
  </div>
</template>

<script>
import Contents from '@/components/data-sharing/Contents';
import Pagination from '@/components/Pagination';
import {
  DATA_SHARING_DOES_NOT_APPLY_PATH,
  DATA_SHARING_OVERVIEW_PATH,
} from '@/router/paths';

export default {
  name: 'DataSharingWhereUsedPage',
  components: {
    Contents,
    Pagination,
  },
  data() {
    return {
      doesNotApplyPath: DATA_SHARING_DOES_NOT_APPLY_PATH,
      overviewPath: DATA_SHARING_OVERVIEW_PATH,
    };
  },
};
</script>
