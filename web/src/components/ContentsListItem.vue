<template>
  <li class="nhsuk-contents-list__item"
      :aria-current="isCurrentPage ? 'page' : undefined">
    <span v-if="isCurrentPage" class="nhsuk-contents-list__current">{{ text }}</span>
    <analytics-tracked-tag v-else
                           :id="id"
                           :href="href"
                           :text="text"
                           tag="a"
                           class="nhsuk-contents-list__link"
                           :click-param="clickParam"
                           :click-func="clickFunc">
      {{ text }}
    </analytics-tracked-tag>
  </li>
</template>

<script>
import AnalyticsTrackedTag from './widgets/AnalyticsTrackedTag';

export default {
  name: 'ContentsListItem',
  components: { AnalyticsTrackedTag },
  props: {
    clickFunc: {
      type: Function,
      default: undefined,
    },
    clickParam: {
      type: [String, Object],
      default: undefined,
    },
    href: {
      type: String,
      default: undefined,
    },
    routeName: {
      type: String,
      default: undefined,
    },
    id: {
      type: String,
      required: true,
    },
    text: {
      type: String,
      default: undefined,
    },
  },
  computed: {
    isCurrentPage() {
      return this.$route.name === this.routeName;
    },
  },
};
</script>
