<template>
  <div v-if="showTemplate">
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <p class="nhsuk-caption-m nhsuk-caption--bottom">
          {{ $t('dataSharing.chooseIfDataFromYourHealthRecordsIsShared') }}
        </p>

        <contents/>

        <p>{{ $t('dataSharing.useThisServiceTo') }}</p>
        <ul>
          <li>{{ $t('dataSharing.chooseIfYourInformationIsUsed') }}</li>
          <li>{{ $t('dataSharing.changeOrCheckYourChoice') }}</li>
        </ul>

        <p>
          {{ $t('dataSharing.ifYouWantToChooseForSomeoneElseFindOutHowOnThe') }}
          <analytics-tracked-tag :href="otherWaysToMakeChoiceUrl"
                                 :text="$t('dataSharing.nhsWebsite')"
                                 class="inline"
                                 tag="a"
                                 target="_blank">
            {{ $t('dataSharing.nhsWebsite') }}</analytics-tracked-tag>.
        </p>

        <p>{{ $t('dataSharing.yourChoiceWillBeAppliedBy') }}</p>
        <ul>
          <li>{{ $t('dataSharing.nhsDigitalAndPublicHealthEngland') }}</li>
          <li>{{ $t('dataSharing.allOtherHealthAndCareOrganisations') }}</li>
        </ul>

        <p>{{ $t('dataSharing.choiceWillNotImpactYourCare') }}</p>

        <inset-text>
          <p>{{ $t('dataSharing.youAreChoosingForHealthAndCareSystems') }}</p>
          <p>{{ $t('dataSharing.youAreNotChoosingForNhsApp') }}</p>
        </inset-text>

        <form id="ndop-token-form"
              ref="ndopTokenForm"
              :action="dataPreferencesUrl"
              target="_self"
              method="POST"
              name="ndopTokenForm">
          <input v-model="ndopToken" type="hidden" name="token">
          <analytics-tracked-tag id="startNowButton"
                                 data-purpose="startNowButton"
                                 :text="$t('dataSharing.startNow')"
                                 class="nhsuk-u-margin-padding-0">
            <generic-button :class="['nhsuk-button']" @click.prevent="startNow">
              {{ $t('dataSharing.startNow') }}
            </generic-button>
          </analytics-tracked-tag>
        </form>

        <pagination :previous-link="doesNotApplyPath"
                    previous-title="When your choice does not apply" />
      </div>
    </div>
  </div>
</template>

<script>
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import Contents from '@/components/data-sharing/Contents';
import GenericButton from '@/components/widgets/GenericButton';
import InsetText from '@/components/InsetText';
import Pagination from '@/components/Pagination';
import { DATA_SHARING_DOES_NOT_APPLY_PATH } from '@/router/paths';
import {
  OTHER_WAYS_TO_MAKE_A_CHOICE_URL,
} from '@/router/externalLinks';

export default {
  name: 'DataSharingMakeYourChoicePage',
  components: {
    AnalyticsTrackedTag,
    Contents,
    GenericButton,
    InsetText,
    Pagination,
  },
  data() {
    return {
      dataPreferencesUrl: this.$store.$env.DATA_PREFERENCES_URL,
      doesNotApplyPath: DATA_SHARING_DOES_NOT_APPLY_PATH,
      ndopToken: undefined,
      otherWaysToMakeChoiceUrl: OTHER_WAYS_TO_MAKE_A_CHOICE_URL,
    };
  },
  methods: {
    async startNow() {
      await this.$store.app.$http.getV1PatientNdop()
        .then(({ token }) => {
          this.ndopToken = token;
        });

      this.$refs.ndopTokenForm.submit();
    },
  },
};
</script>
