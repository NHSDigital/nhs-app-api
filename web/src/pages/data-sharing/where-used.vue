<template>
  <div v-if="showTemplate">
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <p class="nhsuk-caption-m nhsuk-caption--bottom">
          {{ $t('ds01.mainHeader') }}
        </p>

        <contents/>

        <p>{{ $t('ds01.pages.p2.intro.paragraph1') }}</p>
        <ul>
          <li v-for="listItem of $t('ds01.pages.p2.intro.listItems1')" :key="listItem">
            {{ listItem }}
          </li>
        </ul>

        <p>{{ $t('ds01.pages.p2.intro.paragraph2') }}</p>
        <ul>
          <li v-for="listItem of $t('ds01.pages.p2.intro.listItems2')" :key="listItem">
            {{ listItem }}
          </li>
        </ul>

        <h2>{{ $t('ds01.pages.p2.thoseWhoCant.title') }}</h2>
        <p>{{ $t('ds01.pages.p2.thoseWhoCant.paragraph1') }}</p>
        <ul>
          <li v-for="listItem of $t('ds01.pages.p2.thoseWhoCant.listItems')" :key="listItem">
            {{ listItem }}
          </li>
        </ul>
        <p>{{ $t('ds01.pages.p2.thoseWhoCant.paragraph2') }}</p>

        <h2>{{ $t('ds01.pages.p2.dataProtection.title') }}</h2>
        <p>{{ $t('ds01.pages.p2.dataProtection.paragraph1') }}</p>
        <p>{{ $t('ds01.pages.p2.dataProtection.paragraph2') }}</p>
        <ul>
          <li v-for="listItem of $t('ds01.pages.p2.dataProtection.listItems')" :key="listItem">
            {{ listItem }}
          </li>
        </ul>
        <p>{{ $t('ds01.pages.p2.dataProtection.paragraph3') }}</p>

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
