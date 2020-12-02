<template>
  <div>
    <p>{{ $t('profiles.youDoNotHaveAny') }}</p>
    <p>{{ $t('profiles.familyMemberAndCarers') }}</p>
    <p>{{ $t('profiles.youCanBookAppointments') }}</p>
    <p><a id="findOutMoreLink"
          :href="findOutMoreUrl"
          target="_blank"
          rel="noopener noreferrer">
      {{ $t('profiles.findOutMore') }}</a></p>
    <h2 class="nhsuk-u-margin-bottom-0">{{ $t('profiles.addALinkedProfile') }}</h2>
    <p>{{ $t('profiles.contactTheGpSurgery') }}</p>
  </div>
</template>
<script>
import { LINKED_PROFILES_FIND_OUT_MORE_URL } from '@/router/externalLinks';

export default {
  name: 'NoLinkedProfiles',
  data() {
    return {
      findOutMoreUrl: LINKED_PROFILES_FIND_OUT_MORE_URL,
    };
  },
};
</script>
