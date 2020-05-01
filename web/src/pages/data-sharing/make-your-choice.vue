<template>
  <div v-if="showTemplate">
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <p class="nhsuk-caption-m nhsuk-caption--bottom">
          {{ $t('ds01.mainHeader') }}
        </p>

        <contents/>

        <p>{{ $t('ds01.pages.p4.paragraph1') }}</p>
        <ul>
          <li v-for="listItem of $t('ds01.pages.p4.listItems1')" :key="listItem">
            {{ listItem }}
          </li>
        </ul>

        <p>
          {{ $t('ds01.pages.p4.paragraph2.text') }}
          <analytics-tracked-tag :href="otherWaysToMakeChoiceUrl"
                                 :text="$t('ds01.pages.p4.paragraph2.nhsWebsiteLink')"
                                 class="inline"
                                 tag="a"
                                 target="_blank">
            {{ $t('ds01.pages.p4.paragraph2.nhsWebsiteLink') }}</analytics-tracked-tag>.
        </p>

        <p>{{ $t('ds01.pages.p4.paragraph3') }}</p>
        <ul>
          <li v-for="listItem of $t('ds01.pages.p4.listItems2')" :key="listItem">
            {{ listItem }}
          </li>
        </ul>

        <p>{{ $t('ds01.pages.p4.paragraph4') }}</p>

        <inset-text :paragraphs="$t('ds01.pages.ndop.paragraphs')"/>

        <form id="ndop-token-form"
              ref="ndopTokenForm"
              :action="dataPreferencesUrl"
              target="_self"
              method="POST"
              name="ndopTokenForm">
          <input v-model="ndopToken" type="hidden" name="token">
          <analytics-tracked-tag id="startNowButton"
                                 data-purpose="startNowButton"
                                 :text="$t('ds01.startNowButton')"
                                 class="nhsuk-u-margin-padding-0">
            <generic-button :class="['nhsuk-button']" @click.prevent="startNow">
              {{ $t('ds01.startNowButton') }}
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
import { DATA_SHARING_DOES_NOT_APPLY } from '@/lib/routes';

export default {
  layout: 'nhsuk-layout',
  components: {
    AnalyticsTrackedTag,
    Contents,
    GenericButton,
    InsetText,
    Pagination,
  },
  data() {
    return {
      dataPreferencesUrl: this.$store.app.$env.DATA_PREFERENCES_URL,
      doesNotApplyPath: DATA_SHARING_DOES_NOT_APPLY.path,
      ndopToken: undefined,
      otherWaysToMakeChoiceUrl: this.$store.app.$env.OTHER_WAYS_TO_MAKE_A_CHOICE,
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
