<template>
  <div v-if="showTemplate">
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <p class="nhsuk-caption-m nhsuk-caption--bottom">
          {{ $t('ds01.mainHeader') }}
        </p>

        <contents/>

        <p>{{ $t('ds01.pages.p3.intro.paragraph') }}</p>

        <h2>{{ $t('ds01.pages.p3.requiredByLaw.title') }}</h2>
        <p>{{ $t('ds01.pages.p3.requiredByLaw.paragraph') }}</p>

        <h2>{{ $t('ds01.pages.p3.givenConsent.title') }}</h2>
        <p>{{ $t('ds01.pages.p3.givenConsent.paragraph') }}</p>

        <h2>{{ $t('ds01.pages.p3.publicInterest.title') }}</h2>
        <p>{{ $t('ds01.pages.p3.publicInterest.paragraph') }}</p>

        <h2>{{ $t('ds01.pages.p3.informationRemoved.title') }}</h2>
        <p>{{ $t('ds01.pages.p3.informationRemoved.paragraph') }}</p>

        <h2>{{ $t('ds01.pages.p3.specificExclusion.title') }}</h2>
        <p>{{ $t('ds01.pages.p3.specificExclusion.paragraph') }}</p>

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
