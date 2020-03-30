<template>
  <nav class="nhsuk-pagination" role="navigation" aria-label="Pagination">
    <ul class="nhsuk-list nhsuk-pagination__list">
      <li v-if="previousLink && previousTitle" class="nhsuk-pagination-item--previous">
        <analytics-tracked-tag id="previousPagination"
                               class="nhsuk-pagination__link nhsuk-pagination__link--prev"
                               :href="previousLink"
                               :click-param="previousLink"
                               :click-func="goToUrl"
                               :text="previousTitle"
                               tag="a">
          <span class="nhsuk-pagination__title">{{ $t('components.pagination.previous') }}</span>
          <span class="nhsuk-u-visually-hidden">:</span>
          <span class="nhsuk-pagination__page"> {{ previousTitle }} </span>
          <svg class="nhsuk-icon nhsuk-icon__arrow-left" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" aria-hidden="true">
            <path d="M4.1 12.3l2.7 3c.2.2.5.2.7 0 .1-.1.1-.2.1-.3v-2h11c.6 0 1-.4
            1-1s-.4-1-1-1h-11V9c0-.2-.1-.4-.3-.5h-.2c-.1 0-.3.1-.4.2l-2.7 3c0 .2 0 .4.1.6z"/>
          </svg>
        </analytics-tracked-tag>
      </li>
      <li v-if="nextLink && nextTitle" class="nhsuk-pagination-item--next">
        <analytics-tracked-tag id="nextPagination"
                               class="nhsuk-pagination__link nhsuk-pagination__link--next"
                               :href="nextLink"
                               :click-param="nextLink"
                               :click-func="goToUrl"
                               :text="nextTitle"
                               tag="a">
          <span class="nhsuk-pagination__title">{{ $t('components.pagination.next') }}</span>
          <span class="nhsuk-u-visually-hidden">:</span>
          <span class="nhsuk-pagination__page"> {{ nextTitle }} </span>
          <svg class="nhsuk-icon nhsuk-icon__arrow-right" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" aria-hidden="true">
            <path d="M19.6 11.66l-2.73-3A.51.51 0 0 0 16 9v2H5a1 1 0 0 0 0 2h11v2a.5.5
             0 0 0 .32.46.39.39 0 0 0 .18 0 .52.52 0 0 0 .37-.16l2.73-3a.5.5 0 0 0 0-.64z"/>
          </svg>
        </analytics-tracked-tag>
      </li>
    </ul>
  </nav>
</template>

<script>
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';

export default {
  name: 'Pagination',
  components: {
    AnalyticsTrackedTag,
  },
  props: {
    nextLink: {
      type: String,
      default: undefined,
    },
    nextTitle: {
      type: String,
      default: undefined,
    },
    previousLink: {
      type: String,
      default: undefined,
    },
    previousTitle: {
      type: String,
      default: undefined,
    },
  },
};
</script>
