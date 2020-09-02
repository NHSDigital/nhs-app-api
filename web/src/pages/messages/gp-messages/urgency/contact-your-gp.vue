<template>
  <div v-if="showTemplate" class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full">
      <div :class="$style.info">
        <p id="infoMessagingPurpose">{{ $t('messages.messagingIsForNonUrgentAdvice') }}</p>
        <p id="infoWhatToDo" :aria-label="$t('messages.forAdviceNowContactSurgeryOrOneOneOne')">
          {{ $t('messages.forAdviceNowContactSurgery') }}
          <a href="https://111.nhs.uk">{{ $t('messages.nhs111Link') }}</a>
          {{ $t('messages.or') }}
          <a href="tel:111">{{ $t('messages.call111Link') }}</a>.
        </p>
      </div>
      <care-card id="phoneYourGpCareCard"
                 class="nhsuk-u-margin-top-7 nhsuk-u-margin-bottom-7"
                 urgency="urgent"
                 :heading="$t('messages.call999NowIfYouHave')">
        <ul>
          <li v-for="(symptom, index) in $t('messages.call999NowIfYouHaveSymptoms')"
              :key="`symptom-${index}`"
              :aria-label="`${symptom.title} - ${symptom.description}`">
            <strong>{{ symptom.title }}</strong> - {{ symptom.description }}
          </li>
        </ul>
      </care-card>
      <desktop-generic-back-link v-if="!$store.state.device.isNativeApp"
                                 :path="urgencyPath"
                                 @clickAndPrevent="backLinkClicked"/>
    </div>
  </div>
</template>

<script>
import CareCard from '@/components/widgets/CareCard';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import { redirectTo } from '@/lib/utils';
import { GP_MESSAGES_URGENCY_PATH } from '@/router/paths';

export default {
  name: 'GpMessagesUrgencyContactYourGpPage',
  components: {
    CareCard,
    DesktopGenericBackLink,
  },
  data() {
    return {
      urgencyPath: GP_MESSAGES_URGENCY_PATH,
    };
  },
  methods: {
    backLinkClicked() {
      redirectTo(this, this.urgencyPath);
    },
  },
};
</script>

<style module lang="scss" scoped>
.info p > a {
  vertical-align: baseline;
  display: inline;
}
</style>
