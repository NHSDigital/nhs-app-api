<template>
  <div v-if="showTemplate" class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full">
      <div :class="$style.info">
        <p id="infoMessagingPurpose">{{ $t('im03.info.paragraph1') }}</p>
        <p id="infoWhatToDo" :aria-label="$t('im03.info.paragraph2.ariaLabel')">
          {{ $t('im03.info.paragraph2.part1') }}
          <a href="https://111.nhs.uk">{{ $t('im03.info.paragraph2.part2') }}</a>
          {{ $t('im03.info.paragraph2.part3') }}
          <a href="tel:111">{{ $t('im03.info.paragraph2.part4') }}</a>.
        </p>
      </div>
      <care-card id="phoneYourGpCareCard"
                 class="nhsuk-u-margin-top-7 nhsuk-u-margin-bottom-7"
                 urgency="urgent"
                 :heading="$t('im03.careCard.heading')">
        <ul>
          <li v-for="(symptom, index) in $t('im03.careCard.symptoms')"
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
import { GP_MESSAGES_URGENCY } from '@/lib/routes';
import { redirectTo } from '@/lib/utils';

export default {
  layout: 'nhsuk-layout',
  components: {
    CareCard,
    DesktopGenericBackLink,
  },
  data() {
    return {
      urgencyPath: GP_MESSAGES_URGENCY.path,
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
