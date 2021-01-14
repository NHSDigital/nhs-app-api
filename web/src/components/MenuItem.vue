<template>
  <li :class="$style.listMenuItem">
    <analytics-tracked-tag :id="id"
                           :href="href"
                           :text="text"
                           :aria-label="ariaLabel || text"
                           :tag="tag"
                           :target="target"
                           :class="$style.listMenuItemLink"
                           :click-param="clickParam"
                           :prevent-default="preventDefault"
                           :click-func="clickFunc">
      <div :class="[$style.listMenuItemContainer, showMeta ? $style['withMeta'] : '']">
        <component :is="headerTag"
                   :class="['nhsuk-heading-s',
                            $style.break,
                            highlightText ? $style['highlight-text'] : '']"
                   :aria-label="ariaText">{{ text }}</component>

        <p v-if="description"
           :id="descriptionId"
           :data-sid="descriptionDataSid">{{ description }}</p>

        <div v-if="showMeta" :class="$style['listMenuItem__meta']">
          <span v-if="showCount"
                :id="`${id}_countIndicator`"
                :class="$style['listMenuItem__count']"
                aria-hidden="true">{{ count }}</span>
          <span v-if="showIndicator"
                :id="`${id}_discIndicator`"
                :class="$style['listMenuItem__disc']"/>
        </div>
        <slot/>
      </div>
    </analytics-tracked-tag>
  </li>
</template>
<script>
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';

export default {
  name: 'MenuItem',
  components: {
    AnalyticsTrackedTag,
  },
  props: {
    id: {
      type: String,
      required: true,
    },
    href: {
      type: String,
      default: undefined,
    },
    ariaLabel: {
      type: String,
      default: undefined,
    },
    text: {
      type: String,
      default: undefined,
    },
    description: {
      type: String,
      default: undefined,
    },
    descriptionId: {
      type: String,
      default: undefined,
    },
    descriptionDataSid: {
      type: String,
      default: undefined,
    },
    tag: {
      type: String,
      default: 'a',
    },
    target: {
      type: String,
      default: undefined,
    },
    preventDefault: {
      type: Boolean,
      default: true,
    },
    headerTag: {
      type: String,
      default: 'span',
    },
    clickFunc: {
      type: Function,
      default: undefined,
    },
    clickParam: {
      type: [String, Object],
      default: undefined,
    },
    showIndicator: {
      type: Boolean,
      default: false,
    },
    highlightText: {
      type: Boolean,
      default: false,
    },
    count: {
      type: Number,
      default: undefined,
    },
  },
  computed: {
    showCount() {
      return this.count !== undefined;
    },
    showMeta() {
      return this.showCount || this.showIndicator;
    },
    ariaText() {
      if (this.showIndicator) {
        return `${this.text}.${this.$t('messages.youHaveUnreadMessages')}`;
      }
      return this.showCount ? `${`${this.text} (${this.count} `}${this.$t('myRecord.records')})` : this.text;
    },

  },
};
</script>
<style module lang="scss" scoped>
  @import "@/style/custom/menu-item";
</style>
